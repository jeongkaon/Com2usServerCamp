using System;
using System.ComponentModel.DataAnnotations;


namespace HiveServer.Model;



public class LoginHiveRequest
{
    public string? Email { get; set; }
    public string? Password { get; set; }    
}

public class LoginHiveResponse
{
    public ErrorCode Result { get; set; } = ErrorCode.None;

    public int uId {  get; set; }
    public string? Token { get; set; }
}
