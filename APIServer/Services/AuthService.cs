using APIServer.Models.AccountDB;
using APIServer.Repository.Interfaces;
using APIServer.Services.Interface;
using static Humanizer.In;


namespace APIServer.Services;

public class AuthService : IAuthService
{
    readonly ILogger<AuthService> _logger;
    readonly IGameDB _gameDB;
    readonly IAccountDB _accountDB;
    string _hiveServerAPIAddress;


    public AuthService(ILogger<AuthService> logger, IConfiguration configuration, IGameDB gameDb, IAccountDB accountDb)
    {
        _gameDB = gameDb;
        _logger = logger;
        _hiveServerAPIAddress = configuration.GetSection("HiveServerAddress").Value + "/verifytoken";
        _accountDB = accountDb;
    }

    public async Task<ErrorCode> VerifyTokenToHive(string email, string token)
    {
        //하이브로 토큰을 보내서 확인을 한다.
        try
        {
            HttpClient client = new();
            var hiveResponse = await client.PostAsJsonAsync(_hiveServerAPIAddress, new { PlayerId = email, HiveToken = token });

            if (hiveResponse == null || !ValidateHiveResponse(hiveResponse))
            {
                return ErrorCode.FailVerifyToken;
            }

            var authResult = await hiveResponse.Content.ReadFromJsonAsync<ErrorCode>();
            if (!ValidateHiveAuthErrorCode(authResult))
            {
                return ErrorCode.FailHiveInvalidResponse;
            }

            return ErrorCode.None;
        }
        catch
        {
            //오류뜬거 zlog사용해서 해야한다.
        }

        return ErrorCode.None;
    }

    //유저정보 있는지 확인
    public async Task<ErrorCode> VerifyExistUser(string email)
    {
        var account = _accountDB.GetUserAccountByEmail(email);

        if (account == null)
        {
            return ErrorCode.NotExistAccount;
        }



        return ErrorCode.None;
    }

    public bool ValidateHiveResponse(HttpResponseMessage? response)
    {
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            return false;
        }
        return true;
    }


    bool ValidateHiveAuthErrorCode(ErrorCode authResult)
    {
        if (authResult != ErrorCode.None)
        {
            return false;
        }

        return true;
    }
}
