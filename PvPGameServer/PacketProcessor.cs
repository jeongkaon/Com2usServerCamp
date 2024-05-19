using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace PvPGameServer;

public class PacketProcessor
{
    bool _isThreadRunning = false;
    System.Threading.Thread ProcessThread = null;

    public static SuperSocket.SocketBase.Logging.ILog _logger;

    BufferBlock<MemoryPackBinaryRequestInfo> _msgBuffer = new BufferBlock<MemoryPackBinaryRequestInfo>();

    List<Room> _roomList = new List<Room>();
    UserManager _userMgr = new UserManager();

    Dictionary<int, Action<MemoryPackBinaryRequestInfo>> _packetHandlerMap = new Dictionary<int, Action<MemoryPackBinaryRequestInfo>>();

    PacketHandlerCommon _commonPacketHandler = new PacketHandlerCommon();
    PacketHandlerRoom _roomPacketHandler = new PacketHandlerRoom();
    PacketHandlerGame _gamePacketHandler = new PacketHandlerGame();

    public Func<string, byte[], bool> NeworktSendFunc;
    public Func<string, bool> ForceSession;
    public Action<MemoryPackBinaryRequestInfo> DistributeInnerPacketDB;

 
    public void CreateAndStart(List<Room> roomList, PvPServerOption option)
    {
        var MaxUserCount = option.RoomMaxCount * option.RoomMaxUserCount;
        _userMgr.Init(MaxUserCount);

        _roomList = roomList;
        var MinRoomNum = _roomList[0].Number;
        var MaxRoomNum = _roomList[0].Number + _roomList.Count() - 1;

        RegistPacketHandlers();

        _isThreadRunning = true;
        ProcessThread = new System.Threading.Thread(Process);
        ProcessThread.Start();

    }

  
    public void Destroy()
    {
        _logger.Info("PacketProcessor::Destory - begin");

        _isThreadRunning = false;
        _msgBuffer.Complete();
        ProcessThread.Join();

        _logger.Info("PacketProcessor::Destory - end");

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
        _userMgr.SetDistributeInnerPacket(InsertPacket);

        _commonPacketHandler.Init(_userMgr);
        _commonPacketHandler.SetCheckCount(_userMgr.GetMaxUserCount() / 4);
        _commonPacketHandler.GetDistributeGameDB(DistributeInnerPacketDB);
        _commonPacketHandler.RegistPacketHandler(_packetHandlerMap);

        _roomPacketHandler.Init(_userMgr);
        _roomPacketHandler.SetRoomList(_roomList);
        _roomPacketHandler.RegistPacketHandler(_packetHandlerMap);


        _gamePacketHandler.Init(_userMgr);
        _gamePacketHandler.RegistPacketHandler(_packetHandlerMap);
        _gamePacketHandler.SetRoomList(_roomList);


    }
    public string GetUserId(string sessionId)
    {
        var user = _userMgr.GetUser(sessionId);
        return user.ID();
    }
    void Process()
    {
        while (_isThreadRunning)
        {
            try
            {
                var packet = _msgBuffer.Receive();

                var header = new PacketHeadInfo();
                header.Read(packet.Data);

                if (_packetHandlerMap.ContainsKey(header.Id))
                {
                    _packetHandlerMap[header.Id](packet);
                }
       
            }
            catch (Exception ex)
            {
                if (_isThreadRunning)
                {
                    _logger.Error(ex.ToString());
                }
            }
        }
    }



}

