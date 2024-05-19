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
    IMatchWoker _matchWorker;


    public CheckMatching(IMatchWoker matchWorker)
    {
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
            Console.WriteLine($"[매칭컨트롤러] 매칭된 애들 데이터 직렬화된 JSON: {json}");

        return response;
    }


}

