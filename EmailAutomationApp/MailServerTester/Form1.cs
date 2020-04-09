using System;
using System.Text;
using System.Windows.Forms;

namespace MailServerTester
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private int SetDefaultPort(bool isImap, bool isSecure)
        {
            if (isImap)
            {
                return isSecure ? 993 : 143;
            }
            else
            {
                return isSecure ? 995 : 110;
            }
        }

        private void btnDefault_Click(object sender, EventArgs e)
        {
            tbPortNo.Text = SetDefaultPort(rdbImap.Checked, cbIsSecure.Checked).ToString();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            EmailClient client = null;
            var builder = new StringBuilder();

            if (rdbImap.Checked)
            {
                client = new ImapClient(tbHostName.Text.Trim(), Convert.ToInt32(tbPortNo.Text.Trim()), cbIsSecure.Checked);
            }
            else
            {
                client = new Pop3Client(tbHostName.Text.Trim(), Convert.ToInt32(tbPortNo.Text.Trim()), cbIsSecure.Checked);
            }
            builder.AppendLine("% " + tbCommand.Text);
            client.Login(tbUserName.Text.Trim(), tbPassword.Text.Trim());
            client.SelectMailbox();
            builder.Append(client.ExecuteCommand(tbCommand.Text.Trim()));
            client.LogOut();

            tbOutput.Text = builder.ToString();
            builder.Remove(0, builder.Length);
            builder = null;
        }
    }
}