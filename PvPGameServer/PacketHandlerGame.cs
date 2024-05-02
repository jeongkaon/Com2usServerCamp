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
    public void SetRoomList(List<Room> roomList)
    {
        RoomList = roomList;
    }


    public void RegistPacketHandler(Dictionary<int, Action<MemoryPackBinaryRequestInfo>> packetHandlerMap)
    {
       //게임 관련 옮겨야함
        packetHandlerMap.Add((int)PACKET_ID.REQ_PUT_OMOK, ReqOmokPut);

    }
    public void ReqOmokPut(MemoryPackBinaryRequestInfo packetData)
    {

        var sessionId = packetData.SessionID;
        var reqData = MemoryPackSerializer.Deserialize<ReqPutOMok>(packetData.Data);
        var user = UserMgr.GetUser(sessionId);
        var board = RoomList[user.GetRoomNumber()].GetGameBoard();

        board.CheckBaord(reqData.mok, reqData.PosX, reqData.PosY);

    }


}
