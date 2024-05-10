using CommandLine;
using MemoryPack;
using SqlKata;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PvPGameServer;

public class PacketHandlerRoom : PacketHandler
{
    List<Room> _roomList = null;
    int _roomNumberStart;

    int _startCheckRoomNumber = 0;
    int _checkRoomNumberCount;
    int _maxRoomCheckCount;

    public void SetRoomList(List<Room> roomList)
    {
        _roomList = roomList;
        _roomNumberStart = _roomList[0].Number;
        _maxRoomCheckCount = _roomList.Count();
        _checkRoomNumberCount = _maxRoomCheckCount / 4;

    }

    Room GetRoom(int roomNum)
    {
        var idx = roomNum - _roomNumberStart;

        if (idx < 0 || idx >=_roomList.Count())
        {
            return null;
        }

        return _roomList[idx];
    }
    (bool, Room, RoomUser) CheckRoomAndRoomUser(string userSessionId)
    {
        var user = _userMgr.GetUser(userSessionId);
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
        packetHandlerMap.Add((int)PacketId.ReqRoomEnter, RequestRoomEnter);
        packetHandlerMap.Add((int)PacketId.ReqRoomLeave, RequestLeave);
        packetHandlerMap.Add((int)PacketId.NtfIntRoomLeave, NotifyLeaveInternal);
        packetHandlerMap.Add((int)PacketId.ReqRoomChat, RequestChat);
        packetHandlerMap.Add((int)PacketId.ReqReadyGame, RequestGameReadyPacket);
        packetHandlerMap.Add((int)PacketId.NtfInRoomCheck, CheckInRoomState);
    }



    public void CheckInRoomState(MemoryPackBinaryRequestInfo requestData)
    {
        int EndCheckRoomNumber = _startCheckRoomNumber + _checkRoomNumberCount;
        if (EndCheckRoomNumber > _maxRoomCheckCount)
        {
            EndCheckRoomNumber = _maxRoomCheckCount;
        }

        for (int i= _startCheckRoomNumber; i< EndCheckRoomNumber; ++i)
        {
            var room = GetRoom(i);
            if (room.CurrentUserCount() == 0 || room.CheckIsFull()==false  )
            {
                continue;
            }
            var curTime = DateTime.Now;

            int testspan = 10000; //10초
            room.CheckTimeOutPlayerTurn(curTime, testspan);
            room.CheckTooLongGameTime(curTime, 600000);
        }

        _startCheckRoomNumber += _checkRoomNumberCount;

        if (_startCheckRoomNumber >= _roomList.Count())
        {
            _startCheckRoomNumber = 0;
        }

    }
    public void RequestRoomEnter(MemoryPackBinaryRequestInfo packetData)
    {
        var sessionID = packetData.SessionID;
        MainServer.MainLogger.Debug("RequestRoomEnter");

        try
        {
            var user = _userMgr.GetUser(sessionID);
            


            if (user == null || user.IsConfirm(sessionID) == false)
            {
                ResponseEnterRoomToClient(ErrorCode.RoomEnterInvalidUser, sessionID);
                return;
            }

            if (user.IsStateRoom())
            {
                ResponseEnterRoomToClient(ErrorCode.RoomEnterInvalidState, sessionID);
                return;
            }

            var reqData = MemoryPackSerializer.Deserialize<ReqRoomEnterPacket>(packetData.Data);

            var room = GetRoom(reqData.RoomNumber);


            if (room == null)
            {
                ResponseEnterRoomToClient(ErrorCode.RoomEnterInvalidRoomNumber, sessionID);
                return;
            }

            if(room.CheckIsFull())
            {
                ResponseEnterRoomToClient(ErrorCode.RoomEnterFaildUserFull, sessionID);

                Console.WriteLine("방 다참");
                return;
            }

            if (room.AddUser(user.ID(), sessionID) == false)
            {
                ResponseEnterRoomToClient(ErrorCode.RoomEnterFailAddUser, sessionID);
                return;
            }



            user.EnteredRoom(reqData.RoomNumber);

            room.NotifyPacketUserList(sessionID);
            room.NofifyPacketNewUser(sessionID, user.ID());

            ResponseEnterRoomToClient(ErrorCode.None, sessionID);

            MainServer.MainLogger.Debug("RequestEnterInternal - Success");
        }
        catch (Exception ex)
        {
            MainServer.MainLogger.Error(ex.ToString());
        }
    }

    void ResponseEnterRoomToClient(ErrorCode errorCode, string sessionID)
    {
        var resRoomEnter = new ResRoomEnterPacket()
        {
            Result = (short)errorCode
        };

        var sendPacket = MemoryPackSerializer.Serialize(resRoomEnter);
        PacketHeadInfo.Write(sendPacket, PacketId.ResRoomEnter);

        NetworkSendFunc(sessionID, sendPacket);
    }

    public void RequestLeave(MemoryPackBinaryRequestInfo packetData)
    {
        var sessionID = packetData.SessionID;
        MainServer.MainLogger.Debug("방나가기 요청 받음");

        try
        {
            var user = _userMgr.GetUser(sessionID);
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
        Console.WriteLine("LeaveRoomUser ");

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

        var userID = roomUser.UserId;
        room.RemoveUser(roomUser);

        room.NotifyPacketLeaveUser(userID);
        return true;
    }

    void ResponseLeaveRoomToClient(string sessionID)
    {
        var resRoomLeave = new ResRoomLeavePacket()
        {
            Result = (short)ErrorCode.None
        };

        var sendPacket = MemoryPackSerializer.Serialize(resRoomLeave);
        PacketHeadInfo.Write(sendPacket, PacketId.ResRoomLeave);

        NetworkSendFunc(sessionID, sendPacket);
    }

    public void NotifyLeaveInternal(MemoryPackBinaryRequestInfo packetData)
    {
        var sessionID = packetData.SessionID;
        MainServer.MainLogger.Debug($"NotifyLeaveInternal. SessionID: {sessionID}");

        var reqData = MemoryPackSerializer.Deserialize<PKTInternalNtfRoomLeave>(packetData.Data);
        LeaveRoomUser(sessionID, reqData._roomNumber);
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


            var reqData = MemoryPackSerializer.Deserialize<ReqRoomChat>(packetData.Data);

            var notifyPacket = new NtfRoomChat()
            {
                UserID = roomObject.Item3.UserId,
                ChatMessage = reqData.ChatMessage
            };

            var sendPacket = MemoryPackSerializer.Serialize(notifyPacket);
            PacketHeadInfo.Write(sendPacket, PacketId.NtfRoomChat);

            roomObject.Item2.Broadcast("", sendPacket);

            MainServer.MainLogger.Debug("Room RequestChat - Success");
        }
        catch (Exception ex)
        {
            MainServer.MainLogger.Error(ex.ToString());
        }
    }

    public void RequestGameReadyPacket(MemoryPackBinaryRequestInfo packetData)
    {
        var reqData = MemoryPackSerializer.Deserialize<ReqGameReadyPacket>(packetData.Data);
        var roomNumber = reqData.RoomNumber;
        var sessionID = packetData.SessionID;

        var room = GetRoom(roomNumber);
        room.SetRoomUserBeReady(sessionID);

        MainServer.MainLogger.Debug("Room Game Ready Recv - Success");
    }



}
