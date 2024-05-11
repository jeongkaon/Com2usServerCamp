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
    readonly IRedisDB _RedisDB;                 //여기서 레디스는 계정정보 저장하는거임 토큰이랑 id
                                                //유효시간안에 다시 로그인한 유저 하이브에 안가려고.
    readonly IAccountDB _AccountDB;             //유효한 유저라면 유저정보 들거와야함?
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
        //로그인 요청 들어왔음. id, token에 따라 처리해야한다.
        LoginResponse response = new LoginResponse();

        //TODO 추가해야하는것!
        //레디스에 있는지 먼저 확인해야하는거아님??
        //레디스에 없으면 하이브로 가는거임



        //하이브서버에 있는지 확인해야한다.
        ErrorCode error = await _AuthService.VerifyTokenToHive(request.Id, request.Token);
        if(error != ErrorCode.None)
        {
            response.Result = ErrorCode.FailVerifyToken;

            //로그인 실패를 보낸다
            return response;
        }

        //TODO
        //레디스에도 저장해야한다.
        //어디서 저장해야할지..



        //근데 하이브에 있는데 유효하지 않은 유저일수가 있나?
        //-> 그게 아니라 처음 회원가입하고 로그인한애면 DB에 USER정보 생성해야한다.
        // 있는 애라면 걍 들고와야한다.

        //로그인은 한 상황이면 유저정보가 DB에 있는지 확인해야한다.
        error = await _AuthService.CheckUserInDB(request.Id);

        if(error == ErrorCode.None) 
        {
            //한번도 플레이 해본적 없는 유저임 
            //새로운 유저데이터 생성해야한다.
           var res = _GameService.CreateNewUserGameData(request.Id);  
        }


        return response;

    }


}



