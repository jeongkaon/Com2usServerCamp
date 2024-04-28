﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace PvPGameServer;

public class PacketProcessor
{
    bool isThreadRunning = false;
    System.Threading.Thread ProcessThread = null;

    public Func<string, byte[], bool> NeworktSendFunc;

    BufferBlock<MemoryPackBinaryRequestInfo> msgBuffer = new BufferBlock<MemoryPackBinaryRequestInfo>();

    List<Room> RoomList = new List<Room>();
    UserManager UserMgr = new UserManager();

    Dictionary<int, Action<MemoryPackBinaryRequestInfo>> PacketHandlerMap = new Dictionary<int, Action<MemoryPackBinaryRequestInfo>>();

    PacketHandlerCommon CommonPacketHandler = new PacketHandlerCommon();
    PacketHandlerRoom RoomPacketHandler = new PacketHandlerRoom();

    public void CreateAndStart(List<Room> roomList, PvPServerOption option)
    {
        var MaxUserCount = option.RoomMaxCount * option.RoomMaxUserCount;
        UserMgr.Init(MaxUserCount);

        RoomList = roomList;
        var MinRoomNum = RoomList[0].Number;
        var MaxRoomNum = RoomList[0].Number + RoomList.Count() - 1;

        RegistPacketHandlers();

        isThreadRunning = true;
        ProcessThread = new System.Threading.Thread(Process);
        ProcessThread.Start();

    }


    public void Destroy()
    {
        //로그설정
        MainServer.MainLogger.Info("PacketProcessor::Destory - begin");

        isThreadRunning = false;
        msgBuffer.Complete();
        ProcessThread.Join();

        //로그설정
        MainServer.MainLogger.Info("PacketProcessor::Destory - end");

    }

    public void InsertPacket(MemoryPackBinaryRequestInfo data)
    {
        msgBuffer.Post(data);
    }

    void RegistPacketHandlers()
    {
        PacketHandler.NetworkSendFunc = NeworktSendFunc;
        PacketHandler.DistributeInnerPacket = InsertPacket;

        CommonPacketHandler.Init(UserMgr);
        CommonPacketHandler.RegistPacketHandler(PacketHandlerMap);

        RoomPacketHandler.Init(UserMgr);
        RoomPacketHandler.SetRoomList(RoomList);
        RoomPacketHandler.RegistPacketHandler(PacketHandlerMap);
    }

    void Process()
    {
        while (isThreadRunning)
        {
            try
            {
                var packet = msgBuffer.Receive();

                var header = new MemorypackPacketHeadInfo();
                header.Read(packet.Data);

                if (PacketHandlerMap.ContainsKey(header.Id))
                {
                    PacketHandlerMap[header.Id](packet);
                }
       
            }
            catch (Exception ex)
            {
                if (isThreadRunning)
                {
                    MainServer.MainLogger.Error(ex.ToString());
                }
            }
        }
    }



}

