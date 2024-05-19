using MemoryPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PvPGameServer;

public class PacketHandlerGame : PacketHandler
{
    List<Room> RoomList = null;
    //SuperSocket.SocketBase.Logging.ILog _logger;

    //public void SetLogger(SuperSocket.SocketBase.Logging.ILog logger)
    //{
    //    _logger = logger;
    //}

    public void SetRoomList(List<Room> roomList)
    {
        RoomList = roomList;
    }


    public void RegistPacketHandler(Dictionary<int, Action<MemoryPackBinaryRequestInfo>> packetHandlerMap)
    {
        packetHandlerMap.Add((int)PacketId.ReqPutOmok, ReqOmokPut);
        packetHandlerMap.Add((int)PacketId.NtfInUpdateScore, ReqOmokPut);
    }
    public string GetPlayerIdInPacket(MemoryPackBinaryRequestInfo packetData)
    {
        var headerSize = PacketHeadInfo.HeaderSize;
        var dataLen = packetData.Data.Length;
        return FastBinaryRead.String(packetData.Data, headerSize, dataLen - headerSize);

    }
    public void ReqOmokPut(MemoryPackBinaryRequestInfo packetData)
    {
        var sessionId = packetData.SessionID;
        var reqData = MemoryPackSerializer.Deserialize<ReqPutOMok>(packetData.Data);
        var user = _userMgr.GetUser(sessionId);
        var board = RoomList[user.GetRoomNumber()].GetGameBoard();
        board.CheckBaord(reqData.mok, reqData.PosX, reqData.PosY);
    }

 
}
