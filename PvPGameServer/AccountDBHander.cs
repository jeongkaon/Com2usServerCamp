using CloudStructures;
using CloudStructures.Structures;
using MemoryPack;
using SqlKata.Execution;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PvPGameServer;

public class AccountDBHander : PacketHandler
{
    public void RegistPacketHandler(Dictionary<int, Action<RedisConnection, MemoryPackBinaryRequestInfo>> packetHandlerMap)
    {
        packetHandlerMap.Add((int)PacketId.ReqLogin, ReqLoginPacketRedis);
    }

    public void ReqLoginPacketRedis(RedisConnection redisConnection, MemoryPackBinaryRequestInfo packetData)
    {
        var reqData = MemoryPackSerializer.Deserialize<ReqLoginPacket>(packetData.Data);
        var id = reqData.UserID;

        var value = GeValue(redisConnection, id);

        var idDefaultExpiry = TimeSpan.FromDays(1);
        var redisId = new RedisString<string>(redisConnection, id, idDefaultExpiry);
        var res = redisId.SetAsync(id).Result;
        

        if (reqData.AuthToken == value)
        {
            PacketHeadInfo.WritePacketId(packetData.Data, (UInt16)PacketId.NtfInLoginCheck);
        }
        else
        {
            PacketHeadInfo.WritePacketId(packetData.Data, (UInt16)PacketId.NtfInLoginFailedAuthToken);
        }

        DistributeInnerPacket(packetData);
    }
    public string GeValue(RedisConnection redisConnection, string key)
    {
        return redisConnection.GetConnection().GetDatabase().StringGet(key).ToString();
    }
}
