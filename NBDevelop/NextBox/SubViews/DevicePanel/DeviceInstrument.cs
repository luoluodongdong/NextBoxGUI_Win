using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NONATEST;
using NextBox.TestModel;
using System.Threading;
using NextBox.SubForms;

namespace NextBox.SubViews.DevicePanel
{
    public partial class DeviceInstrument : UserControl
    {
        public DeviceInstrument()
        {
            InitializeComponent();
        }
        public JsonHelper.Device theDevice;

        private TestCoreManager coreManager = TestCoreManager.Instance;
        private Interface_3497xx _instrument = new _34970();
        private bool instrIsOpened = false;
        private string instrAddress = "";

        private void DeviceInstrument_Load(object sender, EventArgs e)
        {
            groupBox1.Text = theDevice.Description;
            refreshDeviceUI();
        }
        public void initDevice()
        {
            PortComboBox.Items.Clear();
            List<string> ports = _instrument.ScanDevices();
            foreach (string item in ports)
            {
                PortComboBox.Items.Add(item);
            }
        }
        public void refreshDeviceUI()
        {
            if (instrIsOpened)
            {
                PortComboBox.SelectedItem = instrAddress;
                PortComboBox.Enabled = false;
                OpenBtn.Text = "Close";
                ScanBtn.Enabled = false;
            }
            else
            {
                Console.WriteLine("instrument is not opened");
            }
        }
        public void closeDevice()
        {
            if (instrIsOpened)
            {
                _instrument.Close();
                Console.WriteLine("close device");
            }

        }
        #region Auto Open Port Function
        public bool AutoOpenInstrument()
        {
            int count = PortComboBox.Items.Count;
            if (count == 0)
            {
                MessageBox.Show("Not any port exist!", "Error");
                return false;
            }
            bool find_flag = false;
            for (int i = 0; i < count; i++)
            {
                if (theDevice.Port == PortComboBox.Items[i].ToString())
                {
                    PortComboBox.SelectedIndex = i;
                    find_flag = true;
                    break;
                }
            }
            if (find_flag)
            {
                instrIsOpened = false;
                try
                {
                    int BaudRate = int.Parse(theDevice.BaudRate);
                    instrAddress = PortComboBox.SelectedItem.ToString();
                    bool status = _instrument.Open(instrAddress, 5000, BaudRate);
                    if (status)
                    {
                        instrIsOpened = true;
                        Console.WriteLine("open instrument successful");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error:" + ex.ToString(), "Error");
                }
            }
            else
            {
                MessageBox.Show("Port not exist!", "Error");
            }
            if (!instrIsOpened)
            {
                return false;
            }
            else
            {
                Thread.Sleep(1000);
            }
            return true;
        }
        #endregion

        public bool SendCmd(string cmd)
        {
            Console.WriteLine(string.Format("[TX]:" + cmd));
            if (instrIsOpened == false)
            {
                Console.WriteLine("Error:instrument not opened!");
                return false;
            }
            bool status = false;
            try
            {
                cmd += Environment.NewLine;
                status = _instrument.WriteLine(cmd);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Write Error:" + ex.ToString(), "Error");
                return false;
            }

            return status;
        }
        public bool SendAndReceived(string cmd, double timeout, out string receiveStr)
        {
            receiveStr = "";
            if (instrIsOpened == false)
            {
                Console.WriteLine("Error:instrument port not opened!");
                return false;
            }
            Console.WriteLine(string.Format("[TX]:" + cmd));
            cmd += Environment.NewLine;
            if (_instrument.WriteLine(cmd))
            {
                Thread.Sleep(100);
                receiveStr = _instrument.ReadLine();
                Console.WriteLine(string.Format("[RX]:" + receiveStr));
            }
            else
            {
                return false;
            }

            return true;
        }

        private void ScanBtn_Click(object sender, EventArgs e)
        {
            PortComboBox.Items.Clear();
            List<string> ports = _instrument.ScanDevices();
            foreach (string item in ports)
            {
                PortComboBox.Items.Add(item);
            }
        }

        private void OpenBtn_Click(object sender, EventArgs e)
        {
            if (PortComboBox.SelectedItem == null) return;
            if (OpenBtn.Text.Equals("Open"))
            {
                instrAddress = PortComboBox.SelectedItem.ToString();
                int baudrate = int.Parse(theDevice.BaudRate);
                bool error_flag = false;
                instrIsOpened = false;
                try
                {
                    bool status = _instrument.Open(instrAddress,5000,baudrate);
                    if (status)
                    {
                        OpenBtn.Text = "Close";
                        ScanBtn.Enabled = false;
                        PortComboBox.Enabled = false;
                        instrIsOpened = true;
                    }
                    else
                    {
                        error_flag = true;
                    }
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show("error:" + ex.ToString(), "Error");
                    error_flag = true;
                }

                if (!error_flag)
                {
                    theDevice.Port = instrAddress;
                    coreManager.json_helper.saveCfg();
                    MessageBox.Show(string.Format("Open instrument [{0}] successful!", instrAddress), "Info");

                }

            }
            else
            {
                try
                {
                    _instrument.Close();
                    instrIsOpened = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("error:" + ex.ToString(), "Error");
                }
                finally
                {
                    OpenBtn.Text = "Open";
                    ScanBtn.Enabled = true;
                    PortComboBox.Enabled = true;
                }
            }
        }
    }
}
