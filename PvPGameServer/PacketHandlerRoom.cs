﻿using MemoryPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PvPGameServer;

public class PacketHandlerRoom : PacketHandler
{
    List<Room> RoomList = null;
    int RoomNumberStart;

    public void SetRoomList(List<Room> roomList)
    {
        RoomList = roomList;
        RoomNumberStart = RoomList[0].Number;
    }

    Room GetRoom(int roomNum)
    {
        var idx = roomNum - RoomNumberStart;

        if (idx < 0 || idx >=RoomList.Count())
        {
            return null;
        }

        return RoomList[idx];
    }
    (bool, Room, RoomUser) CheckRoomAndRoomUser(string userSessionId)
    {
        var user = UserMgr.GetUser(userSessionId);
        if (user == null)
        {
            return (false, null, null);
        }

        var roomNumber = user.RoomNumber;
        var room = GetRoom(roomNumber);

        if (room == null)
        {
            return (false, null, null);
        }

        var roomUser = room.GetUserByNetSessionId(userSessionId);

        if (roomUser == null)
        {
            return (false, room, null);
        }

        return (true, room, roomUser);

    }

    public void RegistPacketHandler(Dictionary<int, Action<MemoryPackBinaryRequestInfo>> packetHandlerMap)
    {
        packetHandlerMap.Add((int)PACKET_ID.CS_ROOM_ENTER, RequestRoomEnter);
        packetHandlerMap.Add((int)PACKET_ID.CS_ROOM_LEAVE, RequestLeave);
        packetHandlerMap.Add((int)PACKET_ID.NTF_IN_ROOM_LEAVE, NotifyLeaveInternal);
        packetHandlerMap.Add((int)PACKET_ID.CS_ROOM_CHAT, RequestChat);
    }


    public void RequestRoomEnter(MemoryPackBinaryRequestInfo packetData)
    {
        var sessionID = packetData.SessionID;
        MainServer.MainLogger.Debug("RequestRoomEnter");

        try
        {
            var user = UserMgr.GetUser(sessionID);

            if (user == null || user.IsConfirm(sessionID) == false)
            {
                ResponseEnterRoomToClient(ERROR_CODE.ROOM_ENTER_INVALID_USER, sessionID);
                return;
            }

            if (user.IsStateRoom())
            {
                ResponseEnterRoomToClient(ERROR_CODE.ROOM_ENTER_INVALID_STATE, sessionID);
                return;
            }

            var reqData = MemoryPackSerializer.Deserialize<CSRoomEnterPacket>(packetData.Data);

            var room = GetRoom(reqData.RoomNumber);

            if (room == null)
            {
                ResponseEnterRoomToClient(ERROR_CODE.ROOM_ENTER_INVALID_ROOM_NUMBER, sessionID);
                return;
            }

            if (room.AddUser(user.ID(), sessionID) == false)
            {
                ResponseEnterRoomToClient(ERROR_CODE.ROOM_ENTER_FAIL_ADD_USER, sessionID);
                return;
            }


            user.EnteredRoom(reqData.RoomNumber);

            room.NotifyPacketUserList(sessionID);
            room.NofifyPacketNewUser(sessionID, user.ID());

            ResponseEnterRoomToClient(ERROR_CODE.NONE, sessionID);

            MainServer.MainLogger.Debug("RequestEnterInternal - Success");
        }
        catch (Exception ex)
        {
            MainServer.MainLogger.Error(ex.ToString());
        }
    }

    void ResponseEnterRoomToClient(ERROR_CODE errorCode, string sessionID)
    {
        var resRoomEnter = new SCRoomEnterPacket()
        {
            Result = (short)errorCode
        };

        var sendPacket = MemoryPackSerializer.Serialize(resRoomEnter);
        MemorypackPacketHeadInfo.Write(sendPacket, PACKET_ID.SC_ROOM_LEAVE);

        NetworkSendFunc(sessionID, sendPacket);
    }

    public void RequestLeave(MemoryPackBinaryRequestInfo packetData)
    {
        var sessionID = packetData.SessionID;
        MainServer.MainLogger.Debug("방나가기 요청 받음");

        try
        {
            var user = UserMgr.GetUser(sessionID);
            if (user == null)
            {
                return;
            }

            if (LeaveRoomUser(sessionID, user.RoomNumber) == false)
            {
                return;
            }

            user.LeaveRoom();

            ResponseLeaveRoomToClient(sessionID);

            MainServer.MainLogger.Debug("Room RequestLeave - Success");
        }
        catch (Exception ex)
        {
            MainServer.MainLogger.Error(ex.ToString());
        }
    }

    bool LeaveRoomUser(string sessionID, int roomNumber)
    {
        MainServer.MainLogger.Debug($"LeaveRoomUser. SessionID:{sessionID}");

        var room = GetRoom(roomNumber);
        if (room == null)
        {
            return false;
        }

        var roomUser = room.GetUserByNetSessionId(sessionID);
        if (roomUser == null)
        {
            return false;
        }

        var userID = roomUser.UserID;
        room.RemoveUser(roomUser);

        room.NotifyPacketLeaveUser(userID);
        return true;
    }

    void ResponseLeaveRoomToClient(string sessionID)
    {
        var resRoomLeave = new SCRoomLeavePacket()
        {
            Result = (short)ERROR_CODE.NONE
        };

        var sendPacket = MemoryPackSerializer.Serialize(resRoomLeave);
        MemorypackPacketHeadInfo.Write(sendPacket, PACKET_ID.SC_ROOM_LEAVE);

        NetworkSendFunc(sessionID, sendPacket);
    }

    public void NotifyLeaveInternal(MemoryPackBinaryRequestInfo packetData)
    {
        var sessionID = packetData.SessionID;
        MainServer.MainLogger.Debug($"NotifyLeaveInternal. SessionID: {sessionID}");

        var reqData = MemoryPackSerializer.Deserialize<PKTInternalNtfRoomLeave>(packetData.Data);
        LeaveRoomUser(sessionID, reqData.RoomNumber);
    }

    public void RequestChat(MemoryPackBinaryRequestInfo packetData)
    {
        var sessionID = packetData.SessionID;
        MainServer.MainLogger.Debug("Room RequestChat");

        try
        {
            var roomObject = CheckRoomAndRoomUser(sessionID);

            if (roomObject.Item1 == false)
            {
                return;
            }


            var reqData = MemoryPackSerializer.Deserialize<PKTReqRoomChat>(packetData.Data);

            var notifyPacket = new PKTNtfRoomChat()
            {
                UserID = roomObject.Item3.UserID,
                ChatMessage = reqData.ChatMessage
            };

            var sendPacket = MemoryPackSerializer.Serialize(notifyPacket);
            MemorypackPacketHeadInfo.Write(sendPacket, PACKET_ID.NTF_ROOM_CHAT);

            roomObject.Item2.Broadcast("", sendPacket);

            MainServer.MainLogger.Debug("Room RequestChat - Success");
        }
        catch (Exception ex)
        {
            MainServer.MainLogger.Error(ex.ToString());
        }
    }


}
