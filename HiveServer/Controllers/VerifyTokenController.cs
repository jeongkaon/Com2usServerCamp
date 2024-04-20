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
    readonly IHiveRedis _HiveRedis;
    public VerifyTokenController(IHiveRedis rd)
    {
        _HiveRedis = rd;
    }

    [HttpPost]
    public async Task<VerifyTokenReponse> Create([FromBody] VerifyTokenRequest request)
    {
        VerifyTokenReponse response = new();
        response.Result = await _HiveRedis.VerifyUserToken(request.Email, request.Token);

        return response;
    }


}
