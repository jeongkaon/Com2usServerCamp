namespace APIServer.Services.Interface;

public interface IGameService
{
    public Task<ErrorCode> CreateNewUserGameData(string email);

}

