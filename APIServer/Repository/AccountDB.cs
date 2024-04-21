using APIServer.Repository.Interfaces;
using Microsoft.Extensions.Options;
using MySqlConnector;
using SqlKata.Execution;
using System.Data;

namespace APIServer.Repository;

public class AccountDB : IAccountDB
{
    readonly IOptions<DbConfig> _dbConfig;
    readonly ILogger<AccountDB> _logger;
    IDbConnection _dbCon;
    SqlKata.Compilers.MySqlCompiler _compiler;
    QueryFactory _qFactory;

    public AccountDB(ILogger<AccountDB> logger, IOptions<DbConfig> dbConfig)
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
        _dbCon = new MySqlConnection(_dbConfig.Value.AccountDB);
        _dbCon.Open();

    }
    void Close()
    {
        _dbCon.Close();
    }

}
