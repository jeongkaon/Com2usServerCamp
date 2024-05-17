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
    public async Task<CheckMatchingResponse> Create([FromBody] CheckMatchingRequest request)
    {
        Console.WriteLine("매칭서버 체크하는데에 들오음...!");
        //CheckMatchingResponse test = new CheckMatchingResponse();

        var res = await _matchingService.CheckToMatchServer(request.UserID);
        if(res == null)
        {
            Console.WriteLine("매칭서버 체크 아직안됨");
            return null;
        }
        
        //여기부터 널로 들어가는디?

        Console.WriteLine("매칭서버에서 답이왔다..ㄴ");
        return res;

    }
}
