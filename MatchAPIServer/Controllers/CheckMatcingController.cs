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
public class CheckMatching : Controller
{
    readonly Logger<CheckMatching> _logger;

    IMatchWoker _matchWorker;


    public CheckMatching(Logger<CheckMatching> logger,IMatchWoker matchWorker)
    {
        _logger = logger;
        _matchWorker = matchWorker;
    }

    [HttpPost]
    public CheckMatchingResponse Post(CheckMatchingRequest request)
    {
        (var result, var completeMatchingData) = _matchWorker.GetCompleteMatching(request.UserID);

        if(result == false)
        {
            return null;
        }

        CheckMatchingResponse response = new CheckMatchingResponse()
        {
            Result = ErrorCode.None,
            ServerAddress = completeMatchingData.ServerAddress,
            Port = completeMatchingData.Port,
            RoomNumber = completeMatchingData.RoomNumber
        };


        string json = JsonSerializer.Serialize(response);
        _logger.ZLogInformation($"[CheckMatching] : {json}");

        return response;
    }


}

