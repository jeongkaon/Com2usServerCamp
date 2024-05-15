using Microsoft.Extensions.Logging;
using NLog.Targets;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Logging;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace PvPGameServer;

public class MainServer : AppServer<ClientSession, MemoryPackBinaryRequestInfo>
{
    public static PvPServerOption serverOption;
    public static SuperSocket.SocketBase.Logging.ILog MainLogger;

    SuperSocket.SocketBase.Config.IServerConfig serverConfig;

    PacketProcessor MainPacketProcessor = new PacketProcessor();
    GameDBProcessor DBProcessor = new GameDBProcessor();
    AccountDBProcessor AccountProcessor = new AccountDBProcessor();

    RoomManager RoomMgr = new RoomManager();

    Timer RoomCheckTimer = null; 
    Timer UserCheckTimer = null;


    public MainServer()
        : base(new DefaultReceiveFilterFactory<ReceiveFilter, MemoryPackBinaryRequestInfo>())
    {
        NewSessionConnected += new SessionHandler<ClientSession>(OnConnected);
        SessionClosed += new SessionHandler<ClientSession, CloseReason>(OnClosed);
        NewRequestReceived += new RequestHandler<ClientSession, MemoryPackBinaryRequestInfo>(OnPacketReceived);

    }

    void InnerRoomCheckTimer(object? state)
    {
        MemoryPackBinaryRequestInfo packet = InnerPacketMaker.MakeNTFInnerRoomCheckPacket();
        Distribute(packet);
    }

    void InnerUserCheckTimer(object? state)
    {
        MemoryPackBinaryRequestInfo packet = InnerPacketMaker.MakeNTFInnerUserCheckPacket();
        Distribute(packet);
    }

    public void InitConfig(PvPServerOption option)
    {
        serverOption = option;

        serverConfig = new SuperSocket.SocketBase.Config.ServerConfig
        {
            Name = option.Name,
            Ip = "Any",
            Port = option.Port,
            Mode = SocketMode.Tcp,
            MaxConnectionNumber = option.MaxConnectionNumber,
            MaxRequestLength = option.MaxRequestLength,
            ReceiveBufferSize = option.ReceiveBufferSize,
            SendBufferSize = option.SendBufferSize
        };

       
    }

    public void CreateAndStartServer()
    {
        try
        {
            bool res = Setup(new SuperSocket.SocketBase.Config.RootConfig(), serverConfig, logFactory: new NLogLogFactory("NLog.config"));

            if (res == false)
            {
                return;
            }
            else
            {
                MainLogger = base.Logger;
                MainLogger.Info("서버 초기화 성공");
            }

            CreateComponent();

            RoomCheckTimer = new Timer(InnerRoomCheckTimer, null, 0, serverOption.UserCheckTime);
            UserCheckTimer = new Timer(InnerUserCheckTimer, null, 0, serverOption.RoomCheckTime);

            MainLogger.Debug("서버 생성 성공");

            Start();
        }
        catch (Exception ex)
        {
            MainLogger.Error($"[ERROR] 서버 생성 실패: {ex.ToString()}");
        }
    }

    public ErrorCode CreateComponent()
    {
        Room.NetworkSendFunc = SendData;
        GameBoard.NetworkSendFunc = SendData;
        GameBoard.DistributeInnerDB = DistributeGameDB;

        RoomMgr.CreateRooms(serverOption);

        MainPacketProcessor = new PacketProcessor();
        MainPacketProcessor.NeworktSendFunc = SendData;
        MainPacketProcessor.ForceSession = ForceDisconnectSession;
        MainPacketProcessor.DistributeInnerPacketDB = DistributeGameDB;
        MainPacketProcessor.CreateAndStart(RoomMgr.GetRooms(), serverOption);

        
        DBProcessor = new GameDBProcessor();
        DBProcessor.CreateAndStart();

        AccountProcessor = new AccountDBProcessor();
        AccountProcessor.CreateAndStart();


        return ErrorCode.None;
    }

    public bool ForceDisconnectSession(string sessionId)
    {
        var session = GetAppSessionByID(sessionId);
        if (session == null)
        {
            return false;
        }
        session.Close();
        return true;
    }

    void Distribute(MemoryPackBinaryRequestInfo reqPacket)
    {
        MainPacketProcessor.InsertPacket(reqPacket);
    }
    void DistributeGameDB(MemoryPackBinaryRequestInfo reqPacket)
    {
        DBProcessor.InsertPacket(reqPacket);
    }

    void DistributeAccountDB(MemoryPackBinaryRequestInfo reqPacket)
    {
        AccountProcessor.InsertPacket(reqPacket);
    }

    public bool SendData(string sessionId, byte[] data)
    {
        var session = GetSessionByID(sessionId);
        try
        {
            if (session == null)
            {
                return false;
            }

            session.Send(data, 0, data.Length);
        }
        catch (Exception e)
        {
            session.SendEndWhenSendingTimeOut();
            session.Close();
        }

        return true;
    }
    public void StopServer()
    {
        Stop();
        MainPacketProcessor.Destroy();
        DBProcessor.Destroy();
        AccountProcessor.Destroy();
    }


    void OnConnected(ClientSession session)
    {
        MainLogger.Info($"세션 번호 {session.SessionID} 접속");
        var packet = InnerPacketMaker.MakeNTFInConnectOrDisConnectClientPacket(true, session.SessionID);
        Distribute(packet);

    }
    void OnClosed(ClientSession session, CloseReason reason)
    {
        
        MainLogger.Info($"세션 번호 {session.SessionID} 접속해제: {reason.ToString()}");
        var packet = InnerPacketMaker.MakeNTFInConnectOrDisConnectClientPacket(false, session.SessionID);
        

        Distribute(packet);
        

    }
    void OnPacketReceived(ClientSession session, MemoryPackBinaryRequestInfo reqInfo)
    {
        MainLogger.Debug($"세션 번호 {session.SessionID} 받은 데이터 크기: {reqInfo.Body.Length}, ThreadId: {Thread.CurrentThread.ManagedThreadId}");
        var packetId = FastBinaryRead.UInt16(reqInfo.Data, 3);
        reqInfo.SessionID = session.SessionID;

        if (packetId == (UInt16)PacketId.ReqLogin)
        {
            DistributeAccountDB(reqInfo);
            return;
        }

        if (packetId < (UInt16)PacketId.ReqEnd && packetId > (UInt16)PacketId.ReqBegin)
        {
            Distribute(reqInfo);
            return;
        }
    }
}

public class ClientSession : AppSession<ClientSession, MemoryPackBinaryRequestInfo>
{

}