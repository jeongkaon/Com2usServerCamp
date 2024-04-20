using System;
using System.ComponentModel.DataAnnotations;

namespace HiveServer.Model;

public class CreateHiveAccountRequest
{
    [Required]
    [MinLength(1, ErrorMessage = "EMAIL CANNOT BE EMPTY")]      //에러문자열 말고 숫자로 나오게 변경해야한다.
    [StringLength(50, ErrorMessage = "EMAIL IS TOO LONG")]
    [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid")]
    public string? Email { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "PASSWORD CANNOT BE EMPTY")]
    [StringLength(30, ErrorMessage = "PASSWORD IS TOO LONG")]
    [DataType(DataType.Password)]
    public string? Password { get; set; }
}

public class CreateHiveAccountResponse
{
    [Required]
    public ErrorCode Result { get; set; } = ErrorCode.None;
}

//accountDB의 유저정보인가?
public class AdbUser
{
    public long user_id { get; set; }
    public string email { get; set; }
    public string hashed_pw { get; set; }
    public string salt_value { get; set; }
}