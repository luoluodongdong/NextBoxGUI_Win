using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NextBox.TestModel;
using System.Collections;
using NextBox.SubForms;
using NextBox.TestModel.SubClass;
using System.Threading;

namespace NextBox
{
    public partial class Dashboard : UserControl
    {
        public Dashboard()
        {
            InitializeComponent();
        }
        public event EventFromSubViewDelegate eventFromDashboard;
        //public ArrayList unitSetViewArr = new ArrayList();
        public static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private PasswordForm passwordForm = new PasswordForm();
        private ScanSNForm scanSNForm = new ScanSNForm();
        private TMSnIsOK snIsOK = new TMSnIsOK();
        int currentTestModeIndex = 0;
        private TestCoreManager coreManager = TestCoreManager.Instance;
        private ArrayList unitViewArr = new ArrayList();
        private void Dashboard_Load(object sender, EventArgs e)
        {
            loopLabel.Visible = false;
            string softwareName = coreManager.json_helper.cfgContent.Station_Settings.SoftwareName;
            string softwareVersion = coreManager.json_helper.cfgContent.Station_Settings.SoftwareVersion;
            softwareNameLabel.Text = softwareName;
            versionLabel.Text = softwareVersion;
            settingBtn.Visible = false;
            testModeComboBox.SelectedIndex = 0;
            flowLayoutPanel1.WrapContents = true;
            for(int i = 0; i < coreManager.unitCount; i++){
                UnitView unitView = (UnitView)unitViewArr[i];
                flowLayoutPanel1.Controls.Add(unitView);
            }


        }

        #region Init Dashboard
        public void refreshDashboard()
        {
            if ((bool)coreManager.stationConfigDict["loopEnable"])
            {
                int loopCount = (int)coreManager.stationConfigDict["loopCount"];
                int loopFinished = (int)coreManager.stationConfigDict["loopFinished"];
                loopLabel.Text = string.Format("{0}/{1}", loopCount, loopFinished);
                loopLabel.Visible = true;

            }
            else
            {
                loopLabel.Visible = false;
            }
        }
        public void initDashboard()
        {
            passwordForm.rawPassword = "LuxshareTE";
            //注册password view事件
            passwordForm.eventFromPasswordForm += new EventFromPasswordFormDelegate(EventFromPasswordForm);
            //注册core manager事件
            coreManager.eventFromTestCoreManager += new EventFromTestCoreManagerDelegate(eventFromTestCoreManager);
            //初始化Unit views
            for (int i = 0; i < coreManager.unitCount; i++)
            {
                UnitView unitView = new UnitView();
                unitView.index = i;
                //注册unit view事件
                unitView.eventFromUnitView += new EventFromUnitViewDelegate(EventFromUnitView);
                unitViewArr.Add(unitView);
                

            }

            scanSNForm.eventFromScanSNForm += new EventFromScanSNFormDelegate(EventFromScanSNForm);
        }
        #endregion

        #region Event From ScanSNForm
        private void EventFromScanSNForm(Dictionary<string,object> content)
        {
            string type =(string)content["type"];
            printLog("event from scan sn:" + type);
            if (type.Equals("OK"))
            {
                GC.Collect();
                coreManager.snList = (List<string>)content["data"];
                if ((bool)coreManager.stationConfigDict["loopEnable"])
                {
                    coreManager.stationConfigDict["loopFinished"] = 0;
                    refreshDashboard();
                }
                coreManager.startTest();
            }
        }
        #endregion

        #region Event From TestCoreManager
        private void eventFromTestCoreManager(Dictionary<string, object> content)
        {
            string type = (string)content["type"];
            printLog("event from test core manager:" + type);
            if (type.Equals("start"))
            {
                Invoke((EventHandler)(delegate {
                    testModeComboBox.Enabled = false;
                    scanSNTextBox.Enabled = false;
                    startBtn.Enabled = false;
                }));
            }
            else if (type.Equals("allFinished"))
            {
                

                Thread th = new Thread(new ThreadStart(GoLoopTest));
                th.IsBackground = true;
                th.Start();
            }

        }
        #endregion

        #region Loop Function
        private void GoLoopTest()
        {
            coreManager.stationConfigDict["loopFinished"] = (int)coreManager.stationConfigDict["loopFinished"] + 1;
            Invoke((EventHandler)(delegate {
                refreshDashboard();
            }));
            
            if ((int)coreManager.stationConfigDict["loopFinished"] < (int)coreManager.stationConfigDict["loopCount"])
            {
                GC.Collect();
                Thread.Sleep(2000);
                coreManager.startTest();
            }
            else
            {
                Invoke((EventHandler)(delegate {
                    testModeComboBox.Enabled = true;
                    scanSNTextBox.Enabled = true;
                    startBtn.Enabled = true;
                    scanSNTextBox.Focus();
                }));
            }

        }
        #endregion

        #region Password Event Delegate
        private void EventFromPasswordForm(string result)
        {
            printLog("password verify result:" + result);
            if (result.Equals("OK"))
            {
                currentTestModeIndex = testModeComboBox.SelectedIndex;
                for(int i = 0; i < coreManager.unitCount; i++)
                {
                    UnitView unitView = (UnitView)unitViewArr[i];
                    Dictionary<string, object> dict = new Dictionary<string, object>();
                    dict.Add("type", "modeChanged");
                    dict.Add("index", currentTestModeIndex);
                    unitView.inputEvent(dict);
                }
                if(currentTestModeIndex == 2)
                {
                    settingBtn.Visible = true;
                }
                else
                {
                    settingBtn.Visible = false;
                }
                Dictionary<string, string> dict2 = new Dictionary<string, string>();
                dict2.Add("name", "Dashboard");
                dict2.Add("type", "ModeChanged");
                dict2.Add("index", currentTestModeIndex.ToString());

                eventFromDashboard(dict2);
            }
            else
            {
                testModeComboBox.SelectedIndex = 0;
                currentTestModeIndex = 0;
            }
        }
        #endregion

        #region UnitView Event Delegate (Output)
        private void EventFromUnitView(int index, Dictionary<string,string> content)
        {
            string type = content["type"];
            printLog(string.Format("event:{0} from unit view[{1}]", index, type));
            if (type.Equals("stateClick"))
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                dict.Add("name", "Dashboard");
                dict.Add("type", "UnitClick");
                dict.Add("data", index.ToString());

                eventFromDashboard(dict);
            }
            else if (type.Equals("setClick"))
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                dict.Add("name", "Dashboard");
                dict.Add("type", "UnitSetClick");
                dict.Add("data", index.ToString());

                eventFromDashboard(dict);
            }else if (type.Equals("selectClick"))
            {
                bool status = bool.Parse(content["status"]);
                printLog(string.Format("unit:{0} selected status:{1}", index, content["status"]));

                Dictionary<string, object> dict = new Dictionary<string, object>();
                dict.Add("type","unitSelected");
                dict.Add("index", index);
                dict.Add("status", status);
                coreManager.outputEvent(dict);
                if (coreManager.enabledUnitsCount == 0)
                {
                    scanSNTextBox.Enabled = false;
                    startBtn.Enabled = false;
                }
                else
                {
                    scanSNTextBox.Enabled = true;
                    startBtn.Enabled = true;
                }
            }
            
        }
        #endregion

        #region Control Action
        private void settingBtn_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("name", "Dashboard");
            dict.Add("type", "SetBtnClick");
            eventFromDashboard(dict);
        }

        private void testModeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(testModeComboBox.SelectedIndex == 0)
            {
                settingBtn.Visible = false;
                currentTestModeIndex = testModeComboBox.SelectedIndex;
                for (int i = 0; i < coreManager.unitCount; i++)
                {
                    UnitView unitView = (UnitView)unitViewArr[i];
                    Dictionary<string, object> dict = new Dictionary<string, object>();
                    dict.Add("type", "modeChanged");
                    dict.Add("index", currentTestModeIndex);
                    unitView.inputEvent(dict);
                }
                Dictionary<string, string> dict2 = new Dictionary<string, string>();
                dict2.Add("name", "Dashboard");
                dict2.Add("type", "ModeChanged");
                dict2.Add("index", currentTestModeIndex.ToString());

                eventFromDashboard(dict2);
                return;
            }
            if (testModeComboBox.SelectedIndex !=currentTestModeIndex)
            {
                passwordForm.initView();
                passwordForm.ShowDialog();
            }
        }

        private void startBtn_Click(object sender, EventArgs e)
        {
            //coreManager.startTest();
        }
        private void scanSNTextBox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string sn = scanSNTextBox.Text;
                if (sn.Length == 0)
                {
                    return;
                }
                scanSNTextBox.Text = "";
                scanSNForm.firstSN = sn;
                scanSNForm.initScanSNForm();
                scanSNForm.ShowDialog();
            }
        }
        #endregion
        private void printLog(string txt)
        {
            log.Info(string.Format("[Dashboard] ->log:{0}", txt));
            //Console.WriteLine(string.Format("[Dashboard] ->log:{0}",txt));
        }
    }
}
