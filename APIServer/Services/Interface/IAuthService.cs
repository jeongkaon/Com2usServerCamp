namespace APIServer.Services.Interface;

public interface IAuthService 
{
    public Task<ErrorCode> VerifyTokenToHive(string  id, string token);

}

