using MemoryPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PvPGameServer;

public struct MemorypackPacketHeadInfo
{
    const int PacketHeaderMemorypacStartPos = 1;
    public const int HeaderSize = 6;

    public UInt16 TotalSize;
    public UInt16 Id;
    public byte Type;

    public static UInt16 GetTotalSize(byte[] data, int startPos)
    {
        return FastBinaryRead.UInt16(data, startPos + PacketHeaderMemorypacStartPos);
    }

    public static void WritePacketId(byte[] data, UInt16 packetId)
    {
        FastBinaryWrite.UInt16(data, PacketHeaderMemorypacStartPos + 2, packetId);
    }

    public void Read(byte[] headerData)
    {
        var pos = PacketHeaderMemorypacStartPos;

        TotalSize = FastBinaryRead.UInt16(headerData, pos);
        pos += 2;

        Id = FastBinaryRead.UInt16(headerData, pos);
        pos += 2;

        Type = headerData[pos];
        pos += 1;
    }

    public static void Write(byte[] packetData, PACKET_ID packetId, byte type = 0)
    {
        var pos = PacketHeaderMemorypacStartPos;

        FastBinaryWrite.UInt16(packetData, pos, (UInt16)packetData.Length);
        pos += 2;

        FastBinaryWrite.UInt16(packetData, pos, (UInt16)packetId);
        pos += 2;

        packetData[pos] = type;
    }

    public void DebugConsolOutHeaderInfo()
    {
        Console.WriteLine("DebugConsolOutHeaderInfo");
        Console.WriteLine("TotalSize : " + TotalSize);
        Console.WriteLine("Id : " + Id);
        Console.WriteLine("Type : " + Type);
    }
}

//패킷
[MemoryPackable]
public partial class PacketHeader
{
    public UInt16 TotalSize { get; set; } = 0;
    public UInt16 Id { get; set; } = 0;
    public byte Type { get; set; } = 0;

}

//CS -> Client to Server
//SC -> Server to Client
[MemoryPackable]
public partial class CSLoginPacket : PacketHeader
{
    public string UserID { get; set; }
    public string AuthToken { get; set; }
}

[MemoryPackable]
public partial class SCLoginPacket : PacketHeader
{
    public short Result { get; set; }
}



[MemoryPackable]
public partial class NtfMustClosePacket : PacketHeader
{
    public short Result { get; set; }
}



[MemoryPackable]
public partial class CSRoomEnterPacket : PacketHeader
{
    public int RoomNumber { get; set; }
}

[MemoryPackable]
public partial class SCRoomEnterPacket : PacketHeader
{
    public short Result { get; set; }
}

[MemoryPackable]
public partial class PKTNtfRoomUserList : PacketHeader
{
    public List<string> UserIDList { get; set; } = new List<string>();
}

[MemoryPackable]
public partial class PKTNtfRoomNewUser : PacketHeader
{
    public string UserID { get; set; }
}


[MemoryPackable]
public partial class CSRoomLeavePacket : PacketHeader
{
}

[MemoryPackable]
public partial class SCRoomLeavePacket : PacketHeader
{
    public short Result { get; set; }
}

[MemoryPackable]
public partial class PKTNtfRoomLeaveUser : PacketHeader
{
    public string UserID { get; set; }
}


[MemoryPackable]
public partial class PKTReqRoomChat : PacketHeader
{
    public string ChatMessage { get; set; }
}


[MemoryPackable]
public partial class PKTNtfRoomChat : PacketHeader
{
    public string UserID { get; set; }

    public string ChatMessage { get; set; }
}


