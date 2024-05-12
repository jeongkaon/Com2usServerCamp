using APIServer.Models.GameDB;

namespace APIServer.Models;

public class LoginRequest
{
    public string Id { get; set; }
    public string Token { get; set; }
}

public class LoginResponse
{
    public ErrorCode Result { get; set; }

    //public UserGameDataDB UserGameData { get; set; }    
}


