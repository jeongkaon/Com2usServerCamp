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
    readonly IHiveAccountDB _hiveDB;

    public CreateAccountController(IHiveAccountDB db)
    {
        _hiveDB = db;
    }

    [HttpPost]
    public async Task<CreateHiveAccountResponse> Create([FromBody] CreateHiveAccountRequest request)
    {
        CreateHiveAccountResponse response = new();
        response.Result = await _hiveDB.CreateAccountAsync(request.Id, request.Password);

        return response;
    }

}
