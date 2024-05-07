using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace PvPGameServer;

public class UserManager
{
    int MaxUserCount;
    UInt64 UserSequenceNumber = 0; 

    Dictionary<string, User> UserMap = new Dictionary<string, User>();
    
    User[] UserArr;     //얘도 이름 바꾸고 시픈데..


    public static Action<MemoryPackBinaryRequestInfo> DistributeInnerPacket;

    public void Init(int maxUserCount)
    {
        MaxUserCount = maxUserCount;
        UserArr = new User[MaxUserCount];
    }
    public void SetDistributeInnerPacket(Action<MemoryPackBinaryRequestInfo> action)
    {
        DistributeInnerPacket = action;

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
            endIdx = MaxUserCount;
        }

        var CurTime = DateTime.Now;

        for(int i=beginIdx; i < endIdx; i++)
        {
            if (UserArr[i]== null || UserArr[i].Used ==false)
            {   
                return;
            }

            if (false == UserArr[i].CheckHeartBeatTime(CurTime))
            {
                var interanlpacket = InnerPacketMaker.MakeNTFInnerUserForceClosePacket(UserArr[i].SessionId());
                DistributeInnerPacket(interanlpacket);

                RemoveUser(UserArr[i].SessionId());
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
