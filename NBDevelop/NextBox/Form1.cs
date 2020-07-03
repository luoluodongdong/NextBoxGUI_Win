using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;


namespace NextBox
{
    public delegate void EventFromUnitViewDelegate(int index, Dictionary<string, string> content);
    public delegate void EventFromPasswordFormDelegate(string result);
    public delegate void EventFromSubViewDelegate(Dictionary<string, string> content);

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        ContainerView container = new ContainerView();
        
        private void Form1_Load(object sender, EventArgs e)
        {
            container.Location = new Point(0, 0);
            container.Dock = DockStyle.Fill;
            Controls.Add(container);
        }
        

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            
            e.Cancel = true;
            if (MessageBox.Show("Exit application？", "Exit", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                //Application.Exit();
            }
            else

            {
                //e.Cancel = true;
                return;
            }
            

            //Thread.Sleep(200);
            Thread closeThread = new Thread(new ThreadStart(closeSelf));
            closeThread.IsBackground = true;
            closeThread.Start();
        }
        //close MT self
        private void closeSelf()
        {
            //waiting all slots close self

            container.closeApp();
            //close MT opened devices
            //...
            //exit this application
            Invoke((EventHandler)(delegate
            {
                Dispose();
            }));

            Application.Exit();
        }
    }
}
