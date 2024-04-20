using System;

public enum ErrorCode : UInt16
{
    None = 0,

    FailCreateAccount=1,
    FailVerifyUserAccount=2,

    FailVerifyUserToken=3,

}