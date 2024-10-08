﻿using CloudStructures;
using CloudStructures.Structures;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using StackExchange.Redis;

namespace PvPGameServer;

public class AccountDBProcessor
{
    int _threadNum = 2;
    bool _isThreadRunning = false;
    System.Threading.Thread[] _accountDBThread = null;

    public static SuperSocket.SocketBase.Logging.ILog _logger;

    BufferBlock<MemoryPackBinaryRequestInfo> _msgBuffer = new BufferBlock<MemoryPackBinaryRequestInfo>();

    Dictionary<int, Action<RedisConnection, MemoryPackBinaryRequestInfo>> _accountDBHandlerMap =
        new Dictionary<int, Action<RedisConnection, MemoryPackBinaryRequestInfo>>();

    AccountDBHander _accountDbHandler = new AccountDBHander();
    public void CreateAndStart()
    {
        _accountDbHandler.RegistPacketHandler(_accountDBHandlerMap);
        _accountDBThread = new System.Threading.Thread[_threadNum];
        _isThreadRunning = true;

        for (int i = 0; i < _threadNum; ++i)
        {
            if (_accountDBThread[i] == null)
            {
                _accountDBThread[i] = new System.Threading.Thread(Process);
                _accountDBThread[i].Start();
            }
        }
    }
 

    public void Destroy()
    {
        _logger.Info("AccountDBProcessor::Destory - begin");

        _isThreadRunning = false;
        _msgBuffer.Complete();

        foreach (var th in _accountDBThread)
        {
            th.Join();
        }

        _logger.Info("AccountDBProcessor::Destory - end");
    }
    public void InsertPacket(MemoryPackBinaryRequestInfo data)
    {
        _msgBuffer.Post(data);
    }
    public void Process()
    {
        RedisDB accountDb = new RedisDB();

        while (_isThreadRunning)
        {
            try
            {
                var packet = _msgBuffer.Receive();
                var header = new PacketHeadInfo();
                header.Read(packet.Data);

                if (_accountDBHandlerMap.ContainsKey(header.Id))
                {
                    _accountDBHandlerMap[header.Id](accountDb.GetRedisCon(), packet);
                }
            }
            catch (Exception ex)
            {
                if (_isThreadRunning)
                {
                    _logger.Error(ex.ToString());
                }
            }
        }
    }

}
