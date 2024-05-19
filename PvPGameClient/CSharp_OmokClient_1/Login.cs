using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Windows.Forms;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Protection.PlayReady;
using csharp_test_client;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;
using MemoryPack;
using CloudStructures.Structures;
using System.Linq.Expressions;

namespace OmokClient
{
    public partial class Login : Form
    {
        public static Action<string, string> SettingIdAndPwFunc;
        public static Action<LoginInformation> SettingLoginInfoFunc;

        HttpClient httpClient = new();

        //매칭요청타이머
        System.Windows.Forms.Timer matchingTimer;

        public Login()
        {

            InitializeComponent();

            matchingTimer = new System.Windows.Forms.Timer();
            matchingTimer.Interval = 1000;
            matchingTimer.Tick += MatchingCheckTimer;


        }
        void MatchingCheckTimer(object sender, EventArgs e)
        {
            try
            {
                var ip = APIIP주소입력창.Text + "/CheckMatching";
                var id = API로긴ID입력창.Text;

                //1초에 한번씩 매칭요청되었는지 보내보자.
                var task = httpClient.PostAsJsonAsync(ip, new { UserID = id });
                if (task.Result == null) return;

                var preHiveResResult = task.Result.Content.ReadAsStringAsync().Result;
                if (preHiveResResult == null || preHiveResResult=="") return;   //null이 아니라 ""도 해줘야에러가 안남

                var jsonDocument = JsonDocument.Parse(preHiveResResult);
                if (jsonDocument == null) return;

                if (jsonDocument.RootElement.TryGetProperty("result", out var resultElement) &&
                    jsonDocument.RootElement.TryGetProperty("serverAddress", out var serverAddrElement) &&
                    jsonDocument.RootElement.TryGetProperty("port", out var portElement) &&
                    jsonDocument.RootElement.TryGetProperty("roomNumber", out var roomElement))
                {
                    //여기서 받은 값 기반으로 타이머 멈추고 hide하고 값 메인창에 넘겨줘야한다.
                    var result = resultElement.GetInt16();
                    if (result == (short)ErrorCode.None)
                    {
                        LoginInformation temp = new LoginInformation
                        {
                            ServerAddress = serverAddrElement.GetString(),  //tostring이아니라 getstrign을 써야한다.
                            Port = portElement.GetString(),
                            RoomNumber = roomElement.GetInt32(),
                            UserId = API로긴ID입력창.Text,
                            AuthToken = API토큰입력창.Text
                        };

                        SettingLoginInfoFunc(temp);
                        matchingTimer.Stop();

                        MessageBox.Show("매칭완료!");

                        Hide();

                    }
                    else
                    {
                        MessageBox.Show("매칭에 실패하였습니다.");

                    }

                }

            }
            catch
            {
                matchingTimer.Stop();
                MessageBox.Show("매칭에 실패하였습니다.");

            }
        }
        private void 회원가입버튼_Click(object sender, EventArgs e)
        {
            //회원가입버튼만들거임
            //버튼 클릭하면 ip주소로 http전송해야함

            var ip = 하이브IP입력창.Text + "/CreateAccount";
            var id = 하이브계정생성ID창.Text;
            var pw = 하이브계정생성PW창.Text;

            하이브ID입력창.Text = id;
            API로긴ID입력창.Text = id;

            var task = httpClient.PostAsJsonAsync(ip, new { Id = id, Password = pw });

            if (task.Result == null)
            {
                //곤란해~
            }
           
            var res = task.Result;
            var preResult = res.Content.ReadAsStringAsync().Result;
            var jsonDocument = JsonDocument.Parse(preResult);

            int result;

            if (jsonDocument.RootElement.TryGetProperty("result", out var resultElement) )
            {
                result = resultElement.GetInt16();
                if (result == (short)ErrorCode.None)
                {
                    MessageBox.Show("회원가입이 완료되었습니다");

                }
                else 
                {
                    MessageBox.Show("회원가입에 실패하였습니다.");

                }

            }

            //예외처리 더 해줘야함


        }

        private void 하이브IP입력창_TextChanged(object sender, EventArgs e)
        {
            //하이브 IP 입력창
            
        }

        private void 하이브계정생성ID입력창(object sender, EventArgs e)
        {
            //하이브회원가입 EMALI입력창
            하이브ID입력_TextChanged(sender, e);
        }
        private void 하이브계정생성PW_TextChanged(object sender, EventArgs e)
        {
            //하이브회원가입 비번입력창
            

        }
        private void 하이브로그인버튼_Click(object sender, EventArgs e)
        {
            //하이브로그인버튼
            var ip = 하이브IP입력창.Text + "/Login";

            var id = 하이브ID입력창.Text;
            var pw = 하이브PW입력창.Text;

            //ok뜨면 token창에 넣어주기!
            //패킷 그거 해줘야할듯?

            HttpClient httpClient = new();
            var task = httpClient.PostAsJsonAsync(ip, new { Id = id, Password = pw });

            if (task.Result == null)
            {
                //곤란하지~
            }
            var res = task.Result;
            //var preResult = res.Content.ReadAsStringAsync().Result;
            var preResult = res.Content.ReadAsStringAsync().Result;
            //여기가문제라구요?

            var jsonDocument = JsonDocument.Parse(preResult);

            int result;
            string token = null;

            if (jsonDocument.RootElement.TryGetProperty("result", out var resultElement) &&
                jsonDocument.RootElement.TryGetProperty("token", out var tokenElement))
            {
                result = resultElement.GetInt16();
                token = tokenElement.GetString();
            }
            API로긴ID입력창.Text = id;
            API토큰입력창.Text = token;
        }
        private void 하이브ID입력_TextChanged(object sender, EventArgs e)
        {

        }

        private void 하이브PW입력_TextChanged(object sender, EventArgs e)
        {

        }


      
        private async void API로그인버튼_Click(object sender, EventArgs e)
        {
            //API로그인버튼
            var ip = APIIP주소입력창.Text + "/Login";
            var id = API로긴ID입력창.Text;
            var token = API토큰입력창.Text;

            //버튼누르면 api서버로 보내야한다.


            HttpClient httpClient = new();
            var task = httpClient.PostAsJsonAsync(ip, new { Id = id, Token = token });

            if (task.Result == null)
            {
                //곤란하지~
            }
            var res = task.Result;
            var preResult = res.Content.ReadAsStringAsync().Result;
            if(preResult == null ||preResult == "")
            {
                MessageBox.Show("로그인버튼눌렀을때 돌아온json이 아무것도 없음");
            }
            var jsonDocument = JsonDocument.Parse(preResult);

            if (jsonDocument.RootElement.TryGetProperty("result", out var resultElement))
            {
                var result = resultElement.GetInt16();
                
                //매칭요청을 보낸다.
                if(result == (short)ErrorCode.None)
                {
                    ip = APIIP주소입력창.Text + "/Matching";

                   
                    //매칭요청 보낸다.
                    task = httpClient.PostAsJsonAsync(ip, new { UserID = id });

                    if (task.Result == null)
                    {
                        //곤란하지~
                    }
                    res = task.Result;
                    preResult = res.Content.ReadAsStringAsync().Result;
                    if(preResult == null)
                    {
                        MessageBox.Show("매칭요청전에 ㅇ실패??");

                        return;
                    }
                    jsonDocument = JsonDocument.Parse(preResult);
                    if (jsonDocument.RootElement.TryGetProperty("result", out var resultElement1))
                    {
                        result = resultElement1.GetInt16();
                    }

                    if (result == (short)ErrorCode.None)
                    {
                        //문제가 없다고하면 요청타이머를 보낸다.
                        //이제 요청타이머를 보내자.하트비트처럼~~~
                        MessageBox.Show("매칭요청까지완료");
                        matchingTimer.Start();

                    }



                }
            }
            else
            {
                //이거 ㅜㅁㄴ제?ㄴ
            }
        }


        private void APIIP주소입력_TextChanged(object sender, EventArgs e)
        {

        }
        private void API토큰입력_TextChanged(object sender, EventArgs e)
        {

        }

        private void API로긴ID입력_TextChanged(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    
        public class VerifyTokenReponse
        {
            public ErrorCode Result { get; set; } = ErrorCode.None;

        }
        public class LoginHiveResponse
        {
            public UInt16 Result { get; set; } = 0;

            public string? Token { get; set; }
        }

        public class CreateHiveAccountResponse
        {
            [Required]
            public ErrorCode Result { get; set; } = ErrorCode.None;
        }

    }
}
public class LoginInformation
{
    public string ServerAddress { get; set; } = "";
    public string Port { get; set; }
    public int RoomNumber { get; set; } = 0;
    public string UserId { get; set; }
    public string AuthToken { get; set; }    

        
}