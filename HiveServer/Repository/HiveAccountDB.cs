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

    public async Task<ErrorCode> CreateAccountAsync(string email, string password)
    {

        //이따가 try-catch문으로 바꿔보자.

        var (salt, hashed) = PasswordCheck.GenerateHashValue(password);

        var count = await _qFactory.Query("account").InsertAsync(new
        {
            Email = email,
            SaltValue = salt,
            HashedPassword = hashed
        });

        if(count== 1)
        {
            Console.WriteLine("Insert Data in DataBase SucceSSS11");
            
        }
        else
        {
            Console.WriteLine("Insert Data in DataBase FAILE!!!!!!");

        }


        return ErrorCode.None;

    }

    public async Task<Tuple<ErrorCode, string>> VerifyUserAccount(string email, string password)
    {

        Console.WriteLine("대답....");

        //차후 try-catch로 바꿔야함 

        UserInfoAccountDB userInfo = await _qFactory.Query("account")
            .Where("Email", email).FirstOrDefaultAsync<UserInfoAccountDB>();

        if(userInfo.email== null)
        {
            //없는 user 돌아가...
        }

        //불일치 돌아가~
     //   if (false ==PasswordCheck.VerifyPassword(password, userInfo.salt_value, userInfo.hashed_pw))
       // {
    //        Console.WriteLine("verifyuserAccount fail!!");
//            return new Tuple<ErrorCode, string>(ErrorCode.FailVerifyUserAccount, email);

     //   }
        Console.WriteLine("verifyuserAccount suc!!");

        return new Tuple<ErrorCode, string>(ErrorCode.None, email);



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