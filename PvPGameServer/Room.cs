using MemoryPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PvPGameServer;

public class Room
{
    public const int InvalidRoomNumber = -1;

    public  int Index {  get; set; }
    public int Number { get; private set; }

    int MaxUserCount = 2;

    List<RoomUser> UserList = new List<RoomUser>();

    public static Func<string, byte[], bool> NetworkSendFunc;

    GameBoard board = new GameBoard();




    public bool CheckReady()
    {
        //방안에 있는 모든 애들 ready했는지 체크한다.
        foreach (var user in UserList)
        {
            if (user.IsReady == false)
            {
                return false;
            }
        }

        return true;
    }
    public bool CheckIsFull()
    {
        
        if(UserList.Count() == MaxUserCount)
        {
            return true;
        }
        return false;
    }
    public void SetRoomUserBeReady(string SessionId)
    {
        foreach (var user in UserList)
        {
            if (user.NetSessionID == SessionId)
            {
                user.IsReady = true;
            }

       
        }


    }


    public void Init(int index, int number, int maxUserCount)
    {
        Index = index;
        Number = number;
        MaxUserCount = maxUserCount;
    }

    public bool AddUser(string userId, string netSessionId)
    {
        if (GetUser(userId) != null)
        {
            return false;
        }

        var roomUser = new RoomUser();
        roomUser.Set(userId, netSessionId);
        UserList.Add(roomUser);

        return true;
    }
    public void RemoveUser(string netSessionID)
    {
        var index = UserList.FindIndex(x => x.NetSessionID == netSessionID);
        UserList.RemoveAt(index);
    }

    public bool RemoveUser(RoomUser user)
    {
        return UserList.Remove(user);
    }

    public RoomUser GetUser(string userID)
    {
        return UserList.Find(x => x.UserID == userID);
    }

    public RoomUser GetUserByNetSessionId(string netSessionID)
    {
        return UserList.Find(x => x.NetSessionID == netSessionID);
    }

    public int CurrentUserCount()
    {
        return UserList.Count();
    }

    public void NotifyPacketGameStart(string sessionId)
    {
        var packet = new SCGameStartPacket();

        //선을 누구로하지? 일단 0번ㄱㄱ
        packet.FirstUserID = UserList[0].UserID;


        var sendPacket = MemoryPackSerializer.Serialize(packet);
        MemorypackPacketHeadInfo.Write(sendPacket, PACKET_ID.NTF_START_GAME);

        Broadcast("", sendPacket);

        SetGame();    
    
    }

    public void NotifyPutOmok(int x, int y)
    {
        var temp = new NTFPutOmok();
        temp.PosX = x;
        temp.PosY = y;
        //temp.Mok =

        var sendPacket = MemoryPackSerializer.Serialize(temp);
        MemorypackPacketHeadInfo.Write(sendPacket, PACKET_ID.NTF_PUT_OMOK);


        Broadcast("", sendPacket);

    }

    public void NotifyPacketUserList(string userNetSessionID)
    {
        var packet = new PKTNtfRoomUserList();
        foreach (var user in UserList)
        {
            packet.UserIDList.Add(user.UserID);
        }

        var sendPacket = MemoryPackSerializer.Serialize(packet);
        MemorypackPacketHeadInfo.Write(sendPacket, PACKET_ID.NTF_ROOM_USER_LIST);

        NetworkSendFunc(userNetSessionID, sendPacket);
    }

    public void NofifyPacketNewUser(string newUserNetSessionID, string newUserID)
    {
        var packet = new PKTNtfRoomNewUser();
        packet.UserID = newUserID;

        var sendPacket = MemoryPackSerializer.Serialize(packet);
        MemorypackPacketHeadInfo.Write(sendPacket, PACKET_ID.NTF_ROOM_NEW_USER);

        Broadcast(newUserNetSessionID, sendPacket);
    }

    public void NotifyPacketLeaveUser(string userID)
    {
        if (CurrentUserCount() == 0)
        {
            return;
        }

        var packet = new PKTNtfRoomLeaveUser();
        packet.UserID = userID;

        var sendPacket = MemoryPackSerializer.Serialize(packet);
        MemorypackPacketHeadInfo.Write(sendPacket, PACKET_ID.NTF_ROOM_LEAVE_USER);

        Broadcast("", sendPacket);

    }

    public void Broadcast(string excludeNetSessionID, byte[] sendPacket)
    {
        foreach (var user in UserList)
        {
            if (user.NetSessionID == excludeNetSessionID)
            {
                continue;
            }

            NetworkSendFunc(user.NetSessionID, sendPacket);
        }
    }

    //게임관련 send 여기서
    public void SetGame()
    {
        board.SetPlayer(UserList[0].NetSessionID, UserList[0].UserID, true);
        board.SetPlayer(UserList[1].NetSessionID, UserList[1].UserID, false);

    }

    public void SetBoard(int x,int y)
    {
        board.SetBoard(x,y);
    }


}

public class RoomUser
{
    public string UserID { get; private set; }
    public string NetSessionID { get; private set; }

    public bool IsReady { get; set; }


    public void Set(string userID, string netSessionID)
    {
        UserID = userID;
        NetSessionID = netSessionID;
    }
}
