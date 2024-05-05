﻿using MemoryPack;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;

#pragma warning disable CA1416

namespace csharp_test_client
{
    public partial class mainForm
    {
        Dictionary<UInt16, Action<byte[]>> PacketFuncDic = new Dictionary<UInt16, Action<byte[]>>();

        void SetPacketHandler()
        {
            //PacketFuncDic.Add(PACKET_ID.PACKET_ID_ERROR_NTF, PacketProcess_ErrorNotify);

            //여기다가 추가하면된다.
            PacketFuncDic.Add((int)PACKET_ID.RES_LOGIN, PacketProcess_Loginin);

            PacketFuncDic.Add((int)PACKET_ID.RES_ROOM_ENTER, PacketProcess_RoomEnterResponse);
            PacketFuncDic.Add((int)PACKET_ID.NTF_ROOM_USER_LIST, PacketProcess_RoomUserListNotify);
            PacketFuncDic.Add((int)PACKET_ID.NTF_ROOM_NEW_USER, PacketProcess_RoomNewUserNotify);
            PacketFuncDic.Add((int)PACKET_ID.RES_ROOM_LEAVE, PacketProcess_RoomLeaveResponse);
            PacketFuncDic.Add((int)PACKET_ID.NTF_ROOM_LEAVE_USER, PacketProcess_RoomLeaveUserNotify);
            //PacketFuncDic.Add((int)PACKET_ID.NTF_ROOM_CHAT, PacketProcess_RoomChatResponse);
            PacketFuncDic.Add((int)PACKET_ID.NTF_ROOM_CHAT, PacketProcess_RoomChatNotify);
            PacketFuncDic.Add((int)PACKET_ID.RES_READY_GAME, PacketProcess_ReadyOmokResponse);
            PacketFuncDic.Add((int)PACKET_ID.NTR_READY_GAME, PacketProcess_ReadyOmokNotify);
            PacketFuncDic.Add((int)PACKET_ID.NTF_START_GAME, PacketProcess_StartOmokNotify);
            PacketFuncDic.Add((int)PACKET_ID.RES_PUT_OMOK, PacketProcess_PutMokResponse);

            PacketFuncDic.Add((int)PACKET_ID.NTF_PUT_OMOK, PacketProcess_PutMokNotify);

            PacketFuncDic.Add((int)PACKET_ID.NTF_TIMEOUT_OMOK, PacketProcess_TimeOutNotify);
            PacketFuncDic.Add((ushort)PACKET_ID.NTR_WINNER_OMOK, PacketProcess_EndOmokNotify);

            //PacketFuncDic.Add((ushort)PACKET_ID.NTF_END_GAME, PacketProcess_EndOmokNotify);
        }

        void PacketProcess(byte[] packet)
        {
            var header = new PacketHeadInfo();
            header.Read(packet);

            var packetID = header.Id;

            if (PacketFuncDic.ContainsKey(packetID))
            {
                PacketFuncDic[packetID](packet);
            }
            else
            {
                DevLog.Write("Unknown Packet Id: " + packetID);
            }
        }

        void PacketProcess_PutStoneInfoNotifyResponse(byte[] bodyData)
        {
            /*var responsePkt = new PutStoneNtfPacket();
            responsePkt.FromBytes(bodyData);

            DevLog.Write($"'{responsePkt.userID}' Put Stone  : [{responsePkt.xPos}] , [{responsePkt.yPos}] ");*/

        }

        void PacketProcess_GameStartResultResponse(byte[] bodyData)
        {
            /*var responsePkt = new GameStartResPacket();
            responsePkt.FromBytes(bodyData);

            if ((ERROR_CODE)responsePkt.Result == ERROR_CODE.NOT_READY_EXIST)
            {
                DevLog.Write($"모두 레디상태여야 시작합니다.");
            }
            else
            {
                DevLog.Write($"게임시작 !!!! '{responsePkt.UserID}' turn  ");
            }*/
        }

        void PacketProcess_GameEndResultResponse(byte[] bodyData)
        {
            /*var responsePkt = new GameResultResPacket();
            responsePkt.FromBytes(bodyData);
            
            DevLog.Write($"'{responsePkt.UserID}' WIN , END GAME ");*/

        }

        void PacketProcess_PutStoneResponse(byte[] bodyData)
        {
            /*var responsePkt = new MatchUserResPacket();
            responsePkt.FromBytes(bodyData);

            if((ERROR_CODE)responsePkt.Result != ERROR_CODE.ERROR_NONE)
            {
                DevLog.Write($"Put Stone Error : {(ERROR_CODE)responsePkt.Result}");
            }

            DevLog.Write($"다음 턴 :  {(ERROR_CODE)responsePkt.Result}");*/

        }




        void PacketProcess_ErrorNotify(byte[] packetData)
        {
            /*var notifyPkt = new ErrorNtfPacket();
            notifyPkt.FromBytes(bodyData);

            DevLog.Write($"에러 통보 받음:  {notifyPkt.Error}");*/
        }


        void PacketProcess_Loginin(byte[] packetData)
        {

            var reqData = MemoryPackSerializer.Deserialize<ResLoginPacket>(packetData);
            DevLog.Write($"로그인 결과: {reqData.Result}");

        }

        void PacketProcess_RoomEnterResponse(byte[] packetData)
        {
            var responsePkt = MemoryPackSerializer.Deserialize<ResRoomEnterPacket>(packetData);
            DevLog.Write($"방 입장 결과: {responsePkt.Result}");
        }

        void PacketProcess_RoomUserListNotify(byte[] packetData)
        {
            var notifyPkt =  MemoryPackSerializer.Deserialize<NtfRoomUserList>(packetData);

            for (int i = 0; i < notifyPkt.UserIDList.Count; ++i)
            {
                AddRoomUserList(notifyPkt.UserIDList[i]);
            }

            DevLog.Write($"방의 기존 유저 리스트 받음");
        }

        void PacketProcess_RoomNewUserNotify(byte[] packetData)
        {
            var notifyPkt =  MemoryPackSerializer.Deserialize<NtfRoomNewUser>(packetData);

            AddRoomUserList(notifyPkt.UserID);

            DevLog.Write($"방에 새로 들어온 유저 받음");
        }


        void PacketProcess_RoomLeaveResponse(byte[] packetData)
        {
            var responsePkt =  MemoryPackSerializer.Deserialize<ResRoomLeavePacket>(packetData);
            //var id = responsePkt.
            //RemoveRoomUserList(UserID);


            DevLog.Write($"방 나가기 결과:  {responsePkt.Result}");
        }

        void PacketProcess_RoomLeaveUserNotify(byte[] packetData)
        {
            var notifyPkt =  MemoryPackSerializer.Deserialize<NtfRoomLeaveUser>(packetData);

            RemoveRoomUserList(notifyPkt.UserID);

            DevLog.Write($"방에서 나간 유저 지움????지운거 맞아??");
        }


        void PacketProcess_RoomChatResponse(byte[] packetData)
        {
            //var responsePkt = MemoryPackSerializer.Deserialize<PKTResRoomChat>(packetData);
            
            //DevLog.Write($"방 채팅 결과:  {(ErrorCode)responsePkt.}");
        }


        void PacketProcess_RoomChatNotify(byte[] packetData)
        {
            var notifyPkt =  MemoryPackSerializer.Deserialize<NtfRoomChat>(packetData);

            AddRoomChatMessageList(notifyPkt.UserID, notifyPkt.ChatMessage);
        }

        void AddRoomChatMessageList(string userID, string message)
        {

            if (listBoxRoomChatMsg.Items.Count > 512)
            {
                listBoxRoomChatMsg.Items.Clear();
            }

            listBoxRoomChatMsg.Items.Add($"[{userID}]: {message}");
            listBoxRoomChatMsg.SelectedIndex = listBoxRoomChatMsg.Items.Count - 1;
        }

        void PacketProcess_ReadyOmokResponse(byte[] packetData)
        {
            var res =  MemoryPackSerializer.Deserialize<ResGameReadyPacket>(packetData);
            MyPlayer.PlayerType = res.PlayerStoneType;

          

            DevLog.Write($"게임 준비 완료 요청 결과:  {MyPlayer.Id}는 {MyPlayer.PlayerType}");
        }

        void PacketProcess_ReadyOmokNotify(byte[] packetData)
        {
            //var notifyPkt =  MemoryPackSerializer.Deserialize<PKTNtfReadyOmok>(packetData);

            //if (notifyPkt.IsReady)
            //{
            //    DevLog.Write($"[{notifyPkt.UserID}]님은 게임 준비 완료");
            //}
            //else
            //{
            //    DevLog.Write($"[{notifyPkt.UserID}]님이 게임 준비 완료 취소");
            //}

        }

        void PacketProcess_StartOmokNotify(byte[] packetData)
        {

            var res = MemoryPackSerializer.Deserialize<NftGameStartPacket>(packetData);

            if (res.p1 == MyPlayer.Id)
            {
                if (MyPlayer.PlayerType == STONE_TYPE.BLACK)
                {
                    OtherPlayer.SetPlayer(res.p2, STONE_TYPE.WHITE);
                }
                else
                {
                    OtherPlayer.SetPlayer(res.p2, STONE_TYPE.BLACK);

                }
            }
            else
            {
                if (MyPlayer.PlayerType == STONE_TYPE.BLACK)
                {
                    OtherPlayer.SetPlayer(res.p1, STONE_TYPE.WHITE);
                }
                else
                {
                    OtherPlayer.SetPlayer(res.p1, STONE_TYPE.BLACK);

                }

            }

            
            bool myTurn = MyPlayer.PlayerType == STONE_TYPE.BLACK ? true : false;

            StartGame(myTurn, MyPlayer.Id, OtherPlayer.Id);

            DevLog.Write($"게임 시작. 흑돌 플레이어: {MyPlayer.Id}");
        }
        

        void PacketProcess_PutMokResponse(byte[] packetData)
        {
            var responsePkt =  MemoryPackSerializer.Deserialize<ResPutOMok>(packetData);

            //if(responsePkt.Result != (short)ERROR_CODE.NONE) 
            //{

                //DevLog.Write($"오목 놓기 실패: {responsePkt.Result}");
            //}


            //TODO 방금 놓은 오목 정보를 취소 시켜야 한다
        }
        
        void ChangeTurn(STONE_TYPE cur)
        {
            if (cur == MyPlayer.PlayerType)
            {
                IsMyTurn = false;
            }
            else
            {
                IsMyTurn = true;
            }


            if (cur == STONE_TYPE.WHITE)
            {
                curPlayer = STONE_TYPE.BLACK;
            }
            else
            {
                curPlayer = STONE_TYPE.WHITE;

            }


        }

        void PacketProcess_PutMokNotify(byte[] packetData)
        {

            var notifyPkt =  MemoryPackSerializer.Deserialize<NftPutOmok>(packetData);

            var cur = notifyPkt.mok;
            입력된돌그리기(notifyPkt.PosX, notifyPkt.PosY);

            ChangeTurn(cur);



            DevLog.Write($"오목 정보: X: {notifyPkt.PosX},  Y: {notifyPkt.PosY}");// 알:{notifyPkt.Mok}");
        }
        
        void PacketProcess_TimeOutNotify(byte[] packetData)
        {
            var temp = MemoryPackSerializer.Deserialize<NftPutOmok>(packetData);

            //목 색깔이 자기거랑 똑같으면 턴바꾸고 아니면 갸욷ㄴ다.
            ChangeTurn(temp.mok);
     

            DevLog.Write($"타임아웃패킷 받음 {temp.mok}이 타임아웃됨 나는{MyPlayer.PlayerType}임");// 알:{notifyPkt.Mok}");

        }
        void PacketProcess_EndOmokNotify(byte[] packetData)
        {
            var notifyPkt =  MemoryPackSerializer.Deserialize<NtfOmokWinner>(packetData);

            EndGame();

            DevLog.Write($"오목 GameOver: Win: {notifyPkt.UserId}");
        }
    }
}


