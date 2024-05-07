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

public class MySQLConnectionExample
{
    static void Main(string[] args)
    {
        string connectionString = "Server=your_mysql_server_address;Port=your_mysql_port;Database=your_database_name;Uid=your_username;Pwd=your_password;";

        // MySQL 연결 생성
        MySqlConnection connection = new MySqlConnection(connectionString);

        try
        {
            // MySQL 서버에 연결
            connection.Open();

            // 연결이 열렸으면 작업 수행

            // 연결 닫기
            connection.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine("MySQL 연결 실패: " + ex.Message);
        }
        finally
        {
            // 연결이 열렸으면 반드시 닫기
            if (connection.State == System.Data.ConnectionState.Open)
                connection.Close();
        }
    }
}
