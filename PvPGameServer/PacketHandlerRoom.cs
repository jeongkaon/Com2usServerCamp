using CommandLine;
using MemoryPack;
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
    List<Room> RoomList = null;
    int RoomNumberStart;

    int StartCheckRoomNumber = 0;
    int CheckRoomNumberCount;
    int MaxRoomCheckCount;

    //TEST위해 10초로 일단 설정
    int span = 10000;


    public void SetRoomList(List<Room> roomList)
    {
        RoomList = roomList;
        RoomNumberStart = RoomList[0].Number;
        MaxRoomCheckCount = RoomList.Count();
        CheckRoomNumberCount = MaxRoomCheckCount / 4;

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
        packetHandlerMap.Add((int)PACKET_ID.REQ_ROOM_ENTER, RequestRoomEnter);
        packetHandlerMap.Add((int)PACKET_ID.REQ_ROOM_LEAVE, RequestLeave);
        packetHandlerMap.Add((int)PACKET_ID.NTF_IN_ROOM_LEAVE, NotifyLeaveInternal);
        packetHandlerMap.Add((int)PACKET_ID.REQ_ROOM_CHAT, RequestChat);
        packetHandlerMap.Add((int)PACKET_ID.REQ_READY_GAME, RequestGameReadyPacket);

        packetHandlerMap.Add((int)PACKET_ID.NTF_IN_ROOMCHECK, CheckInRoomState);


    }

    public void CheckInRoomState(MemoryPackBinaryRequestInfo requestData)
    {
        int EndCheckRoomNumber = StartCheckRoomNumber + CheckRoomNumberCount;
        if (EndCheckRoomNumber > MaxRoomCheckCount)
        {
            EndCheckRoomNumber = MaxRoomCheckCount;
        }

        for (int i= StartCheckRoomNumber; i< EndCheckRoomNumber; ++i)
        {
            var room = GetRoom(i);

            if (room.CurrentUserCount() == 0)
            {
                continue;
            }

            var curTime = DateTime.Now;

            //1.게임 시작 안하는 경우 - 입장은 했는데 게임시작을 안하는 경우
            //유저의 입장시간과 체크타임 텀이 긴경우체크
            var res = room.IsNotStartGame(curTime, span);
            //이거걸러야함

  
            //if (room.IsNotStartGame(curTime)!= ERROR_CODE.NONE)
            //{
            //    //TODO
            //    //너무 긴 경우 쫒아내던가 해야함
            //    //leaveroomuser쓰면될듯??
                
            //    //
            //}


            //2.턴체크 - 1명일때는 안해도됨, 근데 한명일때는 이미 위에서 걸러짐
            if (room.IsTimeOutInBoard(curTime, 10000/2))
            {
                room.NftToBoardTimeout();
            }

            //3.전체 게임시간 너무 긴경우 
            if (room.IsTooLongGameTime(curTime,10000))
            {
                //TODO
                //게임너무긴경우 처리해야한다.
            }

        }

        StartCheckRoomNumber += CheckRoomNumberCount;

        if (StartCheckRoomNumber >= RoomList.Count())
        {
            StartCheckRoomNumber = 0;
        }

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

            var reqData = MemoryPackSerializer.Deserialize<ReqRoomEnterPacket>(packetData.Data);

            var room = GetRoom(reqData.RoomNumber);


            if (room == null)
            {
                ResponseEnterRoomToClient(ERROR_CODE.ROOM_ENTER_INVALID_ROOM_NUMBER, sessionID);
                return;
            }

            if(room.CheckIsFull())
            {
                ResponseEnterRoomToClient(ERROR_CODE.ROOM_ENTER_FAILED_USERFULL, sessionID);

                Console.WriteLine("방 다참");
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
        var resRoomEnter = new ResRoomEnterPacket()
        {
            Result = (short)errorCode
        };

        var sendPacket = MemoryPackSerializer.Serialize(resRoomEnter);
        PacketHeadInfo.Write(sendPacket, PACKET_ID.RES_ROOM_ENTER);

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

        var userID = roomUser.UserID;
        room.RemoveUser(roomUser);

        room.NotifyPacketLeaveUser(userID);
        return true;
    }

    void ResponseLeaveRoomToClient(string sessionID)
    {
        var resRoomLeave = new ResRoomLeavePacket()
        {
            Result = (short)ERROR_CODE.NONE
        };

        var sendPacket = MemoryPackSerializer.Serialize(resRoomLeave);
        PacketHeadInfo.Write(sendPacket, PACKET_ID.RES_ROOM_LEAVE);

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


            var reqData = MemoryPackSerializer.Deserialize<ReqRoomChat>(packetData.Data);

            var notifyPacket = new NtfRoomChat()
            {
                UserID = roomObject.Item3.UserID,
                ChatMessage = reqData.ChatMessage
            };

            var sendPacket = MemoryPackSerializer.Serialize(notifyPacket);
            PacketHeadInfo.Write(sendPacket, PACKET_ID.NTF_ROOM_CHAT);

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
