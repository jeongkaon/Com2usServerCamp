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

    public async Task<ErrorCode> CreateNewUserGameData(string id)
    {
        //데이터베이스에 새로운 user정보 저장
        ///값을 쓰는거니까 GmaeServic에서 하는게 좋은가?
        //->근데 그러면 db 정보 다들거와야함 걍 거기서하는게 좋을듯
        var error = _gameDb.CreateUserGameData(id);
       if(error != null)
        {
            return ErrorCode.FailCreateUserGameData;
        }

        return ErrorCode.None;
    }

}
