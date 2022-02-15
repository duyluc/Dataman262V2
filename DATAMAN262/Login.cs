using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DATAMAN262
{
    public partial class Login : Form
    {
        static private Login _instance;
        private readonly string _password = "0107600024";
        private bool _logined = false;
        public delegate void SuccessfulDelegate();
        public event SuccessfulDelegate SuccessfulEvent;
        public Login()
        {
            InitializeComponent();
            //FrmMain.Instance.LogoutEvent += this.LogoutEvent;
        }

        static public Login Instance
        {
            get
            {
                if (_instance == null) _instance = new Login();
                return _instance;
            }

            set
            {
                _instance = value;
            }
        }

        public bool Logined
        {
            get
            {
                return _logined;
            }
        }

        private void LoginBtn_Click(object sender, EventArgs e)
        {
            if (this.PasswordTbx.Text == this._password)
            {
                this.PasswordTbx.Text = "";
                this.SuccessfulEvent();
                this._logined = true;
                this.LoginInform.Text = "";
                this.Hide();
            }
            else
            {
                this._logined = false;
                this.PasswordTbx.Text = "";
                this.LoginInform.Text = "Password was wrong!";
            }
        }

        private void CloseBtn_Click(object sender, EventArgs e)
        {
            this._logined = false;
            this.PasswordTbx.Text = "";
            this.Hide();
        }

        public void LogoutEvent()
        {
            this._logined = false;
        }
    }
}
