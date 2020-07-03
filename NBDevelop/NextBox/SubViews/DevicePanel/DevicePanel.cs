using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NextBox;

namespace NextBox.SubViews.DevicePanel
{
    public delegate void ReceivedDataFromDevice(string name, string data);
    public partial class DevicePanel : UserControl
    {
        public DevicePanel()
        {
            InitializeComponent();
        }
        public static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public List<JsonHelper.Device> devicesList;

        private Dictionary<string, MyDevice> loadDevicesList = new Dictionary<string, MyDevice>();

        private void DevicePanel_Load(object sender, EventArgs e)
        {
            
        }
        #region MyDevice Class
        private class MyDevice
        {
            public string name;
            public string type;
            public DeviceSerial serial;
            public DeviceInstrument instrument;
            public DeviceSocket socket;
            public bool SendDevice(string name, string cmd)
            {
                if (type.Equals("SERIAL"))
                {
                    return serial.SendCmd(cmd);
                }
                else if (type.Equals("INSTR"))
                {
                    return instrument.SendCmd(cmd);
                }
                else if (type.Equals("SOCKET"))
                {
                    return socket.SendCmd(cmd);
                }

                return false;
            }
            public bool QueryDevice(string name, string cmd, out string recv, double timeout)
            {
                recv = "";
                if (type.Equals("SERIAL"))
                {
                    return serial.SendAndReceived(cmd, timeout, out recv);
                }
                else if (type.Equals("INSTR"))
                {
                    return instrument.SendAndReceived(cmd, timeout, out recv);
                }
                else if (type.Equals("SOCKET"))
                {
                    return socket.SendAndReceived(cmd, timeout, out recv);
                }

                return false;
            }
            public void CloseDevice()
            {
                if (type.Equals("SERIAL"))
                {
                    serial.closeDevice();
                }
                else if (type.Equals("INSTR"))
                {
                    instrument.closeDevice(); 
                }
                else if (type.Equals("SOCKET"))
                {
                    socket.closeDevice();
                }
            }
        }
        #endregion

        public void initDevicePanel()
        {
            for (int i = 0; i < devicesList.Count; i++)
            {
                if (devicesList[i].Load.Equals("YES"))
                {
                    if (devicesList[i].Type.Equals("SERIAL"))
                    {
                        DeviceSerial serial = new DeviceSerial();
                        serial.receivedDataFromSerial += new ReceivedDataFromDevice(ReceivedDataFromTheDevice);
                        serial.theDevice = devicesList[i]; ;
                        layoutPanel.Controls.Add(serial);
                        serial.initDevice();
                        bool status = serial.AutoOpenSerialPort();
                        Console.WriteLine(string.Format("auto open serial[{0}] status:{1}", serial.theDevice.Description, status));

                        MyDevice myDevice = new MyDevice();
                        myDevice.name = serial.theDevice.Name;
                        myDevice.type = "SERIAL";
                        myDevice.serial = serial;
                        loadDevicesList.Add(serial.theDevice.Name, myDevice);
                    }
                    else if (devicesList[i].Type.Equals("INSTR"))
                    {
                        DeviceInstrument instrument = new DeviceInstrument();
                        instrument.theDevice = devicesList[i];
                        layoutPanel.Controls.Add(instrument);
                        instrument.initDevice();
                        bool status = instrument.AutoOpenInstrument();
                        Console.WriteLine(string.Format("auto open instrument[{0}] status:{1}", instrument.theDevice.Description, status));

                        MyDevice myDevice = new MyDevice();
                        myDevice.name = instrument.theDevice.Name;
                        myDevice.type = "INSTR";
                        myDevice.instrument = instrument;
                        loadDevicesList.Add(instrument.theDevice.Name, myDevice);
                    }
                    else if (devicesList[i].Type.Equals("SOCKET"))
                    {
                        DeviceSocket socket = new DeviceSocket();
                        socket.receivedDataFromSocket += new ReceivedDataFromDevice(ReceivedDataFromTheDevice);
                        socket.theDevice = devicesList[i];
                        layoutPanel.Controls.Add(socket);
                        socket.initDevice();
                        bool status = socket.AutoOpenSocket();
                        Console.WriteLine(string.Format("auto open socket[{0}] status:{1}", socket.theDevice.Description, status));

                        MyDevice myDevice = new MyDevice();
                        myDevice.name = socket.theDevice.Name;
                        myDevice.type = "SOCKET";
                        myDevice.socket = socket;
                        loadDevicesList.Add(socket.theDevice.Name, myDevice);
                    }

                }


            }
        }
        public void closeAllDevices()
        {
            foreach (KeyValuePair<string,MyDevice> item in loadDevicesList)
            {
                item.Value.CloseDevice();
            }
        }

        public bool SendDevice(string name,string cmd)
        {
            MyDevice device = loadDevicesList[name];
            return device.SendDevice(name, cmd);
        }
        
        public bool QueryDevice(string name, string cmd,out string recv,double timeout)
        {
            recv = "";
            MyDevice device = loadDevicesList[name];
            return device.QueryDevice(name, cmd, out recv, timeout);
        }

        private void ReceivedDataFromTheDevice(string name, string data)
        {
            log.Info(string.Format("[DevicePanel] - name:{0} received:{1}", name, data));
            //Console.WriteLine(string.Format("[DevicePanel] - name:{0} received:{1}", name, data));
        }

    }
}
