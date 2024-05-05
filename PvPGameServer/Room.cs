﻿using MemoryPack;
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

    public GameBoard board = null;

    //방 만들어진 시간저장
    //


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



    public bool AddUser(string userId, string netSessionId)
    {
        if (GetUser(userId) != null)
        {
            return false;
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
        //2명밖에 없으니까 그냥 foreach쓸까?, 아님 함수타고 드러가???
        foreach (var user in RoomUserList)
        {
            if (user.NetSessionID == SessionId)
            {
                var packet = new ResGameReadyPacket();

                //게임플레이 누른 플레이어에게 어떤 돌타입인지 알려주려고 넣은거임
                packet.PlayerStoneType = board.SetPlayer(SessionId, user.UserID);

                var sendPacket = MemoryPackSerializer.Serialize(packet);
                PacketHeadInfo.Write(sendPacket, PACKET_ID.RES_READY_GAME);

                NetworkSendFunc(SessionId, sendPacket);

            }
        }

        //board에서 인원수 체크하기
        if (board.CheckIsFull())
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

        //게임시작보내고 타이머 돌려야함??
        board.GameStart();

    }
}

public class RoomUser
{
    public string UserID { get; private set; }
    public string NetSessionID { get; private set; }

    

    public void Set(string userID, string netSessionID)
    {
        UserID = userID;
        NetSessionID = netSessionID;
    }
}
