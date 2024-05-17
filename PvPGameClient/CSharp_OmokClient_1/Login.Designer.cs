namespace OmokClient
{
    partial class Login
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.회원가입버튼 = new System.Windows.Forms.Button();
            this.하이브IP입력창 = new System.Windows.Forms.TextBox();
            this.하이브계정생성ID창 = new System.Windows.Forms.TextBox();
            this.하이브IP라벨 = new System.Windows.Forms.Label();
            this.하이브ID라벨 = new System.Windows.Forms.Label();
            this.하이브계정생성PW창 = new System.Windows.Forms.TextBox();
            this.하이브PW라벨 = new System.Windows.Forms.Label();
            this.하이브ID입력창 = new System.Windows.Forms.TextBox();
            this.하이브PW입력창 = new System.Windows.Forms.TextBox();
            this.하이브로그인버튼 = new System.Windows.Forms.Button();
            this.APIIP주소입력창 = new System.Windows.Forms.TextBox();
            this.API로긴ID입력창 = new System.Windows.Forms.TextBox();
            this.API로그인버튼 = new System.Windows.Forms.Button();
            this.API토큰입력창 = new System.Windows.Forms.TextBox();
            this.하이브로긴ID라벨 = new System.Windows.Forms.Label();
            this.하이브로긴PW라벨 = new System.Windows.Forms.Label();
            this.APIIP주소라벨 = new System.Windows.Forms.Label();
            this.API로긴ID라벨 = new System.Windows.Forms.Label();
            this.API토큰라벨 = new System.Windows.Forms.Label();
            this.하이브그룹 = new System.Windows.Forms.GroupBox();
            this.API그룹 = new System.Windows.Forms.GroupBox();
            this.하이브그룹.SuspendLayout();
            this.API그룹.SuspendLayout();
            this.SuspendLayout();
            // 
            // 회원가입버튼
            // 
            this.회원가입버튼.Location = new System.Drawing.Point(522, 59);
            this.회원가입버튼.Name = "회원가입버튼";
            this.회원가입버튼.Size = new System.Drawing.Size(137, 50);
            this.회원가입버튼.TabIndex = 0;
            this.회원가입버튼.Text = "회원가입";
            this.회원가입버튼.UseVisualStyleBackColor = true;
            this.회원가입버튼.Click += new System.EventHandler(this.회원가입버튼_Click);
            // 
            // 하이브IP입력창
            // 
            this.하이브IP입력창.Location = new System.Drawing.Point(103, 17);
            this.하이브IP입력창.Name = "하이브IP입력창";
            this.하이브IP입력창.Size = new System.Drawing.Size(402, 21);
            this.하이브IP입력창.TabIndex = 1;
            this.하이브IP입력창.Text = "http://localhost:11500";
            this.하이브IP입력창.TextChanged += new System.EventHandler(this.하이브IP입력창_TextChanged);
            // 
            // 하이브계정생성ID창
            // 
            this.하이브계정생성ID창.Location = new System.Drawing.Point(103, 59);
            this.하이브계정생성ID창.Name = "하이브계정생성ID창";
            this.하이브계정생성ID창.Size = new System.Drawing.Size(402, 21);
            this.하이브계정생성ID창.TabIndex = 2;
            this.하이브계정생성ID창.Text = "회원가입 ID입력창";
            this.하이브계정생성ID창.TextChanged += new System.EventHandler(this.하이브계정생성ID입력창);
            // 
            // 하이브IP라벨
            // 
            this.하이브IP라벨.AutoSize = true;
            this.하이브IP라벨.Location = new System.Drawing.Point(27, 26);
            this.하이브IP라벨.Name = "하이브IP라벨";
            this.하이브IP라벨.Size = new System.Drawing.Size(16, 12);
            this.하이브IP라벨.TabIndex = 3;
            this.하이브IP라벨.Text = "IP";
            // 
            // 하이브ID라벨
            // 
            this.하이브ID라벨.AutoSize = true;
            this.하이브ID라벨.Location = new System.Drawing.Point(27, 62);
            this.하이브ID라벨.Name = "하이브ID라벨";
            this.하이브ID라벨.Size = new System.Drawing.Size(16, 12);
            this.하이브ID라벨.TabIndex = 4;
            this.하이브ID라벨.Text = "ID";
            // 
            // 하이브계정생성PW창
            // 
            this.하이브계정생성PW창.Location = new System.Drawing.Point(103, 96);
            this.하이브계정생성PW창.Name = "하이브계정생성PW창";
            this.하이브계정생성PW창.Size = new System.Drawing.Size(402, 21);
            this.하이브계정생성PW창.TabIndex = 5;
            this.하이브계정생성PW창.Text = "하이브계정생성Pw입력창";
            this.하이브계정생성PW창.TextChanged += new System.EventHandler(this.하이브계정생성PW_TextChanged);
            // 
            // 하이브PW라벨
            // 
            this.하이브PW라벨.AutoSize = true;
            this.하이브PW라벨.Location = new System.Drawing.Point(27, 99);
            this.하이브PW라벨.Name = "하이브PW라벨";
            this.하이브PW라벨.Size = new System.Drawing.Size(23, 12);
            this.하이브PW라벨.TabIndex = 6;
            this.하이브PW라벨.Text = "PW";
            // 
            // 하이브ID입력창
            // 
            this.하이브ID입력창.Location = new System.Drawing.Point(103, 140);
            this.하이브ID입력창.Name = "하이브ID입력창";
            this.하이브ID입력창.Size = new System.Drawing.Size(402, 21);
            this.하이브ID입력창.TabIndex = 7;
            this.하이브ID입력창.Text = "하이브ID입력칸";
            this.하이브ID입력창.TextChanged += new System.EventHandler(this.하이브ID입력_TextChanged);
            // 
            // 하이브PW입력창
            // 
            this.하이브PW입력창.Location = new System.Drawing.Point(103, 179);
            this.하이브PW입력창.Name = "하이브PW입력창";
            this.하이브PW입력창.Size = new System.Drawing.Size(402, 21);
            this.하이브PW입력창.TabIndex = 8;
            this.하이브PW입력창.Text = "하이브PW입력창";
            this.하이브PW입력창.TextChanged += new System.EventHandler(this.하이브PW입력_TextChanged);
            // 
            // 하이브로그인버튼
            // 
            this.하이브로그인버튼.Location = new System.Drawing.Point(522, 140);
            this.하이브로그인버튼.Name = "하이브로그인버튼";
            this.하이브로그인버튼.Size = new System.Drawing.Size(137, 50);
            this.하이브로그인버튼.TabIndex = 9;
            this.하이브로그인버튼.Text = "HIVE 로그인";
            this.하이브로그인버튼.UseVisualStyleBackColor = true;
            this.하이브로그인버튼.Click += new System.EventHandler(this.하이브로그인버튼_Click);
            // 
            // APIIP주소입력창
            // 
            this.APIIP주소입력창.Location = new System.Drawing.Point(103, 20);
            this.APIIP주소입력창.Name = "APIIP주소입력창";
            this.APIIP주소입력창.Size = new System.Drawing.Size(402, 21);
            this.APIIP주소입력창.TabIndex = 10;
            this.APIIP주소입력창.Text = "http://localhost:11501";
            this.APIIP주소입력창.TextChanged += new System.EventHandler(this.APIIP주소입력_TextChanged);
            // 
            // API로긴ID입력창
            // 
            this.API로긴ID입력창.Location = new System.Drawing.Point(103, 71);
            this.API로긴ID입력창.Name = "API로긴ID입력창";
            this.API로긴ID입력창.Size = new System.Drawing.Size(402, 21);
            this.API로긴ID입력창.TabIndex = 11;
            this.API로긴ID입력창.Text = "API로긴ID입력창";
            this.API로긴ID입력창.TextChanged += new System.EventHandler(this.API로긴ID입력_TextChanged);
            // 
            // API로그인버튼
            // 
            this.API로그인버튼.Location = new System.Drawing.Point(522, 62);
            this.API로그인버튼.Name = "API로그인버튼";
            this.API로그인버튼.Size = new System.Drawing.Size(137, 50);
            this.API로그인버튼.TabIndex = 12;
            this.API로그인버튼.Text = "API 로그인";
            this.API로그인버튼.UseVisualStyleBackColor = true;
            this.API로그인버튼.Click += new System.EventHandler(this.API로그인버튼_Click);
            // 
            // API토큰입력창
            // 
            this.API토큰입력창.Location = new System.Drawing.Point(103, 104);
            this.API토큰입력창.Name = "API토큰입력창";
            this.API토큰입력창.Size = new System.Drawing.Size(402, 21);
            this.API토큰입력창.TabIndex = 13;
            this.API토큰입력창.Text = "API토큰입력창입력해";
            this.API토큰입력창.TextChanged += new System.EventHandler(this.API토큰입력_TextChanged);
            // 
            // 하이브로긴ID라벨
            // 
            this.하이브로긴ID라벨.AutoSize = true;
            this.하이브로긴ID라벨.Location = new System.Drawing.Point(27, 149);
            this.하이브로긴ID라벨.Name = "하이브로긴ID라벨";
            this.하이브로긴ID라벨.Size = new System.Drawing.Size(16, 12);
            this.하이브로긴ID라벨.TabIndex = 14;
            this.하이브로긴ID라벨.Text = "ID";
            // 
            // 하이브로긴PW라벨
            // 
            this.하이브로긴PW라벨.AutoSize = true;
            this.하이브로긴PW라벨.Location = new System.Drawing.Point(27, 188);
            this.하이브로긴PW라벨.Name = "하이브로긴PW라벨";
            this.하이브로긴PW라벨.Size = new System.Drawing.Size(23, 12);
            this.하이브로긴PW라벨.TabIndex = 15;
            this.하이브로긴PW라벨.Text = "PW";
            // 
            // APIIP주소라벨
            // 
            this.APIIP주소라벨.AutoSize = true;
            this.APIIP주소라벨.Location = new System.Drawing.Point(17, 29);
            this.APIIP주소라벨.Name = "APIIP주소라벨";
            this.APIIP주소라벨.Size = new System.Drawing.Size(16, 12);
            this.APIIP주소라벨.TabIndex = 16;
            this.APIIP주소라벨.Text = "IP";
            // 
            // API로긴ID라벨
            // 
            this.API로긴ID라벨.AutoSize = true;
            this.API로긴ID라벨.Location = new System.Drawing.Point(17, 71);
            this.API로긴ID라벨.Name = "API로긴ID라벨";
            this.API로긴ID라벨.Size = new System.Drawing.Size(16, 12);
            this.API로긴ID라벨.TabIndex = 17;
            this.API로긴ID라벨.Text = "ID";
            // 
            // API토큰라벨
            // 
            this.API토큰라벨.AutoSize = true;
            this.API토큰라벨.Location = new System.Drawing.Point(17, 113);
            this.API토큰라벨.Name = "API토큰라벨";
            this.API토큰라벨.Size = new System.Drawing.Size(47, 12);
            this.API토큰라벨.TabIndex = 18;
            this.API토큰라벨.Text = "TOKEN";
            // 
            // 하이브그룹
            // 
            this.하이브그룹.Controls.Add(this.하이브로긴PW라벨);
            this.하이브그룹.Controls.Add(this.하이브로긴ID라벨);
            this.하이브그룹.Controls.Add(this.하이브PW입력창);
            this.하이브그룹.Controls.Add(this.하이브ID입력창);
            this.하이브그룹.Controls.Add(this.하이브PW라벨);
            this.하이브그룹.Controls.Add(this.하이브계정생성PW창);
            this.하이브그룹.Controls.Add(this.하이브ID라벨);
            this.하이브그룹.Controls.Add(this.하이브로그인버튼);
            this.하이브그룹.Controls.Add(this.하이브IP라벨);
            this.하이브그룹.Controls.Add(this.회원가입버튼);
            this.하이브그룹.Controls.Add(this.하이브계정생성ID창);
            this.하이브그룹.Controls.Add(this.하이브IP입력창);
            this.하이브그룹.Location = new System.Drawing.Point(37, 25);
            this.하이브그룹.Name = "하이브그룹";
            this.하이브그룹.Size = new System.Drawing.Size(674, 215);
            this.하이브그룹.TabIndex = 19;
            this.하이브그룹.TabStop = false;
            this.하이브그룹.Text = "HIVE";
            this.하이브그룹.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // API그룹
            // 
            this.API그룹.Controls.Add(this.APIIP주소입력창);
            this.API그룹.Controls.Add(this.API로그인버튼);
            this.API그룹.Controls.Add(this.API토큰라벨);
            this.API그룹.Controls.Add(this.API로긴ID라벨);
            this.API그룹.Controls.Add(this.APIIP주소라벨);
            this.API그룹.Controls.Add(this.API토큰입력창);
            this.API그룹.Controls.Add(this.API로긴ID입력창);
            this.API그룹.Location = new System.Drawing.Point(37, 256);
            this.API그룹.Name = "API그룹";
            this.API그룹.Size = new System.Drawing.Size(674, 159);
            this.API그룹.TabIndex = 20;
            this.API그룹.TabStop = false;
            this.API그룹.Text = "API";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(724, 450);
            this.Controls.Add(this.하이브그룹);
            this.Controls.Add(this.API그룹);
            this.Name = "Form1";
            this.Text = "Form1";
            this.하이브그룹.ResumeLayout(false);
            this.하이브그룹.PerformLayout();
            this.API그룹.ResumeLayout(false);
            this.API그룹.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button 회원가입버튼;
        private System.Windows.Forms.Label 하이브IP라벨;
        private System.Windows.Forms.Label 하이브ID라벨;
        private System.Windows.Forms.Label 하이브PW라벨;
        private System.Windows.Forms.TextBox 하이브IP입력창;
        private System.Windows.Forms.TextBox 하이브계정생성ID창;
        private System.Windows.Forms.TextBox 하이브계정생성PW창;


        private System.Windows.Forms.Button 하이브로그인버튼;
        private System.Windows.Forms.Label 하이브로긴ID라벨;
        private System.Windows.Forms.Label 하이브로긴PW라벨;
        private System.Windows.Forms.TextBox 하이브ID입력창;
        private System.Windows.Forms.TextBox 하이브PW입력창;


        private System.Windows.Forms.Button API로그인버튼;
        private System.Windows.Forms.Label APIIP주소라벨;
        private System.Windows.Forms.Label API로긴ID라벨;
        private System.Windows.Forms.Label API토큰라벨;
        private System.Windows.Forms.TextBox APIIP주소입력창;
        private System.Windows.Forms.TextBox API로긴ID입력창;
        private System.Windows.Forms.TextBox API토큰입력창;
        private System.Windows.Forms.GroupBox 하이브그룹;
        private System.Windows.Forms.GroupBox API그룹;
    }
}