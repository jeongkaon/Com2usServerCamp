﻿using System.Diagnostics.Tracing;

namespace APIServer;

public enum ErrorCode 
{
    None = 0,

    FailVerifyAccount=1000,
    FailVerifyToken=1001,
    FailHiveInvalidResponse=1002,
    FailCreateUserGameData=1003,
    FailSetRedisUserToken = 1003,

    FailUserIdToMatchServer = 1004,




    FailVerifyUserToken = 1004,

    NotExistAccount = 2000,

    NotExistRedis = 2001
    
}