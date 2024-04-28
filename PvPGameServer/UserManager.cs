﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PvPGameServer;

public class UserManager
{
    int MaxUserCount;
    UInt64 UserSequenceNumber = 0; 

    Dictionary<string, User> UserMap = new Dictionary<string, User>();

    public void Init(int maxUserCount)
    {
        MaxUserCount = maxUserCount;
    }

    public ERROR_CODE AddUser(string userID, string sessionID)
    {
        if (IsFullUserCount())
        {
            return ERROR_CODE.LOGIN_FULL_USER_COUNT;
        }

        if (UserMap.ContainsKey(sessionID))
        {
            return ERROR_CODE.ADD_USER_DUPLICATION;
        }


        ++UserSequenceNumber;

        var user = new User();
        user.Set(UserSequenceNumber, sessionID, userID);
        UserMap.Add(sessionID, user);

        return ERROR_CODE.NONE;
    }

    public ERROR_CODE RemoveUser(string sessionID)
    {
        if (UserMap.Remove(sessionID) == false)
        {
            return ERROR_CODE.REMOVE_USER_SEARCH_FAILURE_USER_ID;
        }

        return ERROR_CODE.NONE;
    }

    public User GetUser(string sessionID)
    {
        User user = null;
        UserMap.TryGetValue(sessionID, out user);
        return user;
    }
    bool IsFullUserCount()
    {
        return MaxUserCount <= UserMap.Count();
    }

}
