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
    public async Task<string> CheckToMatchServer(string id)
    {
        var checkMatchRes = await client.PostAsJsonAsync(_checkServerAddress, new { UserID = id });
        //변수이름 바꿔야함

        //여기도 값 잘들어옴.
        var json = await checkMatchRes.Content.ReadAsStringAsync();
        //
        //역직렬화 없이 그냥 보내보자 일단 hive위에는 값 잘들어오는거 확인함
        //var hiveResResult = JsonSerializer.Deserialize<CheckMatchingResponse>(preHiveResResult);

        //여기가 안됨 -> 이방법말고 다른방법으로 값 가져와야할듯?
        //근데 null이아니면 걍 역직렬화안시키고 보내도 되는거아님??
        if(json == null)
        {
            return null;
        }
        //결과, 서버주소, 포트번호, 방번호 받아야한다.
        //Console.WriteLine("매칭서버 완료!!!!");
        Console.WriteLine($"[매칭서비스] 매칭서버 -> API로 받은 JSON: {json}");
        
        // 클라가 받는다... 받는쪽 수정해야함
        //받는쪽에서 역직렬화 하면 되는거아님?
        return json;

    }

}
