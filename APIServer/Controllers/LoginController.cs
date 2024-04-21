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

    [HttpPost]
    public async Task<LoginResponse> Create([FromBody] LoginRequest request)
    {
        LoginResponse response = new();

        //하이브서버에 있는지 확인해야한다.
        ErrorCode error = await _AuthService.VerifyTokenToHive(request.Email, request.Token);
        if(error != ErrorCode.None)
        {
            response.Result = ErrorCode.FailVerifyToken;
            return response;
        }


        error = await _AuthService.VerifyExistUser(request.Email);
        if(error != ErrorCode.None) 
        {
            //새로운 유저데이터 생성
           var res = _GameService.CreateNewUserGameData(request.Email);  
        }


        return response;

    }


}



