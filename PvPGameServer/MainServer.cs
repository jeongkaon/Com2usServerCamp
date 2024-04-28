using Microsoft.Extensions.Logging;
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

namespace PvPGameServer;

public class MainServer : AppServer<ClientSession, MemoryPackBinaryRequestInfo>
{
    public static PvPServerOption serverOption;
    public static SuperSocket.SocketBase.Logging.ILog MainLogger;
    
    SuperSocket.SocketBase.Config.IServerConfig serverConfig;

    PacketProcessor MainPacketProcessor = new PacketProcessor();
    RoomManager RoomMgr = new RoomManager();



    public MainServer()
        : base(new DefaultReceiveFilterFactory<ReceiveFilter, MemoryPackBinaryRequestInfo>())
    {
        NewSessionConnected += new SessionHandler<ClientSession>(OnConnected);
        SessionClosed += new SessionHandler<ClientSession, CloseReason>(OnClosed);
        NewRequestReceived += new RequestHandler<ClientSession, MemoryPackBinaryRequestInfo>(OnPacketReceived);

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
            //맨마지막 인자 로그수정 있음!
            bool res = Setup(new SuperSocket.SocketBase.Config.RootConfig(), serverConfig, logFactory: new NLogLogFactory());

            if (res == false)
            {
                Console.WriteLine("네트워크 설정 실패");
                //네트워크 실패
                return;
            }
            else
            {

                MainLogger = base.Logger;
                MainLogger.Info("server init");
                Console.WriteLine("네트워크 설정 성공?");

            }

            CreateComponent();
            MainLogger.Info("서버 생성 성공");

            Start();

        }
        catch (Exception e) 
        {
            MainLogger.Info("서버 생성 실패");

            //서버 생성 실패!   
        }
    }

    public void CreateComponent()
    {
        Room.NetworkSendFunc = SendData;

        RoomMgr.CreateRooms(serverOption);

        MainPacketProcessor = new PacketProcessor();
        MainPacketProcessor.NeworktSendFunc = SendData;
        MainPacketProcessor.CreateAndStart(RoomMgr.GetRooms(), serverOption);

        return;

    }



    void Distribute(MemoryPackBinaryRequestInfo reqPacket)
    {
        MainPacketProcessor.InsertPacket(reqPacket);
    }

    public bool SendData(string sessionId, byte[] data)
    {
        var session = GetSessionByID(sessionId);

        try
        {
            if(session == null)
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
    }


    void OnConnected(ClientSession session)
    {
        //메모리패킷 그거 해야햠
        MainLogger.Info($"세션 번호 {session.SessionID} 접속");


        var packet = InnerPacketMaker.MakeNTFInConnectOrDisConnectClientPacket(true,session.SessionID);
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

        reqInfo.SessionID = session.SessionID;
        Distribute(reqInfo);

    }

}



public class ClientSession : AppSession<ClientSession, MemoryPackBinaryRequestInfo>
{

}