using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NextBox.SubViews
{
    public partial class FatalErrorView : UserControl
    {
        public FatalErrorView()
        {
            InitializeComponent();
        }

        private void FatalErrorView_Load(object sender, EventArgs e)
        {

        }
        public void showErrorMessage(string msg)
        {
            messageLabel.Text = msg;
        }
    }
}
