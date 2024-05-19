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
    readonly ILogger<HiveRedis> _logger;
    
    RedisConnection _redisCon;

    public HiveRedis(ILogger<HiveRedis> logger, IOptions<DbConfig> dbConfig)
    {
        _logger = logger;

        RedisConfig config = new("default", dbConfig.Value.Redis);
        _redisCon = new RedisConnection(config);

    }

    public async Task<ErrorCode> RegistUserAsync(string id, string authToken)
    {
        var idDefaultExpiry = TimeSpan.FromDays(1);
        try
        {
            var redisId = new RedisString<string>(_redisCon, id, idDefaultExpiry);
            await redisId.SetAsync(authToken);

            _logger.ZLogDebug($"[RegistUserAsync] success regist email {id} in redis");
            return ErrorCode.None;
        }
        catch (Exception ex)
        {
            _logger.ZLogError($"[RegistUserAsync] fail regist email {id} in redis");
            return ErrorCode.FailRegistUserInRedis;

        }

    }
    public async Task<ErrorCode> VerifyUserToken(string id, string authToken)
    {
        try
        {
            var idDefaultExpiry = TimeSpan.FromDays(1);
            var redisId = new RedisString<string>(_redisCon, id, idDefaultExpiry);
            var res = await redisId.GetAsync();
            if (res.Value != authToken)
            {
                _logger.ZLogInformation($"[VerifyUserToken] fail verify user[{id}] token");
                return ErrorCode.FailVerifyUserToken;
            }

            _logger.ZLogInformation($"[VerifyUserToken] success verify user[{id}] token");
            return ErrorCode.None;

        }
        catch
        {
            _logger.ZLogError($"[VerifyUserToken] fail verify user[{id}] token");
            return ErrorCode.FailVerifyUserToken;

        }

    }



    public void Dispose()
    {

    }

}
