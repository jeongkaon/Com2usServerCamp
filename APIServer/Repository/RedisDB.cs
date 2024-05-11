using CloudStructures;
using CloudStructures.Structures;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Net;
using System.Threading.Tasks;
using ZLogger;
using APIServer.Repository.Interfaces;
using System.Diagnostics;
using System.Runtime.InteropServices;



namespace APIServer.Repository;
public class RedisDB : IRedisDB
{
    readonly ILogger<RedisDB> _logger;
    public RedisConnection _redisCon;

    public RedisDB(ILogger<RedisDB> logger, IOptions<DbConfig> dbConfig)
    {
        _logger = logger;
        RedisConfig config = new("defult", dbConfig.Value.Redis);
        _redisCon = new RedisConnection(config);
    }

    //유저토큰검증
    public async Task<ErrorCode> VerifyUserToken(string id, string authToken)
    {
        //레디스에 유저랑 토큰이 저장되어있는지 확인
        var idDefaultExpiry = TimeSpan.FromDays(1);

        var redisId = new RedisString<string>(_redisCon, id, idDefaultExpiry);
        var res = await redisId.GetAsync();
        if (res.Value != authToken)
        {
            return ErrorCode.FailVerifyUserToken;
        }


        return ErrorCode.None;

    }

    public async Task<ErrorCode> RegistUserAsync(string id, string authToken)
    {
        //토큰만료+하이브에서 확인된경우
        //유저정보등록 id : toekn 저장하기

        var idDefaultExpiry = TimeSpan.FromDays(1);
        var redisId = new RedisString<string>(_redisCon, id, idDefaultExpiry);
        await redisId.SetAsync(authToken);

        return ErrorCode.None;

    }



    public void Dispose()
    {

    }

}
