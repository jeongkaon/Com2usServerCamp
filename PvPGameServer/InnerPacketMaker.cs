using MemoryPack;
using PvPGameServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PvPGameServer;

public class InnerPacketMaker
{
    public static MemoryPackBinaryRequestInfo MakeNTFInnerRoomLeavePacket(string sessionID, int roomNumber, string userID)
    {

        var packet = new PKTInternalNtfRoomLeave()
        {
            RoomNumber = roomNumber,
            UserID = userID,
        };

        var sendData = MemoryPackSerializer.Serialize(packet);
        MemorypackPacketHeadInfo.Write(sendData, PACKET_ID.NTF_IN_ROOM_LEAVE);

        var memoryPakcPacket = new MemoryPackBinaryRequestInfo(null);
        memoryPakcPacket.Data = sendData;
        memoryPakcPacket.SessionID = sessionID;
        return memoryPakcPacket;
    }

    public static MemoryPackBinaryRequestInfo MakeNTFInConnectOrDisConnectClientPacket(bool isConnect, string sessionID)
    {
        var memoryPakcPacket = new MemoryPackBinaryRequestInfo(null);
        memoryPakcPacket.Data = new byte[MemorypackPacketHeadInfo.HeaderSize];

        if (isConnect)
        {
            MemorypackPacketHeadInfo.WritePacketId(memoryPakcPacket.Data, (UInt16)PACKET_ID.NTF_IN_CONNECT_CLIENT);
        }
        else
        {
            MemorypackPacketHeadInfo.WritePacketId(memoryPakcPacket.Data, (UInt16)PACKET_ID.NTF_IN_DISCONNECT_CLIENT);
        }

        memoryPakcPacket.SessionID = sessionID;
        return memoryPakcPacket;
    }

}
   

[MemoryPackable]
public partial class PKTInternalNtfRoomLeave : PacketHeader
{
    public int RoomNumber { get; set; }
    public string UserID { get; set; }
}
