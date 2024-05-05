﻿using Microsoft.Extensions.Logging;
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
    RoomManager RoomMgr = new RoomManager();

    //타이머하나 두고 -> 이너패킷 핸들러에게 전달한다.
    //MainPacketProcessor -> 여기서 패킷핸들러들에게 할당해서 방조사/하트비트 체크한다.
    Timer timer = null;


    public MainServer()
        : base(new DefaultReceiveFilterFactory<ReceiveFilter, MemoryPackBinaryRequestInfo>())
    {
        NewSessionConnected += new SessionHandler<ClientSession>(OnConnected);
        SessionClosed += new SessionHandler<ClientSession, CloseReason>(OnClosed);
        NewRequestReceived += new RequestHandler<ClientSession, MemoryPackBinaryRequestInfo>(OnPacketReceived);

    }

    private void InnerCheckTimer(object? state)
    {
        //이 패킷 받은 핸들러에서 하트비트, 방조사 하는고임.

      //  Console.WriteLine("Inner User check Timer run... - Mainserver");
        MemoryPackBinaryRequestInfo[] packet =
        {
            InnerPacketMaker.MakeNTFInnerUserCheckPacket(),
            InnerPacketMaker.MakeNTFInnerRoomCheckPacket()

        };

        Distribute(packet[0]);
        Distribute(packet[1]);

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
            timer = new Timer(InnerCheckTimer, null, 0, 1000);

            MainLogger.Debug("서버 생성 성공");

            Start();

        }
        catch (Exception ex)
        {
            MainLogger.Error($"[ERROR] 서버 생성 실패: {ex.ToString()}");

        }
    }

    public ERROR_CODE CreateComponent()
    {
        Room.NetworkSendFunc = SendData;

        RoomMgr.CreateRooms(serverOption);

        MainPacketProcessor = new PacketProcessor();
        MainPacketProcessor.NeworktSendFunc = SendData;
        MainPacketProcessor.CreateAndStart(RoomMgr.GetRooms(), serverOption);

        return ERROR_CODE.NONE;

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

        reqInfo.SessionID = session.SessionID;
        Distribute(reqInfo);
    }

}



public class ClientSession : AppSession<ClientSession, MemoryPackBinaryRequestInfo>
{

}