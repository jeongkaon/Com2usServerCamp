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
        Console.WriteLine("시발 불렷나??");

        LoginHiveResponse response = new();

        // 유저정보 있는지 없는지 검사
        (ErrorCode errorCode, string email) = await _AccountDB.VerifyUserAccount(request.Email, request.Password);
        if (errorCode != ErrorCode.None)
        {
            response.Result = errorCode;

            Console.WriteLine("유저정보없음!!!!!!!!!!!!!!");
            return response;
        }

        Console.WriteLine("유저정보있음!!!!!!!!!!!!!!");


        //토큰 발행 -> 함수 따로 만들어야함, 유효시간도 정해야한다.
        string tok = "Tocken0000";
        response.Token = tok;

        //레디스에 저장하자.
        errorCode= await _HiveRedis.RegistUserAsync(email, tok);
        response.Result = errorCode;
        response.Token = tok;



        return response;
    }

}
