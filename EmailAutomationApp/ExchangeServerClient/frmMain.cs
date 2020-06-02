using Com.Service.Email.ExchangeServer;
using ExchangeServerClient.Data;
using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExchangeServerClient
{
    public partial class frmMain : Form
    {
        readonly Size FORM_DEFAULT_SIZE = new Size(521, 369);
        readonly Point PANEL_DEFAULT_LOCATION = new Point(3, 5);
        readonly Size FORM_DETAIL_SIZE = new Size(806, 490);
        IMSExchangeService exchangeService;

        public frmMain()
        {
            InitializeComponent();
            this.Size = FORM_DEFAULT_SIZE;
            this.pnlConfig.Visible = true;
            this.pnlView.Visible = false;
        }

        private void ChkEmailAddress_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked)
            {
                if (string.IsNullOrEmpty(txtUserName.Text.Trim()))
                    throw new ArgumentNullException();
                txtEmailAddress.Text = txtUserName.Text.Trim();
                txtEmailAddress.ReadOnly = true;
            }
            else
            {
                txtEmailAddress.Text = string.Empty;
                txtEmailAddress.ReadOnly = false;
                txtEmailAddress.Focus();
            }
        }

        private bool validateControls()
        {
            StringBuilder errorMessages = new StringBuilder();

            if (string.IsNullOrEmpty(txtUserName.Text.Trim()))
                errorMessages.AppendLine("User name can\'t be empty.");

            if (string.IsNullOrEmpty(txtPassword.Text.Trim()))
                errorMessages.AppendLine("Password can\'t be empty.");

            if (string.IsNullOrEmpty(txtEmailAddress.Text.Trim()))
                errorMessages.AppendLine("Email address can\'t be empty.");

            if (!string.IsNullOrEmpty(errorMessages.ToString()))
            {
                MessageBox.Show(errorMessages.ToString(), "MS Exchange Client", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private void BtnConnect_Click(object sender, EventArgs e)
        {
            if (validateControls())
            {
                var serviceUri = "https://outlook.office365.com/EWS/Exchange.asmx";
                if (string.IsNullOrEmpty(txtDomain.Text.Trim()))
                {
                    exchangeService = ExchangeServiceFactory.GetExchangeWebClient(txtUserName.Text.Trim(),
                        txtPassword.Text.Trim(), txtEmailAddress.Text.Trim(), serviceUri);
                }
                else
                {
                    exchangeService = ExchangeServiceFactory.GetExchangeClient(txtUserName.Text.Trim(),
                        txtPassword.Text.Trim(), txtDomain.Text.Trim(), txtEmailAddress.Text.Trim(), serviceUri);
                }

                this.BeginInvoke(new MethodInvoker(fetchFolder));
                this.BeginInvoke(new MethodInvoker(setDetailView));
            }
        }

        private void fillListView(IEnumerable<EmailMessage> messages)
        {
            lstView.Items.Clear();
            DbManager dbManager = new DbManager
            {
                //Provider = "SQLOLEDB.1",
                //DataSource = @"localhost\SQLEXPRESS",
                //Database = "EmailTracker",
                //UseWindowAuthentication = true,
                //ExtraParameter = "Integrated Security=SSPI;Persist Security Info=False;"
                CustomConnectionString = "Integrated Security=SSPI;Persist Security Info=False;" + 
                @"Initial Catalog=EmailTracker;Data Source=localhost\SQLEXPRESS"
            };

            MessageBox.Show(dbManager.ConnectionString);
            foreach (var message in messages)
            {
                ListViewItem item = new ListViewItem(message.Sender.ToString());
                item.SubItems.Add(message.Subject);
                item.SubItems.Add(message.DateTimeSent.ToString());
                item.Tag = message;
                lstView.Items.Add(item);
                dbManager.InsertEmailDetails(exchangeService.ReadEmail(message.Id));
            }
        }

        private void fetchFolder()
        {
            var folders = exchangeService.GetFolders();

            folders = folders.Where((f) => { return f.ExtendedProperties[0].Value.Equals(false); }).ToList();

            cmbFolder.DataSource = folders;
            cmbFolder.DisplayMember = "DisplayName";
            cmbFolder.ValueMember = "Id";
            cmbFolder.SelectedIndex = folders.ToList().FindIndex(x => x.DisplayName.ToLower().Equals("inbox"));
        }

        private void setDetailView()
        {
            this.pnlConfig.Visible = false;
            this.pnlView.Visible = true;
            this.pnlView.Location = PANEL_DEFAULT_LOCATION;
            this.Size = FORM_DETAIL_SIZE;
        }

        private void setConfigView()
        {
            this.pnlConfig.Visible = true;
            this.pnlView.Visible = false;
            this.Size = FORM_DEFAULT_SIZE;
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            lstView.Items.Clear();
            setConfigView();
        }

        private void BtnFetchEmail_Click(object sender, EventArgs e)
        {
            this.BeginInvoke(new MethodInvoker(() => {
                var folder = cmbFolder.Text;
                var emails = exchangeService.ReadEmails(folder);
                fillListView(emails);
            }));
        }
    }
}
