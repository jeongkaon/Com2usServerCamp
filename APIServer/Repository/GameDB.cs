using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Options;
using System.Data;
using MySqlConnector;
using SqlKata.Execution;
using Microsoft.Extensions.Logging;
using ZLogger;
using System.Diagnostics;
using System.Security.Principal;
using APIServer.Models;
using Microsoft.AspNetCore.Identity;
using APIServer.Repository.Interfaces;

namespace APIServer.Repository;

public class GameDB : IGameDB
{
    readonly IOptions<DbConfig> _dbConfig;
    readonly ILogger<GameDB> _logger;
    IDbConnection _dbCon;
    SqlKata.Compilers.MySqlCompiler _compiler;
    QueryFactory _qFactory;


    public GameDB(ILogger<GameDB> logger, IOptions<DbConfig> dbConfig)
    {

        _dbConfig = dbConfig;
        _logger = logger;

        Open();

        _compiler = new SqlKata.Compilers.MySqlCompiler();
        _qFactory = new QueryFactory(_dbCon, _compiler);

    }


    public void Dispose()
    {
        Close();
    }



    void Open()
    {
        _dbCon = new MySqlConnection(_dbConfig.Value.GameDB);
        _dbCon.Open();

    }
    void Close()
    {
        _dbCon.Close();
    }
}
