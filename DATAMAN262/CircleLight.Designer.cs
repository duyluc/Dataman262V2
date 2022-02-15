
namespace DATAMAN262
{
    partial class CircleLight
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Host = new System.Windows.Forms.Integration.ElementHost();
            this.SuspendLayout();
            // 
            // Host
            // 
            this.Host.BackColor = System.Drawing.Color.Transparent;
            this.Host.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Host.Location = new System.Drawing.Point(0, 0);
            this.Host.Margin = new System.Windows.Forms.Padding(0);
            this.Host.Name = "Host";
            this.Host.Size = new System.Drawing.Size(109, 103);
            this.Host.TabIndex = 0;
            this.Host.Text = "elementHost1";
            this.Host.Child = null;
            // 
            // CircleLight
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.Host);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "CircleLight";
            this.Size = new System.Drawing.Size(109, 103);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Integration.ElementHost Host;
    }
}
