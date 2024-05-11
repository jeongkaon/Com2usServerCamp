﻿using System;
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
    readonly IHiveRedis _HiveRedis;
    readonly IHiveAccountDB _AccountDB;

    public LoginController(IHiveRedis hiveRedis, IHiveAccountDB accountDB)
    {
        _HiveRedis = hiveRedis;
        _AccountDB = accountDB;
    }


    [HttpPost]
    public async Task<LoginHiveResponse> Create([FromBody] LoginHiveRequest request)
    {

        LoginHiveResponse response = new();

        // 유저정보 있는지 없는지 검사
        (ErrorCode errorCode, string id) = await _AccountDB.VerifyUserAccount(request.Id, request.Password);
        if (errorCode != ErrorCode.None)
        {
            response.Result = errorCode;

            return response;
        }

        //토큰 발행 -> 함수 따로 만들어야함, 유효시간도 정해야한다.
        response.Token = Security.GenerateToken();
        response.Result = await _HiveRedis.RegistUserAsync(id, response.Token);


        return response;
    }

}
