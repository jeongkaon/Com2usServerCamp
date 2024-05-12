namespace APIServer.Services.Interface;

public interface IGameService
{
    public Task<ErrorCode> CheckUserGameDataInDB(string id);

    public Task<ErrorCode> CreateNewUserGameData(string id);

}

