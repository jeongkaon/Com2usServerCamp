﻿using CloudStructures;
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

        if(reqData.AuthToken == value)
        {
            Console.WriteLine("인증토근 완료!!!");
            PacketHeadInfo.WritePacketId(packetData.Data, (UInt16)PacketId.NtfInLoginCheck);
            DistributeInnerPacket(packetData);
        }
        else
        {
            Console.WriteLine("인증토근 인증 불가!!!!!");
            //없어도 보내야함.
        }
    }
    public string GeValue(RedisConnection redisConnection, string key)
    {
        return redisConnection.GetConnection().GetDatabase().StringGet(key).ToString();
    }
}
