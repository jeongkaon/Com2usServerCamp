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

    public async Task<ErrorCode> CheckUserGameDataInDB(string id)
    {
        //NotExistAccount이거 쓰면될듯

        var res = _gameDb.GetUserGameDataById(id);
        if (res == null)
        {
            return ErrorCode.NotExistAccount;
        }

        return ErrorCode.None;
    }

    public async Task<ErrorCode> CreateNewUserGameData(string id)
    {
        var error = _gameDb.CreateUserGameData(id);
       if(error != null)
        {
            return ErrorCode.FailCreateUserGameData;
        }

        return ErrorCode.None;
    }

}
