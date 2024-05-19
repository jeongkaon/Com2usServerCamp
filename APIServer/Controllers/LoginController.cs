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
    readonly ILogger<LoginController> _logger;

    readonly IRedisDB _redisDB;                 
    readonly IAuthService _authService;
    readonly IGameService _gameService;

    public LoginController(ILogger<LoginController> logger,IRedisDB hiveRedis, IAuthService authService, IGameService gameService)
    {
        _logger = logger;
        _redisDB = hiveRedis;
        _authService = authService;
        _gameService = gameService;
    }

    [HttpPost]
    public async Task<LoginResponse> Create([FromBody] LoginRequest request)
    {
        LoginResponse response = new LoginResponse();
        var id = request.Id;
        var token = request.Token;

        var res = await _redisDB.VerifyUserToken(id, token);
        if (res == ErrorCode.None)
        {
            _logger.ZLogInformation($"[LoginController] user token stored in Redis");
            return response;
        }

        res = await _authService.VerifyTokenToHive(id, token);
        if(res != ErrorCode.None)
        {
            _logger.ZLogError($"[LoginController] fail verify token to hive");
            response.Result = ErrorCode.FailVerifyToken;
            return response;
        }

        res = await _redisDB.RegistUserAsync(id, token);
        if(res== ErrorCode.FailSetRedisUserToken)
        {
            _logger.ZLogError($"[AuthService] fail regist hive auth in redis");
            response.Result = ErrorCode.FailSetRedisUserToken;
            return response;
        }

        res = await _gameService.CheckUserGameDataInDB(id);
        if(res == ErrorCode.None)
        {
            _logger.ZLogInformation($"[AuthService] Success Create GameData in MySql");
        }

        response.Result = res;
        return response;
    }


}



