namespace EmailAutomationSample
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.btnFetchEmails = new System.Windows.Forms.Button();
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
            this.pBar = new System.Windows.Forms.ProgressBar();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtDetail = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.btnFetchEmails);
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
            this.groupBox1.Location = new System.Drawing.Point(5, -1);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(404, 204);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(309, 173);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(89, 23);
            this.button1.TabIndex = 20;
            this.button1.Text = "Show Detail >>";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnFetchEmails
            // 
            this.btnFetchEmails.Location = new System.Drawing.Point(9, 173);
            this.btnFetchEmails.Name = "btnFetchEmails";
            this.btnFetchEmails.Size = new System.Drawing.Size(83, 23);
            this.btnFetchEmails.TabIndex = 15;
            this.btnFetchEmails.Text = "Fetch Emails";
            this.btnFetchEmails.UseVisualStyleBackColor = true;
            this.btnFetchEmails.Click += new System.EventHandler(this.btnFetchEmails_Click);
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
            // pBar
            // 
            this.pBar.Location = new System.Drawing.Point(5, 234);
            this.pBar.Name = "pBar";
            this.pBar.Size = new System.Drawing.Size(401, 11);
            this.pBar.TabIndex = 3;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 212);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(0, 13);
            this.label6.TabIndex = 4;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txtDetail);
            this.groupBox3.Location = new System.Drawing.Point(415, -1);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(423, 246);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Details";
            // 
            // txtDetail
            // 
            this.txtDetail.BackColor = System.Drawing.SystemColors.WindowText;
            this.txtDetail.ForeColor = System.Drawing.SystemColors.Window;
            this.txtDetail.Location = new System.Drawing.Point(6, 17);
            this.txtDetail.Multiline = true;
            this.txtDetail.Name = "txtDetail";
            this.txtDetail.ReadOnly = true;
            this.txtDetail.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDetail.Size = new System.Drawing.Size(411, 221);
            this.txtDetail.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(413, 249);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.pBar);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
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
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnFetchEmails;
        private System.Windows.Forms.Button btnDefault;
        private System.Windows.Forms.TextBox tbPortNo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox cbIsSecure;
        private System.Windows.Forms.RadioButton rdbImap;
        private System.Windows.Forms.RadioButton rdbPop;
        private System.Windows.Forms.TextBox tbPassword;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbUserName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbHostName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ProgressBar pBar;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox txtDetail;

    }
}

