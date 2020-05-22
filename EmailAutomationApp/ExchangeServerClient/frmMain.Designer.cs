namespace ExchangeServerClient
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlConfig = new System.Windows.Forms.Panel();
            this.btnConnect = new System.Windows.Forms.Button();
            this.chkEmailAddress = new System.Windows.Forms.CheckBox();
            this.txtEmailAddress = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtDomain = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pnlView = new System.Windows.Forms.Panel();
            this.btnBack = new System.Windows.Forms.Button();
            this.lstView = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label5 = new System.Windows.Forms.Label();
            this.cmbFolder = new System.Windows.Forms.ComboBox();
            this.btnFetchEmail = new System.Windows.Forms.Button();
            this.pnlConfig.SuspendLayout();
            this.pnlView.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlConfig
            // 
            this.pnlConfig.Controls.Add(this.btnConnect);
            this.pnlConfig.Controls.Add(this.chkEmailAddress);
            this.pnlConfig.Controls.Add(this.txtEmailAddress);
            this.pnlConfig.Controls.Add(this.label4);
            this.pnlConfig.Controls.Add(this.txtDomain);
            this.pnlConfig.Controls.Add(this.label3);
            this.pnlConfig.Controls.Add(this.txtPassword);
            this.pnlConfig.Controls.Add(this.label2);
            this.pnlConfig.Controls.Add(this.txtUserName);
            this.pnlConfig.Controls.Add(this.label1);
            this.pnlConfig.Location = new System.Drawing.Point(3, 5);
            this.pnlConfig.Name = "pnlConfig";
            this.pnlConfig.Size = new System.Drawing.Size(521, 369);
            this.pnlConfig.TabIndex = 0;
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(101, 159);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(93, 23);
            this.btnConnect.TabIndex = 9;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.BtnConnect_Click);
            // 
            // chkEmailAddress
            // 
            this.chkEmailAddress.AutoSize = true;
            this.chkEmailAddress.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkEmailAddress.Location = new System.Drawing.Point(27, 98);
            this.chkEmailAddress.Name = "chkEmailAddress";
            this.chkEmailAddress.Size = new System.Drawing.Size(199, 17);
            this.chkEmailAddress.TabIndex = 8;
            this.chkEmailAddress.Text = "Email Address Same As User Name?";
            this.chkEmailAddress.UseVisualStyleBackColor = true;
            this.chkEmailAddress.CheckedChanged += new System.EventHandler(this.ChkEmailAddress_CheckedChanged);
            // 
            // txtEmailAddress
            // 
            this.txtEmailAddress.Location = new System.Drawing.Point(101, 121);
            this.txtEmailAddress.Name = "txtEmailAddress";
            this.txtEmailAddress.Size = new System.Drawing.Size(303, 20);
            this.txtEmailAddress.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(24, 124);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(76, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Email Address:";
            // 
            // txtDomain
            // 
            this.txtDomain.Location = new System.Drawing.Point(101, 73);
            this.txtDomain.Name = "txtDomain";
            this.txtDomain.Size = new System.Drawing.Size(303, 20);
            this.txtDomain.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 76);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Domain:";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(101, 47);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(303, 20);
            this.txtPassword.TabIndex = 3;
            this.txtPassword.UseSystemPasswordChar = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Password:";
            // 
            // txtUserName
            // 
            this.txtUserName.Location = new System.Drawing.Point(101, 21);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(303, 20);
            this.txtUserName.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "User Name:";
            // 
            // pnlView
            // 
            this.pnlView.Controls.Add(this.btnFetchEmail);
            this.pnlView.Controls.Add(this.cmbFolder);
            this.pnlView.Controls.Add(this.label5);
            this.pnlView.Controls.Add(this.btnBack);
            this.pnlView.Controls.Add(this.lstView);
            this.pnlView.Location = new System.Drawing.Point(3, 5);
            this.pnlView.Name = "pnlView";
            this.pnlView.Size = new System.Drawing.Size(784, 443);
            this.pnlView.TabIndex = 1;
            // 
            // btnBack
            // 
            this.btnBack.Location = new System.Drawing.Point(655, 6);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(126, 23);
            this.btnBack.TabIndex = 1;
            this.btnBack.Text = "Back to Configuration";
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.BtnBack_Click);
            // 
            // lstView
            // 
            this.lstView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.lstView.FullRowSelect = true;
            this.lstView.Location = new System.Drawing.Point(3, 35);
            this.lstView.Name = "lstView";
            this.lstView.Size = new System.Drawing.Size(778, 405);
            this.lstView.TabIndex = 0;
            this.lstView.UseCompatibleStateImageBehavior = false;
            this.lstView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "From";
            this.columnHeader1.Width = 134;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Subject";
            this.columnHeader2.Width = 505;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Sent Date";
            this.columnHeader3.Width = 132;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 11);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(72, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "Select Folder:";
            // 
            // cmbFolder
            // 
            this.cmbFolder.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFolder.FormattingEnabled = true;
            this.cmbFolder.Location = new System.Drawing.Point(81, 7);
            this.cmbFolder.Name = "cmbFolder";
            this.cmbFolder.Size = new System.Drawing.Size(145, 21);
            this.cmbFolder.TabIndex = 3;
            // 
            // btnFetchEmail
            // 
            this.btnFetchEmail.Location = new System.Drawing.Point(553, 6);
            this.btnFetchEmail.Name = "btnFetchEmail";
            this.btnFetchEmail.Size = new System.Drawing.Size(96, 23);
            this.btnFetchEmail.TabIndex = 4;
            this.btnFetchEmail.Text = "Fetch Emails";
            this.btnFetchEmail.UseVisualStyleBackColor = true;
            this.btnFetchEmail.Click += new System.EventHandler(this.BtnFetchEmail_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(790, 451);
            this.Controls.Add(this.pnlView);
            this.Controls.Add(this.pnlConfig);
            this.Name = "frmMain";
            this.Text = "frmMain";
            this.pnlConfig.ResumeLayout(false);
            this.pnlConfig.PerformLayout();
            this.pnlView.ResumeLayout(false);
            this.pnlView.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlConfig;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.CheckBox chkEmailAddress;
        private System.Windows.Forms.TextBox txtEmailAddress;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtDomain;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel pnlView;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.ListView lstView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.Button btnFetchEmail;
        private System.Windows.Forms.ComboBox cmbFolder;
        private System.Windows.Forms.Label label5;
    }
}