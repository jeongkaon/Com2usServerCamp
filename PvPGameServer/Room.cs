using MemoryPack;
using NLog.Targets;
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

    List<RoomUser> RoomUserList = new List<RoomUser>();

    public static Func<string, byte[], bool> NetworkSendFunc;

    GameBoard board = null;

    DateTime RoomStartTime;     //한명이 들어오면 체크?
    DateTime GameStartTime;     
    public void Init(int index, int number, int maxUserCount)
    {
        Index = index;
        Number = number;
        MaxUserCount = maxUserCount;

        board = new GameBoard(Number, NetworkSendFunc);
    }


    public bool CheckIsFull()
    {
        
        if(RoomUserList.Count() == MaxUserCount)
        {
            return true;
        }
        return false;
    }

    
    //이함수는 더 생각해봐야한다.!!!
    public ERROR_CODE IsNotStartGame(DateTime cur, int span)
    {
        var diff = cur - RoomStartTime;

        if(diff.TotalMilliseconds <= span)
        {
            return ERROR_CODE.NONE;
        }

        //이미 방대기상태가 긴상태
        //1명만 입장한 상태
        if (RoomUserList.Count() ==1 )
        {
            return ERROR_CODE.ROONCHECK_INPUT_ONEPLYAER;
        }

        //2-1. 2명 다 레디를 안하는 경우
        //TODO_나가게하기
        foreach (var user in RoomUserList)
        {
            if(user.isReady == false)
            {
            }
        }

        if (board.ReadyPlayerCount() == 0)
        {
            return ERROR_CODE.ROOMCHECK_TWOPLAYERS_NOTREADY;
        }

        if (board.ReadyPlayerCount() == 1)
        {
            return ERROR_CODE.ROOMCHECK_ONEPLYAER_NOTREADY;
        }           
        
        return ERROR_CODE.NONE;
    }

    public void CheckTimeOutPlayerTurn(DateTime cur, int TimeSpan)
    {
        if (true == board.TimeOutCheck(cur, TimeSpan))
        {
            NftBoardTurnTimeout();
        }
    }


    public void NftBoardTurnTimeout()
    {
        board.NotifyTimeOut();

        var stone = board.CheckPassCount();
        if(stone != STONE_TYPE.NONE)
        {
            board.EndGame(stone);
            return;
        }

        board.TurnChange();


    }

    public void CheckTooLongGameTime(DateTime cur, int TimeSpan)
    {
        var diff = cur - GameStartTime;

        if (diff.TotalMilliseconds > TimeSpan)
        {
            board.NotifyWinner(STONE_TYPE.NONE);
        }
    }

    public bool AddUser(string userId, string netSessionId)
    {
        if (GetUser(userId) != null)
        {
            return false;
        }

        if (RoomUserList.Count() == 0)
        {
            RoomStartTime = DateTime.Now;   
        }

        var roomUser = new RoomUser();
        roomUser.Set(userId, netSessionId);
        RoomUserList.Add(roomUser);
       

        return true;
    }
    public void RemoveUser(string netSessionID)
    {
        var index = RoomUserList.FindIndex(x => x.NetSessionID == netSessionID);
        RoomUserList.RemoveAt(index);
    }

    public bool RemoveUser(RoomUser user)
    {
        return RoomUserList.Remove(user);
    }

    public RoomUser GetUser(string userID)
    {
        return RoomUserList.Find(x => x.UserID == userID);
    }

    public RoomUser GetUserByNetSessionId(string netSessionID)
    {
        return RoomUserList.Find(x => x.NetSessionID == netSessionID);
    }

    public int CurrentUserCount()
    {
        return RoomUserList.Count();
    }
    
    (string ,string ) GetAllPlayerId()
    {
        return (RoomUserList[0].UserID, RoomUserList[1].UserID);
    }
    
    public GameBoard GetGameBoard()
    {
        return board;
    }

    public void GameStart()
    {
        GameStartTime = DateTime.Now;
        board.GameStart();
    }

    public void NotifyPacketUserList(string userNetSessionID)
    {
        var packet = new NtfRoomUserList();
        foreach (var user in RoomUserList)
        {
            packet.UserIDList.Add(user.UserID);
        }

        var sendPacket = MemoryPackSerializer.Serialize(packet);
        PacketHeadInfo.Write(sendPacket, PACKET_ID.NTF_ROOM_USER_LIST);

        NetworkSendFunc(userNetSessionID, sendPacket);
    }

    public void NofifyPacketNewUser(string newUserNetSessionID, string newUserID)
    {
        var packet = new NtfRoomNewUser();
        packet.UserID = newUserID;

        var sendPacket = MemoryPackSerializer.Serialize(packet);
        PacketHeadInfo.Write(sendPacket, PACKET_ID.NTF_ROOM_NEW_USER);

        Broadcast(newUserNetSessionID, sendPacket);
    }

    public void NotifyPacketLeaveUser(string userID)
    {
        if (CurrentUserCount() == 0)
        {
            return;
        }

        var packet = new NtfRoomLeaveUser();
        packet.UserID = userID;

        var sendPacket = MemoryPackSerializer.Serialize(packet);
        PacketHeadInfo.Write(sendPacket, PACKET_ID.NTF_ROOM_LEAVE_USER);

        Broadcast("", sendPacket);

    }

    public void Broadcast(string excludeNetSessionID, byte[] sendPacket)
    {
        foreach (var user in RoomUserList)
        {
            if (user.NetSessionID == excludeNetSessionID)
            {
                continue;
            }

            NetworkSendFunc(user.NetSessionID, sendPacket);
        }
    }

    public void SetRoomUserBeReady(string SessionId)
    {
        foreach (var user in RoomUserList)
        {
            if (user.NetSessionID == SessionId)
            {
                user.ReadyTime = DateTime.Now;
                user.isReady = true;
                var packet = new ResGameReadyPacket();

                packet.PlayerStoneType = board.SetPlayer(SessionId, user.UserID);

                var sendPacket = MemoryPackSerializer.Serialize(packet);
                PacketHeadInfo.Write(sendPacket, PACKET_ID.RES_READY_GAME);

                NetworkSendFunc(SessionId, sendPacket);

            }
        }

        if (board.ReadyPlayerCount()==2)
        {
            NotifyPlayersGameStart();

        }
    }

    public void NotifyPlayersGameStart()
    {
        var players = GetAllPlayerId();

        var packet = new NftGameStartPacket();
        packet.p1 = players.Item1;
        packet.p2 = players.Item2;

        var sendPacket = MemoryPackSerializer.Serialize(packet);
        PacketHeadInfo.Write(sendPacket, PACKET_ID.NTF_START_GAME);

        Broadcast("", sendPacket);

        MainServer.MainLogger.Debug("GameStart- Success");

        GameStart();
    }
}

public class RoomUser
{
    public string UserID { get; private set; }
    public string NetSessionID { get; private set; }

    public DateTime AcceptTime { get; set; }

    public DateTime ReadyTime { get; set; }

    public bool isReady { get; set; }   

    public void Set(string userID, string netSessionID)
    {
        UserID = userID;
        NetSessionID = netSessionID;

        AcceptTime = DateTime.Now;
    }
}
