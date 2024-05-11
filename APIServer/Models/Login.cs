using APIServer.Models.GameDB;

namespace APIServer.Models;

//유저가 로그인 요청할때 그거임
public class LoginRequest
{
    public string Id { get; set; }
    public string Token { get; set; }    //hive에서 토큰받은거임.
}

public class LoginResponse
{
    public ErrorCode Result { get; set; }

    //토큰을 새로 줘야하나??? 하이브에서 준거로 쓰면안됨?
    //public string Token { get; set; }

    //얘도 줘야하나?? 일단 넘겨
    public UserGameDataDB UserGameData { get; set; }    
}


