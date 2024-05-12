using System;
using System.ComponentModel.DataAnnotations;

namespace HiveServer.Model;

public class CreateHiveAccountRequest
{
    [Required]
    [MinLength(1, ErrorMessage = "id CANNOT BE EMPTY")]      //에러문자열 말고 숫자로 나오게 변경해야한다.
    [StringLength(50, ErrorMessage = "id IS TOO LONG")]
    [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid")]
    public string? Id { get; set; }

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

public class UserInfoAccountDB
{
    public string id { get; set; }
    public string password { get; set; }


    public string saltvalue { get; set; }
    public string hashedpassword { get; set; }
}