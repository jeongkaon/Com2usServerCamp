using APIServer.Controllers;
using APIServer.Model;
using CloudStructures;
using CloudStructures.Structures;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;
using System.Threading;
using ZLogger;

namespace APIServer;

public interface IMatchWoker : IDisposable
{
    public void AddUser(string userID);

    public (bool, CompleteMatchingData) GetCompleteMatching(string userID);
}

public class MatchWoker : IMatchWoker
{
    readonly ILogger<MatchWoker> _logger;

    List<string> _pvpServerAddressList = new();

    System.Threading.Thread _reqWorker = null;      
    System.Threading.Thread _completeWorker = null;     
    
    ConcurrentQueue<string> _reqQueue = new();
    ConcurrentDictionary<string, CompleteMatchingData> _completeDic = new();

    RedisConnection _redisCon;
    string _matchReqRedisName = "match_request";
    string _matchResRedisName = "match_response";

    public MatchWoker(ILogger<MatchWoker> logger, IOptions<MatchingConfig> matchConfig)
    {
        _logger = logger;

        RedisConfig config = new("default", matchConfig.Value.RedisAddress);
        _redisCon = new RedisConnection(config);

        _reqWorker = new System.Threading.Thread(this.RunMatching);
        _reqWorker.Start();

        _completeWorker = new System.Threading.Thread(this.RunMatchingComplete);
        _completeWorker.Start();

    }
 
    public void AddUser(string userID)
    {
        if (_reqQueue.Contains(userID) == false)
        {
            _logger.ZLogInformation($"Add [{userID}] in reqQueue");
            _reqQueue.Enqueue(userID);
        }
    }

    public (bool, CompleteMatchingData) GetCompleteMatching(string userID)
    {
        var res = _completeDic.Remove(userID, out CompleteMatchingData value);
        if (res == false)
        {
            return (false, null);
        }
        return (res, value);
    }

    void RunMatching()
    {
        var idDefaultExpiry = TimeSpan.FromDays(1);
        var redisList = new RedisList<string>(_redisCon, _matchReqRedisName, idDefaultExpiry);
        while (true)
        {
            try
            {
                if (_reqQueue.Count < 2)
                {
                    System.Threading.Thread.Sleep(1);
                    continue;
                }

                if (_reqQueue.TryDequeue(out string player1) && _reqQueue.TryDequeue(out string player2))
                {
                    string matchResult = $"{player1},{player2}";

                    var temp = redisList.LeftPushAsync(matchResult).Result; 
                    if (temp == 0)
                    {
                        _logger.ZLogInformation($"Fail Push In Redis");

                    }
                }

            }
            catch (Exception ex)
            {
                _logger.ZLogError($"{ex}");
            }
        }
    }

    void RunMatchingComplete()      
    {
        var idDefaultExpiry = TimeSpan.FromDays(1);
        var redisList = new RedisList<string>(_redisCon, _matchResRedisName, idDefaultExpiry);

        while (true)
        {
            try
            {
                var temp = redisList.RightPopAsync().Result;

                var deserializedData = JsonSerializer.Deserialize<CompleteMatchingData>(temp.Value);

                _completeDic.TryAdd(deserializedData.User1, deserializedData);
                _completeDic.TryAdd(deserializedData.User2, deserializedData);

                _logger.ZLogInformation($"Match complete: {deserializedData.User1} vs {deserializedData.User2} in Room {deserializedData.RoomNumber}");
            }
            catch (Exception ex)
            {
                _logger.ZLogError($"{ex}");
            }
        }        
    }



    public void Dispose()
    {
        _logger.ZLogInformation($"MatchWoker 소멸자 호출");
    }
}






