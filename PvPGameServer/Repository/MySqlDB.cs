using MySqlConnector;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PvPGameServer.Repository;

public class MySqlDB
{
    IDbConnection MySqlCon;

    SqlKata.Compilers.MySqlCompiler Compiler;
    QueryFactory QueryFactory;

    const string connectionString = "Server=127.0.0.1;user=root;Password=0000;Database=account_db;Pooling=true;Min Pool Size=0;Max Pool Size=40;AllowUserVariables=True;";

    //이름을 어떻게 정하지..
    public void SetMySql()
    {
        MySqlCon = new MySqlConnection(connectionString);
    }
    
}

