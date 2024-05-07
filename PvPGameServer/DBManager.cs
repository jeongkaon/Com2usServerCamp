using CloudStructures;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySqlConnector;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PvPGameServer;

public class DBManager
{
    //mysql
    //readonly IOptions<DbConfig> _dbConfig;

    IDbConnection MySqlCon;
    SqlKata.Compilers.MySqlCompiler Compiler;
    QueryFactory QueryFactory;


    //redis
    public RedisConnection RedisCon;



    public void SetMySqlConnect()
    {
    }

    public void SetDatabases()
    {
        string connectionString = "Server=127.0.0.1;user=root;Password=0000;Database=account_db;Pooling=true;Min Pool Size=0;Max Pool Size=40;AllowUserVariables=True;";
        MySqlCon = new MySqlConnection(connectionString);

        RedisConfig config = new("default", "127.0.0.1:6379");
        RedisCon = new RedisConnection(config);



    }

    //데이터베이스랑 레디스랑 쪼개야하나?

}
//얘를 하나 만들어야할듯?
  //"DbConfig": {
  //  "HiveDB": "Server=127.0.0.1;user=root;Password=0000;Database=account_db;Pooling=true;Min Pool Size=0;Max Pool Size=40;AllowUserVariables=True;",
  //  "Redis": "127.0.0.1:6379"
  //}