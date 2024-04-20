using System;
using System.Threading.Tasks;

namespace HiveServer.Repository;

public interface IHiveRedis : IDisposable
{
 
    public Task<ErrorCode> RegistUserAsync(string email, string authToken);
    public Task<ErrorCode> VerifyUserToken(string email, string authToken);
}
