using System;
using System.ComponentModel.DataAnnotations;

namespace HiveServer.Model;

public class VerifyTockenRequest
{
    [Required]
    public string? Token {  get; set; }
    [Required]
    public int uId { get; set; }

}

public class VerifyTockenResponse
{
    public ErrorCode Result { get; set; } = ErrorCode.None;
}