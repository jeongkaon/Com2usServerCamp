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
        PacketHeadInfo.Write(sendData, PACKET_ID.NTF_IN_ROOM_LEAVE);

        var memoryPakcPacket = new MemoryPackBinaryRequestInfo(null);
        memoryPakcPacket.Data = sendData;
        memoryPakcPacket.SessionID = sessionID;
        return memoryPakcPacket;
    }

    public static MemoryPackBinaryRequestInfo MakeNTFInConnectOrDisConnectClientPacket(bool isConnect, string sessionID)
    {
        var memoryPackPacket = new MemoryPackBinaryRequestInfo(null);
        memoryPackPacket.Data = new byte[PacketHeadInfo.HeaderSize];

        if (isConnect)
        {
            PacketHeadInfo.WritePacketId(memoryPackPacket.Data, (UInt16)PACKET_ID.NTF_IN_CONNECT_CLIENT);
        }
        else
        {
            PacketHeadInfo.WritePacketId(memoryPackPacket.Data, (UInt16)PACKET_ID.NTF_IN_DISCONNECT_CLIENT);
        }

        memoryPackPacket.SessionID = sessionID;
        return memoryPackPacket;
    }

    public static MemoryPackBinaryRequestInfo MakeNTFInnerUserCheckPacket()
    {
        var memoryPackPacket = new MemoryPackBinaryRequestInfo(null);
        memoryPackPacket.Data = new byte[PacketHeadInfo.HeaderSize];


        PacketHeadInfo.WritePacketId(memoryPackPacket.Data, (UInt16)PACKET_ID.NTR_IN_CHECK);

        return memoryPackPacket;

    }

    public static MemoryPackBinaryRequestInfo MakeNTFInnerUserForceClosePacket(string SessionId)
    {
        var memoryPackPacket = new MemoryPackBinaryRequestInfo(null);
        memoryPackPacket.Data = new byte[PacketHeadInfo.HeaderSize];


        PacketHeadInfo.WritePacketId(memoryPackPacket.Data, (UInt16)PACKET_ID.NTF_IN_FORCEDISCONNECT_CLIENT);
        memoryPackPacket.SessionID = SessionId;

        return memoryPackPacket;

    }
    public static MemoryPackBinaryRequestInfo MakeNTFInnerRoomCheckPacket()
    {

        var memoryPackPacket = new MemoryPackBinaryRequestInfo(null);
        memoryPackPacket.Data = new byte[PacketHeadInfo.HeaderSize];


        PacketHeadInfo.WritePacketId(memoryPackPacket.Data, (UInt16)PACKET_ID.NTF_IN_ROOMCHECK);

        return memoryPackPacket;


    }


}
[MemoryPackable]
public partial class PKTInternalNtfRoomLeave : PacketHeader
{
    public int RoomNumber { get; set; }
    public string UserID { get; set; }
}