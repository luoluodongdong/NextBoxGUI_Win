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
using NONATEST;
using System.Threading;

namespace NextBox.SubViews.DevicePanel
{
    public partial class DeviceSocket : UserControl
    {
        public DeviceSocket()
        {
            InitializeComponent();
        }
        public JsonHelper.Device theDevice;
        public event ReceivedDataFromDevice receivedDataFromSocket;
        private TestCoreManager coreManager = TestCoreManager.Instance;
        private string recvStr = "";
        private readonly object _lockRecvData = new object();

        private bool isConnected = false;
        private bool finishedRecvLine = false;
        private string recvBuff = "";
        private _SocketClient socket = new _SocketClient();
        private string ipStr = "127.0.0.1";
        private int port = 123456;

        public void initDevice()
        {
            string[] arrayTemp =  theDevice.Port.Split(':');
            if (arrayTemp.Length > 1)
            {
                ipStr = arrayTemp[0];
                port = int.Parse(arrayTemp[1]);
            }
        }

        public void refreshDeviceUI()
        {
            ipTextBox.Text = ipStr;
            portTextBox.Text = port.ToString();
            if (isConnected)
            {
                ipTextBox.Enabled = false;
                portTextBox.Enabled = false;
                OpenBtn.Text = "Close";
            }
            else
            {
                ipTextBox.Enabled = true;
                portTextBox.Enabled = true;
                OpenBtn.Text = "Open";
            }
        }
        public void closeDevice()
        {
            if (isConnected)
            {
                socket.DisconnectServer();
            }
        }

        public bool AutoOpenSocket()
        {
            bool status = openSocket();
            isConnected = status;
            printLog(string.Format("auto open socket: [{0}:{1}] status:{2}",ipStr,port,status));
            return status;
        }
        public bool SendCmd(string cmd)
        {
            if (isConnected == false)
            {
                return false;
            }
            return socket.Query(cmd);
        }
        public bool SendAndReceived(string cmd, double timeout, out string receiveStr)
        {
            receiveStr = "";
            if (isConnected == false)
            {
                return false;
            }
            recvBuff = "";
            finishedRecvLine = false;
            bool status = socket.Query(cmd);
            if (status ==false)
            {
                return false;
            }
            DateTime startT = DateTime.Now;
            double duration = 0.00;
            bool isTimeOut = true;
            while (duration <= timeout)
            {
                if (finishedRecvLine == true)
                {
                    receiveStr = recvBuff;
                    isTimeOut = false;
                    break;
                }

                TimeSpan span = DateTime.Now - startT;
                duration = span.TotalSeconds;
                Thread.Sleep(20);
            }

            return !isTimeOut;
        }
        private void DeviceSocket_Load(object sender, EventArgs e)
        {
            groupBox1.Text = theDevice.Description;
            refreshDeviceUI();
        }

        private void OpenBtn_Click(object sender, EventArgs e)
        {
            if (isConnected == false)
            {
                ipStr = ipTextBox.Text;
                port = int.Parse(portTextBox.Text);
                bool result = openSocket();
                if (result)
                {
                    OpenBtn.Text = "Close";
                    ipTextBox.Enabled = false;
                    portTextBox.Enabled = false;
                    isConnected = true;
                    theDevice.Port = string.Format("{0}:{1}", ipStr, port);
                    coreManager.json_helper.saveCfg();
                    MessageBox.Show(string.Format("Socket [{0}] connected successful!",theDevice.Name),"Info");
                }
                printLog("[client]connect server:" + result.ToString());
            }
            else
            {
                bool result = socket.DisconnectServer();
                printLog("[client]disconnct:" + result.ToString());
                OpenBtn.Text = "Open";
                ipTextBox.Enabled = true;
                portTextBox.Enabled = true;
                isConnected = false;
            }
        }
        private bool openSocket()
        {
            socket = new _SocketClient();
            //创建客户端对象，默认连接本机127.0.0.1,端口为12345
            socket.InitSocket(ipStr, port);
            socket.client.Property = string.Format("ip:{0}", port);
            //绑定当收到服务器发送的消息后的处理事件
            socket.client.HandleRecMsg = new Action<byte[], SocketClient>((bytes, theClient) =>
            {
                string msg = Encoding.UTF8.GetString(bytes);
                receivedData(msg);
                printLog("[client][RX]" + msg);
            });

            //绑定向服务器发送消息后的处理事件
            socket.client.HandleSendMsg = new Action<byte[], SocketClient>((bytes, theClient) =>
            {
                string msg = Encoding.UTF8.GetString(bytes);
                printLog("[client][TX]" + msg);
            });
            socket.client.HandleException = new Action<Exception>(ex =>
            {
                MessageBox.Show(ex.Message, "Error");
                printLog(ex.Message);
            });

            //开始运行客户端
            return socket.ConnectServer();
        }
        private void receivedData(string data)
        {
            lock (_lockRecvData)
            {
                receivedDataFromSocket(theDevice.Name, data);
                recvBuff = data;
                finishedRecvLine = true;
            }

        }
        private void printLog(string log)
        {
            Console.WriteLine("[socket] - "+log);
        }

    }
}
