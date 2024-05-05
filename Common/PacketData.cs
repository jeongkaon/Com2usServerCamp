using MemoryPack;


public struct PacketHeadInfo       
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

//Req -> 클라에서 서버로 요청한거
//Res -> 서버에서 클라로 응답한거
//Nft -> 요청없는데 클라에게 알려주는거
[MemoryPackable]
public partial class ReqLoginPacket : PacketHeader
{
    public string UserID { get; set; }
    public string AuthToken { get; set; }
}

[MemoryPackable]
public partial class ResLoginPacket : PacketHeader
{
    public short Result { get; set; }
}



[MemoryPackable]
public partial class NtfMustClosePacket : PacketHeader
{
    public short Result { get; set; }
}



[MemoryPackable]
public partial class ReqRoomEnterPacket : PacketHeader
{
    public int RoomNumber { get; set; }
}

[MemoryPackable]
public partial class ResRoomEnterPacket : PacketHeader
{
    public short Result { get; set; }
}

[MemoryPackable]
public partial class NtfRoomUserList : PacketHeader
{
    public List<string> UserIDList { get; set; } = new List<string>();
}

[MemoryPackable]
public partial class NtfRoomNewUser : PacketHeader
{
    public string UserID { get; set; }
}


[MemoryPackable]
public partial class ReqRoomLeavePacket : PacketHeader
{
    //public string UserID { get; set; }
}

[MemoryPackable]
public partial class ResRoomLeavePacket : PacketHeader
{
    public short Result { get; set; }
}

[MemoryPackable]
public partial class NtfRoomLeaveUser : PacketHeader
{
    public string UserID { get; set; }
}


[MemoryPackable]
public partial class ReqRoomChat : PacketHeader
{
    public string ChatMessage { get; set; }
}


[MemoryPackable]
public partial class NtfRoomChat : PacketHeader
{
    public string UserID { get; set; }

    public string ChatMessage { get; set; }
}


//게임관련패킷

[MemoryPackable]
public partial class ReqGameReadyPacket : PacketHeader
{
    public int RoomNumber { get; set; }

}

[MemoryPackable]
public partial class ResGameReadyPacket : PacketHeader
{
    public STONE_TYPE PlayerStoneType { get; set; }

}

[MemoryPackable]
public partial class NftGameStartPacket : PacketHeader
{
   public string p1 { get;  set; }
   public string p2 { get;  set; }


}


[MemoryPackable]
public partial class ReqPutOMok : PacketHeader
{
    public STONE_TYPE mok { get; set; }
    public int PosX { get; set; }
    public int PosY { get; set; }

}
[MemoryPackable]
public partial class ResPutOMok : PacketHeader
{
    public short Result { get; set; }

}

[MemoryPackable]
public partial class NftPutOmok : PacketHeader
{

    public STONE_TYPE mok { get; set; }
    public int PosX { get; set; }
    public int PosY { get; set; }
}

[MemoryPackable]
public partial class NtfTimeOut : PacketHeader
{
    public STONE_TYPE Usertype { get; set; }

}

[MemoryPackable]
public partial class NtfOmokWinner : PacketHeader
{
    public string UserId { get; set; }

}

[MemoryPackable]
public partial class PvPMatchingResult : PacketHeader
{
    public string IP;
    public UInt16 Port;
    public Int32 RoomNumber;
    public Int32 Index;
    public string Token;
}


//하트비트
[MemoryPackable]
public partial class ReqHeartBeatPacket : PacketHeader
{
}

[MemoryPackable]
public partial class ResHeartBeatPacket : PacketHeader
{
    public short Result { get; set; }

}
