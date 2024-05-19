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
    PvPServerOption _serverOption;
    
    SuperSocket.SocketBase.Logging.ILog _mainLogger;
    SuperSocket.SocketBase.Config.IServerConfig _serverConfig;

    PacketProcessor     _mainPacketProcessor= null;
    GameDBProcessor     _dBProcessor        = null;
    AccountDBProcessor  _accountProcessor   = null;
    MatchingProcessor _matchProcessor = null;

    RoomManager         _roomMgr            = new RoomManager();

    Timer _roomCheckTimer = null; 
    Timer _userCheckTimer = null;

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
        _serverOption = option;
        _serverConfig = new SuperSocket.SocketBase.Config.ServerConfig
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
            bool res = Setup(new SuperSocket.SocketBase.Config.RootConfig(), _serverConfig, logFactory: new NLogLogFactory("NLog.config"));

            if (res == false)
            {
                return;
            }
            else
            {
                _mainLogger = base.Logger;
                _mainLogger.Info("서버 초기화 성공");
            }

            CreateComponent();

            _roomCheckTimer = new Timer(InnerRoomCheckTimer, null, 0, _serverOption.UserCheckTime);
            _userCheckTimer = new Timer(InnerUserCheckTimer, null, 0, _serverOption.RoomCheckTime);

            _mainLogger.Debug("서버 생성 성공");

            Start();
        }
        catch (Exception ex)
        {
            _mainLogger.Error($"[ERROR] 서버 생성 실패: {ex.ToString()}");
        }
    }

    public ErrorCode CreateComponent()
    {
        Room.NetworkSendFunc = SendData;
        GameBoard.NetworkSendFunc = SendData;
        GameBoard.DistributeInnerDB = DistributeGameDB;

        _roomMgr.CreateRooms(_serverOption);

        _mainPacketProcessor = new PacketProcessor();
        _mainPacketProcessor.NeworktSendFunc = SendData;
        _mainPacketProcessor.ForceSession = ForceDisconnectSession;
        _mainPacketProcessor.DistributeInnerPacketDB = DistributeGameDB;
        _mainPacketProcessor.CreateAndStart(_roomMgr.GetRooms(), _serverOption);
        _mainPacketProcessor.SetLogger(_mainLogger);

        _dBProcessor = new GameDBProcessor();
        _dBProcessor.CreateAndStart();
        _dBProcessor.SetLogger(_mainLogger);

        _accountProcessor = new AccountDBProcessor();
        _accountProcessor.CreateAndStart();
        _accountProcessor.SetLogger(_mainLogger);

        _matchProcessor = new MatchingProcessor();
        _matchProcessor.CreateAndStart(_roomMgr, _serverOption);
        _matchProcessor.SetLogger(_mainLogger);
        _matchProcessor.SetIpAddress();

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
        _mainPacketProcessor.InsertPacket(reqPacket);
    }
    void DistributeGameDB(MemoryPackBinaryRequestInfo reqPacket)
    {
        _dBProcessor.InsertPacket(reqPacket);
    }
    void DistributeAccountDB(MemoryPackBinaryRequestInfo reqPacket)
    {
        _accountProcessor.InsertPacket(reqPacket);
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
        _mainPacketProcessor.Destroy();
        _dBProcessor.Destroy();
        _accountProcessor.Destroy();
        _matchProcessor.Destory();
    }

    void OnConnected(ClientSession session)
    {
        _mainLogger.Info($"세션 번호 {session.SessionID} 접속");
        var packet = InnerPacketMaker.MakeNTFInConnectOrDisConnectClientPacket(true, session.SessionID);
        Distribute(packet);
    }
    void OnClosed(ClientSession session, CloseReason reason)
    {
        _mainLogger.Info($"세션 번호 {session.SessionID} 접속해제: {reason.ToString()}");
        var packet = InnerPacketMaker.MakeNTFInConnectOrDisConnectClientPacket(false, session.SessionID);
        Distribute(packet);
    }
    void OnPacketReceived(ClientSession session, MemoryPackBinaryRequestInfo reqInfo)
    {
        _mainLogger.Debug($"세션 번호 {session.SessionID} 받은 데이터 크기: {reqInfo.Body.Length}, ThreadId: {Thread.CurrentThread.ManagedThreadId}");
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