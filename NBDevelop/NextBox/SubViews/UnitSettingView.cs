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
    public partial class UnitSettingView : UserControl
    {
        public UnitSettingView()
        {
            InitializeComponent();
        }
        public static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public int index;
        public event EventFromSubViewDelegate eventFromUnitSetView;

        private TestCoreManager coreManager = TestCoreManager.Instance;
        private DevicePanel devicePanel = new DevicePanel();
        public void initView()
        {

            TestUnitSetting unitSetting = (TestUnitSetting)coreManager.unitSettingList[index];
            unitSetting.unitNonUITaskEvent += new UnitNonUITaskDelegate(executeNonUITask);
            

            
            JsonHelper.UnitSettings unitSettings = coreManager.json_helper.cfgContent.Units_Settings[index];
            devicePanel.devicesList = unitSettings.Devices;
            devicePanel.initDevicePanel();
            devicePanel.Location = new Point(50, 60);
            Controls.Add(devicePanel);

        }
        public void closeView()
        {
            devicePanel.closeAllDevices();
        }
        public void reloadView()
        {
            titleLabel.Text = string.Format("Unit-{0} Setting", index + 1);

        }
        private void UnitSettingView_Load(object sender, EventArgs e)
        {
            
        }

        private void backBtn_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> content = new Dictionary<string, string>();
            content.Add("name", "UnitSet");
            content.Add("type", "back");
            content.Add("data", index.ToString());
            eventFromUnitSetView(content);
        }
        //执行UnitNonUI的function测试请求
        private TMRecord executeNonUITask(TMTestItem item)
        {
            printLog(string.Format("execute NonUI Task function:{0}", item.function));
            TMRecord record = new TMRecord();
            record.measurement = "";
            if (item.function.Equals(Function.UnitTestFunc))
            {
                record.measurement = "testUnitValue";
            }
            else if (item.function.Equals(Function.UnitTestDeviceFunc))
            {
                string recvStr = "";
                if(devicePanel.QueryDevice("Instrument","set LED on",out recvStr, 5.0))
                {
                    record.measurement = recvStr;
                }
                else
                {
                    record.measurement = "";
                }
            }
            //-------------dummy testplan function
            else if (item.function.Equals(Function.DMM))
            {
                record.measurement = "0.06";
            }
            else if (item.function.Equals(Function.Arduino))
            {
                string recvStr = "";
                if (devicePanel.QueryDevice("Dut", item.param1, out recvStr, 5.0))
                {
                    recvStr = recvStr.Replace("\r", "");
                    recvStr = recvStr.Replace("\n", "");
                    record.measurement = recvStr;
                }
                else
                {
                    record.measurement = "";
                }
            }
            
            return record;
        }

        private void printLog(string txt)
        {
            log.Info(string.Format("[USV] - {0} ->log:{1}", index + 1, txt));
            //Console.WriteLine(string.Format("[USV] - {0} ->log:{1}", index + 1, txt));
        }
    }
}
