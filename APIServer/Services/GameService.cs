using APIServer.Repository.Interfaces;
using APIServer.Services.Interface;


namespace APIServer.Services;

public class GameService: IGameService
{
    readonly ILogger<GameService> _logger;
    readonly IGameDB _gameDb;

    public GameService(ILogger<GameService> logger, IGameDB gameDb)
    {
        _logger = logger;
        _gameDb = gameDb;
    }

    public async Task<ErrorCode> CreateNewUserGameData(string email)
    {
        //데이터베이스에 새로운 user정보 저장
        var error = _gameDb.CreateUserGameData(email);
       if(error != null)
        {
            return ErrorCode.FailCreateUserGameData;
        }

        return ErrorCode.None;
    }

}
