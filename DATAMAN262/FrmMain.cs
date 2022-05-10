using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Collections;
using System.Threading.Tasks;
using System.IO;
using CsvHelper;
using LiteDB;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;


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
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            try
            {
                this.NotifyIcon.Visible = true;
                this.InputResultBuffer = new List<Helper.DatabaseProvider.RecordView>();
                GetSetting();
                if(_setting.Path != null) Helper.LogCSV.SaveFolderPath = _setting.Path;
                this.TbxSaveFolderPath.Text = Helper.LogCSV.SaveFolderPath;
                tbxCode1.Text = _setting.Code1;
                tbxCode2.Text = _setting.Code2;
                tbxCode3.Text = _setting.Code3;
                ckb1.Checked = _setting.Active1;
                ckb2.Checked = _setting.Active2;
                ckb3.Checked = _setting.Active3;
                //Initial IpInputGroup
                

                //Check dieu kien dau
                if (!Directory.Exists(Helper.DatabaseProvider.DatabaseFilePath)) Directory.CreateDirectory(Helper.DatabaseProvider._databaseFolderPath);
                if (!Directory.Exists(Helper.LogCSV.SaveFolderPath)) Directory.CreateDirectory(Helper.LogCSV.SaveFolderPath);

                //Initial Dataman object
                this.InputDataman = new Helper.Data.Dataman("0x01", this.DVInputDM);
                this.InputDataman.Connected += InputDataman_Connected;
                this.InputDataman.Disconnected += InputDataman_Disconnected;
                this.InputDataman.AddedRecored += InputDataman_AddedRecored;
                //-->Update Count Label
                this.tsCount.Text = $"COUNT: {this.InputDataman.Params.TriggerCount}";
                string[] _ = this.InputDataman.Params.Ip.Split('.');
                TbxDM1Ip1.Text = _[0];
                TbxDM1Ip2.Text = _[1];
                TbxDM1Ip3.Text = _[2];
                TbxDM1Ip4.Text = _[3];
                this.LightConnectInputDataman.CircleSubject.ChangeColor(CircleLightWpf.ColorOption.Red);
            }
            catch(Exception t)
            {
                Helper.ProgramHelper.LogErr("", t);
                this.Close();
            }

        }

        private void InputDataman_AddedRecored()
        {
            this.tsCount.Text = $"COUNT: {this.InputDataman.Params.TriggerCount}";
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
                    Input_Result = inputrecord.Result,
                };
                if (inputrecord.Result == "NOT READ") return;
                Task _ = new Task(() =>
                {
                    try
                    {
                        string name = inputrecord.Result;
                        if (_setting.Active1) name += "_" + _setting.Code1;
                        if (_setting.Active2) name += "_" + _setting.Code2;
                        if (_setting.Active3) name += "_" + _setting.Code3;
                        string filename = Path.Combine(Helper.LogCSV.SaveFolderPath, $"{name}.xlsx");
                        List<string> textFormat = new List<string>();

                        // Read the file and display it line by line.  
                        foreach (string line in System.IO.File.ReadLines(@"CsvFormat.txt"))
                        {
                            textFormat.Add(line);
                        }
                        if(File.Exists(filename)) File.Delete(filename);
                        IWorkbook book = new XSSFWorkbook();
                        ISheet sheet = book.CreateSheet();
                        int _i = 0;
                        foreach(string line in textFormat)
                        {
                            sheet.CreateRow(_i).CreateCell(0).SetCellValue(line);
                            _i++;
                        }
                        using(FileStream file = File.Create(filename))
                        {
                            book.Write(file);
                        }

                    }
                    catch(Exception ex)
                    { }
                    
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
                this.InputDataman = new Helper.Data.Dataman("0x01", this.DVInputDM);
                this.IpInputGroupDM1 = new List<TextBox>();
                this.IpInputGroupDM1.Add(this.TbxDM1Ip1);
                this.IpInputGroupDM1.Add(this.TbxDM1Ip2);
                this.IpInputGroupDM1.Add(this.TbxDM1Ip3);
                this.IpInputGroupDM1.Add(this.TbxDM1Ip4);
                this.InputDataman.Connected += InputDataman_Connected;
                this.InputDataman.Disconnected += InputDataman_Disconnected;
                this.InputDataman.AddedRecored += InputDataman_AddedRecored;
                string ip = Helper.ProgramHelper.ArrangeIp(IpInputGroupDM1);
                this.InputDataman.Connect(ip);
                this.InputDataman.RecordViewList.Clear();
                this.DVInputDM.DataSource = null;
                this.DVInputDM.Refresh();
                this.InputDataman.Params.TriggerCount = 0;
                this.tsCount.Text = $"COUNT: {this.InputDataman.Params.TriggerCount}";
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
            this.tsCount.Text = $"COUNT: {this.InputDataman.Params.TriggerCount}";
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Exit?","Warning",MessageBoxButtons.OKCancel) == DialogResult.Cancel)
            {
                e.Cancel = true;
            }
            else
            {
                try
                {
                    this.InputDataman.Disconect();
                    while (this.InputDataman.IsConnectDataman) ;
                }
                catch
                { }
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
        class cSetting
        {
            public int Id { get; set; }
            public string Path { get; set; }
            public string Code1 { get; set; }
            public string Code2 { get; set; }
            public string Code3 { get; set; }
            public bool Active1 { get; set; }
            public bool Active2 { get; set; }
            public bool Active3 { get; set; }
        }
        private cSetting _setting = new cSetting();

        private void SaveSaveFolderPath()
        {
            try
            {
                using (LiteDatabase db = new LiteDatabase(Helper.DatabaseProvider.DatabaseFilePath))
                {
                    var col = db.GetCollection<cSetting>("SaveFolderPath");
                    _setting.Path = Helper.LogCSV.SaveFolderPath;
                    col.Update(_setting);
                }
            }
            catch (Exception t)
            {

            }
        }

        private void GetSetting()
        {
            try
            {
                using (LiteDatabase db = new LiteDatabase(Helper.DatabaseProvider.DatabaseFilePath))
                {

                    _setting = new cSetting
                    {
                        Id = 1,
                        Path = @"",
                        Code1 = "",
                        Code2 = "",
                        Code3 = "",
                        Active1 = true,
                        Active2 = true,
                        Active3 = true
                    };
                    var col = db.GetCollection<cSetting>("SaveFolderPath");
                    cSetting path = col.FindOne(x => x.Id == 1);
                    if (path == null) col.Insert(_setting);
                    else _setting = path;
                }
            }
            catch (Exception t)
            {

            }
        }
        private void tbxMaThietBi_KeyDown(object sender, KeyEventArgs e)
        {
            if(Keys.Enter == e.KeyCode)
            {
                try
                {
                    using (LiteDatabase db = new LiteDatabase(Helper.DatabaseProvider.DatabaseFilePath))
                    {
                        var col = db.GetCollection<cSetting>("SaveFolderPath");
                        _setting.Code1 = tbxCode1.Text;
                        col.Update(_setting);
                    }
                }
                catch (Exception t)
                {

                }
            }
        }

        private void tbxMaCongDoan_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keys.Enter == e.KeyCode)
            {
                try
                {
                    using (LiteDatabase db = new LiteDatabase(Helper.DatabaseProvider.DatabaseFilePath))
                    {
                        var col = db.GetCollection<cSetting>("SaveFolderPath");
                        _setting.Code2 = tbxCode2.Text;
                        col.Update(_setting);
                    }
                }
                catch (Exception t)
                {

                }
            }
        }

        private void tbxCode3_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keys.Enter == e.KeyCode)
            {
                try
                {
                    using (LiteDatabase db = new LiteDatabase(Helper.DatabaseProvider.DatabaseFilePath))
                    {
                        var col = db.GetCollection<cSetting>("SaveFolderPath");
                        _setting.Code3 = tbxCode3.Text;
                        col.Update(_setting);
                    }
                }
                catch (Exception t)
                {

                }
            }
        }


        private void ckb1_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                using (LiteDatabase db = new LiteDatabase(Helper.DatabaseProvider.DatabaseFilePath))
                {
                    var col = db.GetCollection<cSetting>("SaveFolderPath");
                    _setting.Active1 = ckb1.Checked;
                    col.Update(_setting);
                }
            }
            catch (Exception t)
            {

            }
        }

        private void ckb2_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                using (LiteDatabase db = new LiteDatabase(Helper.DatabaseProvider.DatabaseFilePath))
                {
                    var col = db.GetCollection<cSetting>("SaveFolderPath");
                    _setting.Active2 = ckb2.Checked;
                    col.Update(_setting);
                }
            }
            catch (Exception t)
            {

            }
        }

        private void ckb3_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                using (LiteDatabase db = new LiteDatabase(Helper.DatabaseProvider.DatabaseFilePath))
                {
                    var col = db.GetCollection<cSetting>("SaveFolderPath");
                    _setting.Active3 = ckb3.Checked;
                    col.Update(_setting);
                }
            }
            catch (Exception t)
            {

            }
        }
    }
}
