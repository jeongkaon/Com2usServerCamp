using APIServer.Models;
using APIServer.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace APIServer.Controllers;

[ApiController]
[Route("[controller]")]

public class CheckMatchingController: ControllerBase
{
    readonly IMatchingService _matchingService;
    public CheckMatchingController(IMatchingService matchingService)
    {
        _matchingService = matchingService;
    }

    [HttpPost]
    public async Task<CheckMatchingRes> Create([FromBody] CheckMatchingReq request)
    {
        CheckMatchingRes test = new CheckMatchingRes();

        var res = _matchingService.CheckToMatchServer(request.UserID);

        test.Result = ErrorCode.FailVerifyToken;
        


        Console.WriteLine("매칭서버에서 답이왔다..ㄴ");
        return test;

    }
}
