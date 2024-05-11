namespace APIServer.Models;

//하이브에 토큰맞는지 요청보내는거임
//이거 하이브랑 맞아야하나??
//얘필요없는ㄱ ㅓ같은디?
public class VerifyTokenRequest
{
    public string Id { get; set; }
    public string Token { get; set; }
}
public class VerifyTokenReponse
{
    public ErrorCode Result { get; set; } = ErrorCode.None;

}