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

namespace NextBox
{
    

    public partial class UnitView : UserControl
    {
        public UnitView()
        {
            InitializeComponent();
        }
        public static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public int index;
        public event EventFromUnitViewDelegate eventFromUnitView;


        private TestCoreManager coreManager = TestCoreManager.Instance;
        private Color bgColor;
        private bool isEnabled;

        #region View Event
        private void UnitView_Load(object sender, EventArgs e)
        {
            coreManager.eventFromTestCoreManager += new EventFromTestCoreManagerDelegate(eventFromTestCoreManager);
            isEnabled = true;
            settingBtn.Visible = false;
            titleCheckBox.Text = "Unit-" + (index + 1).ToString();
            messageLabel.Text = "1.Test item 1\r\n2.Test item 2\r\n3.Test item 3";
            //bgColor = Color.LightSalmon;
            snLabel.Text = "";
            testStateLabel.Text = "IDLE";
            messageLabel.Text = "";
            changeBoxColor(Color.White);
        }
        //UnitView是否选中
        private void titleCheckBox_CheckStateChanged(object sender, EventArgs e)
        {
            isEnabled = titleCheckBox.Checked;
            if(isEnabled == true)
            {
                
                snLabel.Text = "";
                testStateLabel.Text = "IDLE";
                messageLabel.Text = "";
                testStateLabel.Enabled = true;
                changeBoxColor(Color.White);
            }
            else
            {
                snLabel.Text = "";
                testStateLabel.Text = "Disabled";
                messageLabel.Text = "";
                testStateLabel.Enabled = false;
                changeBoxColor(Color.Gray);
            }
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("type", "selectClick");
            dict.Add("status", isEnabled.ToString());
            eventFromUnitView(index, dict);
        }
        //Unit 测试状态点击事件
        private void testStateLabel_Click(object sender, EventArgs e)
        {
            printLog("test state label click!");
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("type", "stateClick");
            eventFromUnitView(index, dict);
        }
        //Unit Setting按钮点击事件
        private void settingBtn_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("type", "setClick");
            eventFromUnitView(index, dict);
        }

        //鼠标悬停status label
        private void testStateLabel_MouseEnter(object sender, EventArgs e)
        {
            BackColor = Color.BurlyWood;
        }

        private void testStateLabel_MouseLeave(object sender, EventArgs e)
        {
            BackColor = bgColor;
        }
        #endregion

        #region I/O Input Event
        /**************************
         * OUTPUT
         * 1.Enabled/Disabled Click -> Dashboard -> Test Core Manager
         * 2.Detail Btn Click -> Dashboard -> Container View -> Show DetailView
         * 3.Setting Btn Click -> Dashboard -> Container View -> Show Unit Setting View
         * 
         * ************************
         * INPUT
         * 1.Test Mode Changed <- Dashboard (Selected Test Mode)
         * 2.Testing/Finished <- Test Core Manager
         * 
         * 
         */
        public void inputEvent(Dictionary<string,object> content)
        {
            string type = (string)content["type"];
            if (type.Equals("modeChanged"))
            {
                int modeIndex = (int)content["index"];
                printLog(string.Format("mode change to:{0}", modeIndex));
                if (modeIndex == 2 ) //Enginner mode
                {
                    settingBtn.Visible = true;
                }
                else
                {
                    settingBtn.Visible = false;
                }
            }
        }

        private void eventFromTestCoreManager(Dictionary<string, object> content)
        {
            string type = (string)content["type"];
            printLog("event from test core manager:" + type);
            if (type.Equals("start"))
            {
                Invoke((EventHandler)(delegate {
                    if (isEnabled == true)
                    {
                        snLabel.Text = coreManager.snList[index];
                        testStateLabel.Text = "Test...";
                        messageLabel.Text = ""; 
                        changeBoxColor(Color.Yellow);
                    }
                    titleCheckBox.Enabled = false;
                }));
            }
            else if (type.Equals("unitFinished"))
            {
                int unitIndex = (int)content["index"];
                if(unitIndex == index && isEnabled == true)
                {
                    TestStatus result = (TestStatus)content["result"];
                    //跨线程访问UI控件
                    Invoke((EventHandler)(delegate{
                        testStateLabel.Text = result.ToString();
                        if (result == TestStatus.Pass)
                        {
                            changeBoxColor(Color.LawnGreen);
                        }
                        else
                        {
                            changeBoxColor(Color.Red);
                            List<TMRecord> failList = coreManager.sequencersList[index].failList;
                            int count = 0;
                            string top3item = "";
                            foreach (TMRecord record in failList)
                            {
                                if (count > 2) break;
                                top3item += string.Format("{0}.{1} - [{2}]\r\n", count + 1, record.item.tid, record.measurement);
                                count += 1;
                            }
                            messageLabel.Text = top3item;
                        }
                    }));
                }
            }
            else if (type.Equals("allFinished"))
            {
                Invoke((EventHandler)(delegate {
                    titleCheckBox.Enabled = true;
                }));
            }

        }
        #endregion

        private void changeBoxColor(Color color)
        {
            bgColor = color;
            BackColor = color;
        }
        private void printLog(string txt)
        {
            log.Info(string.Format("[UV] - {0} ->log:{1}", index + 1, txt));
            //Console.WriteLine(string.Format("[UV] - {0} ->log:{1}", index + 1, txt));
        }
    }
}
