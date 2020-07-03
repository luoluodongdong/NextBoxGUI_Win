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
    public partial class DialogWithOK : Form
    {
        public DialogWithOK()
        {
            InitializeComponent();
        }
        public string title = "Title Name";
        public string message = "Is this a message?";

        private void DialogWithOK_Load(object sender, EventArgs e)
        {
            titleLabel.Text = title;
            messageLabel.Text = message;
        }

        private void OKBtn_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
