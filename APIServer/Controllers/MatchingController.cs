using APIServer.Models;
using APIServer.Services;
using APIServer.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace APIServer.Controllers;

[ApiController]
[Route("[controller]")]
public class MatchingController : ControllerBase
{
    readonly ILogger<MatchingController> _logger;
    readonly IMatchingService _matchingService;

    public MatchingController(ILogger<MatchingController> logger, IMatchingService matchingService)
    {
        _logger = logger;
        _matchingService = matchingService;
    }

    [HttpPost]
    public async Task<MatchingResponse> Create([FromBody] MatchingRequst request)
    {

        MatchingResponse response = new MatchingResponse();
        try
        {
            var res = _matchingService.UserIdToMatchServer(request.UserID);

            if (res == null)
            {
                response.Result = ErrorCode.FailUserIdToMatchServer;
            }

            return response;
        }
        catch
        {
            response.Result = ErrorCode.FailUserIdToMatchServer;
            return response;
        }

    }

        

}
