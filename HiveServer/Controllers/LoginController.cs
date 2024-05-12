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
public class LoginController : ControllerBase
{
    readonly IHiveRedis _hiveRedis;
    readonly IHiveAccountDB _accountDB;

    public LoginController(IHiveRedis hiveRedis, IHiveAccountDB accountDB)
    {
        _hiveRedis = hiveRedis;
        _accountDB = accountDB;
    }


    [HttpPost]
    public async Task<LoginHiveResponse> Create([FromBody] LoginHiveRequest request)
    {

        LoginHiveResponse response = new();

        // 유저정보 있는지 없는지 검사
        (ErrorCode errorCode, string id) = await _accountDB.VerifyUserAccount(request.Id, request.Password);
        if (errorCode != ErrorCode.None)
        {
            response.Result = errorCode;

            return response;
        }

        response.Token = Security.GenerateToken();
        response.Result = await _hiveRedis.RegistUserAsync(id, response.Token);


        return response;
    }

}
