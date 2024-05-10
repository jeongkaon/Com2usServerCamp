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

public class DBProcessor
{
    int ThreadNum = 5;

    bool isThreadRunning = false;
    System.Threading.Thread[] GameDBThread = null;

    BufferBlock<MemoryPackBinaryRequestInfo> _msgBuffer = new BufferBlock<MemoryPackBinaryRequestInfo>();

    Dictionary<int, Action<QueryFactory, MemoryPackBinaryRequestInfo>> PacketHandlerMap = 
        new Dictionary<int, Action<QueryFactory, MemoryPackBinaryRequestInfo>>();

    const string connectionString =
    "Server=127.0.0.1;user=root;Password=0000;Database=game_db;Pooling=true;Min Pool Size=0;Max Pool Size=100;AllowUserVariables=True;";

    DBHandler MyDBHandler = new DBHandler();


    public void CreateAndStart()
    {
        MyDBHandler.RegistPacketHandler(PacketHandlerMap);

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
        //쓰레드별로 DB 커넥션을 가져야햔다.
        //GameDB _gameDb = new GameDB();
        //여기 따로 함수로 빼야한다.
        IDbConnection _dbConn = new MySqlConnection(connectionString);
        _dbConn.Open();
        MySqlCompiler _Compiler = new MySqlCompiler();
        QueryFactory _QueryFactory = new QueryFactory(_dbConn, _Compiler);


        while (isThreadRunning)
        {
            try
            {
                var packet = _msgBuffer.Receive();

                var header = new PacketHeadInfo();
                header.Read(packet.Data);

                if (PacketHandlerMap.ContainsKey(header.Id))
                {
                    //쿼리랑 패킷을 인자로 받는 함수를 만들면된다.
                    PacketHandlerMap[header.Id](_QueryFactory,packet);
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
