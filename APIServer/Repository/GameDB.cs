﻿using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Options;
using System.Data;
using MySqlConnector;
using SqlKata.Execution;
using Microsoft.Extensions.Logging;
using ZLogger;
using System.Diagnostics;
using System.Security.Principal;
using APIServer.Models;
using Microsoft.AspNetCore.Identity;
using APIServer.Repository.Interfaces;
using APIServer.Models.GameDB;

namespace APIServer.Repository;

public class GameDB : IGameDB
{
    readonly IOptions<DbConfig> _dbConfig;
    readonly ILogger<GameDB> _logger;
    IDbConnection _dbCon;
    SqlKata.Compilers.MySqlCompiler _compiler;
    QueryFactory _qFactory;


    public GameDB(ILogger<GameDB> logger, IOptions<DbConfig> dbConfig)
    {

        _dbConfig = dbConfig;
        _logger = logger;

        Open();

        _compiler = new SqlKata.Compilers.MySqlCompiler();
        _qFactory = new QueryFactory(_dbCon, _compiler);

    }

    public async Task<ErrorCode> GetUserGameDataById(string id)
    {
        try
        {
            var res = await _qFactory.Query("gamedata")
                .Where("id", id)
                 .FirstOrDefaultAsync<UserGameDataDB>();

            if (res == null)
            {
                return ErrorCode.NotExistAccount;
            }

            return ErrorCode.None;

        }
        catch (Exception ex)
        {
            _logger.ZLogError($"Fail DB :{ex}");
            return ErrorCode.FailGetUserDataInMySql;

        }


    }


    public async Task<ErrorCode> CreateUserGameData(string id)
    {
        var count = await _qFactory.Query("gamedata").InsertAsync(new
        {
            id = id,
            exp = 0,
            win_score =0,
            lose_score=0,
            draw_score=0
            
        });

        if (count != 1)
        {
            return ErrorCode.FailCreateUserGameData;

        }
        return ErrorCode.None;

    }

    public void Dispose()
    {
        Close();
    }

    void Open()
    {
        _dbCon = new MySqlConnection(_dbConfig.Value.GameDB);
        _dbCon.Open();

    }
    void Close()
    {
        _dbCon.Close();
    }
}
