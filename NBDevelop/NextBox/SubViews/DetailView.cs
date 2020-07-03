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
using NextBox.TestModel.TestDataModel;
using System.Runtime.InteropServices;

namespace NextBox
{

    public partial class DetailView : UserControl
    {
        public DetailView()
        {
            InitializeComponent();
        }
        public static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public int index;
        public event EventFromSubViewDelegate eventFromDetailView;
        private TestCoreManager coreManager = TestCoreManager.Instance;
        private BindingList<TestDataRow> bindingList = new BindingList<TestDataRow>();
        private readonly object _insertLock = new object();
        public void initView()
        {
            titleLabel.Text = "Unit-" + (index + 1).ToString();
            timerLabel.Text = "";
            printLog("Test core manager version:" + coreManager.coreVersion);
            dataGridView1.DataSource = bindingList;
            coreManager.eventFromTestCoreManager += new EventFromTestCoreManagerDelegate(eventFromTestCoreManager);

        }
        //向数据源中增加一行数据
        public void insertDataRow(TestDataRow row)
        {
            lock (_insertLock)
            {
                if (dataGridView1.InvokeRequired)
                {
                    Action<TestDataRow> action = x =>
                    {
                        bindingList.Add(x);
                    };
                    dataGridView1.Invoke(action, row);
                }
                else
                {
                    bindingList.Add(row);
                }
                
            }
            
        }
        public void refreshUI()
        {
            printLog("refreshUI funcion");
            timer1.Enabled = true;
            timer1.Start();
            updateTimeLabel();

            titleLabel.Text = "Unit-" + (index + 1).ToString();
        }

        private void DetailView_Load(object sender, EventArgs e)
        {
            
        }

        private void eventFromTestCoreManager(Dictionary<string, object> content)
        {
            string type = (string)content["type"];
            printLog("event from test core manager:" + type);
            if (type.Equals("start"))
            {
                if (dataGridView1.InvokeRequired)
                {
                    Invoke((EventHandler)(delegate
                    { 
                        bindingList.Clear();
                    }));
                }
                else
                {
                    bindingList.Clear();
                }

            }
        }

        private void backBtn_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            timer1.Enabled = false;
            Dictionary<string, string> content = new Dictionary<string, string>();
            content.Add("name", "Detail");
            content.Add("type", "back");
            eventFromDetailView(content);     
        }

        private void printLog(string txt)
        {
            log.Info(string.Format("[DV] - {0} ->log:{1}", index + 1, txt));
            //Console.WriteLine(string.Format("[DV] - {0} ->log:{1}", index + 1, txt));
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            updateTimeLabel();
        }
        //刷新计时器
        private void updateTimeLabel()
        {
            TestSequencer sequencer = coreManager.sequencersList[index];
            if (sequencer.testStatus == TestStatus.Testing)
            {
                TimeSpan ts = DateTime.Now.Subtract(sequencer.startTime);
                double secInterval = Math.Round(ts.TotalSeconds,2);
                timerLabel.Text = secInterval.ToString() + "s";
            }
            else if(sequencer.testStatus == TestStatus.Error || sequencer.testStatus == TestStatus.Fail || sequencer.testStatus == TestStatus.Pass)
            {
                TimeSpan ts = sequencer.endTime.Subtract(sequencer.startTime);
                double secInterval = Math.Round(ts.TotalSeconds,2);
                timerLabel.Text = secInterval.ToString() + "s";
                timer1.Stop();
            }
            lock (dataGridView1)
            {
                dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.Rows.Count - 1;
            }
        }

    }
}
