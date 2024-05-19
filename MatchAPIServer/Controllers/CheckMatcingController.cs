using APIServer.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using ZLogger;
using static APIServer.Controllers.CheckMatching;

namespace APIServer.Controllers;

[ApiController]
[Route("[controller]")]
public class CheckMatching : ControllerBase
{
    readonly ILogger<CheckMatching> _logger;

    IMatchWoker _matchWorker;


    public CheckMatching(ILogger<CheckMatching> logger,IMatchWoker matchWorker)
    {
        _logger = logger;
        _matchWorker = matchWorker;
    }

    [HttpPost]
    public CheckMatchingResponse Post(CheckMatchingRequest request)
    {
        (var result, var completeMatchingData) = _matchWorker.GetCompleteMatching(request.UserID);
        CheckMatchingResponse response = new CheckMatchingResponse();

        if (result == false)
        {
            response.Result = ErrorCode.NotYetMatch;
            return response;
        }


        response.Result = ErrorCode.None;
        response.ServerAddress = completeMatchingData.ServerAddress;
        response.Port = completeMatchingData.Port;
        response.RoomNumber = completeMatchingData.RoomNumber;


        return response;
    }


}

