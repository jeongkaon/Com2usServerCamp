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

namespace HiveServer.Repository;

public class HiveAccountDB : IHiveAccountDB
{
    //로그랑 그런거 일단 나중에하기

    readonly IOptions<DbConfig> _dbConfig;


    //private필드니까 _사용함
    IDbConnection _dbCon;
    SqlKata.Compilers.MySqlCompiler _compiler;
    //실제 쿼리를 만들어주는 애
    QueryFactory _qFactory;


    public HiveAccountDB(IOptions<DbConfig> dbConfig)
    {

        _dbConfig = dbConfig;

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
            Eamil = email,
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



    public void Dispose()
    {

    }



    void Open()
    {
        //db를 열어보자.

        _dbCon = new MySqlConnection(_dbConfig.Value.HiveDB); //여기 안에 환경변수를 넣어줘야한다. 
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




}