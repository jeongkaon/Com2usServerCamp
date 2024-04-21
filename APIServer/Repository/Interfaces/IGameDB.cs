using APIServer.Models.GameDB;
using System;
using System.Threading.Tasks;
namespace APIServer.Repository.Interfaces;

public interface IGameDB : IDisposable
{
    public Task<UserGameDataDB> GetUserGameDataByEmail(string email);

    public Task<ErrorCode> CreateUserGameData(string email);

}
