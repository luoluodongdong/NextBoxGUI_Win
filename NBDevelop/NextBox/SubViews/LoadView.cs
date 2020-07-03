using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace NextBox.SubViews
{
    public partial class LoadView : UserControl
    {
        public LoadView()
        {
            InitializeComponent();
        }

        public event EventFromSubViewDelegate eventFromLoadView;

        public void updateLoadMessage(string msg, int progressValue)
        {
            progressValue = progressValue > 100 ? 100 : progressValue;
            Invoke((EventHandler)(delegate {
                listBox1.Items.Add(msg);
                progressBar1.Value = progressValue;
            }));
            if (progressValue == 100)
            {
                Dictionary<string, string> dict =new Dictionary<string, string>();
                dict.Add("name", "Load");
                dict.Add("type", "done");
                Thread.Sleep(1000);
                eventFromLoadView(dict);
            }
        }

        private void LoadView_Load(object sender, EventArgs e)
        {
            progressBar1.Value = 0;
            listBox1.Items.Clear();
        }
    }
}
