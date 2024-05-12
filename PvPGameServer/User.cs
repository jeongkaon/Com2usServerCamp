using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PvPGameServer;

public class User
{
    UInt64 _sequenceNumber = 0;
    string _sessionId;
    string _userId;

    public bool Used = false;       

    //heartbeat위한 시간
    int _hbTimeSpan;               

    DateTime _hbTime = new DateTime();

    
    public int RoomNumber { get; private set; } = -1;

    public void InitTimeSpan(int timespan)
    {
        _hbTimeSpan = timespan;    
        
    }

    public void Set(UInt64 sequence, string sessionID, string userID, DateTime ping)
    {
        _sequenceNumber = sequence;
        _sessionId = sessionID;
        _userId = userID;
        Used = true;

        _hbTime= ping;

        //테스트로 일단 20초로세팅
        InitTimeSpan(20000);   


    }

    public void UpdateHeartBeatTime(DateTime curTime)
    {
        _hbTime = curTime;
    }

    public bool CheckHeartBeatTime(DateTime curTime)
    {
        var diff = curTime - _hbTime;
        
        if(diff.TotalMilliseconds > _hbTimeSpan)
        {
            return false;
        }

        return true;
    }

    public void DisconnectUser()
    {
        Used = false;
    }

    public bool IsConfirm(string netSessionID)
    {
        return _sessionId == netSessionID;
    }

    public string ID()
    {
        return _userId;
    }
    public string SessionId()
    {
        return _sessionId;
    }
    public void EnteredRoom(int roomNumber)
    {
        RoomNumber = roomNumber;
    }

    public void LeaveRoom()
    {
        RoomNumber = -1;
    }

    public int GetRoomNumber()
    {
        return RoomNumber;
    }

    public bool IsStateLogin() { return _sequenceNumber != 0; }

    public bool IsStateRoom() { return RoomNumber != -1; }

    
}


