using System;
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
    public Func<string, bool> ForceSession;

    BufferBlock<MemoryPackBinaryRequestInfo> _msgBuffer = new BufferBlock<MemoryPackBinaryRequestInfo>();

    List<Room> RoomList = new List<Room>();
    UserManager UserMgr = new UserManager();

    Dictionary<int, Action<MemoryPackBinaryRequestInfo>> PacketHandlerMap = new Dictionary<int, Action<MemoryPackBinaryRequestInfo>>();

    PacketHandlerCommon CommonPacketHandler = new PacketHandlerCommon();
    PacketHandlerRoom RoomPacketHandler = new PacketHandlerRoom();
    PacketHandlerGame GamePacketHandler = new PacketHandlerGame();


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
        MainServer.MainLogger.Info("PacketProcessor::Destory - begin");

        isThreadRunning = false;
        _msgBuffer.Complete();
        ProcessThread.Join();

        MainServer.MainLogger.Info("PacketProcessor::Destory - end");

    }

    public void InsertPacket(MemoryPackBinaryRequestInfo data)
    {
        _msgBuffer.Post(data);
    }

    void RegistPacketHandlers()
    {
        PacketHandler.NetworkSendFunc = NeworktSendFunc;
        PacketHandler.ForceSession = ForceSession;
        PacketHandler.DistributeInnerPacket = InsertPacket;
        UserMgr.SetDistributeInnerPacket(InsertPacket);

        CommonPacketHandler.Init(UserMgr);
        CommonPacketHandler.SetCheckCount(UserMgr.GetMaxUserCount() / 4);
        CommonPacketHandler.RegistPacketHandler(PacketHandlerMap);

        RoomPacketHandler.Init(UserMgr);
        RoomPacketHandler.SetRoomList(RoomList);
        RoomPacketHandler.RegistPacketHandler(PacketHandlerMap);

        GamePacketHandler.Init(UserMgr);
        GamePacketHandler.RegistPacketHandler(PacketHandlerMap);
        GamePacketHandler.SetRoomList(RoomList);


    }

    void Process()
    {
        while (isThreadRunning)
        {
            try
            {
                var packet = _msgBuffer.Receive();

                var header = new PacketHeadInfo();
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

