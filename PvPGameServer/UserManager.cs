using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PvPGameServer;

public class UserManager
{
    int MaxUserCount;
    UInt64 UserSequenceNumber = 0; 

    Dictionary<string, User> UserMap = new Dictionary<string, User>();

    User[] UserArr;
                        //이름 이거로 해??
                        //used중인 user 저장하고 있는 배열
                        //UserMapd이랑 따로 쓰는 이유가 머임?
                        //sessionid 이용해서 찾기 쉽게하려고 딕셔너리도 쓰나?


    public void Init(int maxUserCount)
    {
        MaxUserCount = maxUserCount;
        UserArr = new User[MaxUserCount];
    }

  
    

    public void AddToEmptyArray(User newUser)
    {
        for (int i = 0; i < UserArr.Length; i++)
        {
   
            if (UserArr[i] != null && UserArr[i].Used == true)
            {
                continue;
            }

            UserArr[i] = newUser;
            return;
        }

    }

    public void CheckHeartBeat(int beginIdx, int endIdx)
    {
        if(endIdx > MaxUserCount)
        {
            //4로 안나눠 떨어지는 경우임
            endIdx = MaxUserCount;
        }

        var CurTime = DateTime.Now;

        for(int i=beginIdx; i < endIdx; i++)
        {
            if (UserArr[i]== null)
            {   
                //null인경우는 그뒤에 아무것도 없다.
                return;
            }
            if (false == UserArr[i].CheckHeartBeatTime(CurTime))
            {
                //TODO
                //접속끊어버려야한다.
            }
        }
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
        user.Set(UserSequenceNumber, sessionID, userID, DateTime.Now);
        
        UserMap.Add(sessionID, user);
        AddToEmptyArray(user);

        return ERROR_CODE.NONE;
    }

    public ERROR_CODE RemoveUser(string sessionID)
    {
        UserMap[sessionID].DisconnectUser();

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
    public int GetMaxUserCount()
    {
        return MaxUserCount;
    }

}
