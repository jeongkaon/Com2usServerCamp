using APIServer.Models.GameDB;
using System;
using System.Threading.Tasks;
namespace APIServer.Repository.Interfaces;

public interface IGameDB : IDisposable
{
    public Task<ErrorCode> GetUserGameDataById(string email);


    //이거 빼야할거같은디?

    public Task<ErrorCode> CreateUserGameData(string email);

}
