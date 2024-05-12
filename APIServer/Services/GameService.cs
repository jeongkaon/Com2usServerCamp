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
        if (res.Result == ErrorCode.NotExistAccount)
        {
            //여기서 걍 생성해버리자.

            res = CreateNewUserGameData(id);

            if (res.Result != ErrorCode.None)
            {
                return ErrorCode.FailCreateUserGameData;
            }

        }

        return ErrorCode.None;


    }

    public async Task<ErrorCode> CreateNewUserGameData(string id)
    {
        var error = _gameDb.CreateUserGameData(id);
       if(error.Result != ErrorCode.None)
        {
            return ErrorCode.FailCreateUserGameData;
        }

        return ErrorCode.None;
    }

}
