using MemoryPack;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.UI.Notifications;

#pragma warning disable CA1416

namespace csharp_test_client
{
    public partial class mainForm
    {
        Dictionary<UInt16, Action<byte[]>> PacketFuncDic = new Dictionary<UInt16, Action<byte[]>>();

        void SetPacketHandler()
        {
            //PacketFuncDic.Add(PacketId.PacketId_ERROR_NTF, PacketProcess_ErrorNotify);

            //여기다가 추가하면된다.
            PacketFuncDic.Add((int)PacketId.ResLogin, PacketProcess_Loginin);
            PacketFuncDic.Add((int)PacketId.ResRoomEnter, PacketProcess_RoomEnterResponse);
            PacketFuncDic.Add((int)PacketId.NftRoomUserList, PacketProcess_RoomUserListNotify);
            PacketFuncDic.Add((int)PacketId.NtfRoomNewUser, PacketProcess_RoomNewUserNotify);
            PacketFuncDic.Add((int)PacketId.ResRoomLeave, PacketProcess_RoomLeaveResponse);
            PacketFuncDic.Add((int)PacketId.NtfRoomLeaveUser, PacketProcess_RoomLeaveUserNotify);
            PacketFuncDic.Add((int)PacketId.NtfRoomChat, PacketProcess_RoomChatNotify);
            PacketFuncDic.Add((int)PacketId.ResReadyGame, PacketProcess_ReadyOmokResponse);
            PacketFuncDic.Add((int)PacketId.NtfReadGame, PacketProcess_ReadyOmokNotify);
            PacketFuncDic.Add((int)PacketId.NtfStartGame, PacketProcess_StartOmokNotify);
            PacketFuncDic.Add((int)PacketId.ResPutOmok, PacketProcess_PutMokResponse);
            PacketFuncDic.Add((int)PacketId.NtfPutOmok, PacketProcess_PutMokNotify);
            PacketFuncDic.Add((int)PacketId.NtrTimeOutOmok, PacketProcess_TimeOutNotify);
            PacketFuncDic.Add((ushort)PacketId.NtrWinnerOmok, PacketProcess_EndOmokNotify);

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
                //DevLog.Write("Unknown Packet Id: " + packetID);
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

            if((ERROR_CODE)responsePkt.Result != ERROR_CODE.ERROR_None)
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

            if(reqData.Result == (short)ErrorCode.None)
            {
                RoomEnter();
               

            }


            if (reqData.Result == (short)ErrorCode.FailVerifyUserToken)
            {
                MessageBox.Show("토큰인증에 실패했습니다");
            }


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

            DevLog.Write($"방에서 나간 유저 삭제");
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
            curPlayer = StoneType.Black;

            if (res.p1 == MyPlayer.Id)
            {
                if (MyPlayer.PlayerType == StoneType.Black)
                {
                    OtherPlayer.SetPlayer(res.p2, StoneType.White);
                }
                else
                {
                    OtherPlayer.SetPlayer(res.p2, StoneType.Black);

                }
            }
            else
            {
                if (MyPlayer.PlayerType == StoneType.Black)
                {
                    OtherPlayer.SetPlayer(res.p1, StoneType.White);
                }
                else
                {
                    OtherPlayer.SetPlayer(res.p1, StoneType.Black);

                }

            }

            
            bool myTurn = (MyPlayer.PlayerType == StoneType.Black) ? true : false;

            StartGame(myTurn, MyPlayer.Id, OtherPlayer.Id);

            DevLog.Write($"게임 시작. 흑돌 플레이어: {MyPlayer.Id}");
        }
        

        void PacketProcess_PutMokResponse(byte[] packetData)
        {
            var responsePkt =  MemoryPackSerializer.Deserialize<ResPutOMok>(packetData);

            //if(responsePkt.Result != (short)ERROR_CODE.None) 
            //{

                //DevLog.Write($"오목 놓기 실패: {responsePkt.Result}");
            //}


            //TODO 방금 놓은 오목 정보를 취소 시켜야 한다
        }
        
        void ChangeTurn(StoneType cur)
        {
            if (cur == MyPlayer.PlayerType)
            {
                IsMyTurn = false;
            }
            else
            {
                IsMyTurn = true;
            }


            if (cur == StoneType.White)
            {
                curPlayer = StoneType.Black;
            }
            else
            {
                curPlayer = StoneType.White;

            }


        }

        void PacketProcess_PutMokNotify(byte[] packetData)
        {

            var notifyPkt =  MemoryPackSerializer.Deserialize<NftPutOmok>(packetData);

            var cur = notifyPkt.mok;

            입력된돌그리기(notifyPkt.PosX, notifyPkt.PosY);

            ChangeTurn(cur);

            현재턴_플레이어_정보();

            DevLog.Write($"오목 정보: X: {notifyPkt.PosX},  Y: {notifyPkt.PosY}");// 알:{notifyPkt.Mok}");
        }
        
        void PacketProcess_TimeOutNotify(byte[] packetData)
        {
            var temp = MemoryPackSerializer.Deserialize<NftPutOmok>(packetData);

            ChangeTurn(temp.mok);


            DevLog.Write($"타임아웃패킷 받음 {temp.mok}이 타임아웃");

        }
        void PacketProcess_EndOmokNotify(byte[] packetData)
        {
            var notifyPkt =  MemoryPackSerializer.Deserialize<NtfOmokWinner>(packetData);
            IsMyTurn = false;

            //이긴거 돌타입으로온다.
            var win = notifyPkt.WinStone;

            if(win == StoneType.None)
            {
                MessageBox.Show("무승부입니다");
            }
            else if(win == MyPlayer.PlayerType)
            {
                MessageBox.Show("승리하였습니다");
            }
            else
            {
                MessageBox.Show("패배하였습니다");
            }

            EndGame();

            DevLog.Write($"오목 GameOver: Win: {notifyPkt.WinStone}");
        }
    }
}


