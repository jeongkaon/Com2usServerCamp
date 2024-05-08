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
    Dictionary<string, User> _userMap = new Dictionary<string, User>();
    User[] _userArr;     

    public static Action<MemoryPackBinaryRequestInfo> DistributeInnerPacket;

    public void Init(int maxUserCount)
    {
        MaxUserCount = maxUserCount;
        _userArr = new User[MaxUserCount];
    }
    public void SetDistributeInnerPacket(Action<MemoryPackBinaryRequestInfo> action)
    {
        DistributeInnerPacket = action;

    }
    public void AddToEmptyArray(User newUser)
    {
        for (int i = 0; i < _userArr.Length; i++)
        {
            if (_userArr[i] != null && _userArr[i]._used == true)
            {
                continue;
            }

            _userArr[i] = newUser;
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
            if (_userArr[i] == null || _userArr[i]._used == false)
            {
                return (null, -1, null);
            }

            if (false == _userArr[i].CheckHeartBeatTime(CurTime))
            {
                var interanlpacket = InnerPacketMaker.MakeNTFInnerUserForceClosePacket(_userArr[i].SessionId());
                DistributeInnerPacket(interanlpacket);

                var id = _userArr[i].SessionId();
                (string, int, string) value = (_userArr[i].SessionId(), _userArr[i].RoomNumber, _userArr[i].ID());

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

        if (_userMap.ContainsKey(sessionId))
        {
            return ErrorCode.AddUserDuplication;
        }


        ++UserSequenceNumber;

        var user = new User();
        user.Set(UserSequenceNumber, sessionId, userId, DateTime.Now);
        
        _userMap.Add(sessionId, user);
        AddToEmptyArray(user);

        return ErrorCode.None;
    }

    public ErrorCode RemoveUser(string sessionId)
    {
        _userMap[sessionId].DisconnectUser();

        if (_userMap.Remove(sessionId) == false)
        {
            return ErrorCode.RemoveUserSearchFailureUserId;
        }

        return ErrorCode.None;
    }

    public User GetUser(string sessionId)
    {
        User user = null;
        _userMap.TryGetValue(sessionId, out user);
        return user;
    }
    
    bool IsFullUserCount()
    {
        return MaxUserCount <= _userMap.Count();
    }
    public int GetMaxUserCount()
    {
        return MaxUserCount;
    }

}
