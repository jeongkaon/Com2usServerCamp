using System;

public enum ErrorCode 
{
    None = 0,
    FailCreateAccount=1,

    FailVerifyUserNoid=2000,
    FailVerifyUserNotPassword = 2001,
    FailVerifyUserToken = 2002

}