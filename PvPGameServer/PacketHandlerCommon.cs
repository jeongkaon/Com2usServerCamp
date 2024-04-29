using MemoryPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PvPGameServer;

//공통적인? 일들 처리하는 패킷핸들러
public class PacketHandlerCommon : PacketHandler
{
        
    public void RegistPacketHandler(Dictionary<int, Action<MemoryPackBinaryRequestInfo>> packetHandlerMap)
    {
        packetHandlerMap.Add((int)PACKET_ID.CS_LOGIN, RecvLoginPacket);
        packetHandlerMap.Add((int)PACKET_ID.NTF_IN_CONNECT_CLIENT, NotifyInConnectClient);
        packetHandlerMap.Add((int)PACKET_ID.NTF_IN_DISCONNECT_CLIENT, NotifyInDisConnectClient);

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

    public void RecvLoginPacket(MemoryPackBinaryRequestInfo recvData)
    {
        //로긴패킷 받았을때
        var sessionID = recvData.SessionID;

        try
        {
            if (UserMgr.GetUser(sessionID) != null)
            {
                SendLoginToClient(ERROR_CODE.LOGIN_ALREADY_WORKING, recvData.SessionID);
                return;
            }

            var reqData = MemoryPackSerializer.Deserialize<CSLoginPacket>(recvData.Data);
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
    public void SendLoginToClient(ERROR_CODE errorCode, string sessionID)
    {
        var resLogin = new SCLoginPacket()
        {
            Result = (short)errorCode
        };

        var sendData = MemoryPackSerializer.Serialize(resLogin);
        MemorypackPacketHeadInfo.Write(sendData, PACKET_ID.SC_LOGIN);

        NetworkSendFunc(sessionID, sendData);
    }

    public void NotifyMustCloseToClient(ERROR_CODE errorCode, string sessionID)
    {
        //로그인 실패했을때
        var resLogin = new NtfMustClosePacket()
        {
            Result = (short)errorCode
        };

        var sendData = MemoryPackSerializer.Serialize(resLogin);
        MemorypackPacketHeadInfo.Write(sendData, PACKET_ID.NTF_MUST_CLOSE);

        NetworkSendFunc(sessionID, sendData);
    }


}



