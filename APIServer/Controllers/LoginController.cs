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

        //클라에게 로그인 요청이 들어오면 -> 클라는 id랑 token을 패킷으로 줌

        //0.먼저 레디스에 존재하는지 찾아본다.
        var error = await _RedisDB.VerifyUserToken(id, token);
        if (error == ErrorCode.None)
        {
            return response;
        }

        // 1. 패킷으로 보낸 인증토큰이 유효한지 hive서버에 물어봐야한다.
        error = await _AuthService.VerifyTokenToHive(id, token);
        if(error != ErrorCode.None)
        {
            response.Result = ErrorCode.FailVerifyToken;
            return response;
        }

        //2. 토큰을 레디스에 저장해야한다. id : token으로 저장한다. 하이브와 다른 레디스를 사용한다.
        error = await _RedisDB.RegistUserAsync(id, token);
        if(error== ErrorCode.FailSetRedisUserToken)
        {
            //레디스 저장한거 실패! 
            //TODO - 로그남겨야한다.
        }

        //처음 입장한 애라면 UserGameDataTable 생성해야한다.
        //게임 데이터 테이블에 id로 확인하자!
        var res = await _GameService.CheckUserGameDataInDB(id);
        if(res == ErrorCode.None)
        {

        }
     
        
        return response;
    }


}



