namespace ECTL
{
    using Impinj.OctaneSdk;
    using ECTL.Properties;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows.Forms;

    public class MainForm : Form
    {
        private System.Threading.Timer _alertTimer;
        private TabPage _backupTab;
        private bool _bAlerted;
        private bool _bBeep;
        private bool _bShutdown;
        private CheckBox _chxBeep;
        private CheckBox _chxReader1Enabled;
        private CheckBox _chxReader2Enabled;
        private delegateDisplayTagRead _displayTagRead;
        private DateTime _dtBeep = DateTime.UtcNow;
        private ManualResetEvent _gatingSignal = new ManualResetEvent(false);
        private eGatingValue _gatingValue;
        private HashSet<string> _htTags = new HashSet<string>();
        private Label _lblBeep;
        private eLineConfig _lineConfig = eLineConfig.Dual;
        private double _nGateValue = 1.0;
        private int _nTotalReads;
        private SpeedwayReader _reader1;
        private eReaderChannel _reader1Channel;
        private NumericUpDown _reader1Power;
        private SpeedwayReader _reader2;
        private eReaderChannel _reader2Channel;
        private NumericUpDown _reader2Power;
        private Server _server;
        private ManualResetEvent _shutdownSignal = new ManualResetEvent(false);
        private ManualResetEvent _stopSignal = new ManualResetEvent(false);
        private Stream _stream;
        private TabControl _tabCtrl;
        private Dictionary<string, TagRead> _tagReads = new Dictionary<string, TagRead>();
        private TextBox _tbReader1;
        private TextBox _tbReader2;
        private StreamWriter _writer;
        private bool bTimingPointMissing;
        private Button btnBrowse;
        private Button btnConnect;
        private Button btnDisconnect;
        private Button btnStart;
        private Button btnStop;
        private ComboBox cboF1;
        private IContainer components;
        private DataGridView dataGridViewBackup;
        private DateTime date = DateTime.Now;
        private FolderBrowserDialog fbd = new FolderBrowserDialog();
        private string fileName;
        private GroupBox groupBox13;
        private GroupBox groupBox2;
        private GroupBox groupBox3;
        private GroupBox groupBox4;
        private GroupBox groupBox5;
        public bool isDataNeedToStore = true;
        private Label label10;
        private Label label14;
        private Label label2;
        private Label label22;
        private Label label23;
        private Label label26;
        private Label label27;
        private Label label28;
        private Label label3;
        private Label label30;
        private Label label4;
        private Label label5;
        private Label label8;
        private SearchMode m_searchMode;
        private string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        private PowerStatus power = SystemInformation.PowerStatus;
        private RadioButton radioBtnFinishLine;
        private RadioButton radioBtnStartLine;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private TextBox textBoxCurrentTime;
        private TextBox textBoxLastTagRead;
        private TextBox textBoxLastTimeRead;
        private TextBox textBoxTotalTagReads;
        private TextBox textBoxUniqueTagReads;
        private System.Windows.Forms.Timer timer1;
        private string timingPoint;
        private TextBox txtCheapId;
        private TabPage tabPage3;
        private Label label6;
        private Button button1;
        private Label label24;
        private ComboBox _cbLineConfig;
        private Label label13;
        private ComboBox _cbGatingValue;
        private NumericUpDown _tcpipPort;
        private Label label1;
        private GroupBox groupBox7;
        private Panel panel2;
        private Panel panel3;
        private Button _btnShutdown;
        private Button btnPurge;
        private Panel panel1;
        private ComboBox _cbReader1Channel;
        private Label label25;
        private Label label29;
        private ComboBox _cbReader2Channel;
        private TextBox textBoxTimingPointName;
        private Label label20;
        private GroupBox groupBox6;
        private TextBox txtpath;
        private RadioButton radioButton1;
        private Panel connectionStatusColor;
        private Label connectionStatus;
        private ListView listView1;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private ColumnHeader columnHeader3;
        private ColumnHeader columnHeader4;
        private ColumnHeader columnHeader5;
        private GroupBox groupBox1;
        private Label label7;
        private TextBox total1;
        private TextBox total2;
        private TextBox total6;
        private TextBox total5;
        private Label label15;
        private TextBox total8;
        private TextBox total7;
        private TextBox total4;
        private TextBox total3;
        myGreenButtonObject connectReaderButton;



        public MainForm()
        {
            this.InitializeComponent();
            this._alertTimer = new System.Threading.Timer(new TimerCallback(this.OnAlertTimerCallback));
            this.Text = string.Format("Event Chip Timing LTD", Assembly.GetExecutingAssembly().GetName().Version.ToString());
            this._tabCtrl.TabPages.Remove(this._backupTab);
            this.textBoxLastTagRead.Text = (string) (this.textBoxLastTimeRead.Text = null);
            this._displayTagRead = new delegateDisplayTagRead(this.DisplayTagRead);
            new Thread(new ThreadStart(this.GatingThread)).Start();

            connectReaderButton = new myGreenButtonObject();
            EventHandler connectReaderHandler = new EventHandler(OnConnectClicked);
            connectReaderButton.BackColor = Color.Transparent;
            connectReaderButton.Click += connectReaderHandler;
            connectReaderButton.Location = new System.Drawing.Point(220, 5);
            connectReaderButton.Size = new System.Drawing.Size(101,101);
            btnConnect.Controls.Add(connectReaderButton);

            myGreenButtonObject startReadingButton = new myGreenButtonObject();
            EventHandler startReadingButtonHandler = new EventHandler(btnStart_Click);
            startReadingButton.BackColor = Color.Transparent;
            startReadingButton.Click += startReadingButtonHandler;
            startReadingButton.Location = new System.Drawing.Point(130, 5);
            startReadingButton.Size = new System.Drawing.Size(101,101);
            btnStart.Controls.Add(startReadingButton);

            myRedButtonObject disconnectReaderButton = new myRedButtonObject();
            EventHandler disconnectReaderButtonHandler = new EventHandler(btnDisconnect_Click);
            disconnectReaderButton.BackColor = Color.Transparent;
            disconnectReaderButton.Click += disconnectReaderButtonHandler;
            disconnectReaderButton.Location = new System.Drawing.Point(130, 5);
            disconnectReaderButton.Size = new System.Drawing.Size(101, 101);
            btnDisconnect.Controls.Add(disconnectReaderButton);

            myRedButtonObject stopReadingButton = new myRedButtonObject();
            EventHandler stopReadingButtonHandler = new EventHandler(btnStop_Click);
            stopReadingButton.BackColor = Color.Transparent;
            stopReadingButton.Click += stopReadingButtonHandler;
            stopReadingButton.Location = new System.Drawing.Point(130, 5);
            stopReadingButton.Size = new System.Drawing.Size(101, 101);
            btnStop.Controls.Add(stopReadingButton);
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (this.cboF1.Text != "")
            {
                if (this.btnStart.Enabled && !this.btnStop.Enabled)
                {
                    if (this.fbd.ShowDialog() == DialogResult.OK)
                    {
                        this.txtpath.Text = this.fbd.SelectedPath;
                    }
                }
                else if (MessageBox.Show("Do you want to Stop Process !!!", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
                {
                    this.StopAllReaders();
                    this.btnStart.Text = "Start Readings Tags";
                    this.btnStart.Enabled = true;
                    this.btnStop.Enabled = false;
                    this.btnStart.BackColor = Color.Transparent;
                    this.btnStop.BackColor = Color.Transparent;
                    if (this.fbd.ShowDialog() == DialogResult.OK)
                    {
                        this.txtpath.Text = this.fbd.SelectedPath;
                    }
                }
                ECTL.Properties.Settings.Default.filepath = this.txtpath.Text;
                ECTL.Properties.Settings.Default.Save();
            }
            else
            {
                MessageBox.Show("Please Select The Timing Point", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show(this, "Are you sure you want to Disconnect?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                try
                {
                    this.ClearAndDisconnectAllReaders();
                }
                catch (OctaneSdkException exception)
                {
                    Console.WriteLine("OctaneSdk detected {0}", exception);
                }
                catch (Exception exception2)
                {
                    Console.WriteLine("Exception {0}", exception2);
                }
                this.Reset();
                this.Refresh();
            }
        }

        private void btnPurge_Click(object sender, EventArgs e)
        {
            DirectoryInfo info = new DirectoryInfo(this.txtpath.Text);
            FileInfo[] files = info.GetFiles("*.txt");
            if (files.Length > 0)
            {
                if (MessageBox.Show(string.Format("Do you want to delete {0} file(s) in {1}?", files.Length, info.FullName), "Delete files", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    foreach (FileInfo info2 in files)
                    {
                        try
                        {
                            info2.Delete();
                        }
                        catch (Exception exception)
                        {
                            MessageBox.Show(exception.Message, "Delete files", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("No files to delete.", "Delete files", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        private void btnSavebackup_Click(object sender, EventArgs e)
        {
            this.Savebackup();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                MainForm form;
                this.timingPoint = Convert.ToString(this.cboF1.Text);
                if (!Directory.Exists(this.txtpath.Text))
                {
                    throw new Exception(string.Format("Folder {0} does not exit, select a valid folder on the settings tab.", this.txtpath.Text));
                }
                string str = string.Format("{0}_{1:dd_MM_yyyy_H_mm_ss}.txt", this.cboF1.Text, DateTime.Now);
                string path = Path.Combine(this.txtpath.Text, str);
                Monitor.Enter(form = this);
                try
                {
                    this._server = new Server();
                    this._server.Listen(ECTL.Properties.Settings.Default.TcpIpPort);
                }
                catch
                {
                    this._server.Dispose();
                    this._server = null;
                }
                finally
                {
                    Monitor.Exit(form);
                }
                this.StartAllReaders();
                this.textBoxLastTagRead.Text = (string) (this.textBoxLastTimeRead.Text = null);
                lock (this)
                {
                    this._stream = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
                    this._writer = new StreamWriter(this._stream);
                }
                this.btnStart.BackColor = Color.Green;
                this.btnStart.Text = "Reading Tags";
                this.btnStop.BackColor = Color.Red;
                this.btnStop.ForeColor = Color.White;
                this.btnStart.Enabled = false;
                this.btnStop.Enabled = true;
                this.btnDisconnect.Enabled = false;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Reader Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show(this, "Do you want to Stop Reading?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                this.StopAllReaders();
                lock (this)
                {
                    if (this._server != null)
                    {
                        this._server.Dispose();
                        this._server = null;
                    }
                }
                lock (this)
                {
                    if (this._writer != null)
                    {
                        this._writer.Close();
                        this._writer.Dispose();
                        this._writer = null;
                    }
                    if (this._stream != null)
                    {
                        this._stream.Close();
                        this._stream.Dispose();
                        this._stream = null;
                    }
                }
                this.btnStart.Text = "Start Readings Tags";
                this.btnStart.Enabled = true;
                this.btnStart.BackColor = Color.Transparent;
                this.btnStop.BackColor = Color.Transparent;
                this.btnStop.Enabled = false;
                this.btnDisconnect.Enabled = true;
            }
        }

        private void cboF1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.textBoxTimingPointName.Text = this.cboF1.SelectedItem.ToString();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        internal bool checkLogerValidation()
        {
            if (!this.txtpath.Text.Validate())
            {
                MessageBox.Show(this, "Please Input The File Path ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            else if (!this._tbReader1.Text.Validate() && !this._tbReader2.Text.Validate())
            {
                MessageBox.Show(this, "Please Input The Reader Addres ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            else if (!this._chxReader1Enabled.Checked && !this._chxReader2Enabled.Checked)
            {
                MessageBox.Show(this, "No Readers Enabled", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            else
            {
                return true;
            }
            return false;
        }

        private void chkReader1_CheckedChanged(object sender, EventArgs e)
        {
            ECTL.Properties.Settings.Default.IsReader1 = this._chxReader1Enabled.Checked;
            ECTL.Properties.Settings.Default.Save();
        }

        private void chkReader2_CheckedChanged(object sender, EventArgs e)
        {
            ECTL.Properties.Settings.Default.IsReader2 = this._chxReader2Enabled.Checked;
            ECTL.Properties.Settings.Default.Save();
        }

        public void ClearAndDisconnectAllReaders()
        {
            foreach (SpeedwayReader reader in new SpeedwayReader[] { this._reader1, this._reader2 })
            {
                if (reader != null)
                {
                    reader.TagsReported -= new EventHandler<TagsReportedEventArgs>(this.TagsReportedHandler);
                    try
                    {
                        reader.ClearSettings();
                    }
                    catch (OctaneSdkException exception)
                    {
                        Console.WriteLine("Reader {0} clear: OctaneSdk detected {1}", reader.ReaderIdentity, exception);
                    }
                    catch (Exception exception2)
                    {
                        Console.WriteLine("Reader {0} clear: Exception {1}", reader.ReaderIdentity, exception2);
                    }
                    try
                    {
                        reader.SetGpo(4, false);
                        reader.Disconnect();
                    }
                    catch (OctaneSdkException exception3)
                    {
                        Console.WriteLine("OctaneSdk detected {0}", exception3);
                    }
                    catch (Exception exception4)
                    {
                        Console.WriteLine("Exception {0}", exception4);
                    }
                }
            }
            this._reader1 = (SpeedwayReader) (this._reader2 = null);
        }

        private SpeedwayReader ConnectReader(string strReaderName, eReaderChannel channel, double nPower)
        {
            if (!strReaderName.Validate())
            {
                return null;
            }
            SpeedwayReader reader = new SpeedwayReader {
                ReaderIdentity = strReaderName,
                LogLevel = Impinj.OctaneSdk.LogLevel.Error
            };
            reader.TagsReported += new EventHandler<TagsReportedEventArgs>(this.TagsReportedHandler);
            try
            {
                reader.Connect(strReaderName);
                Impinj.OctaneSdk.Settings settings = reader.QueryFactorySettings();
                settings.ReaderMode = ReaderMode.DenseReaderM8;
                settings.SearchMode = this.m_searchMode;
                settings.Report.Mode = ReportMode.IndividualUnbuffered;
                settings.Report.IncludeFirstSeenTime = false;
                settings.Report.IncludeAntennaPortNumber = true;
                settings.Report.IncludePeakRssi = true;
                if (eReaderChannel.ChannelA == channel)
                {
                    settings.TxFrequenciesInMhz.Add(865.7);
                }
                else if (eReaderChannel.ChannelB == channel)
                {
                    settings.TxFrequenciesInMhz.Add(866.3);
                }
                else if (eReaderChannel.ChannelC == channel)
                {
                    settings.TxFrequenciesInMhz.Add(866.9);
                }
                else if (eReaderChannel.ChannelD == channel)
                {
                    settings.TxFrequenciesInMhz.Add(867.5);
                }
                foreach (AntennaSettings settings2 in settings.Antennas)
                {
                    settings2.TxPowerInDbm = nPower;
                }
                reader.ApplySettings(settings);
                reader.SetGpo(4, false);
            }
            catch (Exception exception)
            {
                MessageBox.Show(this, string.Concat(new object[] { "Exception ", exception, " for reader ", strReaderName }));
                reader.Disconnect();
                reader = null;
            }
            return reader;
        }

        private void DisplayTagRead(TagRead tagRead)
        {
            if (base.InvokeRequired)
            {
                base.Invoke(this._displayTagRead, new object[] { tagRead });
            }
            else
            {
                try
                {
                    this.textBoxLastTimeRead.Text = tagRead.Tag.FirstSeenTime.AddHours(1).ToLongTimeString();
                    this.textBoxLastTagRead.Text = tagRead.EPC;
                    this.textBoxTotalTagReads.Text = this._nTotalReads.ToString();
                    this.textBoxUniqueTagReads.Text = this._htTags.Count.ToString();
                }
                catch
                {
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void GatingThread()
        {
            TagRead[] readArray = null;
            List<TagRead> tagReads = new List<TagRead>();
            WaitHandle[] waitHandles = new WaitHandle[] { this._shutdownSignal, this._gatingSignal };
            while (WaitHandle.WaitAny(waitHandles) != 0)
            {
                while (!this._stopSignal.WaitOne(100))
                {
                    lock (this._tagReads)
                    {
                        readArray = ToArray<TagRead>(this._tagReads.Values);
                    }
                    foreach (TagRead read in readArray)
                    {
                        TimeSpan span = (TimeSpan) (DateTime.UtcNow - read.UTC);
                        if (span.TotalSeconds >= this._nGateValue)
                        {
                            lock (this._tagReads)
                            {
                                this._tagReads.Remove(read.EPC);
                            }
                            tagReads.Add(read);
                        }
                    }
                    if (tagReads.Count > 0)
                    {
                        this.ProcessTagReads(tagReads);
                        tagReads.Clear();
                    }
                }
            }
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.textBoxTimingPointName = new System.Windows.Forms.TextBox();
            this._cbReader1Channel = new System.Windows.Forms.ComboBox();
            this.label20 = new System.Windows.Forms.Label();
            this._cbReader2Channel = new System.Windows.Forms.ComboBox();
            this.label29 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.groupBox13 = new System.Windows.Forms.GroupBox();
            this.label24 = new System.Windows.Forms.Label();
            this.radioBtnFinishLine = new System.Windows.Forms.RadioButton();
            this._cbLineConfig = new System.Windows.Forms.ComboBox();
            this.radioBtnStartLine = new System.Windows.Forms.RadioButton();
            this.label13 = new System.Windows.Forms.Label();
            this._cbGatingValue = new System.Windows.Forms.ComboBox();
            this._chxBeep = new System.Windows.Forms.CheckBox();
            this._lblBeep = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this._reader2Power = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this._tbReader2 = new System.Windows.Forms.TextBox();
            this._chxReader2Enabled = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtpath = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this._reader1Power = new System.Windows.Forms.NumericUpDown();
            this.label27 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this._chxReader1Enabled = new System.Windows.Forms.CheckBox();
            this._tbReader1 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this._backupTab = new System.Windows.Forms.TabPage();
            this.label10 = new System.Windows.Forms.Label();
            this.txtCheapId = new System.Windows.Forms.TextBox();
            this.dataGridViewBackup = new System.Windows.Forms.DataGridView();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.total8 = new System.Windows.Forms.TextBox();
            this.total7 = new System.Windows.Forms.TextBox();
            this.total4 = new System.Windows.Forms.TextBox();
            this.total3 = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.total1 = new System.Windows.Forms.TextBox();
            this.total2 = new System.Windows.Forms.TextBox();
            this.total6 = new System.Windows.Forms.TextBox();
            this.total5 = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label14 = new System.Windows.Forms.Label();
            this.connectionStatusColor = new System.Windows.Forms.Panel();
            this.connectionStatus = new System.Windows.Forms.Label();
            this.cboF1 = new System.Windows.Forms.ComboBox();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxLastTagRead = new System.Windows.Forms.TextBox();
            this.textBoxLastTimeRead = new System.Windows.Forms.TextBox();
            this.textBoxCurrentTime = new System.Windows.Forms.TextBox();
            this.label22 = new System.Windows.Forms.Label();
            this.textBoxUniqueTagReads = new System.Windows.Forms.TextBox();
            this.textBoxTotalTagReads = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this._tabCtrl = new System.Windows.Forms.TabControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.panel3 = new System.Windows.Forms.Panel();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this._btnShutdown = new System.Windows.Forms.Button();
            this._tcpipPort = new System.Windows.Forms.NumericUpDown();
            this.btnPurge = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage2.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox13.SuspendLayout();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._reader2Power)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._reader1Power)).BeginInit();
            this._backupTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewBackup)).BeginInit();
            this.tabPage1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.connectionStatusColor.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this._tabCtrl.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.groupBox7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._tcpipPort)).BeginInit();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.tabPage2.Controls.Add(this.groupBox6);
            this.tabPage2.Controls.Add(this.groupBox13);
            this.tabPage2.Controls.Add(this.groupBox5);
            this.tabPage2.Controls.Add(this.groupBox4);
            this.tabPage2.Controls.Add(this.groupBox3);
            this.tabPage2.Controls.Add(this.panel2);
            this.tabPage2.Location = new System.Drawing.Point(4, 35);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.tabPage2.Size = new System.Drawing.Size(626, 497);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "     Reader Settings     ";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.textBoxTimingPointName);
            this.groupBox6.Controls.Add(this._cbReader1Channel);
            this.groupBox6.Controls.Add(this.label20);
            this.groupBox6.Controls.Add(this._cbReader2Channel);
            this.groupBox6.Controls.Add(this.label29);
            this.groupBox6.Controls.Add(this.label25);
            this.groupBox6.Location = new System.Drawing.Point(108, 304);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(391, 117);
            this.groupBox6.TabIndex = 42;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Hided Elements";
            this.groupBox6.Visible = false;
            // 
            // textBoxTimingPointName
            // 
            this.textBoxTimingPointName.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxTimingPointName.Location = new System.Drawing.Point(17, 67);
            this.textBoxTimingPointName.Name = "textBoxTimingPointName";
            this.textBoxTimingPointName.ReadOnly = true;
            this.textBoxTimingPointName.Size = new System.Drawing.Size(100, 31);
            this.textBoxTimingPointName.TabIndex = 41;
            // 
            // _cbReader1Channel
            // 
            this._cbReader1Channel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbReader1Channel.FormattingEnabled = true;
            this._cbReader1Channel.Items.AddRange(new object[] {
            "Reader Selected",
            "Channel A - 865.70 Mhz",
            "Channel B - 866.30 Mhz",
            "Channel C - 866.90 Mhz",
            "Channel D - 867.50 Mhz"});
            this._cbReader1Channel.Location = new System.Drawing.Point(163, 64);
            this._cbReader1Channel.Name = "_cbReader1Channel";
            this._cbReader1Channel.Size = new System.Drawing.Size(82, 34);
            this._cbReader1Channel.TabIndex = 16;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label20.ForeColor = System.Drawing.Color.Black;
            this.label20.Location = new System.Drawing.Point(14, 36);
            this.label20.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(86, 18);
            this.label20.TabIndex = 40;
            this.label20.Text = "Timing Point";
            // 
            // _cbReader2Channel
            // 
            this._cbReader2Channel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbReader2Channel.FormattingEnabled = true;
            this._cbReader2Channel.Items.AddRange(new object[] {
            "Reader Selected",
            "Channel A - 865.70 Mhz",
            "Channel B - 866.30 Mhz",
            "Channel C - 866.90 Mhz",
            "Channel D - 867.50 Mhz"});
            this._cbReader2Channel.Location = new System.Drawing.Point(290, 64);
            this._cbReader2Channel.Name = "_cbReader2Channel";
            this._cbReader2Channel.Size = new System.Drawing.Size(94, 34);
            this._cbReader2Channel.TabIndex = 17;
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label29.Location = new System.Drawing.Point(160, 36);
            this.label29.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(55, 15);
            this.label29.TabIndex = 18;
            this.label29.Text = "Channel:";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label25.Location = new System.Drawing.Point(287, 38);
            this.label25.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(55, 15);
            this.label25.TabIndex = 15;
            this.label25.Text = "Channel:";
            // 
            // groupBox13
            // 
            this.groupBox13.Controls.Add(this.label24);
            this.groupBox13.Controls.Add(this.radioBtnFinishLine);
            this.groupBox13.Controls.Add(this._cbLineConfig);
            this.groupBox13.Controls.Add(this.radioBtnStartLine);
            this.groupBox13.Controls.Add(this.label13);
            this.groupBox13.Controls.Add(this._cbGatingValue);
            this.groupBox13.Controls.Add(this._chxBeep);
            this.groupBox13.Controls.Add(this._lblBeep);
            this.groupBox13.Controls.Add(this.label23);
            this.groupBox13.Font = new System.Drawing.Font("Calibri", 9.75F);
            this.groupBox13.Location = new System.Drawing.Point(8, 194);
            this.groupBox13.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox13.Name = "groupBox13";
            this.groupBox13.Size = new System.Drawing.Size(609, 99);
            this.groupBox13.TabIndex = 14;
            this.groupBox13.TabStop = false;
            this.groupBox13.Text = "Options";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label24.Location = new System.Drawing.Point(373, 54);
            this.label24.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(80, 18);
            this.label24.TabIndex = 36;
            this.label24.Text = "Line Config:";
            // 
            // radioBtnFinishLine
            // 
            this.radioBtnFinishLine.AutoSize = true;
            this.radioBtnFinishLine.Font = new System.Drawing.Font("Calibri", 14.25F);
            this.radioBtnFinishLine.Location = new System.Drawing.Point(247, 20);
            this.radioBtnFinishLine.Name = "radioBtnFinishLine";
            this.radioBtnFinishLine.Size = new System.Drawing.Size(108, 27);
            this.radioBtnFinishLine.TabIndex = 22;
            this.radioBtnFinishLine.TabStop = true;
            this.radioBtnFinishLine.Text = "Finish Line";
            this.radioBtnFinishLine.UseVisualStyleBackColor = true;
            this.radioBtnFinishLine.CheckedChanged += new System.EventHandler(this.radioBtnFinishLine_CheckedChanged);
            // 
            // _cbLineConfig
            // 
            this._cbLineConfig.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbLineConfig.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._cbLineConfig.FormattingEnabled = true;
            this._cbLineConfig.Items.AddRange(new object[] {
            "Single",
            "Dual"});
            this._cbLineConfig.Location = new System.Drawing.Point(457, 51);
            this._cbLineConfig.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this._cbLineConfig.Name = "_cbLineConfig";
            this._cbLineConfig.Size = new System.Drawing.Size(143, 26);
            this._cbLineConfig.TabIndex = 35;
            // 
            // radioBtnStartLine
            // 
            this.radioBtnStartLine.AutoSize = true;
            this.radioBtnStartLine.Font = new System.Drawing.Font("Calibri", 14.25F);
            this.radioBtnStartLine.Location = new System.Drawing.Point(138, 20);
            this.radioBtnStartLine.Name = "radioBtnStartLine";
            this.radioBtnStartLine.Size = new System.Drawing.Size(100, 27);
            this.radioBtnStartLine.TabIndex = 21;
            this.radioBtnStartLine.TabStop = true;
            this.radioBtnStartLine.Text = "Start Line";
            this.radioBtnStartLine.UseVisualStyleBackColor = true;
            this.radioBtnStartLine.CheckedChanged += new System.EventHandler(this.radioBtnStartLine_CheckedChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(364, 22);
            this.label13.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(89, 18);
            this.label13.TabIndex = 34;
            this.label13.Text = "Gating Value:";
            // 
            // _cbGatingValue
            // 
            this._cbGatingValue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cbGatingValue.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._cbGatingValue.FormattingEnabled = true;
            this._cbGatingValue.Items.AddRange(new object[] {
            "1 Second",
            "3 Seconds",
            "5 Seconds"});
            this._cbGatingValue.Location = new System.Drawing.Point(457, 19);
            this._cbGatingValue.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this._cbGatingValue.Name = "_cbGatingValue";
            this._cbGatingValue.Size = new System.Drawing.Size(143, 26);
            this._cbGatingValue.TabIndex = 33;
            // 
            // _chxBeep
            // 
            this._chxBeep.AutoSize = true;
            this._chxBeep.Location = new System.Drawing.Point(197, 70);
            this._chxBeep.Name = "_chxBeep";
            this._chxBeep.Size = new System.Drawing.Size(15, 14);
            this._chxBeep.TabIndex = 14;
            this._chxBeep.UseVisualStyleBackColor = true;
            this._chxBeep.CheckedChanged += new System.EventHandler(this.OnBeepChanged);
            // 
            // _lblBeep
            // 
            this._lblBeep.AutoSize = true;
            this._lblBeep.Font = new System.Drawing.Font("Calibri", 14.25F);
            this._lblBeep.Location = new System.Drawing.Point(9, 63);
            this._lblBeep.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this._lblBeep.Name = "_lblBeep";
            this._lblBeep.Size = new System.Drawing.Size(178, 23);
            this._lblBeep.TabIndex = 13;
            this._lblBeep.Text = "Beep when tag is read";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Font = new System.Drawing.Font("Calibri", 14.25F);
            this.label23.Location = new System.Drawing.Point(9, 22);
            this.label23.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(116, 23);
            this.label23.TabIndex = 12;
            this.label23.Text = "Search Mode:";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this._reader2Power);
            this.groupBox5.Controls.Add(this.label3);
            this.groupBox5.Controls.Add(this.label28);
            this.groupBox5.Controls.Add(this.label30);
            this.groupBox5.Controls.Add(this._tbReader2);
            this.groupBox5.Controls.Add(this._chxReader2Enabled);
            this.groupBox5.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox5.Location = new System.Drawing.Point(317, 5);
            this.groupBox5.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox5.Size = new System.Drawing.Size(300, 124);
            this.groupBox5.TabIndex = 13;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Reader 2";
            // 
            // _reader2Power
            // 
            this._reader2Power.DecimalPlaces = 2;
            this._reader2Power.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this._reader2Power.Location = new System.Drawing.Point(72, 92);
            this._reader2Power.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this._reader2Power.Name = "_reader2Power";
            this._reader2Power.Size = new System.Drawing.Size(64, 23);
            this._reader2Power.TabIndex = 17;
            this._reader2Power.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this._reader2Power.ValueChanged += new System.EventHandler(this.OnPowerChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(9, 94);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 15);
            this.label3.TabIndex = 16;
            this.label3.Text = "Power:";
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label28.Location = new System.Drawing.Point(9, 28);
            this.label28.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(54, 15);
            this.label28.TabIndex = 15;
            this.label28.Text = "Enabled:";
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label30.Location = new System.Drawing.Point(9, 54);
            this.label30.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label30.MaximumSize = new System.Drawing.Size(75, 0);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(62, 30);
            this.label30.TabIndex = 13;
            this.label30.Text = "Reader IP Address:";
            // 
            // _tbReader2
            // 
            this._tbReader2.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._tbReader2.Location = new System.Drawing.Point(72, 57);
            this._tbReader2.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this._tbReader2.Name = "_tbReader2";
            this._tbReader2.Size = new System.Drawing.Size(206, 23);
            this._tbReader2.TabIndex = 2;
            this._tbReader2.Leave += new System.EventHandler(this.txtReader2_Leave);
            // 
            // _chxReader2Enabled
            // 
            this._chxReader2Enabled.AutoSize = true;
            this._chxReader2Enabled.Checked = true;
            this._chxReader2Enabled.CheckState = System.Windows.Forms.CheckState.Checked;
            this._chxReader2Enabled.Location = new System.Drawing.Point(73, 29);
            this._chxReader2Enabled.Margin = new System.Windows.Forms.Padding(2);
            this._chxReader2Enabled.Name = "_chxReader2Enabled";
            this._chxReader2Enabled.Size = new System.Drawing.Size(15, 14);
            this._chxReader2Enabled.TabIndex = 3;
            this._chxReader2Enabled.UseVisualStyleBackColor = true;
            this._chxReader2Enabled.CheckedChanged += new System.EventHandler(this.chkReader2_CheckedChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.btnBrowse);
            this.groupBox4.Controls.Add(this.txtpath);
            this.groupBox4.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox4.Location = new System.Drawing.Point(7, 131);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox4.Size = new System.Drawing.Size(610, 60);
            this.groupBox4.TabIndex = 10;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Save results file to:";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBrowse.Location = new System.Drawing.Point(360, 20);
            this.btnBrowse.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(79, 25);
            this.btnBrowse.TabIndex = 1;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtpath
            // 
            this.txtpath.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtpath.Location = new System.Drawing.Point(18, 21);
            this.txtpath.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.txtpath.Name = "txtpath";
            this.txtpath.Size = new System.Drawing.Size(338, 23);
            this.txtpath.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this._reader1Power);
            this.groupBox3.Controls.Add(this.label27);
            this.groupBox3.Controls.Add(this.label26);
            this.groupBox3.Controls.Add(this._chxReader1Enabled);
            this.groupBox3.Controls.Add(this._tbReader1);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(8, 5);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox3.Size = new System.Drawing.Size(297, 124);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Reader 1";
            // 
            // _reader1Power
            // 
            this._reader1Power.DecimalPlaces = 2;
            this._reader1Power.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this._reader1Power.Location = new System.Drawing.Point(72, 94);
            this._reader1Power.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this._reader1Power.Name = "_reader1Power";
            this._reader1Power.Size = new System.Drawing.Size(64, 23);
            this._reader1Power.TabIndex = 14;
            this._reader1Power.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this._reader1Power.ValueChanged += new System.EventHandler(this.OnPowerChanged);
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label27.Location = new System.Drawing.Point(10, 96);
            this.label27.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(44, 15);
            this.label27.TabIndex = 12;
            this.label27.Text = "Power:";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label26.Location = new System.Drawing.Point(9, 28);
            this.label26.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(54, 15);
            this.label26.TabIndex = 11;
            this.label26.Text = "Enabled:";
            // 
            // _chxReader1Enabled
            // 
            this._chxReader1Enabled.AutoSize = true;
            this._chxReader1Enabled.Checked = true;
            this._chxReader1Enabled.CheckState = System.Windows.Forms.CheckState.Checked;
            this._chxReader1Enabled.Location = new System.Drawing.Point(73, 29);
            this._chxReader1Enabled.Margin = new System.Windows.Forms.Padding(2);
            this._chxReader1Enabled.Name = "_chxReader1Enabled";
            this._chxReader1Enabled.Size = new System.Drawing.Size(15, 14);
            this._chxReader1Enabled.TabIndex = 1;
            this._chxReader1Enabled.UseVisualStyleBackColor = true;
            this._chxReader1Enabled.CheckedChanged += new System.EventHandler(this.chkReader1_CheckedChanged);
            // 
            // _tbReader1
            // 
            this._tbReader1.Font = new System.Drawing.Font("Calibri", 9.7F);
            this._tbReader1.Location = new System.Drawing.Point(72, 59);
            this._tbReader1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this._tbReader1.Name = "_tbReader1";
            this._tbReader1.Size = new System.Drawing.Size(206, 23);
            this._tbReader1.TabIndex = 0;
            this._tbReader1.Leave += new System.EventHandler(this.txtReader1_Leave);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(9, 54);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.MaximumSize = new System.Drawing.Size(75, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 30);
            this.label2.TabIndex = 5;
            this.label2.Text = "Reader IP Address:";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.BackgroundImage = global::Properties.Resources.Event_chip_timing;
            this.panel2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.panel2.Location = new System.Drawing.Point(-4, 397);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(634, 101);
            this.panel2.TabIndex = 20;
            // 
            // _backupTab
            // 
            this._backupTab.Controls.Add(this.label10);
            this._backupTab.Controls.Add(this.txtCheapId);
            this._backupTab.Controls.Add(this.dataGridViewBackup);
            this._backupTab.Location = new System.Drawing.Point(4, 35);
            this._backupTab.Name = "_backupTab";
            this._backupTab.Size = new System.Drawing.Size(626, 497);
            this._backupTab.TabIndex = 4;
            this._backupTab.Text = "Backup";
            this._backupTab.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(130, 17);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(316, 26);
            this.label10.TabIndex = 6;
            this.label10.Text = "Type a race number and press enter";
            // 
            // txtCheapId
            // 
            this.txtCheapId.Font = new System.Drawing.Font("Calibri", 48F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCheapId.Location = new System.Drawing.Point(8, 59);
            this.txtCheapId.Name = "txtCheapId";
            this.txtCheapId.Size = new System.Drawing.Size(606, 86);
            this.txtCheapId.TabIndex = 5;
            this.txtCheapId.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtCheapId_KeyPress);
            // 
            // dataGridViewBackup
            // 
            this.dataGridViewBackup.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewBackup.Location = new System.Drawing.Point(8, 151);
            this.dataGridViewBackup.Name = "dataGridViewBackup";
            this.dataGridViewBackup.Size = new System.Drawing.Size(610, 224);
            this.dataGridViewBackup.TabIndex = 3;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(121)))), ((int)(((byte)(113)))));
            this.tabPage1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Controls.Add(this.listView1);
            this.tabPage1.Controls.Add(this.label14);
            this.tabPage1.Controls.Add(this.connectionStatusColor);
            this.tabPage1.Controls.Add(this.cboF1);
            this.tabPage1.Controls.Add(this.radioButton1);
            this.tabPage1.Controls.Add(this.panel1);
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Location = new System.Drawing.Point(4, 35);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.tabPage1.Size = new System.Drawing.Size(626, 497);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Tag Reading ";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.total8);
            this.groupBox1.Controls.Add(this.total7);
            this.groupBox1.Controls.Add(this.total4);
            this.groupBox1.Controls.Add(this.total3);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.total1);
            this.groupBox1.Controls.Add(this.total2);
            this.groupBox1.Controls.Add(this.total6);
            this.groupBox1.Controls.Add(this.total5);
            this.groupBox1.Controls.Add(this.label15);
            this.groupBox1.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(14, 118);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(598, 69);
            this.groupBox1.TabIndex = 40;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Total Reads";
            // 
            // total8
            // 
            this.total8.Font = new System.Drawing.Font("Calibri", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.total8.Location = new System.Drawing.Point(486, 39);
            this.total8.Name = "total8";
            this.total8.ReadOnly = true;
            this.total8.Size = new System.Drawing.Size(54, 24);
            this.total8.TabIndex = 43;
            this.total8.Text = "0";
            // 
            // total7
            // 
            this.total7.Font = new System.Drawing.Font("Calibri", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.total7.Location = new System.Drawing.Point(428, 39);
            this.total7.Name = "total7";
            this.total7.ReadOnly = true;
            this.total7.Size = new System.Drawing.Size(52, 24);
            this.total7.TabIndex = 42;
            this.total7.Text = "0";
            // 
            // total4
            // 
            this.total4.Font = new System.Drawing.Font("Calibri", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.total4.Location = new System.Drawing.Point(229, 40);
            this.total4.Name = "total4";
            this.total4.ReadOnly = true;
            this.total4.Size = new System.Drawing.Size(52, 24);
            this.total4.TabIndex = 41;
            this.total4.Text = "0";
            // 
            // total3
            // 
            this.total3.Font = new System.Drawing.Font("Calibri", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.total3.Location = new System.Drawing.Point(175, 40);
            this.total3.Name = "total3";
            this.total3.ReadOnly = true;
            this.total3.Size = new System.Drawing.Size(48, 24);
            this.total3.TabIndex = 40;
            this.total3.Text = "0";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Calibri", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(130, 13);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(84, 24);
            this.label7.TabIndex = 29;
            this.label7.Text = "Reader 1";
            // 
            // total1
            // 
            this.total1.Font = new System.Drawing.Font("Calibri", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.total1.Location = new System.Drawing.Point(65, 40);
            this.total1.Name = "total1";
            this.total1.ReadOnly = true;
            this.total1.Size = new System.Drawing.Size(48, 24);
            this.total1.TabIndex = 31;
            this.total1.Text = "0";
            // 
            // total2
            // 
            this.total2.Font = new System.Drawing.Font("Calibri", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.total2.Location = new System.Drawing.Point(119, 40);
            this.total2.Name = "total2";
            this.total2.ReadOnly = true;
            this.total2.Size = new System.Drawing.Size(50, 24);
            this.total2.TabIndex = 32;
            this.total2.Text = "0";
            // 
            // total6
            // 
            this.total6.Font = new System.Drawing.Font("Calibri", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.total6.Location = new System.Drawing.Point(373, 39);
            this.total6.Name = "total6";
            this.total6.ReadOnly = true;
            this.total6.Size = new System.Drawing.Size(49, 24);
            this.total6.TabIndex = 37;
            this.total6.Text = "0";
            // 
            // total5
            // 
            this.total5.Font = new System.Drawing.Font("Calibri", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.total5.Location = new System.Drawing.Point(317, 39);
            this.total5.Name = "total5";
            this.total5.ReadOnly = true;
            this.total5.Size = new System.Drawing.Size(50, 24);
            this.total5.TabIndex = 36;
            this.total5.Text = "0";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Calibri", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(386, 13);
            this.label15.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(84, 24);
            this.label15.TabIndex = 35;
            this.label15.Text = "Reader 2";
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5});
            this.listView1.Font = new System.Drawing.Font("Calibri", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listView1.GridLines = true;
            this.listView1.Location = new System.Drawing.Point(14, 188);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(598, 90);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Read count";
            this.columnHeader1.Width = 130;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Tag ID";
            this.columnHeader2.Width = 120;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Time";
            this.columnHeader3.Width = 140;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Antenna ID";
            this.columnHeader4.Width = 120;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "IP";
            this.columnHeader5.Width = 65;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(154, 93);
            this.label14.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(93, 18);
            this.label14.TabIndex = 28;
            this.label14.Text = "Timing Point :";
            // 
            // connectionStatusColor
            // 
            this.connectionStatusColor.BackColor = System.Drawing.Color.Red;
            this.connectionStatusColor.Controls.Add(this.connectionStatus);
            this.connectionStatusColor.Location = new System.Drawing.Point(157, 54);
            this.connectionStatusColor.Name = "connectionStatusColor";
            this.connectionStatusColor.Size = new System.Drawing.Size(300, 28);
            this.connectionStatusColor.TabIndex = 22;
            // 
            // connectionStatus
            // 
            this.connectionStatus.Font = new System.Drawing.Font("Calibri", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.connectionStatus.Location = new System.Drawing.Point(0, 0);
            this.connectionStatus.Name = "connectionStatus";
            this.connectionStatus.Size = new System.Drawing.Size(300, 27);
            this.connectionStatus.TabIndex = 0;
            this.connectionStatus.Text = "Disconnected";
            this.connectionStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cboF1
            // 
            this.cboF1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboF1.Font = new System.Drawing.Font("Calibri", 14.25F);
            this.cboF1.FormattingEnabled = true;
            this.cboF1.Location = new System.Drawing.Point(263, 86);
            this.cboF1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.cboF1.Name = "cboF1";
            this.cboF1.Size = new System.Drawing.Size(194, 31);
            this.cboF1.TabIndex = 27;
            this.cboF1.SelectedIndexChanged += new System.EventHandler(this.cboF1_SelectedIndexChanged);
            // 
            // radioButton1
            // 
            this.radioButton1.BackColor = System.Drawing.Color.Gainsboro;
            this.radioButton1.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButton1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.radioButton1.Location = new System.Drawing.Point(157, 4);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Padding = new System.Windows.Forms.Padding(10);
            this.radioButton1.Size = new System.Drawing.Size(300, 50);
            this.radioButton1.TabIndex = 0;
            this.radioButton1.Text = "Connect and Start Reading";
            this.radioButton1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButton1.UseVisualStyleBackColor = false;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            this.radioButton1.Click += new System.EventHandler(this.radioButton1_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BackgroundImage = global::Properties.Resources.Event_chip_timing;
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.panel1.Controls.Add(this.btnConnect);
            this.panel1.Controls.Add(this.btnStart);
            this.panel1.Controls.Add(this.btnDisconnect);
            this.panel1.Controls.Add(this.btnStop);
            this.panel1.Location = new System.Drawing.Point(-4, 397);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(634, 101);
            this.panel1.TabIndex = 21;
            // 
            // btnConnect
            // 
            this.btnConnect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(186)))), ((int)(((byte)(186)))), ((int)(((byte)(186)))));
            this.btnConnect.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConnect.Location = new System.Drawing.Point(11, 16);
            this.btnConnect.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(59, 25);
            this.btnConnect.TabIndex = 0;
            this.btnConnect.Text = "Connect Reader and start reading";
            this.btnConnect.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnConnect.UseVisualStyleBackColor = false;
            this.btnConnect.Visible = false;
            this.btnConnect.Click += new System.EventHandler(this.OnConnectClicked);
            this.btnConnect.MouseEnter += new System.EventHandler(this.btnConnect_MouseEnter);
            // 
            // btnStart
            // 
            this.btnStart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(186)))), ((int)(((byte)(186)))), ((int)(((byte)(186)))));
            this.btnStart.Enabled = false;
            this.btnStart.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStart.Location = new System.Drawing.Point(11, 47);
            this.btnStart.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(59, 21);
            this.btnStart.TabIndex = 2;
            this.btnStart.Text = "Start Reading Tags";
            this.btnStart.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnStart.UseVisualStyleBackColor = false;
            this.btnStart.Visible = false;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(186)))), ((int)(((byte)(186)))), ((int)(((byte)(186)))));
            this.btnDisconnect.Enabled = false;
            this.btnDisconnect.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDisconnect.Location = new System.Drawing.Point(74, 16);
            this.btnDisconnect.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(58, 25);
            this.btnDisconnect.TabIndex = 1;
            this.btnDisconnect.Text = "Disconnect Reader";
            this.btnDisconnect.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDisconnect.UseVisualStyleBackColor = false;
            this.btnDisconnect.Visible = false;
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
            // 
            // btnStop
            // 
            this.btnStop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(186)))), ((int)(((byte)(186)))), ((int)(((byte)(186)))));
            this.btnStop.Enabled = false;
            this.btnStop.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStop.Location = new System.Drawing.Point(74, 47);
            this.btnStop.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(58, 21);
            this.btnStop.TabIndex = 3;
            this.btnStop.Text = "Stop Reading Tags";
            this.btnStop.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnStop.UseVisualStyleBackColor = false;
            this.btnStop.Visible = false;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.button1);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.textBoxLastTagRead);
            this.groupBox2.Controls.Add(this.textBoxLastTimeRead);
            this.groupBox2.Controls.Add(this.textBoxCurrentTime);
            this.groupBox2.Controls.Add(this.label22);
            this.groupBox2.Controls.Add(this.textBoxUniqueTagReads);
            this.groupBox2.Controls.Add(this.textBoxTotalTagReads);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.groupBox2.Location = new System.Drawing.Point(14, 283);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox2.Size = new System.Drawing.Size(598, 113);
            this.groupBox2.TabIndex = 18;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Tags Reads and Stats ";
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(477, 75);
            this.button1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(107, 31);
            this.button1.TabIndex = 33;
            this.button1.Text = "Set Time";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.Black;
            this.label6.Location = new System.Drawing.Point(262, 21);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(44, 18);
            this.label6.TabIndex = 43;
            this.label6.Text = "Tag ID";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textBoxLastTagRead
            // 
            this.textBoxLastTagRead.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxLastTagRead.Location = new System.Drawing.Point(311, 15);
            this.textBoxLastTagRead.Name = "textBoxLastTagRead";
            this.textBoxLastTagRead.ReadOnly = true;
            this.textBoxLastTagRead.Size = new System.Drawing.Size(273, 31);
            this.textBoxLastTagRead.TabIndex = 42;
            // 
            // textBoxLastTimeRead
            // 
            this.textBoxLastTimeRead.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxLastTimeRead.Location = new System.Drawing.Point(116, 15);
            this.textBoxLastTimeRead.Name = "textBoxLastTimeRead";
            this.textBoxLastTimeRead.ReadOnly = true;
            this.textBoxLastTimeRead.Size = new System.Drawing.Size(131, 31);
            this.textBoxLastTimeRead.TabIndex = 41;
            // 
            // textBoxCurrentTime
            // 
            this.textBoxCurrentTime.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxCurrentTime.Location = new System.Drawing.Point(265, 75);
            this.textBoxCurrentTime.Name = "textBoxCurrentTime";
            this.textBoxCurrentTime.ReadOnly = true;
            this.textBoxCurrentTime.Size = new System.Drawing.Size(201, 31);
            this.textBoxCurrentTime.TabIndex = 40;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label22.ForeColor = System.Drawing.Color.Black;
            this.label22.Location = new System.Drawing.Point(262, 52);
            this.label22.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(87, 18);
            this.label22.TabIndex = 30;
            this.label22.Text = "System Time";
            // 
            // textBoxUniqueTagReads
            // 
            this.textBoxUniqueTagReads.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxUniqueTagReads.Location = new System.Drawing.Point(143, 75);
            this.textBoxUniqueTagReads.Name = "textBoxUniqueTagReads";
            this.textBoxUniqueTagReads.ReadOnly = true;
            this.textBoxUniqueTagReads.Size = new System.Drawing.Size(104, 31);
            this.textBoxUniqueTagReads.TabIndex = 38;
            this.textBoxUniqueTagReads.Text = "0";
            // 
            // textBoxTotalTagReads
            // 
            this.textBoxTotalTagReads.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxTotalTagReads.Location = new System.Drawing.Point(27, 75);
            this.textBoxTotalTagReads.Name = "textBoxTotalTagReads";
            this.textBoxTotalTagReads.ReadOnly = true;
            this.textBoxTotalTagReads.Size = new System.Drawing.Size(101, 31);
            this.textBoxTotalTagReads.TabIndex = 37;
            this.textBoxTotalTagReads.Text = "0";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.Black;
            this.label5.Location = new System.Drawing.Point(24, 21);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(66, 18);
            this.label5.TabIndex = 36;
            this.label5.Text = "Last Read";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.Black;
            this.label8.Location = new System.Drawing.Point(140, 52);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(93, 18);
            this.label8.TabIndex = 21;
            this.label8.Text = "Unique Reads";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(24, 52);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(78, 18);
            this.label4.TabIndex = 8;
            this.label4.Text = "Total Reads";
            // 
            // _tabCtrl
            // 
            this._tabCtrl.Controls.Add(this.tabPage1);
            this._tabCtrl.Controls.Add(this._backupTab);
            this._tabCtrl.Controls.Add(this.tabPage2);
            this._tabCtrl.Controls.Add(this.tabPage3);
            this._tabCtrl.Dock = System.Windows.Forms.DockStyle.Fill;
            this._tabCtrl.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._tabCtrl.Location = new System.Drawing.Point(0, 0);
            this._tabCtrl.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this._tabCtrl.Name = "_tabCtrl";
            this._tabCtrl.SelectedIndex = 0;
            this._tabCtrl.Size = new System.Drawing.Size(634, 536);
            this._tabCtrl.TabIndex = 0;
            this._tabCtrl.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPage3
            // 
            this.tabPage3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(186)))), ((int)(((byte)(186)))), ((int)(((byte)(186)))));
            this.tabPage3.Controls.Add(this.panel3);
            this.tabPage3.Controls.Add(this.groupBox7);
            this.tabPage3.Location = new System.Drawing.Point(4, 35);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(626, 497);
            this.tabPage3.TabIndex = 5;
            this.tabPage3.Text = "Other Settings";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.BackgroundImage = global::Properties.Resources.Event_chip_timing;
            this.panel3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.panel3.Location = new System.Drawing.Point(-4, 397);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(634, 101);
            this.panel3.TabIndex = 23;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this._btnShutdown);
            this.groupBox7.Controls.Add(this._tcpipPort);
            this.groupBox7.Controls.Add(this.btnPurge);
            this.groupBox7.Controls.Add(this.label1);
            this.groupBox7.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.groupBox7.Location = new System.Drawing.Point(7, 9);
            this.groupBox7.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox7.Size = new System.Drawing.Size(612, 210);
            this.groupBox7.TabIndex = 22;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Other Settings";
            // 
            // _btnShutdown
            // 
            this._btnShutdown.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._btnShutdown.Location = new System.Drawing.Point(17, 149);
            this._btnShutdown.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this._btnShutdown.Name = "_btnShutdown";
            this._btnShutdown.Size = new System.Drawing.Size(143, 48);
            this._btnShutdown.TabIndex = 25;
            this._btnShutdown.Text = "Shutdown";
            this._btnShutdown.UseVisualStyleBackColor = true;
            this._btnShutdown.Click += new System.EventHandler(this._btnShutdown_Click);
            // 
            // _tcpipPort
            // 
            this._tcpipPort.Font = new System.Drawing.Font("Calibri", 9.75F);
            this._tcpipPort.Location = new System.Drawing.Point(90, 39);
            this._tcpipPort.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this._tcpipPort.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._tcpipPort.Name = "_tcpipPort";
            this._tcpipPort.Size = new System.Drawing.Size(70, 23);
            this._tcpipPort.TabIndex = 20;
            this._tcpipPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this._tcpipPort.Value = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            // 
            // btnPurge
            // 
            this.btnPurge.Font = new System.Drawing.Font("Calibri", 14.25F);
            this.btnPurge.Location = new System.Drawing.Point(17, 82);
            this.btnPurge.Name = "btnPurge";
            this.btnPurge.Size = new System.Drawing.Size(143, 48);
            this.btnPurge.TabIndex = 24;
            this.btnPurge.Text = "Delete Files";
            this.btnPurge.UseVisualStyleBackColor = true;
            this.btnPurge.Click += new System.EventHandler(this.btnPurge_Click_1);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(14, 41);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 15);
            this.label1.TabIndex = 19;
            this.label1.Text = "TCP/IP Port:";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(634, 536);
            this.Controls.Add(this._tabCtrl);
            this.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Event Chip Timing LTD";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.tabPage2.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox13.ResumeLayout(false);
            this.groupBox13.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._reader2Power)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._reader1Power)).EndInit();
            this._backupTab.ResumeLayout(false);
            this._backupTab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewBackup)).EndInit();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.connectionStatusColor.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this._tabCtrl.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._tcpipPort)).EndInit();
            this.ResumeLayout(false);

        }

        private void OnAlertTimerCallback(object state)
        {
            lock (this)
            {
                if (this._bAlerted)
                {
                    this._bAlerted = false;
                    try
                    {
                        if (this._reader1 != null)
                        {
                            this._reader1.SetGpo(4, false);
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }

        private void OnBeepChanged(object sender, EventArgs e)
        {
            ECTL.Properties.Settings.Default.Beep = this._bBeep = this._chxBeep.Checked;
            ECTL.Properties.Settings.Default.Save();
        }

        private void OnConnectClicked(object sender, EventArgs e)
        {
            if (this.checkLogerValidation())
            {
                if (this._chxReader1Enabled.Checked)
                {
                    if (this._tbReader1.Text.Validate())
                    {
                        this._reader1 = this.ConnectReader(this._tbReader1.Text, this._reader1Channel, ECTL.Properties.Settings.Default.Reader1Power);
                    }
                    else
                    {
                        MessageBox.Show(this, "Reader 1's Address is Empty !!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    }
                }
                if (this._chxReader2Enabled.Checked)
                {
                    if (this._tbReader2.Text.Validate())
                    {
                        this._reader2 = this.ConnectReader(this._tbReader2.Text, this._reader2Channel, ECTL.Properties.Settings.Default.Reader2Power);
                    }
                    else
                    {
                        MessageBox.Show("Reader 2's Address is Empty !!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    }
                }
                this.Refresh();
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            this.StopAllReaders();
            lock (this)
            {
                if (this._server != null)
                {
                    this._server.Dispose();
                    this._server = null;
                }
                if (this._alertTimer != null)
                {
                    this._alertTimer.Dispose();
                    this._alertTimer = null;
                }
            }
            this._stopSignal.Set();
            this._shutdownSignal.Set();
            base.OnFormClosed(e);
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            if (!this._bShutdown && !this.bTimingPointMissing)
            {
                if (DialogResult.Yes == MessageBox.Show(this, "Quit Application?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    Application.ExitThread();
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        private void OnGatingValueChanged(object sender, EventArgs e)
        {
            this._gatingValue = (eGatingValue) ((byte) this._cbGatingValue.SelectedIndex);
            if (this._gatingValue == eGatingValue.n1Second)
            {
                this._nGateValue = 1.0;
            }
            else if (eGatingValue.n3Seconds == this._gatingValue)
            {
                this._nGateValue = 3.0;
            }
            else if (eGatingValue.n5Seconds == this._gatingValue)
            {
                this._nGateValue = 5.0;
            }
            ECTL.Properties.Settings.Default.GatingValue = this._cbGatingValue.SelectedIndex;
            ECTL.Properties.Settings.Default.Save();
        }

        private void OnLineConfigChanged(object sender, EventArgs e)
        {
            this._lineConfig = (eLineConfig) ((byte) this._cbLineConfig.SelectedIndex);
            ECTL.Properties.Settings.Default.LineConfig = this._cbLineConfig.SelectedIndex;
            ECTL.Properties.Settings.Default.Save();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.cboF1.Items.Clear();
            if (File.Exists("Timingpoint.txt"))
            {
                foreach (string str in File.ReadAllLines("Timingpoint.txt"))
                {
                    this.cboF1.Items.Add(str);
                }
            }
            else
            {
                this.bTimingPointMissing = true;
                MessageBox.Show("TimingPoint.txt is Missing !!!", "Error");
                Application.Exit();
                return;
            }
            if (ECTL.Properties.Settings.Default.filepath == "")
            {
                this.txtpath.Text = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            }
            else
            {
                this.txtpath.Text = ECTL.Properties.Settings.Default.filepath;
            }
            if (ECTL.Properties.Settings.Default.SearchMode == 1)
            {
                this.m_searchMode = SearchMode.DualTarget;
                this.radioBtnFinishLine.Checked = true;
            }
            else
            {
                this.m_searchMode = SearchMode.SingleTarget;
                this.radioBtnStartLine.Checked = true;
            }
            this._gatingValue = (eGatingValue) ((byte) ECTL.Properties.Settings.Default.GatingValue);
            this._lineConfig = (eLineConfig) ((byte) ECTL.Properties.Settings.Default.LineConfig);
            this._reader1Channel = (eReaderChannel) ((byte) ECTL.Properties.Settings.Default.Reader1Channel);
            this._reader2Channel = (eReaderChannel) ((byte) ECTL.Properties.Settings.Default.Reader2Channel);
            this._cbGatingValue.SelectedIndex = ECTL.Properties.Settings.Default.GatingValue;
            this._cbLineConfig.SelectedIndex = ECTL.Properties.Settings.Default.LineConfig;
            this._cbReader1Channel.SelectedIndex = ECTL.Properties.Settings.Default.Reader1Channel;
            this._cbReader2Channel.SelectedIndex = ECTL.Properties.Settings.Default.Reader2Channel;
            this._reader1Power.Value = (decimal) ECTL.Properties.Settings.Default.Reader1Power;
            this._reader2Power.Value = (decimal) ECTL.Properties.Settings.Default.Reader2Power;
            this._chxReader1Enabled.Checked = ECTL.Properties.Settings.Default.IsReader1;
            this._chxReader2Enabled.Checked = ECTL.Properties.Settings.Default.IsReader2;
            this._tbReader1.Text = ECTL.Properties.Settings.Default.Reader1;
            this._tbReader2.Text = ECTL.Properties.Settings.Default.Reader2;
            this._chxBeep.Checked = ECTL.Properties.Settings.Default.Beep;
            this._tcpipPort.Value = Convert.ToDecimal(ECTL.Properties.Settings.Default.TcpIpPort);
            this.dataGridViewBackup.ColumnCount = 2;
            this.dataGridViewBackup.Columns[0].Width = 200;
            this.dataGridViewBackup.Columns[0].HeaderText = "Race Number";
            this.dataGridViewBackup.Columns[1].Width = 270;
            this.dataGridViewBackup.Columns[1].HeaderText = "Time";
            this.btnConnect.ForeColor = Color.Green;
            this.btnDisconnect.ForeColor = Color.Red;
            string str2 = DateTime.Now.TimeOfDay.ToString();
            this.textBoxCurrentTime.Text = str2.Substring(0, str2.Length - 8);
            this.cboF1.SelectedIndex = 1;
        }

        private void OnPowerChanged(object sender, EventArgs e)
        {
            if (this._reader1Power == sender)
            {
                ECTL.Properties.Settings.Default.Reader1Power = (double) this._reader1Power.Value;
            }
            else if (this._reader2Power == sender)
            {
                ECTL.Properties.Settings.Default.Reader2Power = (double) this._reader2Power.Value;
            }
            ECTL.Properties.Settings.Default.Save();
        }

        private void OnReaderChannelChanged(object sender, EventArgs e)
        {
            if (this._cbReader1Channel == sender)
            {
                this._reader1Channel = (eReaderChannel) ((byte) this._cbReader1Channel.SelectedIndex);
                ECTL.Properties.Settings.Default.Reader1Channel = this._cbReader1Channel.SelectedIndex;
            }
            else if (this._cbReader2Channel == sender)
            {
                this._reader2Channel = (eReaderChannel) ((byte) this._cbReader2Channel.SelectedIndex);
                ECTL.Properties.Settings.Default.Reader2Channel = this._cbReader2Channel.SelectedIndex;
            }
            ECTL.Properties.Settings.Default.Save();
        }

        private void OnSetTimeClicked(object sender, EventArgs e)
        {
            SetTimeDlg dlg2 = new SetTimeDlg {
                StartPosition = FormStartPosition.CenterParent
            };
            using (SetTimeDlg dlg = dlg2)
            {
                dlg.ShowDialog(this);
            }
        }

        private void OnShutdownClicked(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show(this, "Shutdown System?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1))
            {
                Process.Start("shutdown", "/s /t 0 /f");
            }
        }

        private void OnTcpIpPortChanged(object sender, EventArgs e)
        {
            ECTL.Properties.Settings.Default.TcpIpPort = Convert.ToInt32(this._tcpipPort.Value);
            ECTL.Properties.Settings.Default.Save();
        }

        private void ProcessTagReads(ICollection<TagRead> tagReads)
        {
            string str = null;
            string str2 = null;
            string str3 = null;
            foreach (TagRead read in tagReads)
            {
                if(read.Reader == this._reader1)
                    if (read.Reader == this._reader1)
                    {
                        switch (read.Tag.AntennaPortNumber)
                        {

                            case 1:
                                var n1 = int.Parse(total1.Text);
                                total1.Text = (n1 + 1).ToString();
                                break;
                            case 2:
                                var n2 = int.Parse(total2.Text);
                                total2.Text = (n2 + 1).ToString();
                                break;
                            case 3:
                                var n3 = int.Parse(total3.Text);
                                total3.Text = (n3 + 1).ToString();
                                break;
                            case 4:
                                var n4 = int.Parse(total4.Text);
                                total4.Text = (n4 + 1).ToString();
                                break;
                        }
                    }
                    else
                    {
                        switch (read.Tag.AntennaPortNumber)
                        {

                            case 1:
                                var n1 = int.Parse(total5.Text);
                                total5.Text = (n1 + 1).ToString();
                                break;
                            case 2:
                                var n2 = int.Parse(total6.Text);
                                total6.Text = (n2 + 1).ToString();
                                break;
                            case 3:
                                var n3 = int.Parse(total7.Text);
                                total7.Text = (n3 + 1).ToString();
                                break;
                            case 4:
                                var n4 = int.Parse(total8.Text);
                                total8.Text = (n4 + 1).ToString();
                                break;
                        }
                    }

                this._nTotalReads++;
                lock (this._htTags)
                {
                    this._htTags.Add(read.EPC);
                }
                this.DisplayTagRead(read);
                DateTime firstSeenTime = read.Tag.FirstSeenTime;
                str = firstSeenTime.ToString("dd/MM/yyyy");
                str2 = firstSeenTime.AddHours(1).TimeOfDay.ToString();
                str2 = str2.Substring(0, str2.Length - 4);
                if (read.Reader == this._reader1)
                {
                    str3 = "Reader 1";
                }
                else
                {
                    str3 = "Reader 2";
                }
                lock (this)
                {
                    if (this._stream != null)
                    {
                        this._writer.WriteLine(string.Format("{0}, {1}, {2}, {3}, {4}, {5}", new object[] { read.EPC, str, str2, read.Tag.PeakRssiInDbm, read.Tag.AntennaPortNumber, str3 }));
                        // формировать лист
                        ListViewItem _rec = new ListViewItem(tagReads.Count.ToString());
                        _rec.SubItems.Add(read.EPC);
                        _rec.SubItems.Add(str2);
                        _rec.SubItems.Add(read.Tag.AntennaPortNumber.ToString());
                        string _ip = null;
                        if (str3 == "Reader 1") _ip = _tbReader1.Text.Substring(_tbReader1.Text.Length - 3);
                        if (str3 == "Reader 2") _ip = _tbReader2.Text.Substring(_tbReader2.Text.Length - 3);
                        _rec.SubItems.Add(_ip);
                        listView1.Items.Insert(0, _rec);
                        //конец
                        this._writer.Flush();
                    }
                    if (this._server != null)
                    {
                        this._server.OnTagRead(read);
                    }
                }
            }
        }

        private void radioBtnFinishLine_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioBtnFinishLine.Checked)
            {
                ECTL.Properties.Settings.Default.SearchMode = 1;
                ECTL.Properties.Settings.Default.Save();
            }
        }

        private void radioBtnStartLine_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioBtnStartLine.Checked)
            {
                ECTL.Properties.Settings.Default.SearchMode = 2;
                ECTL.Properties.Settings.Default.Save();
            }
        }

        public override void Refresh()
        {
            bool flag = (this._reader1 != null) || (null != this._reader2);
            if (this._reader1 == null)
            {
                //  this.lblReader1.ForeColor = Color.Black;
            }
            else
            {
                //this.lblReader1.ForeColor = Color.Green;
            }
            if (this._reader2 == null)
            {
                // this.lblReader2.ForeColor = Color.Black;
            }
            else
            {
                //this.lblReader2.ForeColor = Color.Green;
            }
            if (flag)
            {
                this.btnConnect.Enabled = false;
                this.btnStart.Enabled = true;
                this.btnConnect.Text = "Reader Connected";
                this.btnConnect.BackColor = Color.Green;
                this.btnConnect.ForeColor = Color.White;
                this.btnDisconnect.Enabled = true;
                this.btnDisconnect.BackColor = Color.Red;
                this.btnDisconnect.ForeColor = Color.White;
            }
            else
            {
                this.btnConnect.Enabled = true;
                this.btnConnect.Text = "Connect Reader";
                this.btnConnect.BackColor = Color.Transparent;
                this.btnConnect.ForeColor = Color.Green;
                this.btnDisconnect.Enabled = false;
                this.btnDisconnect.BackColor = Color.Transparent;
                this.btnDisconnect.ForeColor = Color.Black;
                this.btnStart.Enabled = false;
                this.btnStart.Text = "Start Reading Tags";
                this.btnStart.BackColor = Color.Transparent;
                this.btnStop.Enabled = false;
                this.btnStop.BackColor = Color.Transparent;
                //this.lblReader1.ForeColor = Color.Black;
                //this.lblReader2.ForeColor = Color.Black;
            }
            base.Refresh();
        }

        private void Reset()
        {
            this._htTags.Clear();
            this._nTotalReads = 0;
            this.textBoxTotalTagReads.Text = "0";
            this.textBoxUniqueTagReads.Text = "0";
            this.textBoxLastTagRead.Text = (string) (this.textBoxLastTimeRead.Text = null);
            this._tagReads.Clear();
        }

        private void Savebackup()
        {
            DateTime now = DateTime.Now;
            string str = now.ToString();
            this.dataGridViewBackup.Rows.Insert(0, new object[] { this.txtCheapId.Text, str });
            try
            {
                StreamWriter writer = new StreamWriter(this.path + @"\backup.txt", true);
                writer.WriteLine("{0}, {1}, {2}, {3}", new object[] { this.txtCheapId.Text, now.ToShortDateString(), now.ToString("HH:mm:ss"), 0 });
                writer.Close();
            }
            catch (Exception)
            {
            }
            this.txtCheapId.Text = "";
        }

        public void StartAllReaders()
        {
            lock (this._tagReads)
            {
                this._tagReads.Clear();
            }
            this._stopSignal.Reset();
            this._gatingSignal.Set();
            foreach (SpeedwayReader reader in new SpeedwayReader[] { this._reader1, this._reader2 })
            {
                if (reader != null)
                {
                    try
                    {
                        reader.Start();
                    }
                    catch (OctaneSdkException exception)
                    {
                        MessageBox.Show(exception.Message);
                    }
                    catch (Exception exception2)
                    {
                        MessageBox.Show(exception2.Message);
                    }
                }
            }
        }

        public void StopAllReaders()
        {
            this._gatingSignal.Reset();
            this._stopSignal.Set();
            lock (this._tagReads)
            {
                this._tagReads.Clear();
            }
            foreach (SpeedwayReader reader in new SpeedwayReader[] { this._reader1, this._reader2 })
            {
                if (reader != null)
                {
                    try
                    {
                        reader.Stop();
                    }
                    catch (OctaneSdkException exception)
                    {
                        Console.WriteLine("Reader {0} stop: OctaneSdk detected {1}", reader.ReaderIdentity, exception);
                    }
                    catch (Exception exception2)
                    {
                        Console.WriteLine("Reader {0} stop: Exception {1}", reader.ReaderIdentity, exception2);
                    }
                }
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (((TabControl) sender).SelectedTab.Text == "Backup")
            {
                this.txtCheapId.Focus();
            }
        }

        private void TagsReportedHandler(object sender, TagsReportedEventArgs args)
        {
            string key = null;
            TagRead read = null;
            SpeedwayReader reader = sender as SpeedwayReader;
            foreach (Tag tag in args.TagReport.Tags)
            {
                key = tag.Epc.TrimStart(new char[] { '0' });
                lock (this._tagReads)
                {
                    if (this._tagReads.TryGetValue(key, out read))
                    {
                        if ((((eLineConfig.Dual != this._lineConfig) || (this._reader1 == null)) || (this._reader1 == reader)) && (tag.PeakRssiInDbm > read.Tag.PeakRssiInDbm))
                        {
                            read.Reader = reader;
                            read.Tag = tag;
                        }
                    }
                    else
                    {
                        this._tagReads[key] = new TagRead(reader, key, tag);
                        lock (this)
                        {
                            if (this._bBeep)
                            {
                                if (((this._alertTimer != null) && !this._bAlerted) && (this._reader1 != null))
                                {
                                    this._bAlerted = true;
                                    this._reader1.SetGpo(4, true);
                                    this._alertTimer.Change(500, 0);
                                }
                                TimeSpan span = (TimeSpan) (DateTime.UtcNow - this._dtBeep);
                                if (span.TotalSeconds >= 1.0)
                                {
                                    this._dtBeep = DateTime.UtcNow;
                                    Interop.Beep(800, 200);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void textBoxReadTime_TextChanged(object sender, EventArgs e)
        {
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string str = DateTime.Now.TimeOfDay.ToString();
            this.textBoxCurrentTime.Text = str.Substring(0, str.Length - 8);
        }

        private void timerCheapBlocker_Tick(object sender, EventArgs e)
        {
            this.isDataNeedToStore = true;
        }

        public static T[] ToArray<T>(ICollection collection)
        {
            return ToArray<T>(collection, 0);
        }

        public static T[] ToArray<T>(ICollection collection, int nIndex)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            int count = 0;
            T[] array = null;
            lock (collection.SyncRoot)
            {
                count = collection.Count;
                array = new T[count];
                if (count > 0)
                {
                    collection.CopyTo(array, nIndex);
                }
            }
            return array;
        }

        private void txtCheapId_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                this.Savebackup();
            }
        }

        private void txtReader1_Leave(object sender, EventArgs e)
        {
            ECTL.Properties.Settings.Default.Reader1 = this._tbReader1.Text.ToString().Trim();
            ECTL.Properties.Settings.Default.Save();
        }

        private void txtReader2_Leave(object sender, EventArgs e)
        {
            ECTL.Properties.Settings.Default.Reader2 = this._tbReader2.Text.ToString().Trim();
            ECTL.Properties.Settings.Default.Save();
        }

        private delegate void delegateDisplayTagRead(TagRead tagRead);

        private static class Interop
        {
            [DllImport("kernel32.dll")]
            public static extern bool Beep(uint dwFreq, uint dwDuration);
            [DllImport("user32.dll", CharSet=CharSet.Auto)]
            public static extern bool MessageBeep(uint type);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SetTimeDlg dlg2 = new SetTimeDlg
            {
                StartPosition = FormStartPosition.CenterParent
            };
            using (SetTimeDlg dlg = dlg2)
            {
                dlg.ShowDialog(this);
            }
        }

        private void btnPurge_Click_1(object sender, EventArgs e)
        {
            DirectoryInfo info = new DirectoryInfo(this.txtpath.Text);
            FileInfo[] files = info.GetFiles("*.txt");
            if (files.Length > 0)
            {
                if (MessageBox.Show(string.Format("Do you want to delete {0} file(s) in {1}?", files.Length, info.FullName), "Delete files", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    foreach (FileInfo info2 in files)
                    {
                        try
                        {
                            info2.Delete();
                        }
                        catch (Exception exception)
                        {
                            MessageBox.Show(exception.Message, "Delete files", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("No files to delete.", "Delete files", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        private void _btnShutdown_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show(this, "Shutdown System?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1))
            {
                Process.Start("shutdown", "/s /t 0 /f");
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void btnConnect_MouseEnter(object sender, EventArgs e)
        {

        }

        bool isChecked = false;
        private void radioButton1_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked && !isChecked)
            {
                    btnStop_Click(sender, e);
                    btnDisconnect_Click(sender, e);
                    radioButton1.Text = "Connect and Start reading";
                    radioButton1.BackColor = Color.LightSalmon;
                    connectionStatus.Text = "Disconnected";
                    connectionStatusColor.BackColor = Color.Red;

                    radioButton1.Checked = false;
            }
            else
            {
                if (checkLogerValidation())
                {
                    OnConnectClicked(sender, e);
                    btnStart_Click(sender, e);
                    radioButton1.Text = "Stop Reading and Disconnect";
                    radioButton1.BackColor = Color.LightGreen;
                    connectionStatus.Text = "Connected";
                    connectionStatusColor.BackColor = Color.Lime;

                    radioButton1.Checked = true;
                    isChecked = false;
                }
                else radioButton1.Checked = false;
            }

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            isChecked = radioButton1.Checked;
        }
    }

    public class myRedButtonObject : UserControl
    {
        // Draw the new button. 
        protected override void OnPaint(PaintEventArgs e)
        {
            System.Drawing.SolidBrush myBrush1 = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
            System.Drawing.Graphics formGraphics1 = this.CreateGraphics();
            formGraphics1.FillEllipse(myBrush1, new Rectangle(0, 0, 40, 40));

            System.Drawing.SolidBrush myBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Red);
            System.Drawing.Graphics formGraphics = this.CreateGraphics();
            formGraphics.FillEllipse(myBrush, new Rectangle(2, 2, 35, 35));

            myBrush.Dispose();
            formGraphics.Dispose();
        }


    }

    public class myGreenButtonObject : UserControl
    {
        // Draw the new button. 
        protected override void OnPaint(PaintEventArgs e)
        {
            System.Drawing.SolidBrush myBrush1 = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
            System.Drawing.Graphics formGraphics1 = this.CreateGraphics();
            formGraphics1.FillEllipse(myBrush1, new Rectangle(0, 0, 40, 40));

            System.Drawing.SolidBrush myBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Lime);
            System.Drawing.Graphics formGraphics = this.CreateGraphics();
            formGraphics.FillEllipse(myBrush, new Rectangle(2, 2, 35, 35));

            myBrush.Dispose();
            formGraphics.Dispose();
        }
    }
}

