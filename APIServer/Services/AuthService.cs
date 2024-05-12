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
        try
        {
            HttpClient client = new();
            var hiveResponse = await client.PostAsJsonAsync(_hiveServerAPIAddress, new {Id = id, Token = token});

            if (hiveResponse == null || false ==ValidateHiveResponse(hiveResponse))
            {
                return ErrorCode.FailVerifyToken;
            }

            var preHiveResResult = await hiveResponse.Content.ReadAsStringAsync();
            var hiveResResult = JsonSerializer.Deserialize<VerifyTokenReponse>(preHiveResResult);

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
