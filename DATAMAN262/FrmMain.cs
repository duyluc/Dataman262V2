using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Collections;
using System.Threading.Tasks;
using System.IO;
using CsvHelper;
using LiteDB;


namespace DATAMAN262
{
    public partial class FrmMain : Form
    {
        #region Ho tro di chuyen windown khong co thanh tieu de
        const int WM_NCHITTEST = 0x84;
        const int HTCLIENT = 0x1;
        const int HTCAPTION = 0x2;
        #endregion
         
        private Timer DatetimeRefreshTimer;

        static public FrmMain Instance;

        public Helper.Data.Dataman InputDataman;

        //->utilities variable
        public List<TextBox> IpInputGroupDM1;

        //--> Buffer chua du lieu ket qua
        public List<Helper.DatabaseProvider.RecordView> InputResultBuffer;
        private string referenceday = "Startup";

        public FrmMain()
        {
            InitializeComponent();
            Instance = this;
            ToolTip tooltip = new ToolTip();
            tooltip.AutoPopDelay = 2000;
            tooltip.SetToolTip(this.BtnSelectSaveFolder, "Save To");
            tooltip.SetToolTip(this.BtnClear, "Clear");
            tooltip.SetToolTip(this.BtnConnectDM1, "Connect/DisConnect");
            tooltip.SetToolTip(this.BtnOpenFolder, "Open Save Folder");
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            try
            {
                this.NotifyIcon.Visible = true;
                this.InputResultBuffer = new List<Helper.DatabaseProvider.RecordView>();
                string temppath = this.GetSaveFolderPath();
                if (!string.IsNullOrEmpty(temppath)) Helper.LogCSV.SaveFolderPath = temppath;
                this.TbxSaveFolderPath.Text = Helper.LogCSV.SaveFolderPath;
                //Initial IpInputGroup
                this.IpInputGroupDM1 = new List<TextBox>();
                this.IpInputGroupDM1.Add(this.TbxDM1Ip1);
                this.IpInputGroupDM1.Add(this.TbxDM1Ip2);
                this.IpInputGroupDM1.Add(this.TbxDM1Ip3);
                this.IpInputGroupDM1.Add(this.TbxDM1Ip4);

                //Check dieu kien dau
                if (!Directory.Exists(Helper.DatabaseProvider.DatabaseFilePath)) Directory.CreateDirectory(Helper.DatabaseProvider._databaseFolderPath);
                if (!Directory.Exists(Helper.LogCSV.SaveFolderPath)) Directory.CreateDirectory(Helper.LogCSV.SaveFolderPath);
                
                //Initial Dataman object
                this.InputDataman = new Helper.Data.Dataman("0x01",this.DVInputDM,this.IpInputGroupDM1);
                
                this.InputDataman.Connected += InputDataman_Connected;
                this.InputDataman.Disconnected += InputDataman_Disconnected;
                this.InputDataman.DatamanReady += InputDataman_DatamanReady;
                this.InputDataman.DatamanUnready += InputDataman_DatamanUnready;
                this.InputDataman.AddedRecored += InputDataman_AddedRecored;
                //-->Update Count Label
                this.LabelCount.Invoke(new Action(() =>
                {
                    this.LabelCount.Text = $"COUNT: {this.InputDataman.Params.TriggerCount}";
                }));
                //Initial DatatimeRefreshTimer
                this.DatetimeRefreshTimer = new Timer();
                this.DatetimeRefreshTimer.Interval = 500;

                //Reset Connect status Light 
                this.LightConnectInputDataman.CircleSubject.ChangeColor(CircleLightWpf.ColorOption.Red);
                
                this.LightTriggerReadyDM1.CircleSubject.ChangeColor(CircleLightWpf.ColorOption.Red);
                
            }
            catch(Exception t)
            {
                Helper.ProgramHelper.LogErr("", t);
                this.Close();
            }

        }

        private void InputDataman_AddedRecored()
        {
            this.LabelCount.Invoke(new Action(() =>
            {
                this.LabelCount.Text = $"COUNT: {this.InputDataman.Params.TriggerCount}";
            }));
            OutputDataman_AddedRecored();
        }

        /// <summary>
        /// Goi ham luu ket qua
        /// </summary>
        private void OutputDataman_AddedRecored()
        {
            if (this.InputResultBuffer.Count < 1) return;
            Task _ = this.SaveResultCSV();
            if(this.referenceday != DateTime.Now.ToString("dd")&&this.referenceday != "Startup")
            {
                this.InputDataman.RecordViewList.Clear();
                this.DVInputDM.DataSource = null;
                this.DVInputDM.Refresh();
            }
            this.referenceday = DateTime.Now.ToString("dd");
        }

        /// <summary>
        /// Support class for Save Result Csv
        /// </summary>
        class CsvRecord
        {
            public string Input_TimeLine { get; set; }
            public string Input_DatamanId { get; set; }
            public string Input_TriggerId { get; set; }
            public string Input_Result { get; set; }

            public string Output_TimeLine { get; set; }
            public string Output_DatamanId { get; set; }
            public string Output_TriggerId { get; set; }
            public string Output_Result { get; set; }
        }

        public async Task SaveResultCSV()
        {
            try
            {
                Helper.DatabaseProvider.RecordView inputrecord = this.InputResultBuffer[0];
                this.InputResultBuffer.RemoveAt(0);
                CsvRecord csvrecord = new CsvRecord
                {
                    Input_TimeLine = inputrecord.TimeLine,
                    Input_DatamanId = inputrecord.DatamanId,
                    Input_TriggerId = inputrecord.TriggerId,
                    Input_Result = inputrecord.Result,

                    //Output_TimeLine = this.OutputResultBuffer.TimeLine,
                    //Output_DatamanId = this.OutputResultBuffer.DatamanId,
                    //Output_TriggerId = this.OutputResultBuffer.TriggerId,
                    //Output_Result = this.OutputResultBuffer.Result
                };
                if (inputrecord.Result == "NOT READ") return;
                Task _ = new Task(() =>
                {
                    //var config = new CsvHelper.Configuration.CsvConfiguration()
                    //string timeline = DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss_fff");
                    string filename = Path.Combine(Helper.LogCSV.SaveFolderPath, $"{inputrecord.Result}_{DateTime.Now.ToString("ssfff")}.csv");
                    //File.Create(filename);
                    using (var file = new StreamWriter(filename))
                    using (var csv = new CsvWriter(file, System.Globalization.CultureInfo.InvariantCulture))
                    {
                    //    //csv.WriteHeader<CsvRecord>();
                    //    //csv.NextRecord();
                    //    //csv.WriteRecord(csvrecord);
                    }
                });
                _.Start();
                await _;
            }
            catch(Exception t)
            {
                Helper.ProgramHelper.LogErr("", t);
                MessageBox.Show("Save Record Error!");
            }
        }

        private void InputDataman_DatamanUnready()
        {
            this.LightTriggerReadyDM1.Invoke(new Action(() => { this.LightTriggerReadyDM1.CircleSubject.ChangeColor(CircleLightWpf.ColorOption.Red); }));
        }

        private void InputDataman_DatamanReady()
        {
            this.LightTriggerReadyDM1.Invoke(new Action(() => { this.LightTriggerReadyDM1.CircleSubject.ChangeColor(CircleLightWpf.ColorOption.Green); }));
        }

        private void InputDataman_Disconnected()
        {
            this.LightConnectInputDataman.Invoke(new Action(() =>
            {
                this.LightConnectInputDataman.CircleSubject.ChangeColor(CircleLightWpf.ColorOption.Red);
            }));
            this.BtnConnectDM1.Invoke(new Action(() =>
            {
                this.BtnConnectDM1.BackgroundImage = global::DATAMAN262.Properties.Resources.Connect2;
            }));
        }

        private void InputDataman_Connected()
        {
            this.LightConnectInputDataman.Invoke(new Action(() =>
            {
                this.LightConnectInputDataman.CircleSubject.ChangeColor(CircleLightWpf.ColorOption.Green);
            }));
            this.BtnConnectDM1.Invoke(new Action(() =>
            {
                this.BtnConnectDM1.BackgroundImage = global::DATAMAN262.Properties.Resources.Disconnect;
            }));
        }

        private void BtnQuit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("EXIT?", "WARNING", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                this.Close();
            }
        }
        /// <summary>
        /// Ham ho tro di chuyen windown khong co thanh tieu de
        /// </summary>
        /// <param name="e"></param>
        protected override void WndProc(ref Message message)
        {
            base.WndProc(ref message);
            if (message.Msg == WM_NCHITTEST && (int)message.Result == HTCLIENT)
                message.Result = (IntPtr)HTCAPTION;
        }

        private void tbxIP1_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void BtnConnectDM1_Click(object sender, EventArgs e)
        {
            if (!this.InputDataman.IsConnectDataman)
            {
                string ip = Helper.ProgramHelper.ArrangeIp(IpInputGroupDM1);
                this.InputDataman.Connect(ip);
                while (this.InputDataman.IsRunning) ;
                Task _ = this.InputDataman.Run();
            }
            else
            {
                this.InputDataman.Disconect();
            }
        }
        private void BtnSelectSaveFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDlg = new FolderBrowserDialog();
            folderDlg.ShowNewFolderButton = true;
            DialogResult result = folderDlg.ShowDialog();
            if (result == DialogResult.OK)
            {
                Helper.LogCSV.SaveFolderPath = folderDlg.SelectedPath;
                this.TbxSaveFolderPath.Text = folderDlg.SelectedPath;
                this.SaveSaveFolderPath();
            }
        }

        class SaveFolderPath_Class
        {
            public int Id { get; set; }
            public string Path { get; set; }
        }

        private void SaveSaveFolderPath()
        {
            try
            {
                using(LiteDatabase db = new LiteDatabase(Helper.DatabaseProvider.DatabaseFilePath))
                {
                    SaveFolderPath_Class savefolderpath = new SaveFolderPath_Class
                    {
                        Id = 1,
                        Path = Helper.LogCSV.SaveFolderPath
                    };
                    var col = db.GetCollection<SaveFolderPath_Class>("SaveFolderPath");
                    if (col.FindOne(x => x.Id == 1) == null) col.Insert(savefolderpath);
                    else col.Update(savefolderpath);
                }
            }
            catch (Exception t)
            {

            }
        }

        private string GetSaveFolderPath()
        {
            try
            {
                using (LiteDatabase db = new LiteDatabase(Helper.DatabaseProvider.DatabaseFilePath))
                {
                    
                    var col = db.GetCollection<SaveFolderPath_Class>("SaveFolderPath");
                    SaveFolderPath_Class path = col.FindOne(x => x.Id == 1);
                    if (path == null) return "";
                    else return path.Path;
                }
            }
            catch (Exception t)
            {
                return "";
            }
        }

        private void BtnOpenFolder_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(Helper.LogCSV.SaveFolderPath);
            }
            catch (Exception t)
            {
                Helper.ProgramHelper.LogErr("", t);
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            this.InputDataman.RecordViewList.Clear();
            this.DVInputDM.DataSource = null;
            this.DVInputDM.Refresh();

            this.InputDataman.Params.TriggerCount = 0;

            this.LabelCount.Invoke(new Action(() =>
            {
                this.LabelCount.Text = $"COUNT: {this.InputDataman.Params.TriggerCount}";
            }));
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Exit?","Warning",MessageBoxButtons.OKCancel) == DialogResult.Cancel)
            {
                e.Cancel = true;
            }
            else
            {
                this.InputDataman.Disconect();
                while (this.InputDataman.IsConnectDataman) ;
            }
        }

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            this.NotifyIcon.Visible = false;
            // Cách này
            WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
        }

        private void FrmMain_Resize(object sender, EventArgs e)
        {
            if(this.WindowState == FormWindowState.Minimized)
            {
                this.NotifyIcon.Visible = true;
                this.ShowInTaskbar = false;
            }
        }
    }
}
