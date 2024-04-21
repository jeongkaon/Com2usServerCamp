using CloudStructures;
using CloudStructures.Structures;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Net;
using System.Threading.Tasks;
using ZLogger;


namespace HiveServer.Repository;
public class HiveRedis : IHiveRedis
{
    readonly ILogger<HiveAccountDB> _logger;
    
    public RedisConnection _redisCon;


    public HiveRedis(ILogger<HiveAccountDB> logger, IOptions<DbConfig> dbConfig)
    {
        _logger = logger;

        RedisConfig config = new("default", dbConfig.Value.Redis);
        _redisCon = new RedisConnection(config);

    }

    public async Task<ErrorCode> RegistUserAsync(string email, string authToken)
    {
        var idDefaultExpiry = TimeSpan.FromDays(1);
        var redisId = new RedisString<string>(_redisCon, email, idDefaultExpiry);
        await redisId.SetAsync(authToken);

        return ErrorCode.None;
    }
    public async Task<ErrorCode> VerifyUserToken(string email, string authToken)
    {
        var idDefaultExpiry = TimeSpan.FromDays(1);

        var redisId = new RedisString<string>(_redisCon, email, idDefaultExpiry);
        var res =  await redisId.GetAsync();
        if(res.Value != authToken)
        {
            return ErrorCode.FailVerifyUserToken;
        }

        return ErrorCode.None;
    }



    public void Dispose()
    {

    }

}
