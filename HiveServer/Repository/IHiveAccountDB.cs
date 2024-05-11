using System.Threading.Tasks;

namespace HiveServer.Repository;

public interface IHiveAccountDB : IDisposable
{
    public Task<ErrorCode> CreateAccountAsync(string id, string password);
    public Task<Tuple<ErrorCode, string>> VerifyUserAccount(string id, string password);

}
