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
using NextBox.TestModel.SubClass;
using NextBox.SubViews.DevicePanel;

namespace NextBox
{
    public partial class StationSettingView : UserControl
    {
        public StationSettingView()
        {
            InitializeComponent();
        }
        public static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public event EventFromSubViewDelegate eventFromStationSetView;
        private TestCoreManager coreManager = TestCoreManager.Instance;
        private DevicePanel devicePanel = new DevicePanel();

        public void initView()
        {
            coreManager.stationSetting.stationNonUITaskEvent += new StationNonUITaskDelegate(stationNonUITaskEvent);

            
            devicePanel.devicesList = coreManager.json_helper.cfgContent.Station_Settings.Devices;
            devicePanel.initDevicePanel();
            devicePanel.Location = new Point(50, 60);
            Controls.Add(devicePanel);

            coreManager.stationConfigDict.Add("loopEnable", false);
            coreManager.stationConfigDict.Add("loopCount", 1);
            coreManager.stationConfigDict.Add("loopFinished", 1);

        }

        public void closeView()
        {
            devicePanel.closeAllDevices();
        }

        private void StationSettingView_Load(object sender, EventArgs e)
        {
            
        }

        private void backBtn_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> content = new Dictionary<string, string>();
            content.Add("name", "StationSet");
            content.Add("type", "back");
            eventFromStationSetView(content);
        }
        //执行StationNonUI的function测试请求
        private TMRecord stationNonUITaskEvent(TMRequest request)
        {
            TMTestItem item = request.testItem;
            printLog("station NonUI task func:" + item.function);
            TMRecord record = new TMRecord();
            record.result = TestStatus.NotSet;

            if (item.function.Equals(Function.StationNonUIFuncAsync))
            {
                record.measurement = "stationAsyncValue";
            }
            else if (item.function.Equals(Function.StationNonUIFuncSync))
            {
                
                string receivedStr = "";
                //bool status = devicePanel.QueryDevice("Fixture", "set LED on", out receivedStr, 5.0);
                //bool status = devicePanel.QueryDevice("Instrument", "set LED on", out receivedStr, 5.0);
                bool status = devicePanel.QueryDevice("Fixture2", "test SOCKET client", out receivedStr, 5.0);
                if (status)
                {
                    record.measurement = receivedStr;
                }
                else
                {
                    record.measurement = "";
                    record.failureInfo = "query device failure";
                }
                
                //record.measurement = "stationSyncValue";
            }
            else if (item.function.Equals(Function.Fixture))
            {
                record.measurement = "OK";
            }
            else
            {
                record.failureInfo = "Unknown Func";
            }

            return record;

        }

        private void printLog(string txt)
        {
            log.Info(string.Format("[SSV] ->log:{0}", txt));
            //Console.WriteLine(string.Format("[SSV] ->log:{0}", txt));
        }

        private void loopCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (loopCheckBox.CheckState == CheckState.Checked)
            {
                loopCountTB.Enabled = false;
                coreManager.stationConfigDict["loopEnable"] = true;
                coreManager.stationConfigDict["loopCount"] = int.Parse(loopCountTB.Text);
                coreManager.stationConfigDict["loopFinished"] = 0;
            }
            else
            {
                loopCountTB.Enabled = true;
                coreManager.stationConfigDict["loopEnable"] = false;
                coreManager.stationConfigDict["loopCount"] = 1;
                coreManager.stationConfigDict["loopFinished"] = 0;
            }
        }
    }
}
