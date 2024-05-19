using APIServer.Models;
using APIServer.Services.Interface;
using static Humanizer.In;
using System.Text.Json;
using ZLogger;

namespace APIServer.Services;

public class MatchingService : IMatchingService
{
    readonly ILogger<AuthService> _logger;
    string _matchServerAddress;
    string _checkServerAddress;


    HttpClient client = new();

    public MatchingService(ILogger<AuthService> logger, IConfiguration configuration)
    {
        _logger = logger;
        var ip = configuration.GetSection("MatchingServerAddress").Value;
        _matchServerAddress = ip + "/RequestMatching ";
        _checkServerAddress = ip + "/CheckMatching ";

        //_matchServerAddress = "http://localhost:11502" + "/RequestMatching ";
        //_checkServerAddress = "http://localhost:11502" + "/CheckMatching ";
    }

    public async Task<ErrorCode> UserIdToMatchServer(string id)
    {
        try
        {
            var matchResponse = await client.PostAsJsonAsync(_matchServerAddress, new { UserID = id });
            var preResResult = await matchResponse.Content.ReadAsStringAsync();
            var resResult = JsonSerializer.Deserialize<MatchingResponse>(preResResult);
            if(resResult.Result != ErrorCode.None)
            {
                return ErrorCode.FailUserIdToMatchServer;
            }
            _logger.ZLogInformation($"[MatchingService] success send userid to matching server");

            return ErrorCode.None;

        }
        catch (HttpRequestException ex)
        {
            _logger.ZLogError($"[MatchingService] {ex}, fail send userid to matching server");
            return ErrorCode.FailUserIdToMatchServer;
        }


    }

    public async Task<string> CheckToMatchServer(string id)
    {
        try
        {
            var checkMatchRes = await client.PostAsJsonAsync(_checkServerAddress, new { UserID = id });
            var json = await checkMatchRes.Content.ReadAsStringAsync();
  
            if (json == null)
            {
                return null;
            }
            _logger.ZLogInformation($"[MatchingService] success matching : {json}");
            return json;
        }
        catch
        {
            _logger.ZLogError($"[MatchingService] faile Check To Match Server ");
            return null;
        }

    }

}
