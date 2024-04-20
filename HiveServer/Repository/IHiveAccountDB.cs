using System.Threading.Tasks;

namespace HiveServer.Repository;

public interface IHiveAccountDB : IDisposable
{
    public Task<ErrorCode> CreateAccountAsync(string email, string password);
    public Task<Tuple<ErrorCode, string>> VerifyUserAccount(string email, string password);

}
