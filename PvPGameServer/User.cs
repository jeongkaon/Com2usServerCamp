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

    public int RoomNumber { get; private set; } = -1;



    public void Set(UInt64 sequence, string sessionID, string userID)
    {
        SequenceNumber = sequence;
        SessionID = sessionID;
        UserID = userID;


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

    public bool IsStateLogin() { return SequenceNumber != 0; }

    public bool IsStateRoom() { return RoomNumber != -1; }
}


