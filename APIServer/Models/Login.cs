namespace APIServer.Models;


public class LoginRequest
{
    public string Email { get; set; }
    public string Token { get; set; }    
}

public class LoginResponse
{
    public ErrorCode Result { get; set; }
}
