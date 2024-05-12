using CloudStructures;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PvPGameServer;

public class AccountDBHander
{
    public void RegistPacketHandler(Dictionary<int, Action<RedisConnection, MemoryPackBinaryRequestInfo>> packetHandlerMap)
    {
        //핸들에 등록해야함
        //ReqLoginPacketRedis->이름 바꿔야한다.
        packetHandlerMap.Add(1002333,ReqLoginPacketRedis);
    }

    public string ReqLoginPacketRedis(RedisConnection redisConnection, MemoryPackBinaryRequestInfo packetData)
    {
        var headerSize = PacketHeadInfo.HeaderSize;
        var dataLen = packetData.Data.Length;

        return FastBinaryRead.String(packetData.Data, headerSize, dataLen - headerSize);

    }


}

public class AccoutnDBInfo
{
    //써야함
}