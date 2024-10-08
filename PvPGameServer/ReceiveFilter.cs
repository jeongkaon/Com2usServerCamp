﻿using SuperSocket.Common;
using SuperSocket.SocketBase.Protocol;
using SuperSocket.SocketEngine.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PvPGameServer;
public class MemoryPackBinaryRequestInfo : BinaryRequestInfo
{
    public string SessionID;
    public byte[] Data;

    public const int PACKET_HEADER_MEMORYPACK_START_POS = 1;
    public const int HEADERE_SIZE = 5 + PACKET_HEADER_MEMORYPACK_START_POS;

    public MemoryPackBinaryRequestInfo(byte[] packetData)
        : base(null, packetData)
    {
        Data = packetData;
    }

}

public class ReceiveFilter : FixedHeaderReceiveFilter<MemoryPackBinaryRequestInfo>
{
    public ReceiveFilter() : base(MemoryPackBinaryRequestInfo.HEADERE_SIZE)
    {
    }

    protected override int GetBodyLengthFromHeader(byte[] header, int offset, int length)
    {
        if (!BitConverter.IsLittleEndian)
        {
            Array.Reverse(header, offset, 2);
        }

        var totalSize = BitConverter.ToUInt16(header, offset + MemoryPackBinaryRequestInfo.PACKET_HEADER_MEMORYPACK_START_POS);
        return totalSize - MemoryPackBinaryRequestInfo.HEADERE_SIZE;
    } 

    protected override MemoryPackBinaryRequestInfo ResolveRequestInfo(ArraySegment<byte> header, byte[] readBuffer, int offset, int length)
    {
        if (length > 0)
        {
            if (offset >= MemoryPackBinaryRequestInfo.HEADERE_SIZE)
            {
                var packetStartPos = offset - MemoryPackBinaryRequestInfo.HEADERE_SIZE;
                var packetSize = length + MemoryPackBinaryRequestInfo.HEADERE_SIZE;

                return new MemoryPackBinaryRequestInfo(readBuffer.CloneRange(packetStartPos, packetSize));
            }
            else
            {
                var packetData = new Byte[length + MemoryPackBinaryRequestInfo.HEADERE_SIZE];
                header.CopyTo(packetData, 0);
                Array.Copy(readBuffer, offset, packetData, MemoryPackBinaryRequestInfo.HEADERE_SIZE, length);

                return new MemoryPackBinaryRequestInfo(packetData);
            }
        }

        return new MemoryPackBinaryRequestInfo(header.CloneRange(header.Offset, header.Count));
    }


}
