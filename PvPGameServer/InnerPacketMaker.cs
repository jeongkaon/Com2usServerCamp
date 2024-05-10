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
    public static MemoryPackBinaryRequestInfo MakeNTFInnerRoomLeavePacket(string sessionId, int roomNumber, string userId)
    {

        var packet = new PKTInternalNtfRoomLeave()
        {
            _roomNumber = roomNumber,
            _userId = userId,
        };

        var sendData = MemoryPackSerializer.Serialize(packet);
        PacketHeadInfo.Write(sendData, PacketId.NtfIntRoomLeave);

        var memoryPakcPacket = new MemoryPackBinaryRequestInfo(null);
        memoryPakcPacket.Data = sendData;
        memoryPakcPacket.SessionID = sessionId;
        return memoryPakcPacket;
    }

    public static MemoryPackBinaryRequestInfo MakeNTFInConnectOrDisConnectClientPacket(bool isConnect, string sessionId)
    {
        var memoryPackPacket = new MemoryPackBinaryRequestInfo(null);
        memoryPackPacket.Data = new byte[PacketHeadInfo.HeaderSize];

        if (isConnect)
        {
            PacketHeadInfo.WritePacketId(memoryPackPacket.Data, (UInt16)PacketId.NtfInConnectClient);
        }
        else
        {
            PacketHeadInfo.WritePacketId(memoryPackPacket.Data, (UInt16)PacketId.NtfInDisconnectClient);
        }

        memoryPackPacket.SessionID = sessionId;
        return memoryPackPacket;
    }

    public static MemoryPackBinaryRequestInfo MakeNTFInnerUserCheckPacket()
    {
        var memoryPackPacket = new MemoryPackBinaryRequestInfo(null);
        memoryPackPacket.Data = new byte[PacketHeadInfo.HeaderSize];


        PacketHeadInfo.WritePacketId(memoryPackPacket.Data, (UInt16)PacketId.NtrInUserCheck);

        return memoryPackPacket;

    }

    public static MemoryPackBinaryRequestInfo MakeNTFInnerUserForceClosePacket(string SessionId)
    {
        var memoryPackPacket = new MemoryPackBinaryRequestInfo(null);
        memoryPackPacket.Data = new byte[PacketHeadInfo.HeaderSize];


        PacketHeadInfo.WritePacketId(memoryPackPacket.Data, (UInt16)PacketId.NtfInForceDisconnectClient);
        memoryPackPacket.SessionID = SessionId;

        return memoryPackPacket;

    }
    public static MemoryPackBinaryRequestInfo MakeNTFInnerRoomCheckPacket()
    {

        var memoryPackPacket = new MemoryPackBinaryRequestInfo(null);
        memoryPackPacket.Data = new byte[PacketHeadInfo.HeaderSize];


        PacketHeadInfo.WritePacketId(memoryPackPacket.Data, (UInt16)PacketId.NtfInRoomCheck);

        return memoryPackPacket;


    }

    public static MemoryPackBinaryRequestInfo MakeNTFInnerForDBUpdateScorePacket(string playerId1)
    {

    
        var id = Encoding.UTF8.GetBytes(playerId1);
        var memoryPackPacket = new MemoryPackBinaryRequestInfo(id);


        PacketHeadInfo.WritePacketId(memoryPackPacket.Data, (UInt16)PacketId.NtfInUpdateGameResult);

        return memoryPackPacket;
    }



}
[MemoryPackable]
public partial class PKTInternalNtfRoomLeave : PacketHeader
{
    public int _roomNumber { get; set; }
    public string _userId { get; set; }
}