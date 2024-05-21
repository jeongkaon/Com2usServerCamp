using APIServer.Models;
using APIServer.Repository.Interfaces;
using APIServer.Services.Interface;
using System.Text.Json;
using ZLogger;
using static Humanizer.In;


namespace APIServer.Services;

public class AuthService : IAuthService
{
    readonly ILogger<AuthService> _logger;
    readonly IGameDB _gameDB;
    string _hiveServerAPIAddress;


    public AuthService(ILogger<AuthService> logger, IConfiguration configuration, IGameDB gameDb)
    {
       _hiveServerAPIAddress = configuration.GetSection("HiveServerAddress").Value + "/VerifyToken";

        _logger = logger;
        _gameDB = gameDb;
        
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
                _logger.ZLogError($"[AuthService] validate hive auth failed");
                return ErrorCode.FailHiveInvalidResponse;
            }

            return ErrorCode.None;
        }
        catch (HttpRequestException ex)
        {
            _logger.ZLogError($"[AuthService] {ex.Message}, fail VerifyTokenToHive");
            return ErrorCode.FailHiveInvalidResponse;

        }

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
