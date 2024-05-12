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
            RoomNumber = roomNumber,
            UserId = userId,
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

    public static MemoryPackBinaryRequestInfo MakeNTFInnerForDBUpdateWinPacket(string winnerId)
    {
        var memoryPackPacket = new MemoryPackBinaryRequestInfo(null);

        memoryPackPacket.Data = new byte[PacketHeadInfo.HeaderSize + winnerId.Length];
        FastBinaryWrite.String(memoryPackPacket.Data, PacketHeadInfo.HeaderSize, winnerId);

        PacketHeadInfo.WritePacketId(memoryPackPacket.Data, (UInt16)PacketId.NtfInUpdateWinnerResult);
        return memoryPackPacket;
    }
    public static MemoryPackBinaryRequestInfo MakeNTFInnerForDBUpdateLosePacket(string loserId)
    {
        var memoryPackPacket = new MemoryPackBinaryRequestInfo(null);

        memoryPackPacket.Data = new byte[PacketHeadInfo.HeaderSize + loserId.Length];
        FastBinaryWrite.String(memoryPackPacket.Data, PacketHeadInfo.HeaderSize, loserId);

        PacketHeadInfo.WritePacketId(memoryPackPacket.Data, (UInt16)PacketId.NtfInUpdateLoserResult);
        return memoryPackPacket;
    }

    public static MemoryPackBinaryRequestInfo MakeNTFInnerForDBUpdateDrawPacket(string player)
    {
        var memoryPackPacket = new MemoryPackBinaryRequestInfo(null);

        memoryPackPacket.Data = new byte[PacketHeadInfo.HeaderSize + player.Length];
        FastBinaryWrite.String(memoryPackPacket.Data, PacketHeadInfo.HeaderSize, player);

        PacketHeadInfo.WritePacketId(memoryPackPacket.Data, (UInt16)PacketId.NtfInUpdateDrawResult);
        return memoryPackPacket;
    }

}
