using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PvPGameServer;

public class User
{
    UInt64 SequenceNumber = 0;
    string SessionID;
    string UserID;




    public bool Used = false;        //사용되는 유저인지 확인해야한다.
                                 //커넥트,디스커넥트 할때 바꿔줘야한다

    //heartbeat위한 시간
    int TimeSpan;               

    DateTime hbTime = new DateTime();






    public int RoomNumber { get; private set; } = -1;

    public void InitTimeSpan(int timespan)
    {
        TimeSpan = timespan;    
    }

    public void Set(UInt64 sequence, string sessionID, string userID, DateTime ping)
    {
        SequenceNumber = sequence;
        SessionID = sessionID;
        UserID = userID;
        Used = true;


        // 하트비트위한 구현 여기다가 
        hbTime= ping;
        InitTimeSpan(100000);   // 밀리세컨드 단위로 해야하나??


    }

    public void UpdateHeartBeatTime(DateTime curTime)
    {
        hbTime = curTime;
    }

    public bool CheckHeartBeatTime(DateTime curTime)
    {
        //언제호출하는거밍?

        var diff = curTime - hbTime;
        
        if(diff.Microseconds > TimeSpan)
        {
            return false;
        }

        return true;
    }

    public void DisconnectUser()
    {
        //Used를 false로 만들기 위해사용
        Used = false;
    }

    public bool IsConfirm(string netSessionID)
    {
        return SessionID == netSessionID;
    }

    public string ID()
    {

        return UserID;
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

    public bool IsStateLogin() { return SequenceNumber != 0; }

    public bool IsStateRoom() { return RoomNumber != -1; }

    
}


