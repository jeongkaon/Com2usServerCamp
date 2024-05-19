using System;
using System.ComponentModel.DataAnnotations;

namespace HiveServer.Model;

public class LoginHiveRequest
{
    public string? Id { get; set; }
    public string? Password { get; set; }    
}

public class LoginHiveResponse
{
    public ErrorCode Result { get; set; } = ErrorCode.None;

    public string? Token { get; set; }
}
