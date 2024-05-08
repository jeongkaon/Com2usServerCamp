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
    User[] UserArr;     

    public static Action<MemoryPackBinaryRequestInfo> DistributeInnerPacket;


    void InnerUserCheckTimer(object? state)
    {
        MemoryPackBinaryRequestInfo packet = InnerPacketMaker.MakeNTFInnerRoomCheckPacket();
        DistributeInnerPacket(packet);
    }
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
            if (UserArr[i] != null && UserArr[i]._used == true)
            {
                continue;
            }

            UserArr[i] = newUser;
            return;
        }

    }

    public (string, int, string) CheckHeartBeat(int beginIdx, int endIdx)
    {
        if (endIdx > MaxUserCount)
        {
            endIdx = MaxUserCount;
        }

        var CurTime = DateTime.Now;

        for (int i = beginIdx; i < endIdx; i++)
        {
            if (UserArr[i] == null || UserArr[i]._used == false)
            {
                return (null, -1, null);
            }

            if (false == UserArr[i].CheckHeartBeatTime(CurTime))
            {
                var interanlpacket = InnerPacketMaker.MakeNTFInnerUserForceClosePacket(UserArr[i].SessionId());
                DistributeInnerPacket(interanlpacket);

                var id = UserArr[i].SessionId();
                (string, int, string) value = (UserArr[i].SessionId(), UserArr[i].RoomNumber, UserArr[i].ID());

                RemoveUser(id);

                return value;

            }
        }

        return (null, 0, null);
    }


    public ErrorCode AddUser(string userId, string sessionId)
    {
        if (IsFullUserCount())
        {
            return ErrorCode.LoginFullUserCount;
        }

        if (UserMap.ContainsKey(sessionId))
        {
            return ErrorCode.AddUserDuplication;
        }


        ++UserSequenceNumber;

        var user = new User();
        user.Set(UserSequenceNumber, sessionId, userId, DateTime.Now);
        
        UserMap.Add(sessionId, user);
        AddToEmptyArray(user);

        return ErrorCode.None;
    }

    public ErrorCode RemoveUser(string sessionId)
    {
        UserMap[sessionId].DisconnectUser();

        if (UserMap.Remove(sessionId) == false)
        {
            return ErrorCode.RemoveUserSearchFailureUserId;
        }

        return ErrorCode.None;
    }

    public User GetUser(string sessionId)
    {
        User user = null;
        UserMap.TryGetValue(sessionId, out user);
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
