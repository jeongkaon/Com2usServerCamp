using APIServer.Models.AccountDB;
using SqlKata.Execution;

namespace APIServer.Repository.Interfaces;

public interface IAccountDB : IDisposable
{
    public Task<UserAccountDB> GetUserAccountById(string email);


}
