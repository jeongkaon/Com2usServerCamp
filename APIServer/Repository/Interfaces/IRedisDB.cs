using System;
using System.Threading.Tasks;

namespace APIServer.Repository.Interfaces;
public interface IRedisDB : IDisposable
{
    public Task<ErrorCode> RegistUserAsync(string id, string authToken);
    public Task<ErrorCode> VerifyUserToken(string id, string authToken);

}





