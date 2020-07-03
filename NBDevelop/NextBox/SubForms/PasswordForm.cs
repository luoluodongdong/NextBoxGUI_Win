using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NextBox
{
    public partial class PasswordForm : Form
    {
        public PasswordForm()
        {
            InitializeComponent();
        }
        public event EventFromPasswordFormDelegate eventFromPasswordForm;
        public string rawPassword = "123456";
        private string result = "NG";

        public void initView()
        {
            result = "NG";
            inputTextBox.Text = "";
        }

        private void PasswordForm_Load(object sender, EventArgs e)
        {
            errorMsgLabel.Text = "";
    
        }
        private void backBtn_Click(object sender, EventArgs e)
        {
            result = "NG";
            Close();
        }

        private void PasswordForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            eventFromPasswordForm(result);
        }
        //响应输入后的回车事件
        private void inputTextBox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (inputTextBox.Text.Equals(rawPassword))
                {
                    result = "OK";
                    Close();
                }
                else
                {
                    errorMsgLabel.Text = "password verify failure!";
                    inputTextBox.Text = "";

                }
            }
        }

        private void PasswordForm_Shown(object sender, EventArgs e)
        {
            Activate();
            inputTextBox.Focus();
        }
    }
}
