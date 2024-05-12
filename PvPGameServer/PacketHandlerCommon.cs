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
        //일단 테스트용으로 레디스에 가는지 확인하기위해 login그거를 주석하겠습니다.
        packetHandlerMap.Add((int)PacketId.NtfInLoginCheck, ReqLoginPacket);

        packetHandlerMap.Add((int)PacketId.NtfInConnectClient, NotifyInConnectClient);
        packetHandlerMap.Add((int)PacketId.NtfInDisconnectClient, NotifyInDisConnectClient);
        packetHandlerMap.Add((int)PacketId.ReqHeartBeat, ReqHeartBeatPacket);
        packetHandlerMap.Add((int)PacketId.NtrInUserCheck, NotifyInUserCheck);
        packetHandlerMap.Add((int)PacketId.NtfInForceDisconnectClient, NotifyInForceDisConnectClient);

    }
    public void NotifyInUserCheck(MemoryPackBinaryRequestInfo requestData)
    {
        int endIdx = UserCheckStartIndex + MaxUserCheckCount;
     
        var value = _userMgr.CheckHeartBeat(UserCheckStartIndex, endIdx);


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
        
        //DATE를 뽑아서 타입캐스팅 시키고싶은디..c#은 바이트배열을 클래스로 타입캐스팅하는법없움!
        var reqData = MemoryPackSerializer.Deserialize<ReqLoginPacket>(recvData.Data);
        
        try
        {
            if (_userMgr.GetUser(sessionID) != null)
            {
                SendLoginToClient(ErrorCode.LoginAlreadyWorking, recvData.SessionID);
                return;
            }
            //이미 
            //var reqData = MemoryPackSerializer.Deserialize<ReqLoginPacket>(recvData.Data);

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



