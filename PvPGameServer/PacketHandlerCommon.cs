using MemoryPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PvPGameServer;

public class PacketHandlerCommon : PacketHandler
{

    int MaxUserCheckCount;     
    int UserCheckStartIndex;



    public void SetCheckCount(int maxUserCheck)
    {
        UserCheckStartIndex = 0;
        MaxUserCheckCount = maxUserCheck;
    }

    public void RegistPacketHandler(Dictionary<int, Action<MemoryPackBinaryRequestInfo>> packetHandlerMap)
    {
        packetHandlerMap.Add((int)PACKET_ID.REQ_LOGIN, ReqLoginPacket);
        packetHandlerMap.Add((int)PACKET_ID.NTF_IN_CONNECT_CLIENT, NotifyInConnectClient);
        packetHandlerMap.Add((int)PACKET_ID.NTF_IN_DISCONNECT_CLIENT, NotifyInDisConnectClient);
        packetHandlerMap.Add((int)PACKET_ID.REQ_HEARTBEAT, ReqHeartBeatPacket);
        packetHandlerMap.Add((int)PACKET_ID.NTR_IN_USERCHECK, NotifyInUserCheck);
        packetHandlerMap.Add((int)PACKET_ID.NTF_IN_FORCEDISCONNECT_CLIENT, NotifyInForceDisConnectClient);


    }
    public void NotifyInUserCheck(MemoryPackBinaryRequestInfo requestData)
    {
        int endIdx = UserCheckStartIndex + MaxUserCheckCount;
     
        var value = UserMgr.CheckHeartBeat(UserCheckStartIndex, endIdx);

        //강종당한 user가 만약 방에 들어있었으면 방에 있는사람들한테도 알려줘야한다.
        if(value.Item2 != -1)
        {
            var internalPacket = InnerPacketMaker.MakeNTFInnerRoomLeavePacket(value.Item1, value.Item2, value.Item3);
            DistributeInnerPacket(internalPacket);
        }

        UserCheckStartIndex += endIdx;
        if(UserCheckStartIndex >= MaxUserCheckCount)
        {
            UserCheckStartIndex = 0;
        }
    }

    public void NotifyInConnectClient(MemoryPackBinaryRequestInfo requestData)
    {
    }
    public void NotifyInDisConnectClient(MemoryPackBinaryRequestInfo requestData)
    {
       
        var sessionID = requestData.SessionID;
        var user = UserMgr.GetUser(sessionID);

        if (user != null)
        {
            var roomNum = user.RoomNumber;

            if (roomNum != Room.InvalidRoomNumber)
            {
                var internalPacket = InnerPacketMaker.MakeNTFInnerRoomLeavePacket(sessionID, roomNum, user.ID());
                DistributeInnerPacket(internalPacket);
            }

            UserMgr.RemoveUser(sessionID);
        }

    }
    public void NotifyInForceDisConnectClient(MemoryPackBinaryRequestInfo requestData)
    {
        var sessionID = requestData.SessionID;
        ForceSession(sessionID);
    }

    public void ReqLoginPacket(MemoryPackBinaryRequestInfo recvData)
    {
        var sessionID = recvData.SessionID;

        try
        {
            if (UserMgr.GetUser(sessionID) != null)
            {
                SendLoginToClient(ERROR_CODE.LOGIN_ALREADY_WORKING, recvData.SessionID);
                return;
            }

            var reqData = MemoryPackSerializer.Deserialize<ReqLoginPacket>(recvData.Data);
            var errorCode = UserMgr.AddUser(reqData.UserID, sessionID);
            if (errorCode != ERROR_CODE.NONE)
            {
                SendLoginToClient(errorCode, recvData.SessionID);

                if (errorCode == ERROR_CODE.LOGIN_FULL_USER_COUNT)
                {
                    NotifyMustCloseToClient(ERROR_CODE.LOGIN_FULL_USER_COUNT, recvData.SessionID);
                }

                return;
            }

            SendLoginToClient(errorCode, recvData.SessionID);

            MainServer.MainLogger.Debug($"로그인 결과. UserID:{reqData.UserID}, {errorCode}");

        }
        catch (Exception ex)
        {
            MainServer.MainLogger.Error(ex.ToString());
        }

    }
    public void ReqHeartBeatPacket(MemoryPackBinaryRequestInfo recvData)
    {
        var sessionID = recvData.SessionID;

        var user = UserMgr.GetUser(sessionID);
        if (user != null)
        {
            user.UpdateHeartBeatTime(DateTime.Now);
            ResHeartBeatPacket(ERROR_CODE.NONE, sessionID);

        }

    }
    public void ResHeartBeatPacket(ERROR_CODE errorCode, string sessionId)
    {
        var temp = new ResHeartBeatPacket()
        {
            Result = (short)ERROR_CODE.NONE

        };

        var sendData = MemoryPackSerializer.Serialize(temp);
        PacketHeadInfo.Write(sendData, PACKET_ID.RES_HEARTBEAT);
        NetworkSendFunc(sessionId, sendData);

    }
    public void SendLoginToClient(ERROR_CODE errorCode, string sessionID)
    {
        var resLogin = new ResLoginPacket()
        {
            Result = (short)errorCode
        };

        var sendData = MemoryPackSerializer.Serialize(resLogin);
        PacketHeadInfo.Write(sendData, PACKET_ID.RES_LOGIN);

        NetworkSendFunc(sessionID, sendData);
    }
    public void NotifyMustCloseToClient(ERROR_CODE errorCode, string sessionID)
    {
        var resLogin = new NtfMustClosePacket()
        {
            Result = (short)errorCode
        };

        var sendData = MemoryPackSerializer.Serialize(resLogin);
        PacketHeadInfo.Write(sendData, PACKET_ID.NTF_MUST_CLOSE);

        NetworkSendFunc(sessionID, sendData);
    }

}



