using MemoryPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PvPGameServer;

public class PacketHandlerCommon : PacketHandler
{

    int _maxUserCheckCount;     
    int _userCheckStartIndex;
    Action<MemoryPackBinaryRequestInfo> _distributeInnerPacketDB;

    public void SetCheckCount(int maxUserCheck)
    {
        _userCheckStartIndex = 0;
        _maxUserCheckCount = maxUserCheck;
    }
    public void GetDistributeGameDB(Action<MemoryPackBinaryRequestInfo> distribute)
    {
        _distributeInnerPacketDB = distribute;
    }
    public void RegistPacketHandler(Dictionary<int, Action<MemoryPackBinaryRequestInfo>> packetHandlerMap)
    {
        packetHandlerMap.Add((int)PacketId.NtfInLoginCheck, ReqLoginPacket);
        packetHandlerMap.Add((int)PacketId.NtfInLoginFailedAuthToken, ReqLoginFailAuthTokenPacket);

        packetHandlerMap.Add((int)PacketId.NtfInConnectClient, NotifyInConnectClient);
        packetHandlerMap.Add((int)PacketId.NtfInDisconnectClient, NotifyInDisConnectClient);
        packetHandlerMap.Add((int)PacketId.ReqHeartBeat, ReqHeartBeatPacket);
        packetHandlerMap.Add((int)PacketId.NtrInUserCheck, NotifyInUserCheck);
        packetHandlerMap.Add((int)PacketId.NtfInForceDisconnectClient, NotifyInForceDisConnectClient);
    }
    public void NotifyInUserCheck(MemoryPackBinaryRequestInfo requestData)
    {
        int endIdx = _userCheckStartIndex + _maxUserCheckCount;
     
        var value = _userMgr.CheckHeartBeat(_userCheckStartIndex, endIdx);


        _userCheckStartIndex += endIdx;
        if(_userCheckStartIndex >= _maxUserCheckCount)
        {
            _userCheckStartIndex = 0;
        }
    }
    public void NotifyInConnectClient(MemoryPackBinaryRequestInfo requestData)
    {
    }
    public void NotifyInDisConnectClient(MemoryPackBinaryRequestInfo requestData)
    {
       
        var sessionID = requestData.SessionID;
        var user = _userMgr.GetUser(sessionID);

        if (user != null)
        {
            var roomNum = user.RoomNumber;

            if (roomNum != Room.InvalidRoomNumber)
            {
                var internalPacket = InnerPacketMaker.MakeNTFInnerRoomLeavePacket(sessionID, roomNum, user.ID());
                DistributeInnerPacket(internalPacket);
            }

            _userMgr.RemoveUser(sessionID);
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
        var reqData = MemoryPackSerializer.Deserialize<ReqLoginPacket>(recvData.Data);
        
        try
        {
            if (_userMgr.GetUser(sessionID) != null)
            {
                SendLoginToClient(ErrorCode.LoginAlreadyWorking, recvData.SessionID);
                return;
            }

            var errorCode = _userMgr.AddUser(reqData.UserID, sessionID);
            if (errorCode != ErrorCode.None)
            {
                SendLoginToClient(errorCode, recvData.SessionID);

                if (errorCode == ErrorCode.LoginFullUserCount)
                {
                    NotifyMustCloseToClient(ErrorCode.LoginFullUserCount, recvData.SessionID);
                }

                return;
            }

            SendLoginToClient(errorCode, recvData.SessionID);

            var innerPacket = InnerPacketMaker.MakeNTFInnerGetUserDataInDB(reqData.UserID);
            innerPacket.SessionID = sessionID;
            _distributeInnerPacketDB(innerPacket);

            MainServer.MainLogger.Debug($"로그인 결과. UserID:{reqData.UserID}, {errorCode}");

        }
        catch (Exception ex)
        {
            MainServer.MainLogger.Error(ex.ToString());
        }

    }
    public void ReqLoginFailAuthTokenPacket(MemoryPackBinaryRequestInfo recvData)
    {
        var sessionID = recvData.SessionID;
        SendLoginToClient(ErrorCode.FailVerifyUserToken, recvData.SessionID);
        

    }
    public void ReqHeartBeatPacket(MemoryPackBinaryRequestInfo recvData)
    {
        var sessionID = recvData.SessionID;

        var user = _userMgr.GetUser(sessionID);
        if (user != null)
        {
            user.UpdateHeartBeatTime(DateTime.Now);
            ResHeartBeatPacket(ErrorCode.None, sessionID);

        }

    }
    public void ResHeartBeatPacket(ErrorCode errorCode, string sessionId)
    {
        var temp = new ResHeartBeatPacket()
        {
            Result = (short)ErrorCode.None

        };

        var sendData = MemoryPackSerializer.Serialize(temp);
        PacketHeadInfo.Write(sendData, PacketId.ResHeartBeat);
        NetworkSendFunc(sessionId, sendData);

    }
    public void SendLoginToClient(ErrorCode errorCode, string sessionID)
    {
        var resLogin = new ResLoginPacket()
        {
            Result = (short)errorCode
        };

        var sendData = MemoryPackSerializer.Serialize(resLogin);
        PacketHeadInfo.Write(sendData, PacketId.ResLogin);

        NetworkSendFunc(sessionID, sendData);
    }
    public void NotifyMustCloseToClient(ErrorCode errorCode, string sessionID)
    {
        var resLogin = new NtfMustClosePacket()
        {
            Result = (short)errorCode
        };

        var sendData = MemoryPackSerializer.Serialize(resLogin);
        PacketHeadInfo.Write(sendData, PacketId.NtfMustClose);

        NetworkSendFunc(sessionID, sendData);
    }

}




