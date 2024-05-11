namespace APIServer.Services.Interface;

public interface IAuthService 
{
    public Task<ErrorCode> VerifyTokenToHive(string  id, string token);

    public Task<ErrorCode> CheckUserInDB(string id);

}

