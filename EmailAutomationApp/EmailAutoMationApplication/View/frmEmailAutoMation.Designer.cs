namespace EmailAutoMationApplication.View
{
    partial class frmEmailAutoMation
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
            _presentor.ProgressValueChanged -= frmEmailAutoMation_ProgressValueChanged;
            _presentor.Actionchanged -= frmEmailAutoMation_ActionChanged;

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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnAdvanceView = new System.Windows.Forms.Button();
            this.btnFetchEmails = new System.Windows.Forms.Button();
            this.btnFetch = new System.Windows.Forms.Button();
            this.cmbFolders = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnDefault = new System.Windows.Forms.Button();
            this.tbPortNo = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cbIsSecure = new System.Windows.Forms.CheckBox();
            this.rdbImap = new System.Windows.Forms.RadioButton();
            this.rdbPop = new System.Windows.Forms.RadioButton();
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbUserName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbHostName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lvInbox = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.pBar = new System.Windows.Forms.ProgressBar();
            this.label6 = new System.Windows.Forms.Label();
            this.tbMailCount = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cmbFlag = new System.Windows.Forms.ComboBox();
            this.cbMailFlag = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.cmbDate = new System.Windows.Forms.ComboBox();
            this.tbSentDate = new System.Windows.Forms.TextBox();
            this.cbDate = new System.Windows.Forms.CheckBox();
            this.tbBody = new System.Windows.Forms.TextBox();
            this.cbBody = new System.Windows.Forms.CheckBox();
            this.tbSubject = new System.Windows.Forms.TextBox();
            this.cbSubject = new System.Windows.Forms.CheckBox();
            this.tbSentBcc = new System.Windows.Forms.TextBox();
            this.cbBcc = new System.Windows.Forms.CheckBox();
            this.tbSentCc = new System.Windows.Forms.TextBox();
            this.cbCc = new System.Windows.Forms.CheckBox();
            this.tbSentTo = new System.Windows.Forms.TextBox();
            this.cbTo = new System.Windows.Forms.CheckBox();
            this.tbSentFrom = new System.Windows.Forms.TextBox();
            this.cbFrom = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnAdvanceView);
            this.groupBox1.Controls.Add(this.btnFetchEmails);
            this.groupBox1.Controls.Add(this.btnFetch);
            this.groupBox1.Controls.Add(this.cmbFolders);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.btnDefault);
            this.groupBox1.Controls.Add(this.tbPortNo);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.cbIsSecure);
            this.groupBox1.Controls.Add(this.rdbImap);
            this.groupBox1.Controls.Add(this.rdbPop);
            this.groupBox1.Controls.Add(this.tbPassword);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.tbUserName);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.tbHostName);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(2, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(551, 218);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // btnAdvanceView
            // 
            this.btnAdvanceView.Location = new System.Drawing.Point(98, 190);
            this.btnAdvanceView.Name = "btnAdvanceView";
            this.btnAdvanceView.Size = new System.Drawing.Size(128, 23);
            this.btnAdvanceView.TabIndex = 20;
            this.btnAdvanceView.Text = "Advance View";
            this.btnAdvanceView.UseVisualStyleBackColor = true;
            this.btnAdvanceView.Click += new System.EventHandler(this.btnAdvanceView_Click);
            // 
            // btnFetchEmails
            // 
            this.btnFetchEmails.Location = new System.Drawing.Point(9, 190);
            this.btnFetchEmails.Name = "btnFetchEmails";
            this.btnFetchEmails.Size = new System.Drawing.Size(83, 23);
            this.btnFetchEmails.TabIndex = 15;
            this.btnFetchEmails.Text = "Fetch Emails";
            this.btnFetchEmails.UseVisualStyleBackColor = true;
            this.btnFetchEmails.Click += new System.EventHandler(this.btnFetchEmails_Click);
            // 
            // btnFetch
            // 
            this.btnFetch.Location = new System.Drawing.Point(216, 164);
            this.btnFetch.Name = "btnFetch";
            this.btnFetch.Size = new System.Drawing.Size(83, 23);
            this.btnFetch.TabIndex = 14;
            this.btnFetch.Text = "Email Folders";
            this.btnFetch.UseVisualStyleBackColor = true;
            this.btnFetch.Click += new System.EventHandler(this.btnFetch_Click);
            // 
            // cmbFolders
            // 
            this.cmbFolders.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFolders.FormattingEnabled = true;
            this.cmbFolders.Location = new System.Drawing.Point(83, 166);
            this.cmbFolders.Name = "cmbFolders";
            this.cmbFolders.Size = new System.Drawing.Size(121, 21);
            this.cmbFolders.TabIndex = 13;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 169);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(75, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Email Folders :";
            // 
            // btnDefault
            // 
            this.btnDefault.Location = new System.Drawing.Point(216, 138);
            this.btnDefault.Name = "btnDefault";
            this.btnDefault.Size = new System.Drawing.Size(75, 23);
            this.btnDefault.TabIndex = 11;
            this.btnDefault.Text = "Use Default";
            this.btnDefault.UseVisualStyleBackColor = true;
            this.btnDefault.Click += new System.EventHandler(this.btnDefault_Click);
            // 
            // tbPortNo
            // 
            this.tbPortNo.Location = new System.Drawing.Point(83, 138);
            this.tbPortNo.Name = "tbPortNo";
            this.tbPortNo.ReadOnly = true;
            this.tbPortNo.Size = new System.Drawing.Size(130, 20);
            this.tbPortNo.TabIndex = 10;
            this.tbPortNo.Text = "143";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 141);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(52, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Port No. :";
            // 
            // cbIsSecure
            // 
            this.cbIsSecure.AutoSize = true;
            this.cbIsSecure.Location = new System.Drawing.Point(10, 94);
            this.cbIsSecure.Name = "cbIsSecure";
            this.cbIsSecure.Size = new System.Drawing.Size(173, 17);
            this.cbIsSecure.TabIndex = 8;
            this.cbIsSecure.Text = "Server uses secure connection";
            this.cbIsSecure.UseVisualStyleBackColor = true;
            // 
            // rdbImap
            // 
            this.rdbImap.AutoSize = true;
            this.rdbImap.Checked = true;
            this.rdbImap.Location = new System.Drawing.Point(118, 115);
            this.rdbImap.Name = "rdbImap";
            this.rdbImap.Size = new System.Drawing.Size(54, 17);
            this.rdbImap.TabIndex = 7;
            this.rdbImap.TabStop = true;
            this.rdbImap.Text = "Imap4";
            this.rdbImap.UseVisualStyleBackColor = true;
            this.rdbImap.CheckedChanged += new System.EventHandler(this.rdbImap_CheckedChanged);
            // 
            // rdbPop
            // 
            this.rdbPop.AutoSize = true;
            this.rdbPop.Location = new System.Drawing.Point(10, 115);
            this.rdbPop.Name = "rdbPop";
            this.rdbPop.Size = new System.Drawing.Size(50, 17);
            this.rdbPop.TabIndex = 6;
            this.rdbPop.Text = "Pop3";
            this.rdbPop.UseVisualStyleBackColor = true;
            this.rdbPop.CheckedChanged += new System.EventHandler(this.rdbImap_CheckedChanged);
            // 
            // tbPassword
            // 
            this.tbPassword.Location = new System.Drawing.Point(83, 68);
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.PasswordChar = '*';
            this.tbPassword.Size = new System.Drawing.Size(130, 20);
            this.tbPassword.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 71);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Password :";
            // 
            // tbUserName
            // 
            this.tbUserName.Location = new System.Drawing.Point(83, 42);
            this.tbUserName.Name = "tbUserName";
            this.tbUserName.Size = new System.Drawing.Size(202, 20);
            this.tbUserName.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "User Name :";
            // 
            // tbHostName
            // 
            this.tbHostName.Location = new System.Drawing.Point(83, 14);
            this.tbHostName.Name = "tbHostName";
            this.tbHostName.Size = new System.Drawing.Size(202, 20);
            this.tbHostName.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Host Name :";
            // 
            // lvInbox
            // 
            this.lvInbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvInbox.CheckBoxes = true;
            this.lvInbox.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.lvInbox.GridLines = true;
            this.lvInbox.Location = new System.Drawing.Point(2, 226);
            this.lvInbox.Name = "lvInbox";
            this.lvInbox.Size = new System.Drawing.Size(932, 252);
            this.lvInbox.TabIndex = 1;
            this.lvInbox.UseCompatibleStateImageBehavior = false;
            this.lvInbox.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "From";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Subject";
            this.columnHeader2.Width = 402;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Date";
            this.columnHeader3.Width = 83;
            // 
            // pBar
            // 
            this.pBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pBar.Location = new System.Drawing.Point(684, 482);
            this.pBar.Name = "pBar";
            this.pBar.Size = new System.Drawing.Size(250, 17);
            this.pBar.TabIndex = 2;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(1, 484);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(37, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Show ";
            // 
            // tbMailCount
            // 
            this.tbMailCount.Location = new System.Drawing.Point(37, 482);
            this.tbMailCount.MaxLength = 2;
            this.tbMailCount.Name = "tbMailCount";
            this.tbMailCount.Size = new System.Drawing.Size(39, 20);
            this.tbMailCount.TabIndex = 14;
            this.tbMailCount.Text = "10";
            this.tbMailCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(82, 484);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(98, 13);
            this.label7.TabIndex = 15;
            this.label7.Text = "mail(s) from Mailbox";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cmbFlag);
            this.groupBox3.Controls.Add(this.cbMailFlag);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.cmbDate);
            this.groupBox3.Controls.Add(this.tbSentDate);
            this.groupBox3.Controls.Add(this.cbDate);
            this.groupBox3.Controls.Add(this.tbBody);
            this.groupBox3.Controls.Add(this.cbBody);
            this.groupBox3.Controls.Add(this.tbSubject);
            this.groupBox3.Controls.Add(this.cbSubject);
            this.groupBox3.Controls.Add(this.tbSentBcc);
            this.groupBox3.Controls.Add(this.cbBcc);
            this.groupBox3.Controls.Add(this.tbSentCc);
            this.groupBox3.Controls.Add(this.cbCc);
            this.groupBox3.Controls.Add(this.tbSentTo);
            this.groupBox3.Controls.Add(this.cbTo);
            this.groupBox3.Controls.Add(this.tbSentFrom);
            this.groupBox3.Controls.Add(this.cbFrom);
            this.groupBox3.Location = new System.Drawing.Point(560, 0);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(373, 218);
            this.groupBox3.TabIndex = 16;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Search Criteria";
            // 
            // cmbFlag
            // 
            this.cmbFlag.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFlag.FormattingEnabled = true;
            this.cmbFlag.Items.AddRange(new object[] {
            "Seen",
            "Unseen",
            "Deleted",
            "Not Deleted",
            "Draft",
            "Un Draft"});
            this.cmbFlag.Location = new System.Drawing.Point(177, 190);
            this.cmbFlag.Name = "cmbFlag";
            this.cmbFlag.Size = new System.Drawing.Size(94, 21);
            this.cmbFlag.TabIndex = 17;
            // 
            // cbMailFlag
            // 
            this.cbMailFlag.AutoSize = true;
            this.cbMailFlag.Location = new System.Drawing.Point(10, 193);
            this.cbMailFlag.Name = "cbMailFlag";
            this.cbMailFlag.Size = new System.Drawing.Size(171, 17);
            this.cbMailFlag.TabIndex = 16;
            this.cbMailFlag.Text = "Fetch emails which marked as ";
            this.cbMailFlag.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(262, 167);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(105, 13);
            this.label8.TabIndex = 15;
            this.label8.Text = "Format: MM/dd/yyyy";
            // 
            // cmbDate
            // 
            this.cmbDate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDate.FormattingEnabled = true;
            this.cmbDate.Items.AddRange(new object[] {
            "Before",
            "Since",
            "On"});
            this.cmbDate.Location = new System.Drawing.Point(86, 164);
            this.cmbDate.Name = "cmbDate";
            this.cmbDate.Size = new System.Drawing.Size(49, 21);
            this.cmbDate.TabIndex = 14;
            // 
            // tbSentDate
            // 
            this.tbSentDate.Location = new System.Drawing.Point(140, 164);
            this.tbSentDate.Name = "tbSentDate";
            this.tbSentDate.Size = new System.Drawing.Size(116, 20);
            this.tbSentDate.TabIndex = 13;
            // 
            // cbDate
            // 
            this.cbDate.AutoSize = true;
            this.cbDate.Location = new System.Drawing.Point(10, 167);
            this.cbDate.Name = "cbDate";
            this.cbDate.Size = new System.Drawing.Size(76, 17);
            this.cbDate.TabIndex = 12;
            this.cbDate.Text = "Email Sent";
            this.cbDate.UseVisualStyleBackColor = true;
            // 
            // tbBody
            // 
            this.tbBody.Location = new System.Drawing.Point(140, 139);
            this.tbBody.Name = "tbBody";
            this.tbBody.Size = new System.Drawing.Size(223, 20);
            this.tbBody.TabIndex = 11;
            // 
            // cbBody
            // 
            this.cbBody.AutoSize = true;
            this.cbBody.Location = new System.Drawing.Point(10, 142);
            this.cbBody.Name = "cbBody";
            this.cbBody.Size = new System.Drawing.Size(93, 17);
            this.cbBody.TabIndex = 10;
            this.cbBody.Text = "Body contains";
            this.cbBody.UseVisualStyleBackColor = true;
            // 
            // tbSubject
            // 
            this.tbSubject.Location = new System.Drawing.Point(140, 114);
            this.tbSubject.Name = "tbSubject";
            this.tbSubject.Size = new System.Drawing.Size(223, 20);
            this.tbSubject.TabIndex = 9;
            // 
            // cbSubject
            // 
            this.cbSubject.AutoSize = true;
            this.cbSubject.Location = new System.Drawing.Point(10, 117);
            this.cbSubject.Name = "cbSubject";
            this.cbSubject.Size = new System.Drawing.Size(105, 17);
            this.cbSubject.TabIndex = 8;
            this.cbSubject.Text = "Subject contains";
            this.cbSubject.UseVisualStyleBackColor = true;
            // 
            // tbSentBcc
            // 
            this.tbSentBcc.Location = new System.Drawing.Point(140, 89);
            this.tbSentBcc.Name = "tbSentBcc";
            this.tbSentBcc.Size = new System.Drawing.Size(223, 20);
            this.tbSentBcc.TabIndex = 7;
            // 
            // cbBcc
            // 
            this.cbBcc.AutoSize = true;
            this.cbBcc.Location = new System.Drawing.Point(10, 92);
            this.cbBcc.Name = "cbBcc";
            this.cbBcc.Size = new System.Drawing.Size(45, 17);
            this.cbBcc.TabIndex = 6;
            this.cbBcc.Text = "Bcc";
            this.cbBcc.UseVisualStyleBackColor = true;
            // 
            // tbSentCc
            // 
            this.tbSentCc.Location = new System.Drawing.Point(140, 64);
            this.tbSentCc.Name = "tbSentCc";
            this.tbSentCc.Size = new System.Drawing.Size(223, 20);
            this.tbSentCc.TabIndex = 5;
            // 
            // cbCc
            // 
            this.cbCc.AutoSize = true;
            this.cbCc.Location = new System.Drawing.Point(10, 67);
            this.cbCc.Name = "cbCc";
            this.cbCc.Size = new System.Drawing.Size(39, 17);
            this.cbCc.TabIndex = 4;
            this.cbCc.Text = "Cc";
            this.cbCc.UseVisualStyleBackColor = true;
            // 
            // tbSentTo
            // 
            this.tbSentTo.Location = new System.Drawing.Point(140, 39);
            this.tbSentTo.Name = "tbSentTo";
            this.tbSentTo.Size = new System.Drawing.Size(223, 20);
            this.tbSentTo.TabIndex = 3;
            // 
            // cbTo
            // 
            this.cbTo.AutoSize = true;
            this.cbTo.Location = new System.Drawing.Point(10, 42);
            this.cbTo.Name = "cbTo";
            this.cbTo.Size = new System.Drawing.Size(64, 17);
            this.cbTo.TabIndex = 2;
            this.cbTo.Text = "Sent To";
            this.cbTo.UseVisualStyleBackColor = true;
            // 
            // tbSentFrom
            // 
            this.tbSentFrom.Location = new System.Drawing.Point(140, 14);
            this.tbSentFrom.Name = "tbSentFrom";
            this.tbSentFrom.Size = new System.Drawing.Size(223, 20);
            this.tbSentFrom.TabIndex = 1;
            // 
            // cbFrom
            // 
            this.cbFrom.AutoSize = true;
            this.cbFrom.Location = new System.Drawing.Point(11, 17);
            this.cbFrom.Name = "cbFrom";
            this.cbFrom.Size = new System.Drawing.Size(49, 17);
            this.cbFrom.TabIndex = 0;
            this.cbFrom.Text = "From";
            this.cbFrom.UseVisualStyleBackColor = true;
            // 
            // frmEmailAutoMation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(937, 503);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.tbMailCount);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.pBar);
            this.Controls.Add(this.lvInbox);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "frmEmailAutoMation";
            this.Text = "Email Automation";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rdbImap;
        private System.Windows.Forms.RadioButton rdbPop;
        private System.Windows.Forms.TextBox tbPassword;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbUserName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbHostName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnDefault;
        private System.Windows.Forms.TextBox tbPortNo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox cbIsSecure;
        private System.Windows.Forms.Button btnFetch;
        private System.Windows.Forms.ComboBox cmbFolders;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnFetchEmails;
        private System.Windows.Forms.ListView lvInbox;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ProgressBar pBar;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbMailCount;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnAdvanceView;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox cmbDate;
        private System.Windows.Forms.TextBox tbSentDate;
        private System.Windows.Forms.CheckBox cbDate;
        private System.Windows.Forms.TextBox tbBody;
        private System.Windows.Forms.CheckBox cbBody;
        private System.Windows.Forms.TextBox tbSubject;
        private System.Windows.Forms.CheckBox cbSubject;
        private System.Windows.Forms.TextBox tbSentBcc;
        private System.Windows.Forms.CheckBox cbBcc;
        private System.Windows.Forms.TextBox tbSentCc;
        private System.Windows.Forms.CheckBox cbCc;
        private System.Windows.Forms.TextBox tbSentTo;
        private System.Windows.Forms.CheckBox cbTo;
        private System.Windows.Forms.TextBox tbSentFrom;
        private System.Windows.Forms.CheckBox cbFrom;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox cmbFlag;
        private System.Windows.Forms.CheckBox cbMailFlag;
    }
}

