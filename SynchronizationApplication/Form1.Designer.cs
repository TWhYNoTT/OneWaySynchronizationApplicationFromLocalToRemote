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
                _systemTrayManager?.Dispose();
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
            components = new System.ComponentModel.Container();
            tabControl = new TabControl();
            tabConnections = new TabPage();
            groupBoxRecordCounts = new GroupBox();
            lblRemoteUserCount = new Label();
            lblLocalUserCount = new Label();
            btnRefreshCounts = new Button();
            lblRemoteCustomerCount = new Label();
            lblLocalCustomerCount = new Label();
            groupBoxRemote = new GroupBox();
            lblRemoteHelp = new Label();
            cboRemoteDbType = new ComboBox();
            lblRemoteDbType = new Label();
            btnTestRemoteConnection = new Button();
            txtRemotePassword = new TextBox();
            txtRemoteUsername = new TextBox();
            txtRemoteDatabase = new TextBox();
            txtRemoteServer = new TextBox();
            label5 = new Label();
            label6 = new Label();
            label7 = new Label();
            label8 = new Label();
            pnlRemoteStatus = new Panel();
            groupBoxLocal = new GroupBox();
            lblLocalHelp = new Label();
            cboLocalDbType = new ComboBox();
            lblLocalDbType = new Label();
            btnTestLocalConnection = new Button();
            txtLocalPassword = new TextBox();
            txtLocalUsername = new TextBox();
            txtLocalDatabase = new TextBox();
            txtLocalServer = new TextBox();
            label4 = new Label();
            label3 = new Label();
            label2 = new Label();
            label1 = new Label();
            pnlLocalStatus = new Panel();
            tabSettings = new TabPage();
            groupBoxAppSettings = new GroupBox();
            chkAutoSyncOnStartup = new CheckBox();
            chkSyncUsers = new CheckBox();
            chkSyncCustomers = new CheckBox();
            chkStartMinimized = new CheckBox();
            chkMinimizeToTray = new CheckBox();
            chkRunAtStartup = new CheckBox();
            btnSaveSettings = new Button();
            tabSyncCustomer = new TabPage();
            lblProgressCustomerSync = new Label();
            lblCustomerSyncStatus = new Label();
            progressBarCustomerSync = new ProgressBar();
            chkTruncateBeforeCustomerSync = new CheckBox();
            btnStartCustomerSync = new Button();
            tabSyncUser = new TabPage();
            lblProgressUserSync = new Label();
            lblUserSyncStatus = new Label();
            progressBarUserSync = new ProgressBar();
            chkTruncateBeforeUserSync = new CheckBox();
            btnStartUserSync = new Button();
            tabResumeCustomer = new TabPage();
            lblProgressCustomerResume = new Label();
            lblCustomerResumeStatus = new Label();
            progressBarCustomerResume = new ProgressBar();
            btnResetCustomerSync = new Button();
            btnResumeCustomerSync = new Button();
            tabResumeUser = new TabPage();
            lblProgressUserResume = new Label();
            lblUserResumeStatus = new Label();
            progressBarUserResume = new ProgressBar();
            btnResetUserSync = new Button();
            btnResumeUserSync = new Button();
            tabFailedCustomer = new TabPage();
            lblCustomerFailedStatus = new Label();
            btnRetryFailedCustomers = new Button();
            tabFailedUser = new TabPage();
            lblUserFailedStatus = new Label();
            btnRetryFailedUsers = new Button();
            tabAutoSync = new TabPage();
            groupBoxAutoSyncLog = new GroupBox();
            txtAutoSyncLog = new TextBox();
            groupBoxAutoSyncSettings = new GroupBox();
            lblAutoSyncStatusCounts = new Label();
            btnViewFailedAutoSync = new Button();
            lblLastSync = new Label();
            chkSyncDeletes = new CheckBox();
            label11 = new Label();
            numSyncInterval = new NumericUpDown();
            label10 = new Label();
            btnToggleAutoSync = new Button();
            lblAutoSyncStatus = new Label();
            progressAutoSync = new ProgressBar();
            tabTriggers = new TabPage();
            groupBoxEntitySelection = new GroupBox();
            rbUser = new RadioButton();
            rbCustomer = new RadioButton();
            groupBoxTriggerStatus = new GroupBox();
            lblTriggerStatus = new Label();
            btnCheckTriggers = new Button();
            groupBoxTriggerOperations = new GroupBox();
            btnRemoveTriggers = new Button();
            btnCreateTriggers = new Button();
            txtTriggerDetails = new TextBox();
            btnExit = new Button();
            timerAutoSync = new System.Windows.Forms.Timer(components);
            statusStrip = new StatusStrip();
            statusLabel = new ToolStripStatusLabel();
            btnHideToTray = new Button();
            tabControl.SuspendLayout();
            tabConnections.SuspendLayout();
            groupBoxRecordCounts.SuspendLayout();
            groupBoxRemote.SuspendLayout();
            groupBoxLocal.SuspendLayout();
            tabSettings.SuspendLayout();
            groupBoxAppSettings.SuspendLayout();
            tabSyncCustomer.SuspendLayout();
            tabSyncUser.SuspendLayout();
            tabResumeCustomer.SuspendLayout();
            tabResumeUser.SuspendLayout();
            tabFailedCustomer.SuspendLayout();
            tabFailedUser.SuspendLayout();
            tabAutoSync.SuspendLayout();
            groupBoxAutoSyncLog.SuspendLayout();
            groupBoxAutoSyncSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numSyncInterval).BeginInit();
            tabTriggers.SuspendLayout();
            groupBoxEntitySelection.SuspendLayout();
            groupBoxTriggerStatus.SuspendLayout();
            groupBoxTriggerOperations.SuspendLayout();
            statusStrip.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl
            // 
            tabControl.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabControl.Controls.Add(tabConnections);
            tabControl.Controls.Add(tabSettings);
            tabControl.Controls.Add(tabSyncCustomer);
            tabControl.Controls.Add(tabSyncUser);
            tabControl.Controls.Add(tabResumeCustomer);
            tabControl.Controls.Add(tabResumeUser);
            tabControl.Controls.Add(tabFailedCustomer);
            tabControl.Controls.Add(tabFailedUser);
            tabControl.Controls.Add(tabAutoSync);
            tabControl.Controls.Add(tabTriggers);
            tabControl.Location = new Point(10, 9);
            tabControl.Margin = new Padding(3, 2, 3, 2);
            tabControl.Name = "tabControl";
            tabControl.SelectedIndex = 0;
            tabControl.Size = new Size(831, 311);
            tabControl.TabIndex = 0;
            // 
            // tabConnections
            // 
            tabConnections.Controls.Add(groupBoxRecordCounts);
            tabConnections.Controls.Add(groupBoxRemote);
            tabConnections.Controls.Add(groupBoxLocal);
            tabConnections.Location = new Point(4, 24);
            tabConnections.Margin = new Padding(3, 2, 3, 2);
            tabConnections.Name = "tabConnections";
            tabConnections.Padding = new Padding(3, 2, 3, 2);
            tabConnections.Size = new Size(823, 283);
            tabConnections.TabIndex = 0;
            tabConnections.Text = "Connections";
            tabConnections.UseVisualStyleBackColor = true;
            // 
            // groupBoxRecordCounts
            // 
            groupBoxRecordCounts.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBoxRecordCounts.Controls.Add(lblRemoteUserCount);
            groupBoxRecordCounts.Controls.Add(lblLocalUserCount);
            groupBoxRecordCounts.Controls.Add(btnRefreshCounts);
            groupBoxRecordCounts.Controls.Add(lblRemoteCustomerCount);
            groupBoxRecordCounts.Controls.Add(lblLocalCustomerCount);
            groupBoxRecordCounts.Location = new Point(5, 224);
            groupBoxRecordCounts.Margin = new Padding(3, 2, 3, 2);
            groupBoxRecordCounts.Name = "groupBoxRecordCounts";
            groupBoxRecordCounts.Padding = new Padding(3, 2, 3, 2);
            groupBoxRecordCounts.Size = new Size(814, 58);
            groupBoxRecordCounts.TabIndex = 2;
            groupBoxRecordCounts.TabStop = false;
            groupBoxRecordCounts.Text = "Record Counts";
            // 
            // lblRemoteUserCount
            // 
            lblRemoteUserCount.AutoSize = true;
            lblRemoteUserCount.Location = new Point(328, 30);
            lblRemoteUserCount.Name = "lblRemoteUserCount";
            lblRemoteUserCount.Size = new Size(115, 15);
            lblRemoteUserCount.TabIndex = 4;
            lblRemoteUserCount.Text = "Remote Users: (N/A)";
            // 
            // lblLocalUserCount
            // 
            lblLocalUserCount.AutoSize = true;
            lblLocalUserCount.Location = new Point(16, 34);
            lblLocalUserCount.Name = "lblLocalUserCount";
            lblLocalUserCount.Size = new Size(102, 15);
            lblLocalUserCount.TabIndex = 3;
            lblLocalUserCount.Text = "Local Users: (N/A)";
            // 
            // btnRefreshCounts
            // 
            btnRefreshCounts.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRefreshCounts.Location = new Point(703, 17);
            btnRefreshCounts.Margin = new Padding(3, 2, 3, 2);
            btnRefreshCounts.Name = "btnRefreshCounts";
            btnRefreshCounts.Size = new Size(105, 22);
            btnRefreshCounts.TabIndex = 2;
            btnRefreshCounts.Text = "Refresh Counts";
            btnRefreshCounts.UseVisualStyleBackColor = true;
            btnRefreshCounts.Click += btnRefreshCounts_Click;
            // 
            // lblRemoteCustomerCount
            // 
            lblRemoteCustomerCount.AutoSize = true;
            lblRemoteCustomerCount.Location = new Point(328, 15);
            lblRemoteCustomerCount.Name = "lblRemoteCustomerCount";
            lblRemoteCustomerCount.Size = new Size(144, 15);
            lblRemoteCustomerCount.TabIndex = 1;
            lblRemoteCustomerCount.Text = "Remote Customers: (N/A)";
            // 
            // lblLocalCustomerCount
            // 
            lblLocalCustomerCount.AutoSize = true;
            lblLocalCustomerCount.Location = new Point(16, 15);
            lblLocalCustomerCount.Name = "lblLocalCustomerCount";
            lblLocalCustomerCount.Size = new Size(131, 15);
            lblLocalCustomerCount.TabIndex = 0;
            lblLocalCustomerCount.Text = "Local Customers: (N/A)";
            // 
            // groupBoxRemote
            // 
            groupBoxRemote.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBoxRemote.Controls.Add(lblRemoteHelp);
            groupBoxRemote.Controls.Add(cboRemoteDbType);
            groupBoxRemote.Controls.Add(lblRemoteDbType);
            groupBoxRemote.Controls.Add(btnTestRemoteConnection);
            groupBoxRemote.Controls.Add(txtRemotePassword);
            groupBoxRemote.Controls.Add(txtRemoteUsername);
            groupBoxRemote.Controls.Add(txtRemoteDatabase);
            groupBoxRemote.Controls.Add(txtRemoteServer);
            groupBoxRemote.Controls.Add(label5);
            groupBoxRemote.Controls.Add(label6);
            groupBoxRemote.Controls.Add(label7);
            groupBoxRemote.Controls.Add(label8);
            groupBoxRemote.Controls.Add(pnlRemoteStatus);
            groupBoxRemote.Location = new Point(407, 4);
            groupBoxRemote.Margin = new Padding(3, 2, 3, 2);
            groupBoxRemote.Name = "groupBoxRemote";
            groupBoxRemote.Padding = new Padding(3, 2, 3, 2);
            groupBoxRemote.Size = new Size(412, 215);
            groupBoxRemote.TabIndex = 1;
            groupBoxRemote.TabStop = false;
            groupBoxRemote.Text = "Remote Database";
            // 
            // lblRemoteHelp
            // 
            lblRemoteHelp.AutoSize = true;
            lblRemoteHelp.Font = new Font("Segoe UI", 8F, FontStyle.Italic);
            lblRemoteHelp.Location = new Point(13, 135);
            lblRemoteHelp.Name = "lblRemoteHelp";
            lblRemoteHelp.Size = new Size(0, 13);
            lblRemoteHelp.TabIndex = 20;
            // 
            // cboRemoteDbType
            // 
            cboRemoteDbType.DropDownStyle = ComboBoxStyle.DropDownList;
            cboRemoteDbType.FormattingEnabled = true;
            cboRemoteDbType.Location = new Point(95, 126);
            cboRemoteDbType.Margin = new Padding(3, 2, 3, 2);
            cboRemoteDbType.Name = "cboRemoteDbType";
            cboRemoteDbType.Size = new Size(125, 23);
            cboRemoteDbType.TabIndex = 19;
            // 
            // lblRemoteDbType
            // 
            lblRemoteDbType.AutoSize = true;
            lblRemoteDbType.Location = new Point(12, 128);
            lblRemoteDbType.Name = "lblRemoteDbType";
            lblRemoteDbType.Size = new Size(34, 15);
            lblRemoteDbType.TabIndex = 18;
            lblRemoteDbType.Text = "Type:";
            // 
            // btnTestRemoteConnection
            // 
            btnTestRemoteConnection.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnTestRemoteConnection.Location = new Point(301, 150);
            btnTestRemoteConnection.Margin = new Padding(3, 2, 3, 2);
            btnTestRemoteConnection.Name = "btnTestRemoteConnection";
            btnTestRemoteConnection.Size = new Size(105, 22);
            btnTestRemoteConnection.TabIndex = 17;
            btnTestRemoteConnection.Text = "Test Connection";
            btnTestRemoteConnection.UseVisualStyleBackColor = true;
            btnTestRemoteConnection.Click += btnTestRemoteConnection_Click;
            // 
            // txtRemotePassword
            // 
            txtRemotePassword.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtRemotePassword.Location = new Point(95, 101);
            txtRemotePassword.Margin = new Padding(3, 2, 3, 2);
            txtRemotePassword.Name = "txtRemotePassword";
            txtRemotePassword.PasswordChar = '*';
            txtRemotePassword.Size = new Size(311, 23);
            txtRemotePassword.TabIndex = 16;
            // 
            // txtRemoteUsername
            // 
            txtRemoteUsername.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtRemoteUsername.Location = new Point(95, 76);
            txtRemoteUsername.Margin = new Padding(3, 2, 3, 2);
            txtRemoteUsername.Name = "txtRemoteUsername";
            txtRemoteUsername.Size = new Size(311, 23);
            txtRemoteUsername.TabIndex = 15;
            // 
            // txtRemoteDatabase
            // 
            txtRemoteDatabase.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtRemoteDatabase.Location = new Point(95, 52);
            txtRemoteDatabase.Margin = new Padding(3, 2, 3, 2);
            txtRemoteDatabase.Name = "txtRemoteDatabase";
            txtRemoteDatabase.Size = new Size(311, 23);
            txtRemoteDatabase.TabIndex = 14;
            // 
            // txtRemoteServer
            // 
            txtRemoteServer.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtRemoteServer.Location = new Point(95, 27);
            txtRemoteServer.Margin = new Padding(3, 2, 3, 2);
            txtRemoteServer.Name = "txtRemoteServer";
            txtRemoteServer.Size = new Size(311, 23);
            txtRemoteServer.TabIndex = 13;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(12, 104);
            label5.Name = "label5";
            label5.Size = new Size(60, 15);
            label5.TabIndex = 12;
            label5.Text = "Password:";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(12, 79);
            label6.Name = "label6";
            label6.Size = new Size(63, 15);
            label6.TabIndex = 11;
            label6.Text = "Username:";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(12, 54);
            label7.Name = "label7";
            label7.Size = new Size(58, 15);
            label7.TabIndex = 10;
            label7.Text = "Database:";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(12, 29);
            label8.Name = "label8";
            label8.Size = new Size(42, 15);
            label8.TabIndex = 9;
            label8.Text = "Server:";
            // 
            // pnlRemoteStatus
            // 
            pnlRemoteStatus.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pnlRemoteStatus.BackColor = Color.Gray;
            pnlRemoteStatus.BorderStyle = BorderStyle.FixedSingle;
            pnlRemoteStatus.Location = new Point(394, 15);
            pnlRemoteStatus.Margin = new Padding(3, 2, 3, 2);
            pnlRemoteStatus.Name = "pnlRemoteStatus";
            pnlRemoteStatus.Size = new Size(12, 11);
            pnlRemoteStatus.TabIndex = 22;
            // 
            // groupBoxLocal
            // 
            groupBoxLocal.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            groupBoxLocal.Controls.Add(lblLocalHelp);
            groupBoxLocal.Controls.Add(cboLocalDbType);
            groupBoxLocal.Controls.Add(lblLocalDbType);
            groupBoxLocal.Controls.Add(btnTestLocalConnection);
            groupBoxLocal.Controls.Add(txtLocalPassword);
            groupBoxLocal.Controls.Add(txtLocalUsername);
            groupBoxLocal.Controls.Add(txtLocalDatabase);
            groupBoxLocal.Controls.Add(txtLocalServer);
            groupBoxLocal.Controls.Add(label4);
            groupBoxLocal.Controls.Add(label3);
            groupBoxLocal.Controls.Add(label2);
            groupBoxLocal.Controls.Add(label1);
            groupBoxLocal.Controls.Add(pnlLocalStatus);
            groupBoxLocal.Location = new Point(5, 4);
            groupBoxLocal.Margin = new Padding(3, 2, 3, 2);
            groupBoxLocal.Name = "groupBoxLocal";
            groupBoxLocal.Padding = new Padding(3, 2, 3, 2);
            groupBoxLocal.Size = new Size(401, 215);
            groupBoxLocal.TabIndex = 0;
            groupBoxLocal.TabStop = false;
            groupBoxLocal.Text = "Local Database";
            // 
            // lblLocalHelp
            // 
            lblLocalHelp.AutoSize = true;
            lblLocalHelp.Font = new Font("Segoe UI", 8F, FontStyle.Italic);
            lblLocalHelp.Location = new Point(13, 135);
            lblLocalHelp.Name = "lblLocalHelp";
            lblLocalHelp.Size = new Size(0, 13);
            lblLocalHelp.TabIndex = 11;
            // 
            // cboLocalDbType
            // 
            cboLocalDbType.DropDownStyle = ComboBoxStyle.DropDownList;
            cboLocalDbType.FormattingEnabled = true;
            cboLocalDbType.Location = new Point(96, 123);
            cboLocalDbType.Margin = new Padding(3, 2, 3, 2);
            cboLocalDbType.Name = "cboLocalDbType";
            cboLocalDbType.Size = new Size(125, 23);
            cboLocalDbType.TabIndex = 10;
            // 
            // lblLocalDbType
            // 
            lblLocalDbType.AutoSize = true;
            lblLocalDbType.Location = new Point(13, 125);
            lblLocalDbType.Name = "lblLocalDbType";
            lblLocalDbType.Size = new Size(34, 15);
            lblLocalDbType.TabIndex = 9;
            lblLocalDbType.Text = "Type:";
            // 
            // btnTestLocalConnection
            // 
            btnTestLocalConnection.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnTestLocalConnection.Location = new Point(296, 150);
            btnTestLocalConnection.Margin = new Padding(3, 2, 3, 2);
            btnTestLocalConnection.Name = "btnTestLocalConnection";
            btnTestLocalConnection.Size = new Size(105, 22);
            btnTestLocalConnection.TabIndex = 8;
            btnTestLocalConnection.Text = "Test Connection";
            btnTestLocalConnection.UseVisualStyleBackColor = true;
            btnTestLocalConnection.Click += btnTestLocalConnection_Click;
            // 
            // txtLocalPassword
            // 
            txtLocalPassword.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtLocalPassword.Location = new Point(96, 98);
            txtLocalPassword.Margin = new Padding(3, 2, 3, 2);
            txtLocalPassword.Name = "txtLocalPassword";
            txtLocalPassword.PasswordChar = '*';
            txtLocalPassword.Size = new Size(300, 23);
            txtLocalPassword.TabIndex = 7;
            // 
            // txtLocalUsername
            // 
            txtLocalUsername.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtLocalUsername.Location = new Point(96, 73);
            txtLocalUsername.Margin = new Padding(3, 2, 3, 2);
            txtLocalUsername.Name = "txtLocalUsername";
            txtLocalUsername.Size = new Size(300, 23);
            txtLocalUsername.TabIndex = 6;
            // 
            // txtLocalDatabase
            // 
            txtLocalDatabase.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtLocalDatabase.Location = new Point(96, 49);
            txtLocalDatabase.Margin = new Padding(3, 2, 3, 2);
            txtLocalDatabase.Name = "txtLocalDatabase";
            txtLocalDatabase.Size = new Size(300, 23);
            txtLocalDatabase.TabIndex = 5;
            // 
            // txtLocalServer
            // 
            txtLocalServer.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtLocalServer.Location = new Point(96, 24);
            txtLocalServer.Margin = new Padding(3, 2, 3, 2);
            txtLocalServer.Name = "txtLocalServer";
            txtLocalServer.Size = new Size(300, 23);
            txtLocalServer.TabIndex = 4;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(13, 101);
            label4.Name = "label4";
            label4.Size = new Size(60, 15);
            label4.TabIndex = 3;
            label4.Text = "Password:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(13, 76);
            label3.Name = "label3";
            label3.Size = new Size(63, 15);
            label3.TabIndex = 2;
            label3.Text = "Username:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(13, 51);
            label2.Name = "label2";
            label2.Size = new Size(58, 15);
            label2.TabIndex = 1;
            label2.Text = "Database:";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(13, 26);
            label1.Name = "label1";
            label1.Size = new Size(42, 15);
            label1.TabIndex = 0;
            label1.Text = "Server:";
            // 
            // pnlLocalStatus
            // 
            pnlLocalStatus.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pnlLocalStatus.BackColor = Color.Gray;
            pnlLocalStatus.BorderStyle = BorderStyle.FixedSingle;
            pnlLocalStatus.Location = new Point(383, 15);
            pnlLocalStatus.Margin = new Padding(3, 2, 3, 2);
            pnlLocalStatus.Name = "pnlLocalStatus";
            pnlLocalStatus.Size = new Size(12, 11);
            pnlLocalStatus.TabIndex = 21;
            // 
            // tabSettings
            // 
            tabSettings.Controls.Add(groupBoxAppSettings);
            tabSettings.Location = new Point(4, 24);
            tabSettings.Margin = new Padding(3, 2, 3, 2);
            tabSettings.Name = "tabSettings";
            tabSettings.Padding = new Padding(3, 2, 3, 2);
            tabSettings.Size = new Size(823, 283);
            tabSettings.TabIndex = 10;
            tabSettings.Text = "App Settings";
            tabSettings.UseVisualStyleBackColor = true;
            // 
            // groupBoxAppSettings
            // 
            groupBoxAppSettings.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBoxAppSettings.Controls.Add(chkAutoSyncOnStartup);
            groupBoxAppSettings.Controls.Add(chkSyncUsers);
            groupBoxAppSettings.Controls.Add(chkSyncCustomers);
            groupBoxAppSettings.Controls.Add(chkStartMinimized);
            groupBoxAppSettings.Controls.Add(chkMinimizeToTray);
            groupBoxAppSettings.Controls.Add(chkRunAtStartup);
            groupBoxAppSettings.Controls.Add(btnSaveSettings);
            groupBoxAppSettings.Location = new Point(5, 4);
            groupBoxAppSettings.Margin = new Padding(3, 2, 3, 2);
            groupBoxAppSettings.Name = "groupBoxAppSettings";
            groupBoxAppSettings.Padding = new Padding(3, 2, 3, 2);
            groupBoxAppSettings.Size = new Size(646, 220);
            groupBoxAppSettings.TabIndex = 0;
            groupBoxAppSettings.TabStop = false;
            groupBoxAppSettings.Text = "Application Settings";
            // 
            // chkAutoSyncOnStartup
            // 
            chkAutoSyncOnStartup.AutoSize = true;
            chkAutoSyncOnStartup.Location = new Point(20, 94);
            chkAutoSyncOnStartup.Margin = new Padding(3, 2, 3, 2);
            chkAutoSyncOnStartup.Name = "chkAutoSyncOnStartup";
            chkAutoSyncOnStartup.Size = new Size(190, 19);
            chkAutoSyncOnStartup.TabIndex = 6;
            chkAutoSyncOnStartup.Text = "Start auto sync when app starts";
            chkAutoSyncOnStartup.UseVisualStyleBackColor = true;
            // 
            // chkSyncUsers
            // 
            chkSyncUsers.AutoSize = true;
            chkSyncUsers.Checked = true;
            chkSyncUsers.CheckState = CheckState.Checked;
            chkSyncUsers.Location = new Point(20, 140);
            chkSyncUsers.Margin = new Padding(3, 2, 3, 2);
            chkSyncUsers.Name = "chkSyncUsers";
            chkSyncUsers.Size = new Size(121, 19);
            chkSyncUsers.TabIndex = 5;
            chkSyncUsers.Text = "Synchronize Users";
            chkSyncUsers.UseVisualStyleBackColor = true;
            // 
            // chkSyncCustomers
            // 
            chkSyncCustomers.AutoSize = true;
            chkSyncCustomers.Checked = true;
            chkSyncCustomers.CheckState = CheckState.Checked;
            chkSyncCustomers.Location = new Point(20, 117);
            chkSyncCustomers.Margin = new Padding(3, 2, 3, 2);
            chkSyncCustomers.Name = "chkSyncCustomers";
            chkSyncCustomers.Size = new Size(150, 19);
            chkSyncCustomers.TabIndex = 4;
            chkSyncCustomers.Text = "Synchronize Customers";
            chkSyncCustomers.UseVisualStyleBackColor = true;
            // 
            // chkStartMinimized
            // 
            chkStartMinimized.AutoSize = true;
            chkStartMinimized.Location = new Point(20, 72);
            chkStartMinimized.Margin = new Padding(3, 2, 3, 2);
            chkStartMinimized.Name = "chkStartMinimized";
            chkStartMinimized.Size = new Size(171, 19);
            chkStartMinimized.TabIndex = 3;
            chkStartMinimized.Text = "Start application minimized";
            chkStartMinimized.UseVisualStyleBackColor = true;
            // 
            // chkMinimizeToTray
            // 
            chkMinimizeToTray.AutoSize = true;
            chkMinimizeToTray.Checked = true;
            chkMinimizeToTray.CheckState = CheckState.Checked;
            chkMinimizeToTray.Location = new Point(20, 50);
            chkMinimizeToTray.Margin = new Padding(3, 2, 3, 2);
            chkMinimizeToTray.Name = "chkMinimizeToTray";
            chkMinimizeToTray.Size = new Size(221, 19);
            chkMinimizeToTray.TabIndex = 2;
            chkMinimizeToTray.Text = "Minimize to system tray when closed";
            chkMinimizeToTray.UseVisualStyleBackColor = true;
            // 
            // chkRunAtStartup
            // 
            chkRunAtStartup.AutoSize = true;
            chkRunAtStartup.Location = new Point(20, 27);
            chkRunAtStartup.Margin = new Padding(3, 2, 3, 2);
            chkRunAtStartup.Name = "chkRunAtStartup";
            chkRunAtStartup.Size = new Size(214, 19);
            chkRunAtStartup.TabIndex = 1;
            chkRunAtStartup.Text = "Run application at Windows startup";
            chkRunAtStartup.UseVisualStyleBackColor = true;
            // 
            // btnSaveSettings
            // 
            btnSaveSettings.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnSaveSettings.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnSaveSettings.ForeColor = Color.Green;
            btnSaveSettings.Location = new Point(519, 194);
            btnSaveSettings.Margin = new Padding(3, 2, 3, 2);
            btnSaveSettings.Name = "btnSaveSettings";
            btnSaveSettings.Size = new Size(122, 22);
            btnSaveSettings.TabIndex = 0;
            btnSaveSettings.Text = "Save Settings";
            btnSaveSettings.UseVisualStyleBackColor = true;
            btnSaveSettings.Click += btnSaveSettings_Click;
            // 
            // tabSyncCustomer
            // 
            tabSyncCustomer.Controls.Add(lblProgressCustomerSync);
            tabSyncCustomer.Controls.Add(lblCustomerSyncStatus);
            tabSyncCustomer.Controls.Add(progressBarCustomerSync);
            tabSyncCustomer.Controls.Add(chkTruncateBeforeCustomerSync);
            tabSyncCustomer.Controls.Add(btnStartCustomerSync);
            tabSyncCustomer.Location = new Point(4, 24);
            tabSyncCustomer.Margin = new Padding(3, 2, 3, 2);
            tabSyncCustomer.Name = "tabSyncCustomer";
            tabSyncCustomer.Padding = new Padding(3, 2, 3, 2);
            tabSyncCustomer.Size = new Size(823, 283);
            tabSyncCustomer.TabIndex = 2;
            tabSyncCustomer.Text = "Sync Customers";
            tabSyncCustomer.UseVisualStyleBackColor = true;
            // 
            // lblProgressCustomerSync
            // 
            lblProgressCustomerSync.AutoSize = true;
            lblProgressCustomerSync.Location = new Point(19, 174);
            lblProgressCustomerSync.Name = "lblProgressCustomerSync";
            lblProgressCustomerSync.Size = new Size(55, 15);
            lblProgressCustomerSync.TabIndex = 9;
            lblProgressCustomerSync.Text = "Progress:";
            // 
            // lblCustomerSyncStatus
            // 
            lblCustomerSyncStatus.AutoSize = true;
            lblCustomerSyncStatus.Location = new Point(19, 86);
            lblCustomerSyncStatus.Name = "lblCustomerSyncStatus";
            lblCustomerSyncStatus.Size = new Size(89, 15);
            lblCustomerSyncStatus.TabIndex = 8;
            lblCustomerSyncStatus.Text = "Ready to sync...";
            // 
            // progressBarCustomerSync
            // 
            progressBarCustomerSync.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            progressBarCustomerSync.Location = new Point(19, 118);
            progressBarCustomerSync.Margin = new Padding(3, 2, 3, 2);
            progressBarCustomerSync.Name = "progressBarCustomerSync";
            progressBarCustomerSync.Size = new Size(619, 22);
            progressBarCustomerSync.TabIndex = 7;
            // 
            // chkTruncateBeforeCustomerSync
            // 
            chkTruncateBeforeCustomerSync.AutoSize = true;
            chkTruncateBeforeCustomerSync.Location = new Point(19, 13);
            chkTruncateBeforeCustomerSync.Margin = new Padding(3, 2, 3, 2);
            chkTruncateBeforeCustomerSync.Name = "chkTruncateBeforeCustomerSync";
            chkTruncateBeforeCustomerSync.Size = new Size(282, 19);
            chkTruncateBeforeCustomerSync.TabIndex = 6;
            chkTruncateBeforeCustomerSync.Text = "Truncate remote Customers table before syncing";
            chkTruncateBeforeCustomerSync.UseVisualStyleBackColor = true;
            // 
            // btnStartCustomerSync
            // 
            btnStartCustomerSync.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnStartCustomerSync.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnStartCustomerSync.ForeColor = Color.Green;
            btnStartCustomerSync.Location = new Point(518, 11);
            btnStartCustomerSync.Margin = new Padding(3, 2, 3, 2);
            btnStartCustomerSync.Name = "btnStartCustomerSync";
            btnStartCustomerSync.Size = new Size(120, 22);
            btnStartCustomerSync.TabIndex = 5;
            btnStartCustomerSync.Text = "Start Sync";
            btnStartCustomerSync.UseVisualStyleBackColor = true;
            btnStartCustomerSync.Click += btnStartCustomerSync_Click;
            // 
            // tabSyncUser
            // 
            tabSyncUser.Controls.Add(lblProgressUserSync);
            tabSyncUser.Controls.Add(lblUserSyncStatus);
            tabSyncUser.Controls.Add(progressBarUserSync);
            tabSyncUser.Controls.Add(chkTruncateBeforeUserSync);
            tabSyncUser.Controls.Add(btnStartUserSync);
            tabSyncUser.Location = new Point(4, 24);
            tabSyncUser.Margin = new Padding(3, 2, 3, 2);
            tabSyncUser.Name = "tabSyncUser";
            tabSyncUser.Padding = new Padding(3, 2, 3, 2);
            tabSyncUser.Size = new Size(823, 283);
            tabSyncUser.TabIndex = 11;
            tabSyncUser.Text = "Sync Users";
            tabSyncUser.UseVisualStyleBackColor = true;
            // 
            // lblProgressUserSync
            // 
            lblProgressUserSync.AutoSize = true;
            lblProgressUserSync.Location = new Point(19, 174);
            lblProgressUserSync.Name = "lblProgressUserSync";
            lblProgressUserSync.Size = new Size(55, 15);
            lblProgressUserSync.TabIndex = 14;
            lblProgressUserSync.Text = "Progress:";
            // 
            // lblUserSyncStatus
            // 
            lblUserSyncStatus.AutoSize = true;
            lblUserSyncStatus.Location = new Point(19, 86);
            lblUserSyncStatus.Name = "lblUserSyncStatus";
            lblUserSyncStatus.Size = new Size(89, 15);
            lblUserSyncStatus.TabIndex = 13;
            lblUserSyncStatus.Text = "Ready to sync...";
            // 
            // progressBarUserSync
            // 
            progressBarUserSync.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            progressBarUserSync.Location = new Point(19, 118);
            progressBarUserSync.Margin = new Padding(3, 2, 3, 2);
            progressBarUserSync.Name = "progressBarUserSync";
            progressBarUserSync.Size = new Size(619, 22);
            progressBarUserSync.TabIndex = 12;
            // 
            // chkTruncateBeforeUserSync
            // 
            chkTruncateBeforeUserSync.AutoSize = true;
            chkTruncateBeforeUserSync.Location = new Point(19, 13);
            chkTruncateBeforeUserSync.Margin = new Padding(3, 2, 3, 2);
            chkTruncateBeforeUserSync.Name = "chkTruncateBeforeUserSync";
            chkTruncateBeforeUserSync.Size = new Size(253, 19);
            chkTruncateBeforeUserSync.TabIndex = 11;
            chkTruncateBeforeUserSync.Text = "Truncate remote Users table before syncing";
            chkTruncateBeforeUserSync.UseVisualStyleBackColor = true;
            // 
            // btnStartUserSync
            // 
            btnStartUserSync.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnStartUserSync.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnStartUserSync.ForeColor = Color.Green;
            btnStartUserSync.Location = new Point(518, 11);
            btnStartUserSync.Margin = new Padding(3, 2, 3, 2);
            btnStartUserSync.Name = "btnStartUserSync";
            btnStartUserSync.Size = new Size(120, 22);
            btnStartUserSync.TabIndex = 10;
            btnStartUserSync.Text = "Start Sync";
            btnStartUserSync.UseVisualStyleBackColor = true;
            btnStartUserSync.Click += btnStartUserSync_Click;
            // 
            // tabResumeCustomer
            // 
            tabResumeCustomer.Controls.Add(lblProgressCustomerResume);
            tabResumeCustomer.Controls.Add(lblCustomerResumeStatus);
            tabResumeCustomer.Controls.Add(progressBarCustomerResume);
            tabResumeCustomer.Controls.Add(btnResetCustomerSync);
            tabResumeCustomer.Controls.Add(btnResumeCustomerSync);
            tabResumeCustomer.Location = new Point(4, 24);
            tabResumeCustomer.Margin = new Padding(3, 2, 3, 2);
            tabResumeCustomer.Name = "tabResumeCustomer";
            tabResumeCustomer.Padding = new Padding(3, 2, 3, 2);
            tabResumeCustomer.Size = new Size(823, 283);
            tabResumeCustomer.TabIndex = 3;
            tabResumeCustomer.Text = "Resume Customer";
            tabResumeCustomer.UseVisualStyleBackColor = true;
            // 
            // lblProgressCustomerResume
            // 
            lblProgressCustomerResume.AutoSize = true;
            lblProgressCustomerResume.Location = new Point(19, 174);
            lblProgressCustomerResume.Name = "lblProgressCustomerResume";
            lblProgressCustomerResume.Size = new Size(55, 15);
            lblProgressCustomerResume.TabIndex = 14;
            lblProgressCustomerResume.Text = "Progress:";
            // 
            // lblCustomerResumeStatus
            // 
            lblCustomerResumeStatus.AutoSize = true;
            lblCustomerResumeStatus.Location = new Point(19, 86);
            lblCustomerResumeStatus.Name = "lblCustomerResumeStatus";
            lblCustomerResumeStatus.Size = new Size(151, 15);
            lblCustomerResumeStatus.TabIndex = 13;
            lblCustomerResumeStatus.Text = "No incomplete sync found.";
            // 
            // progressBarCustomerResume
            // 
            progressBarCustomerResume.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            progressBarCustomerResume.Location = new Point(19, 118);
            progressBarCustomerResume.Margin = new Padding(3, 2, 3, 2);
            progressBarCustomerResume.Name = "progressBarCustomerResume";
            progressBarCustomerResume.Size = new Size(619, 22);
            progressBarCustomerResume.TabIndex = 12;
            // 
            // btnResetCustomerSync
            // 
            btnResetCustomerSync.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnResetCustomerSync.Location = new Point(518, 38);
            btnResetCustomerSync.Margin = new Padding(3, 2, 3, 2);
            btnResetCustomerSync.Name = "btnResetCustomerSync";
            btnResetCustomerSync.Size = new Size(120, 22);
            btnResetCustomerSync.TabIndex = 11;
            btnResetCustomerSync.Text = "Reset Sync";
            btnResetCustomerSync.UseVisualStyleBackColor = true;
            btnResetCustomerSync.Click += btnResetCustomerSync_Click;
            // 
            // btnResumeCustomerSync
            // 
            btnResumeCustomerSync.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnResumeCustomerSync.Enabled = false;
            btnResumeCustomerSync.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnResumeCustomerSync.ForeColor = Color.Blue;
            btnResumeCustomerSync.Location = new Point(518, 11);
            btnResumeCustomerSync.Margin = new Padding(3, 2, 3, 2);
            btnResumeCustomerSync.Name = "btnResumeCustomerSync";
            btnResumeCustomerSync.Size = new Size(120, 22);
            btnResumeCustomerSync.TabIndex = 10;
            btnResumeCustomerSync.Text = "Resume Sync";
            btnResumeCustomerSync.UseVisualStyleBackColor = true;
            btnResumeCustomerSync.Click += btnResumeCustomerSync_Click;
            // 
            // tabResumeUser
            // 
            tabResumeUser.Controls.Add(lblProgressUserResume);
            tabResumeUser.Controls.Add(lblUserResumeStatus);
            tabResumeUser.Controls.Add(progressBarUserResume);
            tabResumeUser.Controls.Add(btnResetUserSync);
            tabResumeUser.Controls.Add(btnResumeUserSync);
            tabResumeUser.Location = new Point(4, 24);
            tabResumeUser.Margin = new Padding(3, 2, 3, 2);
            tabResumeUser.Name = "tabResumeUser";
            tabResumeUser.Padding = new Padding(3, 2, 3, 2);
            tabResumeUser.Size = new Size(823, 283);
            tabResumeUser.TabIndex = 12;
            tabResumeUser.Text = "Resume User";
            tabResumeUser.UseVisualStyleBackColor = true;
            // 
            // lblProgressUserResume
            // 
            lblProgressUserResume.AutoSize = true;
            lblProgressUserResume.Location = new Point(19, 174);
            lblProgressUserResume.Name = "lblProgressUserResume";
            lblProgressUserResume.Size = new Size(55, 15);
            lblProgressUserResume.TabIndex = 19;
            lblProgressUserResume.Text = "Progress:";
            // 
            // lblUserResumeStatus
            // 
            lblUserResumeStatus.AutoSize = true;
            lblUserResumeStatus.Location = new Point(19, 86);
            lblUserResumeStatus.Name = "lblUserResumeStatus";
            lblUserResumeStatus.Size = new Size(151, 15);
            lblUserResumeStatus.TabIndex = 18;
            lblUserResumeStatus.Text = "No incomplete sync found.";
            // 
            // progressBarUserResume
            // 
            progressBarUserResume.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            progressBarUserResume.Location = new Point(19, 118);
            progressBarUserResume.Margin = new Padding(3, 2, 3, 2);
            progressBarUserResume.Name = "progressBarUserResume";
            progressBarUserResume.Size = new Size(619, 22);
            progressBarUserResume.TabIndex = 17;
            // 
            // btnResetUserSync
            // 
            btnResetUserSync.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnResetUserSync.Location = new Point(518, 38);
            btnResetUserSync.Margin = new Padding(3, 2, 3, 2);
            btnResetUserSync.Name = "btnResetUserSync";
            btnResetUserSync.Size = new Size(120, 22);
            btnResetUserSync.TabIndex = 16;
            btnResetUserSync.Text = "Reset Sync";
            btnResetUserSync.UseVisualStyleBackColor = true;
            btnResetUserSync.Click += btnResetUserSync_Click;
            // 
            // btnResumeUserSync
            // 
            btnResumeUserSync.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnResumeUserSync.Enabled = false;
            btnResumeUserSync.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnResumeUserSync.ForeColor = Color.Blue;
            btnResumeUserSync.Location = new Point(518, 11);
            btnResumeUserSync.Margin = new Padding(3, 2, 3, 2);
            btnResumeUserSync.Name = "btnResumeUserSync";
            btnResumeUserSync.Size = new Size(120, 22);
            btnResumeUserSync.TabIndex = 15;
            btnResumeUserSync.Text = "Resume Sync";
            btnResumeUserSync.UseVisualStyleBackColor = true;
            btnResumeUserSync.Click += btnResumeUserSync_Click;
            // 
            // tabFailedCustomer
            // 
            tabFailedCustomer.Controls.Add(lblCustomerFailedStatus);
            tabFailedCustomer.Controls.Add(btnRetryFailedCustomers);
            tabFailedCustomer.Location = new Point(4, 24);
            tabFailedCustomer.Margin = new Padding(3, 2, 3, 2);
            tabFailedCustomer.Name = "tabFailedCustomer";
            tabFailedCustomer.Padding = new Padding(3, 2, 3, 2);
            tabFailedCustomer.Size = new Size(823, 283);
            tabFailedCustomer.TabIndex = 4;
            tabFailedCustomer.Text = "Failed Customers";
            tabFailedCustomer.UseVisualStyleBackColor = true;
            // 
            // lblCustomerFailedStatus
            // 
            lblCustomerFailedStatus.AutoSize = true;
            lblCustomerFailedStatus.Location = new Point(19, 86);
            lblCustomerFailedStatus.Name = "lblCustomerFailedStatus";
            lblCustomerFailedStatus.Size = new Size(188, 15);
            lblCustomerFailedStatus.TabIndex = 14;
            lblCustomerFailedStatus.Text = "No failed customer records found.";
            // 
            // btnRetryFailedCustomers
            // 
            btnRetryFailedCustomers.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRetryFailedCustomers.Enabled = false;
            btnRetryFailedCustomers.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnRetryFailedCustomers.ForeColor = Color.Red;
            btnRetryFailedCustomers.Location = new Point(518, 11);
            btnRetryFailedCustomers.Margin = new Padding(3, 2, 3, 2);
            btnRetryFailedCustomers.Name = "btnRetryFailedCustomers";
            btnRetryFailedCustomers.Size = new Size(120, 22);
            btnRetryFailedCustomers.TabIndex = 12;
            btnRetryFailedCustomers.Text = "Retry Failed";
            btnRetryFailedCustomers.UseVisualStyleBackColor = true;
            btnRetryFailedCustomers.Click += btnRetryFailedCustomers_Click;
            // 
            // tabFailedUser
            // 
            tabFailedUser.Controls.Add(lblUserFailedStatus);
            tabFailedUser.Controls.Add(btnRetryFailedUsers);
            tabFailedUser.Location = new Point(4, 24);
            tabFailedUser.Margin = new Padding(3, 2, 3, 2);
            tabFailedUser.Name = "tabFailedUser";
            tabFailedUser.Padding = new Padding(3, 2, 3, 2);
            tabFailedUser.Size = new Size(823, 283);
            tabFailedUser.TabIndex = 13;
            tabFailedUser.Text = "Failed Users";
            tabFailedUser.UseVisualStyleBackColor = true;
            // 
            // lblUserFailedStatus
            // 
            lblUserFailedStatus.AutoSize = true;
            lblUserFailedStatus.Location = new Point(19, 86);
            lblUserFailedStatus.Name = "lblUserFailedStatus";
            lblUserFailedStatus.Size = new Size(160, 15);
            lblUserFailedStatus.TabIndex = 16;
            lblUserFailedStatus.Text = "No failed user records found.";
            // 
            // btnRetryFailedUsers
            // 
            btnRetryFailedUsers.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRetryFailedUsers.Enabled = false;
            btnRetryFailedUsers.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnRetryFailedUsers.ForeColor = Color.Red;
            btnRetryFailedUsers.Location = new Point(518, 11);
            btnRetryFailedUsers.Margin = new Padding(3, 2, 3, 2);
            btnRetryFailedUsers.Name = "btnRetryFailedUsers";
            btnRetryFailedUsers.Size = new Size(120, 22);
            btnRetryFailedUsers.TabIndex = 15;
            btnRetryFailedUsers.Text = "Retry Failed";
            btnRetryFailedUsers.UseVisualStyleBackColor = true;
            btnRetryFailedUsers.Click += btnRetryFailedUsers_Click;
            // 
            // tabAutoSync
            // 
            tabAutoSync.Controls.Add(groupBoxAutoSyncLog);
            tabAutoSync.Controls.Add(groupBoxAutoSyncSettings);
            tabAutoSync.Controls.Add(lblAutoSyncStatus);
            tabAutoSync.Controls.Add(progressAutoSync);
            tabAutoSync.Location = new Point(4, 24);
            tabAutoSync.Margin = new Padding(3, 2, 3, 2);
            tabAutoSync.Name = "tabAutoSync";
            tabAutoSync.Padding = new Padding(3, 2, 3, 2);
            tabAutoSync.Size = new Size(823, 283);
            tabAutoSync.TabIndex = 5;
            tabAutoSync.Text = "Auto Sync";
            tabAutoSync.UseVisualStyleBackColor = true;
            // 
            // groupBoxAutoSyncLog
            // 
            groupBoxAutoSyncLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBoxAutoSyncLog.Controls.Add(txtAutoSyncLog);
            groupBoxAutoSyncLog.Location = new Point(7, 116);
            groupBoxAutoSyncLog.Margin = new Padding(3, 2, 3, 2);
            groupBoxAutoSyncLog.Name = "groupBoxAutoSyncLog";
            groupBoxAutoSyncLog.Padding = new Padding(3, 2, 3, 2);
            groupBoxAutoSyncLog.Size = new Size(642, 110);
            groupBoxAutoSyncLog.TabIndex = 17;
            groupBoxAutoSyncLog.TabStop = false;
            groupBoxAutoSyncLog.Text = "Auto Sync Log";
            // 
            // txtAutoSyncLog
            // 
            txtAutoSyncLog.BackColor = SystemColors.Window;
            txtAutoSyncLog.Dock = DockStyle.Fill;
            txtAutoSyncLog.Location = new Point(3, 18);
            txtAutoSyncLog.Margin = new Padding(3, 2, 3, 2);
            txtAutoSyncLog.Multiline = true;
            txtAutoSyncLog.Name = "txtAutoSyncLog";
            txtAutoSyncLog.ReadOnly = true;
            txtAutoSyncLog.ScrollBars = ScrollBars.Both;
            txtAutoSyncLog.Size = new Size(636, 90);
            txtAutoSyncLog.TabIndex = 0;
            // 
            // groupBoxAutoSyncSettings
            // 
            groupBoxAutoSyncSettings.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBoxAutoSyncSettings.Controls.Add(lblAutoSyncStatusCounts);
            groupBoxAutoSyncSettings.Controls.Add(btnViewFailedAutoSync);
            groupBoxAutoSyncSettings.Controls.Add(lblLastSync);
            groupBoxAutoSyncSettings.Controls.Add(chkSyncDeletes);
            groupBoxAutoSyncSettings.Controls.Add(label11);
            groupBoxAutoSyncSettings.Controls.Add(numSyncInterval);
            groupBoxAutoSyncSettings.Controls.Add(label10);
            groupBoxAutoSyncSettings.Controls.Add(btnToggleAutoSync);
            groupBoxAutoSyncSettings.Location = new Point(7, 4);
            groupBoxAutoSyncSettings.Margin = new Padding(3, 2, 3, 2);
            groupBoxAutoSyncSettings.Name = "groupBoxAutoSyncSettings";
            groupBoxAutoSyncSettings.Padding = new Padding(3, 2, 3, 2);
            groupBoxAutoSyncSettings.Size = new Size(642, 76);
            groupBoxAutoSyncSettings.TabIndex = 16;
            groupBoxAutoSyncSettings.TabStop = false;
            groupBoxAutoSyncSettings.Text = "Auto Sync Settings";
            // 
            // lblAutoSyncStatusCounts
            // 
            lblAutoSyncStatusCounts.AutoSize = true;
            lblAutoSyncStatusCounts.Location = new Point(27, 52);
            lblAutoSyncStatusCounts.Name = "lblAutoSyncStatusCounts";
            lblAutoSyncStatusCounts.Size = new Size(77, 15);
            lblAutoSyncStatusCounts.TabIndex = 19;
            lblAutoSyncStatusCounts.Text = "Status: Ready";
            // 
            // btnViewFailedAutoSync
            // 
            btnViewFailedAutoSync.Enabled = false;
            btnViewFailedAutoSync.Location = new Point(350, 22);
            btnViewFailedAutoSync.Margin = new Padding(3, 2, 3, 2);
            btnViewFailedAutoSync.Name = "btnViewFailedAutoSync";
            btnViewFailedAutoSync.Size = new Size(149, 22);
            btnViewFailedAutoSync.TabIndex = 18;
            btnViewFailedAutoSync.Text = "No Failed Records";
            btnViewFailedAutoSync.UseVisualStyleBackColor = true;
            btnViewFailedAutoSync.Click += btnViewFailedAutoSync_Click;
            // 
            // lblLastSync
            // 
            lblLastSync.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblLastSync.AutoSize = true;
            lblLastSync.Location = new Point(368, 52);
            lblLastSync.Name = "lblLastSync";
            lblLastSync.Size = new Size(72, 15);
            lblLastSync.TabIndex = 17;
            lblLastSync.Text = "Last Sync: --";
            // 
            // chkSyncDeletes
            // 
            chkSyncDeletes.AutoSize = true;
            chkSyncDeletes.Checked = true;
            chkSyncDeletes.CheckState = CheckState.Checked;
            chkSyncDeletes.Location = new Point(27, 51);
            chkSyncDeletes.Margin = new Padding(3, 2, 3, 2);
            chkSyncDeletes.Name = "chkSyncDeletes";
            chkSyncDeletes.Size = new Size(216, 19);
            chkSyncDeletes.TabIndex = 16;
            chkSyncDeletes.Text = "Synchronize record deletions as well";
            chkSyncDeletes.UseVisualStyleBackColor = true;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(208, 24);
            label11.Name = "label11";
            label11.Size = new Size(50, 15);
            label11.TabIndex = 15;
            label11.Text = "seconds";
            // 
            // numSyncInterval
            // 
            numSyncInterval.Location = new Point(130, 22);
            numSyncInterval.Margin = new Padding(3, 2, 3, 2);
            numSyncInterval.Maximum = new decimal(new int[] { 3600, 0, 0, 0 });
            numSyncInterval.Minimum = new decimal(new int[] { 5, 0, 0, 0 });
            numSyncInterval.Name = "numSyncInterval";
            numSyncInterval.Size = new Size(74, 23);
            numSyncInterval.TabIndex = 14;
            numSyncInterval.Value = new decimal(new int[] { 30, 0, 0, 0 });
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(27, 24);
            label10.Name = "label10";
            label10.Size = new Size(66, 15);
            label10.TabIndex = 13;
            label10.Text = "Sync every:";
            // 
            // btnToggleAutoSync
            // 
            btnToggleAutoSync.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnToggleAutoSync.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnToggleAutoSync.ForeColor = Color.Green;
            btnToggleAutoSync.Location = new Point(517, 21);
            btnToggleAutoSync.Margin = new Padding(3, 2, 3, 2);
            btnToggleAutoSync.Name = "btnToggleAutoSync";
            btnToggleAutoSync.Size = new Size(120, 22);
            btnToggleAutoSync.TabIndex = 12;
            btnToggleAutoSync.Text = "Start Auto Sync";
            btnToggleAutoSync.UseVisualStyleBackColor = true;
            btnToggleAutoSync.Click += btnToggleAutoSync_Click;
            // 
            // lblAutoSyncStatus
            // 
            lblAutoSyncStatus.AutoSize = true;
            lblAutoSyncStatus.Location = new Point(27, 83);
            lblAutoSyncStatus.Name = "lblAutoSyncStatus";
            lblAutoSyncStatus.Size = new Size(64, 15);
            lblAutoSyncStatus.TabIndex = 15;
            lblAutoSyncStatus.Text = "Auto Sync:";
            // 
            // progressAutoSync
            // 
            progressAutoSync.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            progressAutoSync.Location = new Point(178, 83);
            progressAutoSync.Margin = new Padding(3, 2, 3, 2);
            progressAutoSync.Name = "progressAutoSync";
            progressAutoSync.Size = new Size(471, 14);
            progressAutoSync.TabIndex = 14;
            // 
            // tabTriggers
            // 
            tabTriggers.Controls.Add(groupBoxEntitySelection);
            tabTriggers.Controls.Add(groupBoxTriggerStatus);
            tabTriggers.Controls.Add(groupBoxTriggerOperations);
            tabTriggers.Controls.Add(txtTriggerDetails);
            tabTriggers.Location = new Point(4, 24);
            tabTriggers.Margin = new Padding(3, 2, 3, 2);
            tabTriggers.Name = "tabTriggers";
            tabTriggers.Padding = new Padding(3, 2, 3, 2);
            tabTriggers.Size = new Size(823, 283);
            tabTriggers.TabIndex = 6;
            tabTriggers.Text = "Triggers";
            tabTriggers.UseVisualStyleBackColor = true;
            // 
            // groupBoxEntitySelection
            // 
            groupBoxEntitySelection.Controls.Add(rbUser);
            groupBoxEntitySelection.Controls.Add(rbCustomer);
            groupBoxEntitySelection.Location = new Point(430, 71);
            groupBoxEntitySelection.Margin = new Padding(3, 2, 3, 2);
            groupBoxEntitySelection.Name = "groupBoxEntitySelection";
            groupBoxEntitySelection.Padding = new Padding(3, 2, 3, 2);
            groupBoxEntitySelection.Size = new Size(220, 48);
            groupBoxEntitySelection.TabIndex = 3;
            groupBoxEntitySelection.TabStop = false;
            groupBoxEntitySelection.Text = "Table Selection";
            // 
            // rbUser
            // 
            rbUser.AutoSize = true;
            rbUser.Location = new Point(110, 20);
            rbUser.Margin = new Padding(3, 2, 3, 2);
            rbUser.Name = "rbUser";
            rbUser.Size = new Size(83, 19);
            rbUser.TabIndex = 1;
            rbUser.Text = "Users Table";
            rbUser.UseVisualStyleBackColor = true;
            rbUser.CheckedChanged += rbEntityType_CheckedChanged;
            // 
            // rbCustomer
            // 
            rbCustomer.AutoSize = true;
            rbCustomer.Checked = true;
            rbCustomer.Location = new Point(5, 20);
            rbCustomer.Margin = new Padding(3, 2, 3, 2);
            rbCustomer.Name = "rbCustomer";
            rbCustomer.Size = new Size(112, 19);
            rbCustomer.TabIndex = 0;
            rbCustomer.TabStop = true;
            rbCustomer.Text = "Customers Table";
            rbCustomer.UseVisualStyleBackColor = true;
            rbCustomer.CheckedChanged += rbEntityType_CheckedChanged;
            // 
            // groupBoxTriggerStatus
            // 
            groupBoxTriggerStatus.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBoxTriggerStatus.Controls.Add(lblTriggerStatus);
            groupBoxTriggerStatus.Controls.Add(btnCheckTriggers);
            groupBoxTriggerStatus.Location = new Point(7, 4);
            groupBoxTriggerStatus.Margin = new Padding(3, 2, 3, 2);
            groupBoxTriggerStatus.Name = "groupBoxTriggerStatus";
            groupBoxTriggerStatus.Padding = new Padding(3, 2, 3, 2);
            groupBoxTriggerStatus.Size = new Size(642, 61);
            groupBoxTriggerStatus.TabIndex = 2;
            groupBoxTriggerStatus.TabStop = false;
            groupBoxTriggerStatus.Text = "Trigger Status";
            // 
            // lblTriggerStatus
            // 
            lblTriggerStatus.AutoSize = true;
            lblTriggerStatus.Location = new Point(18, 26);
            lblTriggerStatus.Name = "lblTriggerStatus";
            lblTriggerStatus.Size = new Size(193, 15);
            lblTriggerStatus.TabIndex = 1;
            lblTriggerStatus.Text = "Click Check Triggers to verify status";
            // 
            // btnCheckTriggers
            // 
            btnCheckTriggers.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCheckTriggers.Location = new Point(517, 22);
            btnCheckTriggers.Margin = new Padding(3, 2, 3, 2);
            btnCheckTriggers.Name = "btnCheckTriggers";
            btnCheckTriggers.Size = new Size(120, 22);
            btnCheckTriggers.TabIndex = 0;
            btnCheckTriggers.Text = "Check Triggers";
            btnCheckTriggers.UseVisualStyleBackColor = true;
            btnCheckTriggers.Click += btnCheckTriggers_Click;
            // 
            // groupBoxTriggerOperations
            // 
            groupBoxTriggerOperations.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBoxTriggerOperations.Controls.Add(btnRemoveTriggers);
            groupBoxTriggerOperations.Controls.Add(btnCreateTriggers);
            groupBoxTriggerOperations.Location = new Point(7, 70);
            groupBoxTriggerOperations.Margin = new Padding(3, 2, 3, 2);
            groupBoxTriggerOperations.Name = "groupBoxTriggerOperations";
            groupBoxTriggerOperations.Padding = new Padding(3, 2, 3, 2);
            groupBoxTriggerOperations.Size = new Size(417, 50);
            groupBoxTriggerOperations.TabIndex = 1;
            groupBoxTriggerOperations.TabStop = false;
            groupBoxTriggerOperations.Text = "Trigger Operations";
            // 
            // btnRemoveTriggers
            // 
            btnRemoveTriggers.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRemoveTriggers.ForeColor = Color.Red;
            btnRemoveTriggers.Location = new Point(292, 20);
            btnRemoveTriggers.Margin = new Padding(3, 2, 3, 2);
            btnRemoveTriggers.Name = "btnRemoveTriggers";
            btnRemoveTriggers.Size = new Size(120, 22);
            btnRemoveTriggers.TabIndex = 1;
            btnRemoveTriggers.Text = "Remove Triggers";
            btnRemoveTriggers.UseVisualStyleBackColor = true;
            btnRemoveTriggers.Click += btnRemoveTriggers_Click;
            // 
            // btnCreateTriggers
            // 
            btnCreateTriggers.ForeColor = Color.Green;
            btnCreateTriggers.Location = new Point(18, 20);
            btnCreateTriggers.Margin = new Padding(3, 2, 3, 2);
            btnCreateTriggers.Name = "btnCreateTriggers";
            btnCreateTriggers.Size = new Size(120, 22);
            btnCreateTriggers.TabIndex = 0;
            btnCreateTriggers.Text = "Create Triggers";
            btnCreateTriggers.UseVisualStyleBackColor = true;
            btnCreateTriggers.Click += btnCreateTriggers_Click;
            // 
            // txtTriggerDetails
            // 
            txtTriggerDetails.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtTriggerDetails.BackColor = SystemColors.Window;
            txtTriggerDetails.Location = new Point(7, 124);
            txtTriggerDetails.Margin = new Padding(3, 2, 3, 2);
            txtTriggerDetails.Multiline = true;
            txtTriggerDetails.Name = "txtTriggerDetails";
            txtTriggerDetails.ReadOnly = true;
            txtTriggerDetails.ScrollBars = ScrollBars.Both;
            txtTriggerDetails.Size = new Size(643, 102);
            txtTriggerDetails.TabIndex = 0;
            // 
            // btnExit
            // 
            btnExit.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnExit.Location = new Point(722, 325);
            btnExit.Margin = new Padding(3, 2, 3, 2);
            btnExit.Name = "btnExit";
            btnExit.Size = new Size(120, 22);
            btnExit.TabIndex = 5;
            btnExit.Text = "Exit";
            btnExit.UseVisualStyleBackColor = true;
            btnExit.Click += btnExit_Click;
            // 
            // timerAutoSync
            // 
            timerAutoSync.Interval = 30000;
            timerAutoSync.Tick += timerAutoSync_Tick;
            // 
            // statusStrip
            // 
            statusStrip.ImageScalingSize = new Size(20, 20);
            statusStrip.Items.AddRange(new ToolStripItem[] { statusLabel });
            statusStrip.Location = new Point(0, 349);
            statusStrip.Name = "statusStrip";
            statusStrip.Padding = new Padding(1, 0, 12, 0);
            statusStrip.Size = new Size(854, 22);
            statusStrip.TabIndex = 6;
            statusStrip.Text = "statusStrip1";
            // 
            // statusLabel
            // 
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(39, 17);
            statusLabel.Text = "Ready";
            // 
            // btnHideToTray
            // 
            btnHideToTray.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnHideToTray.Location = new Point(14, 325);
            btnHideToTray.Margin = new Padding(3, 2, 3, 2);
            btnHideToTray.Name = "btnHideToTray";
            btnHideToTray.Size = new Size(120, 22);
            btnHideToTray.TabIndex = 7;
            btnHideToTray.Text = "Hide to Tray";
            btnHideToTray.UseVisualStyleBackColor = true;
            btnHideToTray.Click += btnHideToTray_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(854, 371);
            Controls.Add(btnHideToTray);
            Controls.Add(statusStrip);
            Controls.Add(btnExit);
            Controls.Add(tabControl);
            Margin = new Padding(3, 2, 3, 2);
            MinimumSize = new Size(702, 347);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Database Synchronization";
            WindowState = FormWindowState.Minimized;
            FormClosing += MainForm_FormClosing;
            Load += MainForm_Load;
            Resize += MainForm_Resize;
            tabControl.ResumeLayout(false);
            tabConnections.ResumeLayout(false);
            groupBoxRecordCounts.ResumeLayout(false);
            groupBoxRecordCounts.PerformLayout();
            groupBoxRemote.ResumeLayout(false);
            groupBoxRemote.PerformLayout();
            groupBoxLocal.ResumeLayout(false);
            groupBoxLocal.PerformLayout();
            tabSettings.ResumeLayout(false);
            groupBoxAppSettings.ResumeLayout(false);
            groupBoxAppSettings.PerformLayout();
            tabSyncCustomer.ResumeLayout(false);
            tabSyncCustomer.PerformLayout();
            tabSyncUser.ResumeLayout(false);
            tabSyncUser.PerformLayout();
            tabResumeCustomer.ResumeLayout(false);
            tabResumeCustomer.PerformLayout();
            tabResumeUser.ResumeLayout(false);
            tabResumeUser.PerformLayout();
            tabFailedCustomer.ResumeLayout(false);
            tabFailedCustomer.PerformLayout();
            tabFailedUser.ResumeLayout(false);
            tabFailedUser.PerformLayout();
            tabAutoSync.ResumeLayout(false);
            tabAutoSync.PerformLayout();
            groupBoxAutoSyncLog.ResumeLayout(false);
            groupBoxAutoSyncLog.PerformLayout();
            groupBoxAutoSyncSettings.ResumeLayout(false);
            groupBoxAutoSyncSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numSyncInterval).EndInit();
            tabTriggers.ResumeLayout(false);
            tabTriggers.PerformLayout();
            groupBoxEntitySelection.ResumeLayout(false);
            groupBoxEntitySelection.PerformLayout();
            groupBoxTriggerStatus.ResumeLayout(false);
            groupBoxTriggerStatus.PerformLayout();
            groupBoxTriggerOperations.ResumeLayout(false);
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TabControl tabControl;
        private TabPage tabConnections;
        private TabPage tabSyncCustomer;
        private Label lblProgressCustomerSync;
        private Label lblCustomerSyncStatus;
        private ProgressBar progressBarCustomerSync;
        private CheckBox chkTruncateBeforeCustomerSync;
        private Button btnStartCustomerSync;
        private Button btnExit;
        private TabPage tabResumeCustomer;
        private Label lblProgressCustomerResume;
        private Label lblCustomerResumeStatus;
        private ProgressBar progressBarCustomerResume;
        private Button btnResetCustomerSync;
        private Button btnResumeCustomerSync;
        private TabPage tabFailedCustomer;
        private Label lblCustomerFailedStatus;
        private Button btnRetryFailedCustomers;
        private TabPage tabAutoSync;
        private TabPage tabTriggers;
        private GroupBox groupBoxTriggerOperations;
        private Button btnCreateTriggers;
        private Button btnRemoveTriggers;
        private TextBox txtTriggerDetails;
        private GroupBox groupBoxTriggerStatus;
        private Label lblTriggerStatus;
        private Button btnCheckTriggers;
        private Panel pnlLocalStatus;
        private Panel pnlRemoteStatus;
        private GroupBox groupBoxLocal;
        private Label lblLocalHelp;
        private ComboBox cboLocalDbType;
        private Label lblLocalDbType;
        private Button btnTestLocalConnection;
        private TextBox txtLocalPassword;
        private TextBox txtLocalUsername;
        private TextBox txtLocalDatabase;
        private TextBox txtLocalServer;
        private Label label4;
        private Label label3;
        private Label label2;
        private Label label1;
        private GroupBox groupBoxRemote;
        private Label lblRemoteHelp;
        private ComboBox cboRemoteDbType;
        private Label lblRemoteDbType;
        private Button btnTestRemoteConnection;
        private TextBox txtRemotePassword;
        private TextBox txtRemoteUsername;
        private TextBox txtRemoteDatabase;
        private TextBox txtRemoteServer;
        private Label label5;
        private Label label6;
        private Label label7;
        private Label label8;
        private GroupBox groupBoxRecordCounts;
        private Button btnRefreshCounts;
        private Label lblRemoteCustomerCount;
        private Label lblLocalCustomerCount;
        private GroupBox groupBoxAutoSyncSettings;
        private Label lblAutoSyncStatus;
        private ProgressBar progressAutoSync;
        private Button btnToggleAutoSync;
        private Label label11;
        private NumericUpDown numSyncInterval;
        private Label label10;
        private CheckBox chkSyncDeletes;
        private GroupBox groupBoxAutoSyncLog;
        private TextBox txtAutoSyncLog;
        private System.Windows.Forms.Timer timerAutoSync;
        private Label lblLastSync;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel statusLabel;
        private Button btnViewFailedAutoSync;
        private Label lblAutoSyncStatusCounts;
        private TabPage tabSettings;
        private GroupBox groupBoxAppSettings;
        private CheckBox chkMinimizeToTray;
        private CheckBox chkRunAtStartup;
        private Button btnSaveSettings;
        private CheckBox chkStartMinimized;
        private CheckBox chkAutoSyncOnStartup;
        private CheckBox chkSyncUsers;
        private CheckBox chkSyncCustomers;
        private Button btnHideToTray;
        private TabPage tabSyncUser;
        private Label lblProgressUserSync;
        private Label lblUserSyncStatus;
        private ProgressBar progressBarUserSync;
        private CheckBox chkTruncateBeforeUserSync;
        private Button btnStartUserSync;
        private TabPage tabResumeUser;
        private Label lblProgressUserResume;
        private Label lblUserResumeStatus;
        private ProgressBar progressBarUserResume;
        private Button btnResetUserSync;
        private Button btnResumeUserSync;
        private TabPage tabFailedUser;
        private Label lblUserFailedStatus;
        private Button btnRetryFailedUsers;
        private Label lblRemoteUserCount;
        private Label lblLocalUserCount;
        private GroupBox groupBoxEntitySelection;
        private RadioButton rbUser;
        private RadioButton rbCustomer;

        // Field for the system tray manager
        private SystemTrayManager _systemTrayManager;

        // Flag to track if the application should start minimized
        public bool StartMinimized { get; set; }
    }
}