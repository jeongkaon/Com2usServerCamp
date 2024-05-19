using System;
using System.ComponentModel.DataAnnotations;

namespace HiveServer.Model;
public class VerifyTokenRequest
{
    public string Id { get; set; } 
    public string Token { get; set; }
}
public class VerifyTokenReponse
{
    public ErrorCode Result { get; set; } = ErrorCode.None;

}
