using System;

public enum ErrorCode : UInt16
{
    None = 0,
    FailCreateAccount=1,

    FailVerifyUserNoid=1000,
    FailVerifyUserNotPassword = 1001,
    FailVerifyUserToken = 1002

}