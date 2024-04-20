using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Options;
using System.Data;
using MySqlConnector;
using SqlKata.Execution;
using Microsoft.Extensions.Logging;
using ZLogger;
using HiveServer.Model;
using System.Diagnostics;
using System.Security.Principal;

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
        //securty해서 해싱하고 해야되는데 일단 +string올 해보자 연결이 먼저임

        var count = await _qFactory.Query("account").InsertAsync(new
        {
            Email = email,
            SaltValue = password,
            HashedPassword = "HassehdPassworld@@@@@"
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
        //계정검증하는 코드
        //실제 존재하는 유저인지 검사 - sql에서 가져와야한다.

        //차후 try-catch로 바꿔야함 
        AdbUser userInfo = await _qFactory.Query("account")
            .Where("Email", email).FirstOrDefaultAsync<AdbUser>();

        //값 가져온거로 예외처리해야함 일단 넘어가~

        //테스트
        if("HassehdPassworld@@@@@" != userInfo.hashed_pw)
        {
            //안됨 돌아가
        }


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