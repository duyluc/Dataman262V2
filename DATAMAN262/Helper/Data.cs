using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sres.Net.EEIP;
using System.Net.NetworkInformation;
using System.Collections.Specialized;
using System.IO;
using LiteDB;

namespace DATAMAN262.Helper
{
    public class Data
    {
        public class Dataman
        {
            /// <summary>
            /// recieve data potocol
            /// </summary>
            public enum InputTopocolKey
            {
                TRIGGERID = 6,
                RESULTID = 8,
                RESULTCODE = 10,
                RESULTEXTENDED = 12,
                RESULTDATALENGTH = 14,
                RESULTDATA = 16,
            }
            public Dictionary<InputTopocolKey, int> InputTopocol = new Dictionary<InputTopocolKey, int>()
            {
                {InputTopocolKey.TRIGGERID,6 },
                {InputTopocolKey.RESULTID, 8 },
                {InputTopocolKey.RESULTCODE,10 },
                {InputTopocolKey.RESULTEXTENDED,12 },
                {InputTopocolKey.RESULTDATALENGTH,14 },
                {InputTopocolKey.RESULTDATA,16 }
            };
            //-> Event
            public delegate void ConnectedDelegate();
            public event ConnectedDelegate Connected;

            public delegate void DisconnectedDelegate();
            public event ConnectedDelegate Disconnected;

            public delegate void AddedRecorddelegate();
            public event AddedRecorddelegate AddedRecored;

            public delegate void DatamanReadyDelegate();
            public event DatamanReadyDelegate DatamanReady;

            public delegate void DatamanUnreadyDelegate();
            public event DatamanUnreadyDelegate DatamanUnready;

            //-->Variable
            private bool _isReady = false;
            private bool _isRunning = false;
            private bool _isConnectDataman = false;
            public bool IsReady
            {
                get
                {
                    return _isReady;
                }

                set
                {
                    if (_isReady == value) return;
                    _isReady = value;
                    if (_isReady) this.DatamanReady();
                    else this.DatamanUnready();
                }
            }
            /// <summary>
            /// Run() status
            /// </summary>
            public bool IsRunning
            {
                get
                {
                    return _isRunning;
                }

                set
                {
                    _isRunning = value;
                }
            }
            /// <summary>
            /// trang thai ket noi cua Dataman
            /// </summary>
            public bool IsConnectDataman
            {
                get
                {
                    return _isConnectDataman;
                }

                set
                {
                    if (_isConnectDataman == value) return;
                    _isConnectDataman = value;
                    if (value)
                    {
                        Connected();
                    }
                    else
                    {
                        Disconnected();
                    }
                }
            }

            /// <summary>
            /// Cac thong so cua dataman
            /// </summary>
            public DatabaseProvider.DatamanParam Params
            {
                get
                {
                    return _params;
                }

                set
                {
                    _params = value;
                }
            }
            private DatabaseProvider.DatamanParam _params;
            
            /// <summary>
            /// Ban ghi ket qua hien tai
            /// </summary>
            public DatabaseProvider.Record CurrentRecord
            {
                get
                {
                    return _currentRecord;
                }

                set
                {
                    _currentRecord = value;
                }
            }
            private DatabaseProvider.Record _currentRecord;
            /// <summary>
            /// Ban ghi ket qua hien tai
            /// </summary>
            public bool FirstTime
            {
                get
                {
                    return _firstTime;
                }

                set
                {
                    _firstTime = value;
                }
            }
            private bool _firstTime;
            /// <summary>
            /// danh sach ket qua doc duoc
            /// </summary>
            public List<DatabaseProvider.Record> RecordList
            {
                get
                {
                    return _recordList;
                }

                set
                {
                    _recordList = value;
                }
            }
            private List<DatabaseProvider.Record> _recordList;

            public List<DatabaseProvider.RecordView> RecordViewList
            {
                get
                {
                    return _recordViewList;
                }

                set
                {
                    _recordViewList = value;
                }
            }
            private List<DatabaseProvider.RecordView> _recordViewList;

            System.Windows.Forms.DataGridView TableView;

            public EEIPClient Eeip;
            /// <summary>
            /// Construction
            /// </summary>
            /// <param name="datamanid"></param>
            /// <param name="datagridview"></param>
            /// <param name="ipinputgoup"></param>
            public Dataman(string datamanid, System.Windows.Forms.DataGridView datagridview)
            {
                this._databasefilepath = Path.Combine(Helper.DatabaseProvider._databaseFolderPath, datamanid + ".db");
                this.Eeip = new EEIPClient();
                this.RecordList = new List<DatabaseProvider.Record>();
                this.RecordViewList = new List<DatabaseProvider.RecordView>();
                this.TableView = datagridview;
                this.Connected += Dataman_Connected;
                this.Disconnected += Dataman_Disconnected;
                this.AddedRecored += Dataman_AddedRecored;
                this.DatamanReady += Dataman_DatamanReady;
                this.DatamanUnready += Dataman_DatamanUnready;
                this.CurrentRecord = new DatabaseProvider.Record
                {
                    TimeLine = "",
                    DatamanId = datamanid,
                    TriggerId = "INITIAL",
                    ResultId = "",
                    Result = ""
                };
                if (this.ReadParams())
                {
                    
                }
            }

            private string _databasefilepath;
            /// <summary>
            /// Database
            /// </summary>
            public bool SaveParams()
            {
                try
                {
                    using (LiteDatabase db = new LiteDatabase(this._databasefilepath))
                    {
                        var col = db.GetCollection<DatabaseProvider.DatamanParam>();
                        this.Params.Id = 1;
                        if (col.FindOne(x => x.Id == 1) == null) col.Insert(this.Params);
                        else col.Update(this.Params);
                    }
                    return true;
                }
                catch(Exception t)
                {
                    ProgramHelper.LogErr("",t);
                    return false;
                }
            }

            public bool ReadParams()
            {
                try
                {
                    using (LiteDatabase db = new LiteDatabase(this._databasefilepath))
                    {
                        var col = db.GetCollection<DatabaseProvider.DatamanParam>();
                        DatabaseProvider.DatamanParam tempparams = col.FindOne(x => x.Id == 1);
                        if(tempparams != null)
                        {
                            this.Params = tempparams;
                        }
                        else
                        {
                            this.Params = new DatabaseProvider.DatamanParam()
                            {
                                DatamanId = "1",
                                Ip = "192.168.1.111",
                                LastResultId = "",
                                TriggerCount = 0,
                                LastTriggerId = "INITIAL",
                                ResultCount = 0,
                                LastResult = ""
                            };
                        }
                    }
                    return true;
                }
                catch(Exception t)
                {
                    ProgramHelper.LogErr("",t);
                    return false;
                }
            }

            private void Dataman_DatamanUnready()
            {
            }

            private void Dataman_DatamanReady()
            {
            }

            private void Dataman_AddedRecored()
            {
            }

            public void SetIp(string ip)
            {
                try
                {
                    if (string.IsNullOrEmpty(ip)) throw new Exception("Ip is Invalid");
                    this.Params.Ip = ip;
                }
                catch(Exception t)
                {
                    ProgramHelper.LogErr("", t);
                }
                
            }

            public void AddRecord(DatabaseProvider.Record record)
            {
                this.RecordList.Add(record);
                if (this.RecordList.Count > 100) this.RecordList.RemoveAt(0);
            }

            public void AddRecordView(DatabaseProvider.RecordView record)
            {
                this.RecordViewList.Add(record);
                if (this.RecordViewList.Count > 100) this.RecordViewList.RemoveAt(0);
                this.TableView.Invoke(new Action(() =>
                {
                    this.TableView.DataSource = null;
                    this.TableView.DataSource = this.RecordViewList;
                    this.TableView.FirstDisplayedScrollingRowIndex = this.TableView.RowCount - 1;
                    this.TableView.Refresh();
                }));
                if(this.Params.DatamanId == "0x02")
                {
                }
                else
                {
                    FrmMain.Instance.InputResultBuffer.Add(record);
                }
                this.AddedRecored();
            }

            public async Task Run()
            {
                try
                {
                    Task _ = new Task(() =>
                    {
                        this.SaveParams();
                        if (IsRunning) return;
                        this.IsConnectDataman = true;
                        this.IsRunning = true;
                        while (this.IsConnectDataman)
                        {
                            if (string.IsNullOrEmpty(this.Params.Ip))
                            {
                                this.IsConnectDataman = false;
                                break;
                            }
                            Ping ping = new Ping();
                            PingReply re = ping.Send(this.Params.Ip,1000);
                            if (!re.Status.ToString().Equals("Success"))
                            {
                                this.IsConnectDataman = false;
                                break;
                            }
                            this.Eeip.IPAddress = this.Params.Ip;
                            this.Eeip.RegisterSession();
                            byte[] data = this.Eeip.AssemblyObject.getInstance(11);
                            if (data.Length != 500)
                            {
                                using(StreamWriter writer = new StreamWriter(".\\LogError.txt"))
                                {
                                    writer.WriteLine($"ReceiveData Error: Datalength-{data.Length}");
                                }
                                continue;
                            };
                            // trigger ready
                            char i = Convert.ToString(data[0], toBase: 2)[0];
                            if (i == '1')
                            {
                                this.IsReady = true;
                            }
                            else this.IsReady = false;
                            Int16 _triggerid;
                            Int16 _resultid;
                            Int16 _resultcode;
                            Int16 _resultextended;
                            Int16 _resutldatalength;
                            string _resultdata;
                            ProcessRecieveData(data, out _triggerid, out _resultid, out _resultcode, out _resultextended, out _resutldatalength, out _resultdata);
                            // Co trigger moi
                            if(_triggerid.ToString() != this.Params.LastTriggerId && this.Params.LastTriggerId != "INITIAL")
                            {
                                this.Params.LastTriggerId = _triggerid.ToString();
                                if(!FirstTime)
                                {
                                    this.Params.LastResultId = _resultid.ToString();
                                    FirstTime = true;
                                }
                                //Co ket qua moi
                                if (_resultid.ToString() != this.Params.LastResultId)
                                {
                                    //-->TestMode
                                    if (this.Params.DatamanId == "0x02") System.Threading.Thread.Sleep(1000);
                                    //-->TestMode
                                    this.Params.LastResultId = _resultid.ToString();
                                    if (_resultdata == "NOT READ") System.Media.SystemSounds.Asterisk.Play();
                                    //Tao doi tuong Record cho database
                                    DatabaseProvider.Record newrecord = new DatabaseProvider.Record()
                                    {
                                        TimeLine = DateTime.Now.ToString("dd/MM/yy HH:mm:ss:fff"),
                                        DatamanId = this.Params.DatamanId,
                                        TriggerId = _triggerid.ToString(),
                                        ResultId = _triggerid.ToString(),
                                        Result = _resultdata
                                    };
                                    //Tao doi tuong Record cho View va Csv
                                    DatabaseProvider.RecordView newrecordview = new DatabaseProvider.RecordView()
                                    {
                                        TimeLine = DateTime.Now.ToString("dd/MM/yy HH:mm:ss:fff"),
                                        //DatamanId = this.Params.DatamanId,
                                        //TriggerId = _triggerid.ToString(),
                                        Result = _resultdata
                                    };
                                    //Update Params
                                    this.Params.TriggerCount += 1;
                                    this.Params.ResultCount += 1;
                                    this.Params.LastResult = _resultdata;
                                    this.SaveParams();
                                    this.AddRecord(newrecord);
                                    this.AddRecordView(newrecordview);
                                    
                                }
                            }
                            else
                            {
                                this.Params.LastTriggerId = _triggerid.ToString();
                            }
                        }
                        this.IsRunning = false;
                    });
                    _.Start();
                    await _;
                    this.IsRunning = false;
                }
                catch(System.IO.IOException)
                {
                    this.IsRunning = false;
                    this.IsConnectDataman = false;
                    try
                    {
                        this.Eeip.UnRegisterSession();
                    }
                    catch
                    { }
                }
                catch(Exception ex)
                {
                    try
                    {
                        this.Eeip.UnRegisterSession();
                    }
                    catch
                    { }
                    this.IsConnectDataman = false;
                    this.IsRunning = false;
                }
            }

            public void ProcessRecieveData(byte[] data, out Int16 triggerid, out Int16 resultid, out Int16 resultcode, out Int16 resultextended, out Int16 resultdatalength, out string resultdata)
            {
                triggerid = BitConverter.ToInt16(data, InputTopocol[InputTopocolKey.TRIGGERID]);
                resultid = BitConverter.ToInt16(data, InputTopocol[InputTopocolKey.RESULTID]);
                resultcode = BitConverter.ToInt16(data, InputTopocol[InputTopocolKey.RESULTCODE]);
                resultextended = BitConverter.ToInt16(data, InputTopocol[InputTopocolKey.RESULTEXTENDED]);
                resultdatalength = BitConverter.ToInt16(data, InputTopocol[InputTopocolKey.RESULTDATALENGTH]);
                if (resultdatalength > 0)
                {
                    byte[] tempdata = new byte[resultdatalength];
                    Array.Copy(data, InputTopocol[InputTopocolKey.RESULTDATA], tempdata, 0, resultdatalength);
                    resultdata = Encoding.ASCII.GetString(tempdata);
                }
                else
                {
                    resultdata = "NOT READ";
                }
            }

            public bool Connect(string ip)
            {
                
                try
                {
                    if (this.IsConnectDataman)
                    {
                        this.Disconect();
                    }
                    this.SetIp(ip);
                    Ping ping = new Ping();
                    PingReply rep = ping.Send(ip,1000);
                    if (rep.Status.ToString().Equals("Success"))
                    {
                        this.Eeip.IPAddress = ip;
                    }
                    while (this.IsRunning) ;
                    Task _ = this.Run();
                    this.Params.LastTriggerId = "-1";
                    return true;
                }
                catch(Exception t)
                {
                    ProgramHelper.LogErr("", t);
                    return false;
                }
            }

            public bool Disconect()
            {
                try
                {
                    if (this.IsConnectDataman)
                    {
                        this.IsConnectDataman = false;
                    }
                    return true;
                }
                catch (Exception t)
                {
                    ProgramHelper.LogErr("", t);
                    return false;
                }
            }

            private void Dataman_Disconnected()
            {
                //throw new NotImplementedException();
            }

            private void Dataman_Connected()
            {
                //throw new NotImplementedException();
            }
        }

        
    }
}
