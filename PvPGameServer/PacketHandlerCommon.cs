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
        packetHandlerMap.Add((int)PACKET_ID.NTR_IN_CHECK, NotifyInUserCheck);
        packetHandlerMap.Add((int)PACKET_ID.NTF_IN_FORCEDISCONNECT_CLIENT, NotifyInForceDisConnectClient);


    }
    public void NotifyInUserCheck(MemoryPackBinaryRequestInfo requestData)
    {
        //User 조사
        //valid한 유저인지를 조사해야함

        //user mgr 리스트에 들어가서 하트비트 체크하면된다.
        int endIdx = UserCheckStartIndex + MaxUserCheckCount;
     
        UserMgr.CheckHeartBeat(UserCheckStartIndex, endIdx);


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
        var user = UserMgr.GetUser(sessionID);

        if (user != null)
        {
            UserMgr.RemoveUser(sessionID);
            ForceSession(sessionID);
        }
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
        //하트비트 받았으면 
        var sessionID = recvData.SessionID;

        var user = UserMgr.GetUser(sessionID);
        if (user != null)
        {
            //유저 없다고 알려줘야하나? 일단 넘겨~
            //ResHeartBeatPacket(ERROR_CODE.)
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



