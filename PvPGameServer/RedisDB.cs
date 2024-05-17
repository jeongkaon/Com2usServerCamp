using CloudStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PvPGameServer;
public class RedisDB
{
    RedisConnection _redisCon;
    const string ConnectionString = "127.0.0.1:6379";
    public RedisDB()
    {
        RedisConfig config = new("default", ConnectionString);
        _redisCon = new RedisConnection(config);
    }
    public RedisConnection GetRedisCon()
    {
        return _redisCon;
    }
}

