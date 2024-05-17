using APIServer.Models;
using APIServer.Services;
using APIServer.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace APIServer.Controllers;

[ApiController]
[Route("[controller]")]
public class MatchingController : ControllerBase
{
    //서비스를 가지고 있어야함
    readonly IMatchingService _matchingService;

    public MatchingController(IMatchingService matchingService)
    {
        _matchingService = matchingService;
    }

    [HttpPost]
    public async Task<MatchingResponse> Create([FromBody] MatchingRequst request)
    {
        //매칭 요청 받으면 매칭 서버에 넘겨줘야한다.
        MatchingResponse test = new MatchingResponse();

        //여기다가 넘겨주면된다.
        var res = _matchingService.UserIdToMatchServer(request.UserID);




        test.Result = ErrorCode.FailVerifyToken;



        Console.WriteLine("매칭서버에 들어엄");
        return test;
    }

        

}
