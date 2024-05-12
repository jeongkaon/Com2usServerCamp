using System;
using System.IO;
using System.Text.Json;
using APIServer.Repository;
using APIServer.Repository.Interfaces;
using APIServer.Services;
using APIServer.Services.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SqlKata;
using ZLogger;

var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = builder.Configuration;

builder.Services.Configure<DbConfig>(configuration.GetSection(nameof(DbConfig)));

builder.Services.AddSingleton<IRedisDB, RedisDB>();
builder.Services.AddTransient<IGameDB,GameDB>();
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddTransient<IGameService, GameService>();
builder.Services.AddControllers();

var app = builder.Build();

app.MapDefaultControllerRoute();

app.UseRouting();
#pragma warning disable ASP0014
app.UseEndpoints(endpoints => { _ = endpoints.MapControllers(); });
#pragma warning restore ASP0014

app.Run(configuration["ServerAddress"]);


public class DbConfig
{
    public string? GameDB { get; set; }
    public string? Redis { get; set; }

}