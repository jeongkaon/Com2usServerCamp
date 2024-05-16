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
    readonly ILogger<VerifyTokenController> _logger;
    public VerifyTokenController(IHiveRedis rd, ILogger<VerifyTokenController> logger)
    {
        _hiveRedis = rd;
        _logger = logger;
    }

    [HttpPost]
    public async Task<VerifyTokenReponse> Create([FromBody] VerifyTokenRequest request)
    {
        _logger.ZLogDebug(
            $"[VerifyTokenController]  Verify요청 옴");

        VerifyTokenReponse response = new()
        {
            Result = ErrorCode.None
        };

        var res = await _hiveRedis.VerifyUserToken(request.Id, request.Token);
        if (res != ErrorCode.None)
        {
            _logger.ZLogDebug(
                $"[VerifyTokenController] ErrorCode: {ErrorCode.FailVerifyUserToken}");
            response.Result = ErrorCode.FailVerifyUserToken;
        }

        _logger.ZLogDebug(
              $"[VerifyTokenController] Verify요청 성공!");
        return response;
    }
}
