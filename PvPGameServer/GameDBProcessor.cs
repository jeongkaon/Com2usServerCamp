using MySqlConnector;
using SqlKata.Compilers;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace PvPGameServer;

public class GameDB
{
    IDbConnection _dbConn;
    MySqlCompiler _compiler;
    QueryFactory _queryFactory;


    const string connectionString =
        "Server=127.0.0.1;user=kaon;Password=0000;Database=game_db;Pooling=true;Min Pool Size=0;Max Pool Size=100;AllowUserVariables=True;";
    public GameDB()
    {
        _dbConn = new MySqlConnection(connectionString);
        _dbConn.Open();
        _compiler = new MySqlCompiler();
        _queryFactory = new QueryFactory(_dbConn, _compiler);
    }
    
    public QueryFactory GetQueryFactory()
    {
        return _queryFactory;
    }
}
public class GameDBProcessor
{
    int _threadNum = 5;

    bool _isThreadRunning = false;
    System.Threading.Thread[] _gameDBThread = null;

    public static SuperSocket.SocketBase.Logging.ILog _logger;

    BufferBlock<MemoryPackBinaryRequestInfo> _msgBuffer = new BufferBlock<MemoryPackBinaryRequestInfo>();

    Dictionary<int, Action<QueryFactory, MemoryPackBinaryRequestInfo>> _dBHandlerMap = 
        new Dictionary<int, Action<QueryFactory, MemoryPackBinaryRequestInfo>>();

    GameDBHandler _myDBHandler = new GameDBHandler();


    public void CreateAndStart()
    {
        _myDBHandler.RegistPacketHandler(_dBHandlerMap);

        _gameDBThread = new System.Threading.Thread[_threadNum];
        _isThreadRunning = true;

        for (int i = 0; i < _threadNum; ++i)
        {
            if (_gameDBThread[i] == null)
            {
                _gameDBThread[i] = new System.Threading.Thread(Process);
                _gameDBThread[i].Start();
            }
        }
    }

    public void Destroy()
    {
        _logger.Info("DBProcessor::Destory - begin");

        _isThreadRunning = false;
        _msgBuffer.Complete();

        foreach(var th in _gameDBThread)
        {
            th.Join();
        }

        _logger.Info("DBProcessor::Destory - end");
    }

    public void InsertPacket(MemoryPackBinaryRequestInfo data)
    {
        _msgBuffer.Post(data);
    }

    public void Process()
    {
        GameDB gameDb = new GameDB();

        while (_isThreadRunning)
        {
            try
            {
                var packet = _msgBuffer.Receive();
                var header = new PacketHeadInfo();
                header.Read(packet.Data);

                if (_dBHandlerMap.ContainsKey(header.Id))
                {
                    _dBHandlerMap[header.Id](gameDb.GetQueryFactory(), packet);
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


