using Microsoft.Extensions.Options;
using MySqlConnector;
using NLog.Config;
using SqlKata.Compilers;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PvPGameServer;

public class GameDB
{

    IDbConnection dbConn;
    MySqlCompiler Compiler;
    QueryFactory QueryFactory;

    const string connectionString =
        "Server=127.0.0.1;user=root;Password=0000;Database=game_db;Pooling=true;Min Pool Size=0;Max Pool Size=100;AllowUserVariables=True;";
    public GameDB()
    {
        Open();

        Compiler = new MySqlCompiler();
        QueryFactory = new QueryFactory(dbConn, Compiler);

    }
    void Open()
    {
        dbConn = new MySqlConnection(connectionString);
        dbConn.Open();

    }

    public void UpdateWinScore(string id)
    {
        var query = QueryFactory.Query("gamedata").Where("id", id).FirstOrDefault<GameDBInfo>();
        var update = query.win_score + 1;

        var count = QueryFactory.Query("gamedata").Where("id", id).Update(new { win_score = update });
    }

    public void UpdateLoseScore(string id)
    {
        var query = QueryFactory.Query("gamedata").Where("id", id).FirstOrDefault<GameDBInfo>();
        var update = query.lose_score + 1;

        var count = QueryFactory.Query("gamedata").Where("id", id).Update(new { lose_score = update });
    }
    public void UpdateTieScore(string id)
    {
        var query = QueryFactory.Query("gamedata").Where("id", id).FirstOrDefault<GameDBInfo>();
        var update = query.tie_score + 1;

        var count = QueryFactory.Query("gamedata").Where("id", id).Update(new { tie_score = update });
    }


    void close()
    {
        dbConn.Close();
    }


}


public class GameDBInfo
{
    public string id { get; set; }
    public int exp { get; set; }
    public int win_score { get; set; }
    public int lose_score { get; set; }
    public int tie_score { get; set; }

}