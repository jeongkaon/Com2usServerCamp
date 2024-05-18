using MemoryPack;
using OmokClient;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Sockets;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using Windows.Media.Core;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
#pragma warning disable CA1416


namespace csharp_test_client
{
    [SupportedOSPlatform("windows10.0.177630")]
    public partial class mainForm : Form
    {
        //하트비트용 타이머 
        System.Windows.Forms.Timer hbTimer;


        ClientSimpleTcp Network = new ClientSimpleTcp();

        bool IsNetworkThreadRunning = false;
        bool IsBackGroundProcessRunning = false;

        System.Threading.Thread NetworkReadThread = null;
        System.Threading.Thread NetworkSendThread = null;

        PacketBufferManager PacketBuffer = new PacketBufferManager();
        ConcurrentQueue<byte[]> RecvPacketQueue = new ConcurrentQueue<byte[]>();
        ConcurrentQueue<byte[]> SendPacketQueue = new ConcurrentQueue<byte[]>();

        //얘는 또 머야
        System.Windows.Forms.Timer dispatcherUITimer = new();

        public static Player MyPlayer = new Player();
        public static Player OtherPlayer = new Player();


        public mainForm()
        {
            InitializeComponent();

            Login.SettingIdAndPwFunc = SettingIdAndPw;
            Login.SettingLoginInfoFunc = SettingLoginInfo;

            hbTimer = new System.Windows.Forms.Timer();
            hbTimer.Interval = 1000;
            hbTimer.Tick += HeartBeatTimer;

            //hbTimer = null;
        }
        void HeartBeatTimer(object sender, EventArgs e)
        {
            var packet = MemoryPackSerializer.Serialize(new ReqHeartBeatPacket());
            PostSendPacket(PacketId.ReqHeartBeat, packet);


        }

        private void mainForm_Load(object sender, EventArgs e)
        {
            PacketBuffer.Init((8096 * 10), PacketHeadInfo.HeaderSize, 2048);

            IsNetworkThreadRunning = true;
            NetworkReadThread = new System.Threading.Thread(this.NetworkReadProcess);
            NetworkReadThread.Start();
            NetworkSendThread = new System.Threading.Thread(this.NetworkSendProcess);
            NetworkSendThread.Start();

            IsBackGroundProcessRunning = true;
            dispatcherUITimer.Tick += new EventHandler(BackGroundProcess);
            dispatcherUITimer.Interval = 100;
            dispatcherUITimer.Start();

            btnDisconnect.Enabled = false;

            SetPacketHandler();

            //시작하자마자 뜨게헤볼게
            Login loginForm = new Login();
            loginForm.ShowDialog();

           


            Omok_Init();
            DevLog.Write("프로그램 시작 !!!", LOG_LEVEL.INFO);
        }


      
        private void mainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            IsNetworkThreadRunning = false;
            IsBackGroundProcessRunning = false;

            Network.Close();
        }

        //id, authToken 셋팅창
        public void SettingIdAndPw(string id, string token)
        {
            아이디입력칸.Text = id;
            비밀번호입력칸.Text = token;
        }
        public void SettingLoginInfo(LoginInformation res)
        {
            아이디입력칸.Text = res.UserId;
            비밀번호입력칸.Text = res.AuthToken;
            서버주소입력창.Text = res.ServerAddress;
            포트입력칸.Text = res.Port;
            방번호입력칸.Text = res.RoomNumber.ToString();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            string address = 서버주소입력창.Text;

            

            if (checkBoxLocalHostIP.Checked)
            {
                address = "127.0.0.1";
            }

            int port = Convert.ToInt32(포트입력칸.Text);

            if (Network.Connect(address, port))
            {
                //여기부터 TIMER 세팅해서 시작하자
                labelStatus.Text = string.Format("{0}. 서버에 접속 중", DateTime.Now);
                btnConnect.Enabled = false;
                btnDisconnect.Enabled = true;

                //접속할때부터 하트비트 보내는게 좋을듯?? 
                //->오래 방 입장안하면 끊어버려야하기때문.
                hbTimer.Start();


                DevLog.Write($"서버에 접속 중", LOG_LEVEL.INFO);
            }
            else
            {
                labelStatus.Text = string.Format("{0}. 서버에 접속 실패", DateTime.Now);
            }

            PacketBuffer.Clear();
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            SetDisconnectd();
            Network.Close();
        }

        

        void NetworkReadProcess()
        {
            while (IsNetworkThreadRunning)
            {
                if (Network.IsConnected() == false)
                {
                    System.Threading.Thread.Sleep(1);
                    continue;
                }

                var recvData = Network.Receive();

                if (recvData != null)
                {
                    PacketBuffer.Write(recvData.Item2, 0, recvData.Item1);

                    while (true)
                    {
                        var data = PacketBuffer.Read();
                        if (data == null)
                        {
                            break;
                        }
                        
                        RecvPacketQueue.Enqueue(data);
                    }
                    DevLog.Write($"받은 데이터: {recvData.Item2}", LOG_LEVEL.INFO);
                }
                else
                {
                    Network.Close();
                    SetDisconnectd();
                    DevLog.Write("서버와 접속 종료 !!!", LOG_LEVEL.INFO);
                }
            }
        }

        void NetworkSendProcess()
        {
            while (IsNetworkThreadRunning)
            {
                System.Threading.Thread.Sleep(1);

                if (Network.IsConnected() == false)
                {
                    continue;
                }

                
                if (SendPacketQueue.TryDequeue(out var packet))
                {
                    Network.Send(packet);



                }
            }
        }


        void BackGroundProcess(object sender, EventArgs e)
        {
            ProcessLog();

            try
            {
                byte[] packet = null;

                if(RecvPacketQueue.TryDequeue(out packet))
                {
                    PacketProcess(packet);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("BackGroundProcess. error:{0}", ex.Message));
            }
        }

        private void ProcessLog()
        {
            // 너무 이 작업만 할 수 없으므로 일정 작업 이상을 하면 일단 패스한다.
            int logWorkCount = 0;

            while (IsBackGroundProcessRunning)
            {
                System.Threading.Thread.Sleep(1);

                string msg;

                if (DevLog.GetLog(out msg))
                {
                    ++logWorkCount;

                    if (listBoxLog.Items.Count > 512)
                    {
                        listBoxLog.Items.Clear();
                    }

                    listBoxLog.Items.Add(msg);
                    listBoxLog.SelectedIndex = listBoxLog.Items.Count - 1;
                }
                else
                {
                    break;
                }

                if (logWorkCount > 8)
                {
                    break;
                }
            }
        }


        public void SetDisconnectd()
        {
            if (btnConnect.Enabled == false)
            {
                btnConnect.Enabled = true;
                btnDisconnect.Enabled = false;
            }

            while (true)
            {
                if (SendPacketQueue.TryDequeue(out var temp) == false)
                {
                    break;
                }
            }

            listBoxRoomChatMsg.Items.Clear();
            listBoxRoomUserList.Items.Clear();

            EndGame();

            labelStatus.Text = "서버 접속이 끊어짐";
        }

        void PostSendPacket(PacketId packetID, byte[] packetData)
        {
            if (Network.IsConnected() == false)
            {
                //DevLog.Write("서버 연결이 되어 있지 않습니다", LOG_LEVEL.ERROR);
                return;
            }


            if (packetData != null)
            {
                var sendData = MemoryPackSerializer.Serialize(packetData);
                PacketHeadInfo.Write(packetData, packetID);

            }
          

            SendPacketQueue.Enqueue(packetData);
        }

        
        void AddRoomUserList(string userID)
        {
            listBoxRoomUserList.Items.Add(userID);
        }

        void RemoveRoomUserList(string userID)
        {
            object removeItem = null;

            foreach( var user in listBoxRoomUserList.Items)
            {
                if((string)user == userID)
                {
                    removeItem = user;
                    break;
                }
            }

            if (removeItem != null)
            {
                listBoxRoomUserList.Items.Remove(removeItem);
            }
        }

        string GetOtherPlayer(string myName)
        {
            if(listBoxRoomUserList.Items.Count != 2)
            {
                return null;
            }

            var firstName = (string)listBoxRoomUserList.Items[0];
            if (firstName == myName)
            {
                return firstName;
            }
            else 
            {
                return (string)listBoxRoomUserList.Items[1];
            }
        }


        // 로그인 요청 -> 일단 여기서 매칭요청하는거로 함수정해야함
        private void 로그인창_클릭(object sender, EventArgs e)
        {
     
            var loginReq = new ReqLoginPacket();
            loginReq.UserID = 아이디입력칸.Text;
            loginReq.AuthToken = 비밀번호입력칸.Text;
            var packet = MemoryPackSerializer.Serialize(loginReq);

            MyPlayer.Id = loginReq.UserID;

            PostSendPacket(PacketId.ReqLogin, packet);
            DevLog.Write($"로그인 요청:  {아이디입력칸.Text}, {비밀번호입력칸.Text}");

        }

        private void btn_RoomEnter_Click(object sender, EventArgs e)
        {

            int result;
            int.TryParse(방번호입력칸.Text, out result);
            MyPlayer.PlayRoom = result;

            var temp = new ReqRoomEnterPacket()
            {
                RoomNumber = result
            };
            var packet = MemoryPackSerializer.Serialize(temp);

            PostSendPacket(PacketId.ReqRoomEnter, packet);
            DevLog.Write($"방 입장 요청:  {방번호입력칸.Text} 번");
        }

        private void btn_RoomLeave_Click(object sender, EventArgs e)
        {
            PostSendPacket(PacketId.ReqRoomLeave, new byte[PacketHeadInfo.HeaderSize]);
            RemoveRoomUserList(아이디입력칸.Text);

            DevLog.Write($"방 퇴장 요청:  {아이디입력칸.Text} 번");
        }

        private void btnRoomChat_Click(object sender, EventArgs e)
        {
            if(textBoxRoomSendMsg.Text.IsEmpty())
            {
                MessageBox.Show("채팅 메시지를 입력하세요");
                return;
            }

            var requestPkt = new ReqRoomChat();
            requestPkt.ChatMessage = textBoxRoomSendMsg.Text;

            var sendPacketData = MemoryPackSerializer.Serialize(requestPkt);

            PostSendPacket(PacketId.ReqRoomChat, sendPacketData);

            DevLog.Write($"방 채팅 요청");
        }

        private void button3_Click(object sender, EventArgs e)
        {

            //예외처리할거있음 -> 방 입장 안했는데 눌리면 곤란 처리해야함
            int result;
            int.TryParse(방번호입력칸.Text, out result);

            var temp = new ReqGameReadyPacket()
            {
                RoomNumber = result
            };
            var packet = MemoryPackSerializer.Serialize(temp);

            PostSendPacket(PacketId.ReqReadyGame, packet);

            DevLog.Write($"게임 준비 완료 요청");
        }
        private void btnMatching_Click(object sender, EventArgs e)
        {
            DevLog.Write($"매칭 요청");
           // 여기서 방 매치를 해보자!!
           // api 서버로 매칭 요청을 보내보자
            var ip = "http://localhost:11501" + "/Matching";


            HttpClient httpClient = new();
            var id = 아이디입력칸.Text;
            var task = httpClient.PostAsJsonAsync(ip, new { UserID = id });

            if (task.Result == null)
            {
                //곤란하지~
            }
            var res = task.Result;
            var preResult = res.Content.ReadAsStringAsync().Result;
            var jsonDocument = JsonDocument.Parse(preResult);
        }


        private void listBoxRoomChatMsg_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBoxRelay_TextChanged(object sender, EventArgs e)
        {

        }

       
        void SendPacketOmokPut(object sender, MouseEventArgs e)
        {
            if (!IsMyTurn)
            {
                return;
            }
            int x = (e.X - 시작위치 + 10) / 눈금크기;
            int y = (e.Y - 시작위치 + 10) / 눈금크기;

            var temp = new ReqPutOMok
            {
                mok = curPlayer,
                PosX = x,
                PosY = y
            };

            var packet = MemoryPackSerializer.Serialize(temp);
            PostSendPacket(PacketId.ReqPutOmok, packet);

            DevLog.Write($"put stone 요청 : x  [ {x} ], y: [ {y} ] ");
        }

        private void btn_GameStartClick(object sender, EventArgs e)
        {
            //PostSendPacket(PacketId.GAME_START_REQ, null);
            if (MyPlayer.PlayerType == StoneType.Black)
            {

                StartGame(true, MyPlayer.Id, OtherPlayer.Id);
            }
            else
            {
                StartGame(false, MyPlayer.Id, OtherPlayer.Id);

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddUser("test1");
            AddUser("test2");
        }

        void AddUser(string userID)
        {
            var value = new PvPMatchingResult
            {
                IP = "127.0.0.1",
                Port = 32452,
                RoomNumber = 0,
                Index = 1,
                Token = "123qwe"
            };
            var saveValue =  MemoryPackSerializer.Serialize(value);

            var key = "ret_matching_" + userID;

            var redisConfig = new CloudStructures.RedisConfig("omok", "127.0.0.1");
            var RedisConnection = new CloudStructures.RedisConnection(redisConfig);

            var v = new CloudStructures.Structures.RedisString<byte[]>(RedisConnection, key, null);
            var ret = v.SetAsync(saveValue).Result;
        }



 
    }


    
}
