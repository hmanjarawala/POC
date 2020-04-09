using System;
using System.Text;
using System.Windows.Forms;

namespace EmailAutomationSample
{
    using Classes;
    using System.Collections.Generic;

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        #region Private Methods

        private string SetDefaultPort(bool isImap, bool isSecure)
        {
            if (isImap)
            {
                return isSecure ? "993" : "143";
            }
            else
            {
                return isSecure ? "995" : "110";
            }
        }

        private bool ValidateUI()
        {
            string _errorMessage = "";

            if (string.IsNullOrEmpty(tbHostName.Text.Trim()))
            {
                _errorMessage = "Please enter Host name";
            }

            if (string.IsNullOrEmpty(tbUserName.Text.Trim()))
            {
                _errorMessage = "Please enter User name";
            }

            if (string.IsNullOrEmpty(tbPassword.Text.Trim()))
            {
                _errorMessage = "Please enter Password";
            }

            if (string.IsNullOrEmpty(tbPortNo.Text.Trim()))
            {
                _errorMessage = "Please enter Port number";
            }

            if (!string.IsNullOrEmpty(_errorMessage.Trim()))
            {
                MessageBox.Show(_errorMessage);
                return false;
            }
            return true;
        }

        private void UpdateProgressBar(ProgressBar bar, int maxValue, int current)
        {
            if (bar.InvokeRequired)
            {
                bar.BeginInvoke(new Action<ProgressBar, int, int>(UpdateProgressBar), bar, maxValue, current);
            }
            else
            {
                bar.Maximum = maxValue;
                bar.Value = current;
                bar.Update();
                bar.Parent.Update();
            }
        }

        private void UpdateStatus(Label lbl, string status)
        {
            if (lbl.InvokeRequired)
            {
                lbl.BeginInvoke(new Action<Label, string>(UpdateStatus), lbl, status);
            }
            else
            {
                lbl.Text = status;
                lbl.Update();
                lbl.Parent.Update();
            }
        }

        private void UpdateStatus(TextBox lbl, string status)
        {
            if (lbl.InvokeRequired)
            {
                lbl.BeginInvoke(new Action<TextBox, string>(UpdateStatus), lbl, status);
            }
            else
            {
                lbl.Text = status;
                lbl.Update();
                lbl.Parent.Update();
            }
        }

        #endregion

        #region Events

        private void btnFetchEmails_Click(object sender, EventArgs e)
        {
            if (ValidateUI())
            {
                btnFetchEmails.Enabled = false;
                IMailClient client = null;

                if (rdbImap.Checked)
                    client = new ImapClient(tbHostName.Text.Trim(), Convert.ToInt32(tbPortNo.Text.Trim()), cbIsSecure.Checked);
                else
                    client = new PopClient(tbHostName.Text.Trim(), Convert.ToInt32(tbPortNo.Text.Trim()), cbIsSecure.Checked);

                EmailAutomation emailAutomation = new EmailAutomation(client, tbUserName.Text.Trim(), tbPassword.Text.Trim());
                emailAutomation.ProgressValueChanged += new ProgressValueChangedEventHandler(emailAutomation_ProgressValueChanged);
                emailAutomation.StatusChanged += new StatusChangedEventHandler(emailAutomation_StatusChanged);

                System.Threading.ThreadStart start = new System.Threading.ThreadStart(emailAutomation.PlayEmailAutomation);
                System.Threading.Thread t = new System.Threading.Thread(start);
                t.Start();

                while (t.IsAlive)
                {
                    Application.DoEvents();
                    System.Threading.Thread.Sleep(200);
                }

                emailAutomation.ProgressValueChanged -= new ProgressValueChangedEventHandler(emailAutomation_ProgressValueChanged);
                emailAutomation.StatusChanged -= new StatusChangedEventHandler(emailAutomation_StatusChanged);
                btnFetchEmails.Enabled = true;
            }
        }

        private void btnDefault_Click(object sender, EventArgs e)
        {
            tbPortNo.Text = SetDefaultPort(rdbImap.Checked, cbIsSecure.Checked);
        }

        private void emailAutomation_ProgressValueChanged(object sender, ProgressValueChangeEventArgs e)
        {
            UpdateProgressBar(pBar, e.MaxValue, e.Value);
        }

        private void emailAutomation_StatusChanged(object sender, StatusChangeEventArgs e)
        {
            UpdateStatus(label6, e.CurrentStatus);
            UpdateStatus(txtDetail, e.PreviousStatus);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text.StartsWith("Show"))
            {
                this.Width = 860;
                button1.Text = "Hide Detail <<";
            }
            else
            {
                this.Width = 430;
                button1.Text = "Show Detail >>";
                txtDetail.Text = string.Empty;
            }
        }

        //---------------------------------------------------
        #endregion

        #region Private Class

        private class EmailAutomation
        {
            readonly IMailClient client = null;
            readonly string username, password;
            StringBuilder retValue = null;

            public EmailAutomation(IMailClient client, string username, string password)
            {
                this.client = client;
                this.username = username;
                this.password = password;
            }

            public event ProgressValueChangedEventHandler ProgressValueChanged
            {
                add { this.progressValueChangedEventHandler += value; }
                remove { this.progressValueChangedEventHandler -= value; }
            }
            private ProgressValueChangedEventHandler progressValueChangedEventHandler;

            public event StatusChangedEventHandler StatusChanged
            {
                add { this.statusChangedEventHandler += value; }
                remove { this.statusChangedEventHandler -= value; }
            }
            private StatusChangedEventHandler statusChangedEventHandler;

            private void OnProgressValueChanged(ProgressValueChangeEventArgs e)
            {
                this.progressValueChangedEventHandler?.Invoke(this, e);
            }

            private void OnStatusChanged(StatusChangeEventArgs e)
            {
                this.statusChangedEventHandler?.Invoke(this, e);
            }

            private void TerminateEmailAutomation(string status, string prevousStatus)
            {
                ProgressValueChangeEventArgs pvce = new ProgressValueChangeEventArgs() { MinValue = 0, MaxValue = 4 };
                StatusChangeEventArgs sce = new StatusChangeEventArgs();

                sce.CurrentStatus = status;
                sce.PreviousStatus = prevousStatus;

                OnStatusChanged(sce);

                pvce.Value = 4;

                OnProgressValueChanged(pvce);
            }

            private string PrepareMessage(string message)
            {
                return string.Format("{0}\r\n{1}\r\n\r\n", message, "---------------------");
            }

            private void TraceLog(object sender, TraceEventArg e)
            {
                retValue.AppendLine(PrepareMessage(e.MessageDelegate()));
            }

            public void PlayEmailAutomation()
            {
                retValue = new StringBuilder("--------Start--------\r\n\r\n");
                int countMail = 0;

                ((ITraceable)client).LogTrace += new TraceEventHandler(TraceLog);

                ProgressValueChangeEventArgs pvce = new ProgressValueChangeEventArgs() { MinValue = 0, MaxValue = 4 };
                StatusChangeEventArgs sce = new StatusChangeEventArgs() { CurrentStatus = "Connecting " + client.GetHostName() + "...", PreviousStatus = "--------Start--------" };

                OnStatusChanged(sce);

                var result = client.Login(username, password);

                if (string.IsNullOrEmpty(result))
                {
                    pvce.Value = 1;

                    OnProgressValueChanged(pvce);

                    sce.CurrentStatus = "Authenticating client...";
                    sce.PreviousStatus = retValue.ToString();

                    OnStatusChanged(sce);

                    pvce.Value = 2;

                    OnProgressValueChanged(pvce);

                    sce.CurrentStatus = "Selecting Inbox...";
                    sce.PreviousStatus = retValue.ToString();

                    OnStatusChanged(sce);

                    if (string.IsNullOrEmpty(result))
                    {
                        pvce.Value = 3;

                        OnProgressValueChanged(pvce);

                        sce.CurrentStatus = "Getting total mail from Inbox...";
                        sce.PreviousStatus = retValue.ToString();

                        OnStatusChanged(sce);

                        result = client.GetTotalMail(out countMail);

                        if (string.IsNullOrEmpty(result))
                        {
                            if (countMail > 0)
                            {
                                sce.CurrentStatus = "Getting mail detail from Inbox...";
                                sce.PreviousStatus = retValue.ToString();

                                OnStatusChanged(sce);

                                IEnumerable<int> msgIds = null;

                                result = client.GetAllMessageFlags(MailFlag.All, out msgIds);

                                if (!string.IsNullOrEmpty(result))
                                    TerminateEmailAutomation("Problem in getting mail(s).", retValue.ToString());
                            }

                            pvce.Value = 4;

                            OnProgressValueChanged(pvce);

                            sce.CurrentStatus = "Logout from Inbox...";
                            sce.PreviousStatus = retValue.ToString();

                            OnStatusChanged(sce);

                            client.LogOut();

                            sce.CurrentStatus = "Logout completed...";
                            sce.PreviousStatus = retValue.ToString();

                            OnStatusChanged(sce);
                        }
                        else
                        {
                            TerminateEmailAutomation("Problem in getting mail count.", retValue.ToString());
                        }
                    }
                    else
                    {
                        TerminateEmailAutomation("Problem in getting mail count.", retValue.ToString());
                    }

                ((IDisposable)client).Dispose();
                    ((ITraceable)client).LogTrace -= new TraceEventHandler(TraceLog);
                }
            }
        }

        #endregion
    }

    delegate void ProgressValueChangedEventHandler(object sender, ProgressValueChangeEventArgs e);
    delegate void StatusChangedEventHandler(object sender, StatusChangeEventArgs e);

    public class ProgressValueChangeEventArgs
    {
        public int MinValue { get; set; }
        public int MaxValue { get; set; }
        public int Value { get; set; }
    }

    public class StatusChangeEventArgs
    {
        public string CurrentStatus { get; set; }
        public string PreviousStatus { get; set; }
    }
}
