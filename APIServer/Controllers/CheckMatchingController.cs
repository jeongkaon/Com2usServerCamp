using APIServer.Models;
using APIServer.Services;
using APIServer.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using ZLogger;

namespace APIServer.Controllers;

[ApiController]
[Route("[controller]")]

public class CheckMatchingController: ControllerBase
{
    readonly ILogger<CheckMatchingController> _logger;
    readonly IMatchingService _matchingService;

    public CheckMatchingController(ILogger<CheckMatchingController> logger, IMatchingService matchingService)
    {
        _logger = logger;
        _matchingService = matchingService;
    }

    [HttpPost]
    public async Task<string> Create([FromBody] CheckMatchingRequest request)
    {
        var res = await _matchingService.CheckToMatchServer(request.UserID);
        if(res == null || res=="")
        {
            return "";
        }
        return res;
    }
}
