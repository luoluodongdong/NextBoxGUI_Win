using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using NextBox;
using NextBox.TestModel;
using System.Threading;

namespace NextBox.SubViews.DevicePanel
{
    public partial class DeviceSerial : UserControl
    {
        public DeviceSerial()
        {
            InitializeComponent();
        }
        
        public JsonHelper.Device theDevice;
        public event ReceivedDataFromDevice receivedDataFromSerial;
        private TestCoreManager coreManager = TestCoreManager.Instance;
        private string recvStr = "";
        private readonly object _lockRecvData = new object();

        private bool finishedRecvLine = false;
        private string recvBuff = "";

        private void DeviceSerial_Load(object sender, EventArgs e)
        {
            groupBox1.Text = theDevice.Description;
            refreshDeviceUI();
            
        }
        public void initDevice()
        {
            portComboBox.Items.Clear();
            portComboBox.Items.AddRange(SerialPort.GetPortNames());

            if (portComboBox.Items.Count != 0)
            {
                portComboBox.SelectedIndex = 0;
            }
        }

        public void refreshDeviceUI()
        {
            if (serialPort1.IsOpen)
            {
                portComboBox.SelectedItem = serialPort1.PortName;
                portComboBox.Enabled = false;
                OpenBtn.Text = "Close";
                ScanBtn.Enabled = false;
            }
            else
            {
                Console.WriteLine("serial port not opened!");
            }
        }
        public void closeDevice()
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
                Console.WriteLine("close device");
            }
            
        }
        #region Auto Open Port Function
        public bool AutoOpenSerialPort()
        {
            int count = portComboBox.Items.Count;
            if (count == 0)
            {
                MessageBox.Show("Not any port exist!", "Error");
                return false;
            }
            bool find_flag = false;
            for (int i = 0; i < count; i++)
            {
                if (theDevice.Port == portComboBox.Items[i].ToString())
                {
                    portComboBox.SelectedIndex = i;
                    find_flag = true;
                    break;
                }
            }
            if (find_flag)
            {

                try
                {
                    serialPort1.BaudRate = int.Parse(theDevice.BaudRate);
                    serialPort1.PortName = theDevice.Port;
                    serialPort1.Open();
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
            if (!serialPort1.IsOpen)
            {
                return false;
            }
            else
            {
                Thread.Sleep(1000);
                serialPort1.ReadExisting();
            }
            return true;
        }
        #endregion

        public bool SendCmd(string cmd)
        {
            Console.WriteLine(string.Format("[TX]:" + cmd));
            if (serialPort1.IsOpen == false)
            {
                Console.WriteLine("Error:serial port not opened!");
                return false;
            }
            try
            {
                cmd += Environment.NewLine;
                serialPort1.WriteLine(cmd);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Write Error:" + ex.ToString(),"Error");
                return false;
            }

            return true;
        }
        public bool SendAndReceived(string cmd, double timeout, out string receiveStr)
        {
            receiveStr = "";
            if (serialPort1.IsOpen == false)
            {
                Console.WriteLine("Error:serial port not opened!");
                return false;
            }
            bool isTimeOut = true;
            Console.WriteLine(string.Format("[TX]:" + cmd));
            try
            {
                serialPort1.ReadExisting();
                cmd += Environment.NewLine;
                serialPort1.WriteLine(cmd);

            }
            catch (Exception ex)
            {
                //showMessageBox("Write Error:" + ex.ToString());
                return false;
            }

            DateTime startT = DateTime.Now;
            double duration = 0.00;
            //string receiveStr = "";
            recvBuff = "";
            finishedRecvLine = false;
            while (duration <= timeout)
            {
                
                //receiveStr += serialPort1.ReadExisting();
                //if (recvStr.EndsWith(Environment.NewLine)) //\r\n
                if(finishedRecvLine == true)
                {
                    receiveStr = recvBuff;
                    isTimeOut = false;
                    break;
                }

                TimeSpan span = DateTime.Now - startT;
                duration = span.TotalSeconds;
                Thread.Sleep(20);
            }
            Console.WriteLine(string.Format("[RX]:" + receiveStr));
            recvBuff = "";
            return !isTimeOut;
        }

        private void portComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void ScanBtn_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
                OpenBtn.Text = "Open";
            }
            portComboBox.Items.Clear();
            portComboBox.Items.AddRange(SerialPort.GetPortNames());
            if (portComboBox.Items.Count != 0)
            {
                portComboBox.SelectedIndex = 0;
            }
            
        }

        private void OpenBtn_Click(object sender, EventArgs e)
        {
            if (portComboBox.SelectedItem == null) return;
            if (OpenBtn.Text.Equals("Open"))
            {
                serialPort1.BaudRate = int.Parse(theDevice.BaudRate);
                serialPort1.PortName = portComboBox.SelectedItem.ToString();
                bool error_flag = false;
                try
                {
                    serialPort1.Open();
                    OpenBtn.Text = "Close";
                    ScanBtn.Enabled = false;
                    portComboBox.Enabled = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("error:" + ex.ToString(), "Error");
                    error_flag = true;
                }

                if (!error_flag)
                {
                    theDevice.Port = serialPort1.PortName;
                    coreManager.json_helper.saveCfg();
                    MessageBox.Show(string.Format("Open serial [{0}] successful!",serialPort1.PortName) , "Info");
                    
                }

            }
            else
            {
                try
                {
                    serialPort1.Close();

                }
                catch (Exception ex)
                {
                    MessageBox.Show("error:" + ex.ToString(), "Error");
                }
                finally
                {
                    OpenBtn.Text = "Open";
                    ScanBtn.Enabled = true;
                    portComboBox.Enabled = true;
                }
            }
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            lock (_lockRecvData)
            {
                SerialPort sp = (SerialPort)sender;
                string recv = sp.ReadExisting();
                recvStr += recv;
                if (recvStr.EndsWith(Environment.NewLine))
                {
                    recvBuff = recvStr;
                    finishedRecvLine = true;
                    Console.WriteLine(string.Format("Event:{0} RECV:{1}", e.EventType, recvStr));
                    receivedDataFromSerial(theDevice.Name, recvStr);
                    recvStr = "";
                }
                
            }
        }

        private void serialPort1_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {

        }

        
    }
}
