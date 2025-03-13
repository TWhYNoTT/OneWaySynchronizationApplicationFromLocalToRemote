namespace SynchronizationApplication
{
    partial class MainForm
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
            this.panelLocalDb = new System.Windows.Forms.Panel();
            this.btnTestLocalConnection = new System.Windows.Forms.Button();
            this.txtLocalPassword = new System.Windows.Forms.TextBox();
            this.txtLocalUsername = new System.Windows.Forms.TextBox();
            this.txtLocalDatabase = new System.Windows.Forms.TextBox();
            this.txtLocalServer = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panelRemoteDb = new System.Windows.Forms.Panel();
            this.btnTestRemoteConnection = new System.Windows.Forms.Button();
            this.txtRemotePassword = new System.Windows.Forms.TextBox();
            this.txtRemoteUsername = new System.Windows.Forms.TextBox();
            this.txtRemoteDatabase = new System.Windows.Forms.TextBox();
            this.txtRemoteServer = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.panelSync = new System.Windows.Forms.Panel();
            this.lblProgress = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.chkTruncateBeforeSync = new System.Windows.Forms.CheckBox();
            this.btnStartSync = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnExit = new System.Windows.Forms.Button();
            this.panelLocalDb.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panelRemoteDb.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.panelSync.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelLocalDb
            // 
            this.panelLocalDb.Controls.Add(this.btnTestLocalConnection);
            this.panelLocalDb.Controls.Add(this.txtLocalPassword);
            this.panelLocalDb.Controls.Add(this.txtLocalUsername);
            this.panelLocalDb.Controls.Add(this.txtLocalDatabase);
            this.panelLocalDb.Controls.Add(this.txtLocalServer);
            this.panelLocalDb.Controls.Add(this.label4);
            this.panelLocalDb.Controls.Add(this.label3);
            this.panelLocalDb.Controls.Add(this.label2);
            this.panelLocalDb.Controls.Add(this.label1);
            this.panelLocalDb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelLocalDb.Location = new System.Drawing.Point(3, 23);
            this.panelLocalDb.Name = "panelLocalDb";
            this.panelLocalDb.Size = new System.Drawing.Size(371, 161);
            this.panelLocalDb.TabIndex = 0;
            // 
            // btnTestLocalConnection
            // 
            this.btnTestLocalConnection.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTestLocalConnection.Location = new System.Drawing.Point(240, 123);
            this.btnTestLocalConnection.Name = "btnTestLocalConnection";
            this.btnTestLocalConnection.Size = new System.Drawing.Size(117, 29);
            this.btnTestLocalConnection.TabIndex = 8;
            this.btnTestLocalConnection.Text = "Test Connection";
            this.btnTestLocalConnection.UseVisualStyleBackColor = true;
            this.btnTestLocalConnection.Click += new System.EventHandler(this.btnTestLocalConnection_Click);
            // 
            // txtLocalPassword
            // 
            this.txtLocalPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLocalPassword.Location = new System.Drawing.Point(117, 93);
            this.txtLocalPassword.Name = "txtLocalPassword";
            this.txtLocalPassword.PasswordChar = '*';
            this.txtLocalPassword.Size = new System.Drawing.Size(240, 27);
            this.txtLocalPassword.TabIndex = 7;
            // 
            // txtLocalUsername
            // 
            this.txtLocalUsername.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLocalUsername.Location = new System.Drawing.Point(117, 63);
            this.txtLocalUsername.Name = "txtLocalUsername";
            this.txtLocalUsername.Size = new System.Drawing.Size(240, 27);
            this.txtLocalUsername.TabIndex = 6;
            // 
            // txtLocalDatabase
            // 
            this.txtLocalDatabase.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLocalDatabase.Location = new System.Drawing.Point(117, 33);
            this.txtLocalDatabase.Name = "txtLocalDatabase";
            this.txtLocalDatabase.Size = new System.Drawing.Size(240, 27);
            this.txtLocalDatabase.TabIndex = 5;
            // 
            // txtLocalServer
            // 
            this.txtLocalServer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLocalServer.Location = new System.Drawing.Point(117, 3);
            this.txtLocalServer.Name = "txtLocalServer";
            this.txtLocalServer.Size = new System.Drawing.Size(240, 27);
            this.txtLocalServer.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 96);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(70, 20);
            this.label4.TabIndex = 3;
            this.label4.Text = "Password";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 66);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 20);
            this.label3.TabIndex = 2;
            this.label3.Text = "Username";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 20);
            this.label2.TabIndex = 1;
            this.label2.Text = "Database";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Server";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.panelLocalDb);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(377, 187);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Local Database";
            // 
            // panelRemoteDb
            // 
            this.panelRemoteDb.Controls.Add(this.btnTestRemoteConnection);
            this.panelRemoteDb.Controls.Add(this.txtRemotePassword);
            this.panelRemoteDb.Controls.Add(this.txtRemoteUsername);
            this.panelRemoteDb.Controls.Add(this.txtRemoteDatabase);
            this.panelRemoteDb.Controls.Add(this.txtRemoteServer);
            this.panelRemoteDb.Controls.Add(this.label5);
            this.panelRemoteDb.Controls.Add(this.label6);
            this.panelRemoteDb.Controls.Add(this.label7);
            this.panelRemoteDb.Controls.Add(this.label8);
            this.panelRemoteDb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelRemoteDb.Location = new System.Drawing.Point(3, 23);
            this.panelRemoteDb.Name = "panelRemoteDb";
            this.panelRemoteDb.Size = new System.Drawing.Size(371, 161);
            this.panelRemoteDb.TabIndex = 0;
            // 
            // btnTestRemoteConnection
            // 
            this.btnTestRemoteConnection.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTestRemoteConnection.Location = new System.Drawing.Point(240, 123);
            this.btnTestRemoteConnection.Name = "btnTestRemoteConnection";
            this.btnTestRemoteConnection.Size = new System.Drawing.Size(117, 29);
            this.btnTestRemoteConnection.TabIndex = 8;
            this.btnTestRemoteConnection.Text = "Test Connection";
            this.btnTestRemoteConnection.UseVisualStyleBackColor = true;
            this.btnTestRemoteConnection.Click += new System.EventHandler(this.btnTestRemoteConnection_Click);
            // 
            // txtRemotePassword
            // 
            this.txtRemotePassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRemotePassword.Location = new System.Drawing.Point(117, 93);
            this.txtRemotePassword.Name = "txtRemotePassword";
            this.txtRemotePassword.PasswordChar = '*';
            this.txtRemotePassword.Size = new System.Drawing.Size(240, 27);
            this.txtRemotePassword.TabIndex = 7;
            // 
            // txtRemoteUsername
            // 
            this.txtRemoteUsername.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRemoteUsername.Location = new System.Drawing.Point(117, 63);
            this.txtRemoteUsername.Name = "txtRemoteUsername";
            this.txtRemoteUsername.Size = new System.Drawing.Size(240, 27);
            this.txtRemoteUsername.TabIndex = 6;
            // 
            // txtRemoteDatabase
            // 
            this.txtRemoteDatabase.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRemoteDatabase.Location = new System.Drawing.Point(117, 33);
            this.txtRemoteDatabase.Name = "txtRemoteDatabase";
            this.txtRemoteDatabase.Size = new System.Drawing.Size(240, 27);
            this.txtRemoteDatabase.TabIndex = 5;
            // 
            // txtRemoteServer
            // 
            this.txtRemoteServer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRemoteServer.Location = new System.Drawing.Point(117, 3);
            this.txtRemoteServer.Name = "txtRemoteServer";
            this.txtRemoteServer.Size = new System.Drawing.Size(240, 27);
            this.txtRemoteServer.TabIndex = 4;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 96);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(70, 20);
            this.label5.TabIndex = 3;
            this.label5.Text = "Password";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(14, 66);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(75, 20);
            this.label6.TabIndex = 2;
            this.label6.Text = "Username";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(14, 36);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(72, 20);
            this.label7.TabIndex = 1;
            this.label7.Text = "Database";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(14, 6);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(50, 20);
            this.label8.TabIndex = 0;
            this.label8.Text = "Server";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.panelRemoteDb);
            this.groupBox2.Location = new System.Drawing.Point(410, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(377, 187);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Remote Database";
            // 
            // panelSync
            // 
            this.panelSync.Controls.Add(this.lblProgress);
            this.panelSync.Controls.Add(this.lblStatus);
            this.panelSync.Controls.Add(this.progressBar);
            this.panelSync.Controls.Add(this.chkTruncateBeforeSync);
            this.panelSync.Controls.Add(this.btnStartSync);
            this.panelSync.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelSync.Location = new System.Drawing.Point(3, 23);
            this.panelSync.Name = "panelSync";
            this.panelSync.Size = new System.Drawing.Size(769, 120);
            this.panelSync.TabIndex = 0;
            // 
            // lblProgress
            // 
            this.lblProgress.AutoSize = true;
            this.lblProgress.Location = new System.Drawing.Point(22, 89);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(69, 20);
            this.lblProgress.TabIndex = 4;
            this.lblProgress.Text = "Progress:";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(22, 32);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(125, 20);
            this.lblStatus.TabIndex = 3;
            this.lblStatus.Text = "Ready to sync...";
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(22, 55);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(728, 29);
            this.progressBar.TabIndex = 2;
            // 
            // chkTruncateBeforeSync
            // 
            this.chkTruncateBeforeSync.AutoSize = true;
            this.chkTruncateBeforeSync.Location = new System.Drawing.Point(22, 5);
            this.chkTruncateBeforeSync.Name = "chkTruncateBeforeSync";
            this.chkTruncateBeforeSync.Size = new System.Drawing.Size(298, 24);
            this.chkTruncateBeforeSync.TabIndex = 1;
            this.chkTruncateBeforeSync.Text = "Truncate remote table before syncing";
            this.chkTruncateBeforeSync.UseVisualStyleBackColor = true;
            // 
            // btnStartSync
            // 
            this.btnStartSync.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStartSync.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnStartSync.ForeColor = System.Drawing.Color.Green;
            this.btnStartSync.Location = new System.Drawing.Point(613, 5);
            this.btnStartSync.Name = "btnStartSync";
            this.btnStartSync.Size = new System.Drawing.Size(137, 29);
            this.btnStartSync.TabIndex = 0;
            this.btnStartSync.Text = "Start Sync";
            this.btnStartSync.UseVisualStyleBackColor = true;
            this.btnStartSync.Click += new System.EventHandler(this.btnStartSync_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.panelSync);
            this.groupBox3.Location = new System.Drawing.Point(12, 205);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(775, 146);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Synchronization";
            // 
            // btnExit
            // 
            this.btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExit.Location = new System.Drawing.Point(650, 357);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(137, 29);
            this.btnExit.TabIndex = 4;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 398);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Customer Database Synchronization";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.panelLocalDb.ResumeLayout(false);
            this.panelLocalDb.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.panelRemoteDb.ResumeLayout(false);
            this.panelRemoteDb.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.panelSync.ResumeLayout(false);
            this.panelSync.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

            // Call extension method
            InitializeComponentExtension();
        }

        #endregion

        private Panel panelLocalDb;
        private TextBox txtLocalPassword;
        private TextBox txtLocalUsername;
        private TextBox txtLocalDatabase;
        private TextBox txtLocalServer;
        private Label label4;
        private Label label3;
        private Label label2;
        private Label label1;
        private GroupBox groupBox1;
        private Button btnTestLocalConnection;
        private Panel panelRemoteDb;
        private Button btnTestRemoteConnection;
        private TextBox txtRemotePassword;
        private TextBox txtRemoteUsername;
        private TextBox txtRemoteDatabase;
        private TextBox txtRemoteServer;
        private Label label5;
        private Label label6;
        private Label label7;
        private Label label8;
        private GroupBox groupBox2;
        private Panel panelSync;
        private Button btnStartSync;
        private GroupBox groupBox3;
        private CheckBox chkTruncateBeforeSync;
        private ProgressBar progressBar;
        private Label lblStatus;
        private Label lblProgress;
        private Button btnExit;

        partial void InitializeComponentExtension();
    }
}