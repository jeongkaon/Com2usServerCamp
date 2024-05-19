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
        var idDefaultExpiry = TimeSpan.FromDays(1);

        var redisId = new RedisString<string>(_redisCon, id, idDefaultExpiry);
        var res = await redisId.GetAsync();
        if(res.HasValue == false)
        {
            return ErrorCode.NotExistRedis;
        }

        if (res.Value != authToken)
        {
            return ErrorCode.FailVerifyToken;
        }

        return ErrorCode.None;
    }

    public async Task<ErrorCode> RegistUserAsync(string id, string authToken)
    {
        var idDefaultExpiry = TimeSpan.FromDays(1);
        var redisId = new RedisString<string>(_redisCon, id, idDefaultExpiry);
        var res = await redisId.SetAsync(authToken);

        if(res == false)
        {
            return ErrorCode.FailSetRedisUserToken;
        }

        return ErrorCode.None;
    }

    public void Dispose()
    {

    }

}
