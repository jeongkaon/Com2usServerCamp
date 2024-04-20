using System.Threading.Tasks;

namespace HiveServer.Repository;

public interface IHiveAccountDB : IDisposable
{
    public Task<ErrorCode> CreateAccountAsync(string email, string password);
  //  public Task<(ErrorCode, Int64)> VerifyAccount(string email, string password);

}
