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
        "Server=127.0.0.1;com2us=kaon;Password=0000;Database=game_db;Pooling=true;Min Pool Size=0;Max Pool Size=100;AllowUserVariables=True;";

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
public class DBProcessor
{
    int ThreadNum = 5;

    bool isThreadRunning = false;
    System.Threading.Thread[] GameDBThread = null;

    BufferBlock<MemoryPackBinaryRequestInfo> _msgBuffer = new BufferBlock<MemoryPackBinaryRequestInfo>();

    Dictionary<int, Action<QueryFactory, MemoryPackBinaryRequestInfo>> DBHandlerMap = 
        new Dictionary<int, Action<QueryFactory, MemoryPackBinaryRequestInfo>>();

    DBHandler MyDBHandler = new DBHandler();


    public void CreateAndStart()
    {
        MyDBHandler.RegistPacketHandler(DBHandlerMap);

        GameDBThread = new System.Threading.Thread[ThreadNum];
        isThreadRunning = true;

        for (int i = 0; i < ThreadNum; ++i)
        {
            if (GameDBThread[i] == null)
            {
                GameDBThread[i] = new System.Threading.Thread(Process);
                GameDBThread[i].Start();
            }


        }

        
    }
    
    public void Destroy()
    {
        MainServer.MainLogger.Info("DBProcessor::Destory - begin");

        isThreadRunning = false;
        _msgBuffer.Complete();

        foreach(var th in GameDBThread)
        {
            th.Join();
        }

        MainServer.MainLogger.Info("DBProcessor::Destory - end");
    }

    public void InsertPacket(MemoryPackBinaryRequestInfo data)
    {
        _msgBuffer.Post(data);
    }

    public void Process()
    {
        GameDB gameDb = new GameDB();

        while (isThreadRunning)
        {
            try
            {
                var packet = _msgBuffer.Receive();
                var header = new PacketHeadInfo();
                header.Read(packet.Data);

                if (DBHandlerMap.ContainsKey(header.Id))
                {
                    DBHandlerMap[header.Id](gameDb.GetQueryFactory(), packet);
                }
            }
            catch (Exception ex)
            {
                if (isThreadRunning)
                {
                    MainServer.MainLogger.Error(ex.ToString());
                }
            }
        }
    }

}


