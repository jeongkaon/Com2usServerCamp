using System;
using System.IO;
using System.Text.Json;
using APIServer.Repository;
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

//서비스 추가해야한다.

builder.Services.AddControllers();


var app = builder.Build();


app.MapDefaultControllerRoute();

app.UseRouting();
#pragma warning disable ASP0014
app.UseEndpoints(endpoints => { _ = endpoints.MapControllers(); });
#pragma warning restore ASP0014

app.Run(configuration["ServerAddress"]);