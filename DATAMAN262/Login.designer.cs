
namespace DATAMAN262
{
    partial class Login
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.PasswordTbx = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.LoginInform = new System.Windows.Forms.Label();
            this.LoginBtn = new System.Windows.Forms.Button();
            this.CloseBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // PasswordTbx
            // 
            this.PasswordTbx.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PasswordTbx.Location = new System.Drawing.Point(48, 65);
            this.PasswordTbx.Name = "PasswordTbx";
            this.PasswordTbx.Size = new System.Drawing.Size(317, 26);
            this.PasswordTbx.TabIndex = 0;
            this.PasswordTbx.UseSystemPasswordChar = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft YaHei UI", 20.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.Highlight;
            this.label1.Location = new System.Drawing.Point(154, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(105, 36);
            this.label1.TabIndex = 1;
            this.label1.Text = "LOGIN";
            // 
            // LoginInform
            // 
            this.LoginInform.AutoSize = true;
            this.LoginInform.ForeColor = System.Drawing.Color.DarkRed;
            this.LoginInform.Location = new System.Drawing.Point(149, 149);
            this.LoginInform.Name = "LoginInform";
            this.LoginInform.Size = new System.Drawing.Size(0, 13);
            this.LoginInform.TabIndex = 2;
            // 
            // LoginBtn
            // 
            this.LoginBtn.BackColor = System.Drawing.Color.DarkSeaGreen;
            this.LoginBtn.Location = new System.Drawing.Point(177, 108);
            this.LoginBtn.Name = "LoginBtn";
            this.LoginBtn.Size = new System.Drawing.Size(58, 26);
            this.LoginBtn.TabIndex = 3;
            this.LoginBtn.Text = "Login";
            this.LoginBtn.UseVisualStyleBackColor = false;
            this.LoginBtn.Click += new System.EventHandler(this.LoginBtn_Click);
            // 
            // CloseBtn
            // 
            this.CloseBtn.BackColor = System.Drawing.Color.Brown;
            this.CloseBtn.Location = new System.Drawing.Point(381, 3);
            this.CloseBtn.Name = "CloseBtn";
            this.CloseBtn.Size = new System.Drawing.Size(30, 26);
            this.CloseBtn.TabIndex = 4;
            this.CloseBtn.Text = "X";
            this.CloseBtn.UseVisualStyleBackColor = false;
            this.CloseBtn.Click += new System.EventHandler(this.CloseBtn_Click);
            // 
            // Login
            // 
            this.AcceptButton = this.LoginBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(414, 171);
            this.Controls.Add(this.CloseBtn);
            this.Controls.Add(this.LoginBtn);
            this.Controls.Add(this.LoginInform);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.PasswordTbx);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Login";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Login";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox PasswordTbx;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label LoginInform;
        private System.Windows.Forms.Button LoginBtn;
        private System.Windows.Forms.Button CloseBtn;
    }
}