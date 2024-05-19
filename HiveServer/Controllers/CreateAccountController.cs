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
public class CreateAccountController : ControllerBase
{
    readonly ILogger<CreateAccountController> _logger;
    readonly IHiveAccountDB _hiveDB;

    public CreateAccountController(ILogger<CreateAccountController> logger,IHiveAccountDB db)
    {
        _hiveDB = db;
        _logger = logger;
    }

    [HttpPost]
    public async Task<CreateHiveAccountResponse> Create([FromBody] CreateHiveAccountRequest request)
    {
        CreateHiveAccountResponse response = new();
        response.Result = await _hiveDB.CreateAccountAsync(request.Id, request.Password);

        if (response.Result != ErrorCode.None)
        {
            _logger.ZLogInformation(
                $"[CreateAccountController] Account Create Fiail ErrorCode: {ErrorCode.FailVerifyUserToken}");
            response.Result = ErrorCode.FailVerifyUserToken;
        }

        _logger.ZLogDebug(
                $"[CreateAccountController] Account Create Susccess");

        return response;
    }

}
