using NextBox.TestModel;
using NextBox.TestModel.SubClass;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NextBox.SubForms
{
    public delegate void EventFromScanSNFormDelegate(Dictionary<string, object> content);
    public partial class ScanSNForm : Form
    {
        public ScanSNForm()
        {
            InitializeComponent();
        }
        public string firstSN = "";
        public event EventFromScanSNFormDelegate eventFromScanSNForm;

        private int nextSelectedIndex = 0;
        private TestCoreManager coreManager = TestCoreManager.Instance;
        private TMSnIsOK snIsOK = new TMSnIsOK();
        private List<string> snList = new List<string>();
        private bool hasSendFinishedEvent = false;
        private void ScanSNForm_Load(object sender, EventArgs e)
        {
            scanTextBox.Text = "";
            scanTextBox.Focus();
            listBox1.Items.Clear();
        }
        private void ScanSNForm_Shown(object sender, EventArgs e)
        {
            nextSelectedIndex = saveSnAndGetNextIndex(nextSelectedIndex, firstSN);
            if (nextSelectedIndex == -1)
            {
                finishedScanSN();
            }
        }
        public void initScanSNForm()
        {
            snList = new List<string>();
            snIsOK.init();
            scanTextBox.Text = "";
            scanTextBox.Focus();
            listBox1.Items.Clear();
            nextSelectedIndex = 0;
            hasSendFinishedEvent = false;
        }

        private void scanTextBox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string sn = scanTextBox.Text;
                if (sn.Length == 0)
                {
                    return;
                }
                nextSelectedIndex = saveSnAndGetNextIndex(nextSelectedIndex, sn);
                Console.WriteLine("sn:" + sn);
                scanTextBox.Text = "";
                if (nextSelectedIndex == -1)
                {
                    finishedScanSN();
                }
            }
        }
        private int saveSnAndGetNextIndex(int currentIndex, string sn)
        {
            int nextIndex = currentIndex + 1;
            for (int i = currentIndex; i < coreManager.unitSelectedArr.Count; i++)
            {
                if (coreManager.unitSelectedArr[i])
                {

                    if (!snIsOK.checkSN(sn))
                    {
                        MessageBox.Show(string.Format("SN:[{0}] check failure!", sn), "Error");
                        return nextIndex - 1;
                    }
                    snList.Add(sn);
                    listBox1.Items.Add(string.Format("{0}.{1}", (i + 1).ToString(), sn));
                    break;
                }
                else
                {
                    snList.Add("");
                    listBox1.Items.Add(string.Format("{0}.{1}", (i + 1).ToString(), "SKIP"));
                    nextIndex += 1;
                }
            }
            if (nextIndex == coreManager.unitSelectedArr.Count) return -1;

            for (int i = nextIndex; i < coreManager.unitSelectedArr.Count; i++)
            {
                if (coreManager.unitSelectedArr[i])
                {
                    break;
                }
                else
                {
                    snList.Add("");
                    listBox1.Items.Add(string.Format("{0}.{1}", (i + 1).ToString(), "SKIP"));
                    nextIndex += 1;
                }
            }

            if (nextIndex == coreManager.unitSelectedArr.Count) return -1;

            return nextIndex;
        }

        private void finishedScanSN()
        {
            Console.WriteLine("finished scan");
            foreach (string item in snList)
            {
                Console.WriteLine("sn:" + item);
            }
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("type", "OK");
            dict.Add("data", snList);
            eventFromScanSNForm(dict);
            hasSendFinishedEvent = true;
            Close();
        }

        private void ScanSNForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (hasSendFinishedEvent)
            {
                return;
            }
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("type", "NG");
            dict.Add("data", snList);
            eventFromScanSNForm(dict);
        }
    }
}
