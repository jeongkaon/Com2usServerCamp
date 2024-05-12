using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OmokClient
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
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

            MessageBox.Show(ip + " "+id);
        }

        private void 하이브IP입력창_TextChanged(object sender, EventArgs e)
        {
            //하이브 IP 입력창
            
        }

        private void 하이브계정생성ID입력창(object sender, EventArgs e)
        {
        }
    
        private void 하이브로그인버튼_Click(object sender, EventArgs e)
        {
            //하이브로그인버튼
            var ip = 하이브IP입력창.Text + "/Login";

            var id = 하이브ID입력창.Text;
            var pw = 하이브PW입력창.Text;

            //ok뜨면 token창에 넣어주기!

        }
         
        private void API로그인버튼_Click(object sender, EventArgs e)
        {
            //API로그인버튼
            var ip = APIIP주소입력창.Text + "/Login";
            var id = API로긴ID입력창.Text;
            var token = API토큰입력창.Text;

            //버튼누르면 api서버로 보내야한다.

        }
 

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

    }
}
