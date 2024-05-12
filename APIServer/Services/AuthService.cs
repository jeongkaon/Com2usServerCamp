using APIServer.Models;
using APIServer.Repository.Interfaces;
using APIServer.Services.Interface;
using System.Text.Json;
using static Humanizer.In;


namespace APIServer.Services;

public class AuthService : IAuthService
{
    readonly ILogger<AuthService> _logger;

    readonly IGameDB _gameDB;

    //여기어 그 하이브로 보낼 주소? 저장되어있음
    string _hiveServerAPIAddress;


    public AuthService(ILogger<AuthService> logger, IConfiguration configuration, IGameDB gameDb)
    {
       //물어볼 hive 서버 주소 
        //_hiveServerAPIAddress = configuration.GetSection("HiveServerAddress").Value + "/VerifyToken";
    
        _hiveServerAPIAddress = "http://localhost:11500" + "/VerifyToken";

        _gameDB = gameDb;
        _logger = logger;
        
    }

    public async Task<ErrorCode> VerifyTokenToHive(string id, string token)
    {
        //하이브로 토큰을 보내서 확인을 한다.
        try
        {
            HttpClient client = new();
            var hiveResponse = await client.PostAsJsonAsync(_hiveServerAPIAddress, new {Id = id, Token = token});

            //하이브에서 요청이 없거나 이상한경우.
            if (hiveResponse == null || false ==ValidateHiveResponse(hiveResponse))
            {
                return ErrorCode.FailVerifyToken;
            }

            var preHiveResResult = await hiveResponse.Content.ReadAsStringAsync();
            var hiveResResult = JsonSerializer.Deserialize<VerifyTokenReponse>(preHiveResResult);
            //직렬화 안하면 안됨!
            if (false==ValidateHiveAuthErrorCode(hiveResResult.Result))
            {
                return ErrorCode.FailHiveInvalidResponse;
            }

            return ErrorCode.None;
        }
        catch (HttpRequestException ex)
        {

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
