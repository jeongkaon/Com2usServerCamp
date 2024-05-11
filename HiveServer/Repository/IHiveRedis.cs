using System;
using System.Threading.Tasks;

namespace HiveServer.Repository;

public interface IHiveRedis : IDisposable
{
 
    public Task<ErrorCode> RegistUserAsync(string id, string authToken);
    public Task<ErrorCode> VerifyUserToken(string id, string authToken);
}
