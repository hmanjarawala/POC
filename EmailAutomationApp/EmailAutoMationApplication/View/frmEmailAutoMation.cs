using System;
using System.Linq;
using System.Windows.Forms;
using EmailAutoMationApplication.Presentor;
using EmailAutoMationApplication.Model;
using EmailAutomationLibrary.Imap;
using System.Globalization;

namespace EmailAutoMationApplication.View
{
    internal partial class frmEmailAutoMation : Form, IEmailAutomationView
    {
        readonly IEmailAutomationPresentor _presentor;

        enum ViewMode { Normal, Advance};

        ViewMode _mode = ViewMode.Normal;

        public string Hostname
        {
            get
            {
                if (string.IsNullOrEmpty(tbHostName.Text.Trim()))
                {
                    throw new Exception("Please enter Host name");
                }
                return tbHostName.Text.Trim();
            }

            set
            {
                tbHostName.Text = value;
            }
        }

        public int PortNumber
        {
            get
            {
                if (string.IsNullOrEmpty(tbPortNo.Text.Trim()))
                {
                    throw new Exception("Please enter Port number");
                }
                return Convert.ToInt32(tbPortNo.Text);
            }

            set
            {
                tbPortNo.Text = Convert.ToString(value);
            }
        }

        public string Username
        {
            get
            {
                if (string.IsNullOrEmpty(tbUserName.Text.Trim()))
                {
                    throw new Exception("Please enter User name");
                }
                return tbUserName.Text.Trim();
            }

            set
            {
                tbUserName.Text = value;
            }
        }

        public string Password
        {
            get
            {
                if (string.IsNullOrEmpty(tbPassword.Text.Trim()))
                {
                    throw new Exception("Please enter Password");
                }
                return tbPassword.Text.Trim();
            }

            set
            {
                tbPassword.Text = value;
            }
        }

        public bool IsSecure
        {
            get
            {
                return cbIsSecure.Checked;
            }

            set
            {
                cbIsSecure.Checked = value;
            }
        }

        public frmEmailAutoMation(IEmailAutomationPresentor presentor)
        {
            InitializeComponent();
            this.Width = 572;
            cmbDate.SelectedIndex = 2;
            _presentor = presentor;
            _presentor.SetView(this);
            _presentor.Actionchanged += frmEmailAutoMation_ActionChanged;
            _presentor.ProgressValueChanged += frmEmailAutoMation_ProgressValueChanged;
        }

        public int SetDefaultPort(bool isImap, bool isSecure)
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

        private void frmEmailAutoMation_ProgressValueChanged(object sender, ProgressValueChangeEventArgs e)
        {
            UpdateProgressBar(pBar, e.MaxValue, e.Value);
        }

        private void frmEmailAutoMation_ActionChanged(object sender, ActionEventArgs e)
        {
            UpdateListView(lvInbox, e.Item);
        }

        private void SetUI(bool isImap)
        {
            cmbFolders.Visible = isImap;
            btnFetch.Visible = isImap;
            btnAdvanceView.Visible = isImap;
            label5.Visible = isImap;

            if (!isImap)
            {
                btnFetchEmails.Location = new System.Drawing.Point(btnFetchEmails.Location.X, label5.Location.Y);
                _mode = ViewMode.Advance;
                ToggleViewMode();
            }
            else
            {
                btnFetchEmails.Location = new System.Drawing.Point(btnFetchEmails.Location.X, 190);
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

        private void btnDefault_Click(object sender, EventArgs e)
        {
            tbPortNo.Text = SetDefaultPort(rdbImap.Checked, cbIsSecure.Checked).ToString();
        }

        private void rdbImap_CheckedChanged(object sender, EventArgs e)
        {
            SetUI(rdbImap.Checked);
        }        

        private void btnFetch_Click(object sender, EventArgs e)
        {
            this.BeginInvoke(new MethodInvoker(FetchEmailFolder));
        }

        private void UpdateListView(ListView view, ListViewItem item)
        {
            if (view.InvokeRequired)
            {
                view.BeginInvoke(new Action<ListView, ListViewItem>(UpdateListView), view, item);
            }
            else
            {
                view.Items.Add(item);
                if (view.Items.Count > 10)
                    view.TopItem = view.Items[view.Items.Count - 10];
                view.Update();
                view.Parent.Update();
            }
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

        public void FetchEmailFolder()
        {
            try
            {
                EmailAutomationModel model = generateModel();

                var list = _presentor.FetchMailboxes(model);

                cmbFolders.DataSource = list;
                cmbFolders.DisplayMember = "Name";
                cmbFolders.ValueMember = "FullName";
                cmbFolders.SelectedIndex = list.ToList().FindIndex(x => x.Name.ToUpper() == "INBOX");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private EmailAutomationModel generateModel()
        {
            return new EmailAutomationModel
            {
                Hostname = this.Hostname,
                Port = this.PortNumber,
                Username = this.Username,
                Password = this.Password,
                IsSecure = this.IsSecure,
                IsProtocolImap = rdbImap.Checked,
            };
        }

        private MailSearchCriteria createCriteria(MailSearchCriteria source, MailSearchCriteria other)
        {
            if (source != null)
                return source.And(other);
            else
                return other;
        }

        private string generateSearchCriteria()
        {
            string retVal = string.Empty;

            if (rdbImap.Checked)
            {
                MailSearchCriteria criteria = null;

                if (cbFrom.Checked)
                {
                    criteria = createCriteria(criteria, MailSearchCriteria.From(tbSentFrom.Text.Trim()));
                }
                if (cbTo.Checked)
                {
                    criteria = createCriteria(criteria, MailSearchCriteria.To(tbSentTo.Text.Trim()));
                }
                if (cbCc.Checked)
                {
                    criteria = createCriteria(criteria, MailSearchCriteria.Cc(tbSentCc.Text.Trim()));
                }
                if (cbBcc.Checked)
                {
                    criteria = createCriteria(criteria, MailSearchCriteria.BCC(tbSentBcc.Text.Trim()));
                }
                if (cbSubject.Checked)
                {
                    criteria = createCriteria(criteria, MailSearchCriteria.Subject(tbSubject.Text.Trim()));
                }
                if (cbBody.Checked)
                {
                    criteria = createCriteria(criteria, MailSearchCriteria.Body(tbBody.Text.Trim()));
                }
                if (cbDate.Checked)
                {
                    criteria = createCriteria(criteria, createDateCriteria());
                }
                if (cbMailFlag.Checked)
                {
                    criteria = createCriteria(criteria, createCriteriaForMessageFlag());
                }

                if (criteria != null)
                    retVal = criteria.ToString();
            }

            return retVal;
        }

        private MailSearchCriteria createCriteriaForMessageFlag()
        {
            MailSearchCriteria temp;
            switch (cmbFlag.SelectedIndex)
            {
                case 0:
                    temp = MailSearchCriteria.Seen();
                    break;
                case 1:
                    temp = MailSearchCriteria.Unseen();
                    break;
                case 2:
                    temp = MailSearchCriteria.Deleted();
                    break;
                case 3:
                    temp = MailSearchCriteria.Undeleted();
                    break;
                case 4:
                    temp = MailSearchCriteria.Draft();
                    break;
                case 5:
                    temp = MailSearchCriteria.Undraft();
                    break;
                default:
                    temp = MailSearchCriteria.All();
                    break;
            }

            return temp;
        }

        private MailSearchCriteria createDateCriteria()
        {
            MailSearchCriteria temp;
            DateTime date = DateTime.ParseExact(tbSentDate.Text.Trim(), "MM/dd/yyyy", CultureInfo.InvariantCulture);
            switch (cmbDate.SelectedIndex)
            {
                case 0:
                    temp = MailSearchCriteria.SentBefore(date);
                    break;
                case 1:
                    temp = MailSearchCriteria.SentSince(date);
                    break;
                case 2:
                default:
                    temp = MailSearchCriteria.SentOn(date);
                    break;
            }

            return temp;
        }

        private void btnFetchEmails_Click(object sender, EventArgs e)
        {
            lvInbox.Items.Clear();

            try
            {
                EmailAutomationModel model = generateModel();

                model.MailSearchCriteria = generateSearchCriteria();

                model.MailsToDisplay = Convert.ToInt32(tbMailCount.Text.Trim());

                model.MailBox = (rdbImap.Checked) ? cmbFolders.SelectedValue.ToString() : "Inbox";

                System.Threading.ParameterizedThreadStart start = new System.Threading.ParameterizedThreadStart(_presentor.PlayEmailAutomation);
                System.Threading.Thread t = new System.Threading.Thread(start);
                t.Start(model);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnAdvanceView_Click(object sender, EventArgs e)
        {
            ToggleViewMode();
        }

        private void ToggleViewMode()
        {
            if (_mode == ViewMode.Advance)
            {
                _mode = ViewMode.Normal;
                this.Width = 572;
                btnAdvanceView.Text = "Advance View";
                lvInbox.Columns[0].Width = 60;
                lvInbox.Columns[2].Width = 83;
            }
            else
            {
                _mode = ViewMode.Advance;
                this.Width = 953;
                btnAdvanceView.Text = "Normal View";
                lvInbox.Columns[0].Width = 333;
                lvInbox.Columns[2].Width = 188;
            }
        }
    }
}
