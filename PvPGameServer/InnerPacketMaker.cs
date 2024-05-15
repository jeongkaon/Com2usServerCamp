using MemoryPack;
using PvPGameServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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

    public static MemoryPackBinaryRequestInfo MakeNTFInnerGetUserDataInDB(string id)
    {
        var memoryPackPacket = new MemoryPackBinaryRequestInfo(null);

        memoryPackPacket.Data = new byte[PacketHeadInfo.HeaderSize + id.Length];
        FastBinaryWrite.String(memoryPackPacket.Data, PacketHeadInfo.HeaderSize, id);

        PacketHeadInfo.WritePacketId(memoryPackPacket.Data, (UInt16)PacketId.NtfInGetUserData);
        return memoryPackPacket;
    }

    public static MemoryPackBinaryRequestInfo MakeNTFInnerUpdateDB(string id)
    {
        var memoryPackPacket = new MemoryPackBinaryRequestInfo(null);
        memoryPackPacket.Data = new byte[PacketHeadInfo.HeaderSize + id.Length];
        FastBinaryWrite.String(memoryPackPacket.Data, PacketHeadInfo.HeaderSize, id);
        PacketHeadInfo.WritePacketId(memoryPackPacket.Data, (UInt16)PacketId.NtfInUpdateUserData);

        return memoryPackPacket;
    }


   
}
