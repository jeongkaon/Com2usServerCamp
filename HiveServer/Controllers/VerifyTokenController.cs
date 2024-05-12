using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using HiveServer.Model;
using HiveServer.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ZLogger;
namespace HiveServer.Controllers;


[ApiController]
[Route("[controller]")]
public class VerifyTokenController : ControllerBase
{
    readonly IHiveRedis _hiveRedis;
    public VerifyTokenController(IHiveRedis rd)
    {
        _hiveRedis = rd;
    }

    [HttpPost]
    public async Task<VerifyTokenReponse> Create([FromBody] VerifyTokenRequest request)
    {
        Console.WriteLine("VeriyToeken 요청옴");
        VerifyTokenReponse response = new();

        response.Result = await _hiveRedis.VerifyUserToken(request.Id, request.Token);

        return response;
    }
}
