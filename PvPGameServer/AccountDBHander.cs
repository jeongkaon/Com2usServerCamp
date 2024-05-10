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
        //packetHandlerMap.Add((int)PacketId.NtfInUpdateWinnerResult, UpdateWinnerScoreInDB);
    }

}

public class AccoutnDBInfo
{
    //써야함
}