namespace APIServer.Services.Interface;

public interface IAuthService 
{
    public Task<ErrorCode> VerifyTokenToHive(string  email, string token);

    public Task<ErrorCode> VerifyExistUser(string email);

}

