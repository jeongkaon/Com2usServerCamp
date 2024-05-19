using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using APIServer.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ZLogger;


namespace APIServer.Controllers;

[ApiController]
[Route("[controller]")]
public class RequestMatching : ControllerBase
{
    readonly ILogger<RequestMatching> _logger;
    IMatchWoker _matchWorker;

    public RequestMatching(ILogger<RequestMatching>  logger, IMatchWoker matchWorker)
    {
        _logger = logger;
        _matchWorker = matchWorker;
    }

    [HttpPost]
    public MatchResponse Post(MatchingRequest request)
    {
        MatchResponse response = new MatchResponse();
        _matchWorker.AddUser(request.UserID);
        
        return response;
    }

    
}
