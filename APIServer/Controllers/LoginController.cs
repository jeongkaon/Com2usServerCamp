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
    readonly IAccountDB _AccountDB;             
    readonly IAuthService _AuthService;
    readonly IGameService _GameService;

    public LoginController(IRedisDB hiveRedis, IAccountDB accountDB, IAuthService authService, IGameService gameService)
    {
        _RedisDB = hiveRedis;
        _AccountDB = accountDB;
        _AuthService = authService;
        _GameService = gameService;
    }

    [HttpPost]
    public async Task<LoginResponse> Create([FromBody] LoginRequest request)
    {
        LoginResponse response = new LoginResponse();

        //TODO 추가해야하는것!
        //레디스에 있는지 먼저 확인해야하는거아님??
        //레디스에 없으면 하이브로 가는거임
        //->redis가 들고있는게 맞는지 아님 auth로 따로빼는게 맞는지 고민..


        ErrorCode error = await _AuthService.VerifyTokenToHive(request.Id, request.Token);
        if(error != ErrorCode.None)
        {
            response.Result = ErrorCode.FailVerifyToken;
            return response;
        }

        //TODO - 레디스 저장위치가 여기가 맞는가?
        var result = await _RedisDB.RegistUserAsync(request.Id, request.Token);
        if(result == ErrorCode.None)
        {
            Console.WriteLine("레디스에 잘 저장됨!");
        }

        //근데 하이브에 있는데 유효하지 않은 유저일수가 있나?
        //-> 그게 아니라 처음 회원가입하고 로그인한애면 DB에 USER정보 생성해야한다.
        // 있는 애라면 걍 들고와야한다.

        //로그인은 한 상황이면 유저정보가 DB에 있는지 확인해야한다.
        error = await _AuthService.CheckUserInDB(request.Id);

        if(error == ErrorCode.None) 
        {
           var res = _GameService.CreateNewUserGameData(request.Id);  
        }
       
        //TODO-userdata 세팅해야한다.

        return response;
    }


}



