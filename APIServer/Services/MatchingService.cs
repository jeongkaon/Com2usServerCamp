using APIServer.Models;
using APIServer.Services.Interface;
using static Humanizer.In;
using System.Text.Json;

namespace APIServer.Services;

public class MatchingService : IMatchingService
{
    readonly ILogger<AuthService> _logger;
    string _matchServerAddress;
    string _checkServerAddress;


    HttpClient client = new();

    public MatchingService(ILogger<AuthService> logger)
    {
        _logger = logger;
        _matchServerAddress = "http://localhost:11502" + "/RequestMatching ";
        _checkServerAddress = "http://localhost:11502" + "/CheckMatching ";


    }

    public async Task<ErrorCode> UserIdToMatchServer(string id)
    {
        //여기서 http로 요청보내야한다.
        var matchResponse = await client.PostAsJsonAsync(_matchServerAddress, new { UserID = id });

        //변수이름 바꿔야함
        var preHiveResResult = await matchResponse.Content.ReadAsStringAsync();
        var hiveResResult = JsonSerializer.Deserialize<MatchingResponse>(preHiveResResult);

        Console.WriteLine("매칭서버로 전송완료!");
     

        return ErrorCode.None;
    }

    //매칭서버에 결과 확인하는거 해야함
    public async Task<CheckMatchingResponse> CheckToMatchServer(string id)
    {
        var checkMatchRes = await client.PostAsJsonAsync(_checkServerAddress, new { UserID = id });
        //변수이름 바꿔야함

        var preHiveResResult = await checkMatchRes.Content.ReadAsStringAsync();
        var hiveResResult = JsonSerializer.Deserialize<CheckMatchingResponse>(preHiveResResult);

        if(hiveResResult == null ||hiveResResult.Result != ErrorCode.None)
        {
            Console.WriteLine("매칭서버에서 아직 매칭안됨!");

            return null;
        }
        //결과, 서버주소, 포트번호, 방번호 받아야한다.
        Console.WriteLine("매칭서버 완료!!!!");

        return hiveResResult;

    }

}
