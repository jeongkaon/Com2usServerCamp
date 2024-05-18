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
    public async Task<string> Create([FromBody] CheckMatchingRequest request)
    {
        Console.WriteLine("매칭서버 체크하는데에 들오음...!");
        //CheckMatchingResponse test = new CheckMatchingResponse();

        //매칭서버에 물어본다. 매칭서버에서 결과 받아온다.
        //res에 결과가 들어가있는거임
        //매칭 안됫으면 아무것도 안오고, 매칭됬으면 json그대로 온다.
        var res = await _matchingService.CheckToMatchServer(request.UserID);
        if(res == null)
        {
            return null;
        }
        
        //여기부터 널로 들어가는디?

//        Console.WriteLine("매칭서버에서 답이왔다..ㄴ");
        return res;

    }
}
