﻿using CloudStructures.Structures;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PvPGameServer;

public class MatchingProcessor
{
    bool isThreadRunning = false;
    System.Threading.Thread _matchingThread = null;
    RedisDB _matchRedis = new RedisDB(0);

    static public SuperSocket.SocketBase.Logging.ILog _logger;
  

    RoomManager _roomMgr = null;

    string _matchReqRedisName = "match_request";
    string _matchResRedisName = "match_response";

    string _ip = null;      
    string _port = null;

    public void ExternalSetIpAddress()
    {
        HttpClient client = new HttpClient();
        try{
            string metadataUrl = "http://metadata.google.internal/computeMetadata/v1/instance/network-interfaces/0/access-configs/0/external-ip";
            client.DefaultRequestHeaders.Add("Metadata-Flavor", "Google");

            // 외부 IP 주소 가져오기
            string externalIp = client.GetStringAsync(metadataUrl).Result;
            _ip = externalIp;
            _logger.Info($"외부 IP 주소: {externalIp}");
        }
        catch (HttpRequestException e)
        {
            _logger.Info($"요청 오류: {e.Message}");
        }
    }
    public void InternalSetIpAddress()
    {
        string hostName = Dns.GetHostName();
        IPAddress[] addresses = Dns.GetHostAddresses(hostName);

        foreach (IPAddress address in addresses)
        {
            if (address.AddressFamily == AddressFamily.InterNetwork)
            {
                _ip = address.ToString();
                _logger.Debug($"내부 IP 주소: {address}");
                return;
            }
        }

    }

    public void CreateAndStart(RoomManager roomMgr, PvPServerOption serverOption)
    {
        //InternalSetIpAddress();
        _roomMgr = roomMgr;
        _port = serverOption.Port.ToString();

        isThreadRunning = true;
        _matchingThread = new System.Threading.Thread(Process);
        _matchingThread.Start();

    }
    
    public void Destory()
    {
        _matchingThread.Join();
    }

    public void Process()
    {
        var idDefaultExpiry = TimeSpan.FromDays(1);
        var reqRedisList = new RedisList<string>(_matchRedis.GetRedisCon(), _matchReqRedisName, idDefaultExpiry);
        var resRedisList = new RedisList<string>(_matchRedis.GetRedisCon(), _matchResRedisName, idDefaultExpiry);

        while (isThreadRunning)
        {
            if (_roomMgr.IsEmptyRoomList())
            {
                continue;
            }
            var temp = reqRedisList.RightPopAsync().Result;
            if (temp.HasValue == false)
            {
                continue;
            }

            string[] players = temp.Value.Split(',');
            var roomNumer = _roomMgr.DequeEmptyRoomList();

            var matchingData = new CompleteMatchingData()
            {
                User1 = players[0],
                User2 = players[1],
                ServerAddress = _ip,
                Port = _port,
                RoomNumber = roomNumer
            };

            string json = JsonSerializer.Serialize(matchingData);
            _logger.Debug($"직렬화된 JSON: {json}");


            var res = resRedisList.LeftPushAsync(json).Result;  
        }
    }
}


public class CompleteMatchingData
{
    public string User1 { get; set; }
    public string User2 { get; set; }
    public string ServerAddress { get; set; }
    public string Port { get; set; }
    public int RoomNumber { get; set; }
}