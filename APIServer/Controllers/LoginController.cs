using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using APIServer.Models;
using APIServer.Repository;
using APIServer.Repository.Interfaces;
using APIServer.Services;
using APIServer.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ZLogger;

namespace APIServer.Controllers;

[ApiController]
[Route("[controller]")]
public class LoginController : ControllerBase
{
    readonly IRedisDB _RedisDB;                 
    readonly IAuthService _AuthService;
    readonly IGameService _GameService;

    public LoginController(IRedisDB hiveRedis, IAuthService authService, IGameService gameService)
    {
        _RedisDB = hiveRedis;
        _AuthService = authService;
        _GameService = gameService;
    }

    [HttpPost]
    public async Task<LoginResponse> Create([FromBody] LoginRequest request)
    {
        LoginResponse response = new LoginResponse();
        var id = request.Id;
        var token = request.Token;

        var error = await _RedisDB.VerifyUserToken(id, token);
        if (error == ErrorCode.None)
        {
            return response;
        }

        error = await _AuthService.VerifyTokenToHive(id, token);
        if(error != ErrorCode.None)
        {
            response.Result = ErrorCode.FailVerifyToken;
            return response;
        }

        error = await _RedisDB.RegistUserAsync(id, token);

        if(error== ErrorCode.FailSetRedisUserToken)
        {
            //레디스 저장한거 실패! 
            //TODO - 로그남겨야한다.
        }

        error = await _GameService.CheckUserGameDataInDB(id);
        if(error == ErrorCode.None)
        {
            //TODO - 로그 변경
            Console.WriteLine("데이터테이블에까지 생성완료");
        }

        response.Result = error;
        return response;
    }


}



