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

    public void Dispose()
    {

    }
}
