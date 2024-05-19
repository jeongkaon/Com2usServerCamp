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
using HiveServer.Model;
using Microsoft.AspNetCore.Identity;


namespace HiveServer.Repository;

public class HiveAccountDB : IHiveAccountDB
{

    readonly IOptions<DbConfig> _dbConfig;
    readonly ILogger<HiveAccountDB> _logger;

    IDbConnection _dbCon;
    SqlKata.Compilers.MySqlCompiler _compiler;
    QueryFactory _qFactory;


    public HiveAccountDB(ILogger<HiveAccountDB> logger,IOptions<DbConfig> dbConfig)
    {

        _dbConfig = dbConfig;
        _logger = logger;

        Open();

        _compiler = new SqlKata.Compilers.MySqlCompiler();
        _qFactory = new SqlKata.Execution.QueryFactory(_dbCon, _compiler);

    }

    public async Task<ErrorCode> CreateAccountAsync(string id, string password)
    {

        try
        {
            var (salt, hashed) = Security.GenerateHashValue(password);

            var count = await _qFactory.Query("account").InsertAsync(new
            {
                id = id,
                SaltValue = salt,
                HashedPassword = hashed
            });

            if (1 != count)
            {
                _logger.ZLogDebug($"[CreateAccount] email: {id} Failid");
                return ErrorCode.FailCreateAccount;
            }

            _logger.ZLogDebug($"[CreateAccount] email: {id}, salt_value : {salt}, hashed_pw:{hashed}");
            return ErrorCode.None;

        }
        catch 
        {
            _logger.ZLogError($"[CreateAccount] ErrorCode: {ErrorCode.FailCreateAccount}");
            return ErrorCode.FailCreateAccount;
        }

    }

    public async Task<Tuple<ErrorCode, string>> VerifyUserAccount(string id, string password)
    {
        UserInfoAccountDB userInfo = await _qFactory.Query("account")
            .Where("id", id).FirstOrDefaultAsync<UserInfoAccountDB>();

        if(userInfo.id== null)
        {
            return new Tuple<ErrorCode, string>(ErrorCode.FailVerifyUserNoid, id);
        }

        if (false == Security.VerifyPassword(password, userInfo.saltvalue, userInfo.hashedpassword))
        {
            return new Tuple<ErrorCode, string>(ErrorCode.FailVerifyUserNotPassword, id);
        }
        Console.WriteLine("verifyuserAccount suc!!");

        return new Tuple<ErrorCode, string>(ErrorCode.None, id);

    }
    public void Dispose()
    {
        Close();
    }
    void Open()
    {
        _dbCon = new MySqlConnection(_dbConfig.Value.HiveDB); 
        _dbCon.Open();

    }
    void Close()
    {
        _dbCon.Close();
    }

}

public class DbConfig
{
    public string? HiveDB { get; set; }

    public string? Redis { get; set; }


}