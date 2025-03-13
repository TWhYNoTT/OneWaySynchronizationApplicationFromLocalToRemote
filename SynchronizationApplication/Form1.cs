using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SynchronizationApplication.Models;
using SynchronizationApplication.Services;
using SynchronizationApplication.Services.Interfaces;

namespace SynchronizationApplication
{
    public partial class MainForm : Form
    {
        private readonly IConfigurationService _configService;
        private readonly IDatabaseService _dbService;
        private readonly ICustomerDataService _customerDataService;
        private readonly ISynchronizationService _syncService;
        private readonly ILogService _logService;

        private CancellationTokenSource? _cancellationTokenSource;
        private AppSettings _appSettings = new AppSettings();

        // Connection status indicators
        private Panel pnlLocalStatus;
        private Panel pnlRemoteStatus;
        private bool _localConnected = false;
        private bool _remoteConnected = false;

        // Path for failed records
        private readonly string _failedRecordsPath;

        // Button for failed records
        private Button btnViewFailedRecords;

        public MainForm(
            IConfigurationService configService,
            IDatabaseService dbService,
            ICustomerDataService customerDataService,
            ISynchronizationService syncService,
            ILogService logService)
        {
            InitializeComponent();

            // Initialize services
            _configService = configService;
            _dbService = dbService;
            _customerDataService = customerDataService;
            _syncService = syncService;
            _logService = logService;

            // Create directory for failed records if it doesn't exist
            string appDataDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "CustomerSync");

            if (!Directory.Exists(appDataDir))
                Directory.CreateDirectory(appDataDir);

            _failedRecordsPath = Path.Combine(appDataDir, "failed_records.csv");

            // Add connection status indicators
            InitializeStatusIndicators();

            // Load settings when form loads
            Load += async (s, e) => await LoadSettingsAsync();
        }





        private void InitializeStatusIndicators()
        {
            // Local status indicator - Position it next to the Local Database title
            pnlLocalStatus = new Panel
            {
                Size = new Size(12, 12),
                BackColor = Color.Red,
                Location = new Point(200, 11), // Position it next to the "Local Database" text in the GroupBox
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(3)
            };
            groupBox1.Controls.Add(pnlLocalStatus);

            // Remote status indicator - Position it next to the Remote Database title
            pnlRemoteStatus = new Panel
            {
                Size = new Size(12, 12),
                BackColor = Color.Red,
                Location = new Point(200, 11), // Position it next to the "Remote Database" text in the GroupBox
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(3)
            };
            groupBox2.Controls.Add(pnlRemoteStatus);

            // Bring indicators to front to ensure visibility
            pnlLocalStatus.BringToFront();
            pnlRemoteStatus.BringToFront();

            // Add tooltips
            var toolTip = new ToolTip();
            toolTip.SetToolTip(pnlLocalStatus, "Local database connection status");
            toolTip.SetToolTip(pnlRemoteStatus, "Remote database connection status");
        }





        // Proper declaration for the partial method
        partial void AddFailedRecordsButton();

        // Implementation of the partial method

        partial void AddFailedRecordsButton()
        {
            // Create the Failed Records button
            btnViewFailedRecords = new Button
            {
                Text = "View Failed Records",
                Size = new Size(150, 29),
                Enabled = false,
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };

            // Position it to the left of the Start Sync button
            btnViewFailedRecords.Location = new Point(
                btnStartSync.Left - btnViewFailedRecords.Width - 10,
                btnStartSync.Top);

            btnViewFailedRecords.Click += btnViewFailedRecords_Click;
            panelSync.Controls.Add(btnViewFailedRecords);
            btnViewFailedRecords.BringToFront();
        }



        // Update InitializeComponent to call this after the designer code
        partial void InitializeComponentExtension()
        {
            AddFailedRecordsButton();
        }

        private async Task LoadSettingsAsync()
        {
            try
            {
                _appSettings = await _configService.LoadSettingsAsync();

                // Update UI with loaded settings
                txtLocalServer.Text = _appSettings.LocalDatabase.Server;
                txtLocalDatabase.Text = _appSettings.LocalDatabase.Database;
                txtLocalUsername.Text = _appSettings.LocalDatabase.Username;
                txtLocalPassword.Text = _appSettings.LocalDatabase.Password;

                txtRemoteServer.Text = _appSettings.RemoteDatabase.Server;
                txtRemoteDatabase.Text = _appSettings.RemoteDatabase.Database;
                txtRemoteUsername.Text = _appSettings.RemoteDatabase.Username;
                txtRemotePassword.Text = _appSettings.RemoteDatabase.Password;

                chkTruncateBeforeSync.Checked = _appSettings.TruncateBeforeSync;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading settings: {ex.Message}", "Settings Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private AppSettings GetSettingsFromUI()
        {
            return new AppSettings
            {
                LocalDatabase = new DatabaseSettings
                {
                    Server = txtLocalServer.Text,
                    Database = txtLocalDatabase.Text,
                    Username = txtLocalUsername.Text,
                    Password = txtLocalPassword.Text
                },
                RemoteDatabase = new DatabaseSettings
                {
                    Server = txtRemoteServer.Text,
                    Database = txtRemoteDatabase.Text,
                    Username = txtRemoteUsername.Text,
                    Password = txtRemotePassword.Text
                },
                TruncateBeforeSync = chkTruncateBeforeSync.Checked
            };
        }

        private async void btnTestLocalConnection_Click(object sender, EventArgs e)
        {
            await TestConnectionAsync(true);
        }

        private async void btnTestRemoteConnection_Click(object sender, EventArgs e)
        {
            await TestConnectionAsync(false);
        }

        private async Task TestConnectionAsync(bool isLocal)
        {
            btnTestLocalConnection.Enabled = false;
            btnTestRemoteConnection.Enabled = false;

            try
            {
                DatabaseSettings settings = isLocal
                    ? GetSettingsFromUI().LocalDatabase
                    : GetSettingsFromUI().RemoteDatabase;

                bool success = await _dbService.TestConnectionAsync(settings);

                // Update the connection status indicator
                if (isLocal)
                {
                    _localConnected = success;
                    pnlLocalStatus.BackColor = success ? Color.LightGreen : Color.Red;
                }
                else
                {
                    _remoteConnected = success;
                    pnlRemoteStatus.BackColor = success ? Color.LightGreen : Color.Red;
                }

                MessageBox.Show(
                    success
                        ? $"Connection to {(isLocal ? "local" : "remote")} database successful!"
                        : $"Connection to {(isLocal ? "local" : "remote")} database failed.",
                    "Connection Test",
                    MessageBoxButtons.OK,
                    success ? MessageBoxIcon.Information : MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                // Update the connection status indicator to show failure
                if (isLocal)
                {
                    _localConnected = false;
                    pnlLocalStatus.BackColor = Color.Red;
                }
                else
                {
                    _remoteConnected = false;
                    pnlRemoteStatus.BackColor = Color.Red;
                }

                MessageBox.Show(
                    $"Error testing connection: {ex.Message}",
                    "Connection Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                btnTestLocalConnection.Enabled = true;
                btnTestRemoteConnection.Enabled = true;
            }
        }

        private async void btnStartSync_Click(object sender, EventArgs e)
        {
            if (!ValidateSettings())
                return;

            if (_cancellationTokenSource != null)
            {
                // Cancel the running sync
                _cancellationTokenSource.Cancel();
                btnStartSync.Text = "Start Sync";
                return;
            }

            // Save settings
            _appSettings = GetSettingsFromUI();
            await _configService.SaveSettingsAsync(_appSettings);

            // Reset UI
            progressBar.Value = 0;
            lblStatus.Text = "Testing connections...";
            lblProgress.Text = "Progress: 0%";

            // Test connections first
            try
            {
                var (localSuccess, remoteSuccess) = await _syncService.TestConnectionsAsync(_appSettings);

                // Update status indicators
                _localConnected = localSuccess;
                _remoteConnected = remoteSuccess;
                pnlLocalStatus.BackColor = localSuccess ? Color.LightGreen : Color.Red;
                pnlRemoteStatus.BackColor = remoteSuccess ? Color.LightGreen : Color.Red;

                if (!localSuccess)
                {
                    MessageBox.Show(
                        "Cannot connect to local database. Please check your settings.",
                        "Connection Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                if (!remoteSuccess)
                {
                    MessageBox.Show(
                        "Cannot connect to remote database. Please check your settings.",
                        "Connection Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error testing connections: {ex.Message}",
                    "Connection Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            // Disable UI during sync
            SetControlsEnabled(false);
            btnStartSync.Text = "Cancel Sync";
            btnStartSync.ForeColor = Color.Red;

            // Create cancellation token
            _cancellationTokenSource = new CancellationTokenSource();

            try
            {
                // Create progress reporter
                var progress = new Progress<SyncProgress>(OnSyncProgressChanged);

                // Start sync
                SyncResult result = await _syncService.SynchronizeAsync(
                    _appSettings,
                    progress,
                    _cancellationTokenSource.Token);

                // Show results
                ShowSyncResults(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error during synchronization: {ex.Message}",
                    "Synchronization Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                // Cleanup
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;

                // Reset UI
                btnStartSync.Text = "Start Sync";
                btnStartSync.ForeColor = Color.LightGreen;
                SetControlsEnabled(true);
            }
        }

        private void OnSyncProgressChanged(SyncProgress progress)
        {
            lblStatus.Text = progress.StatusMessage;
            lblProgress.Text = $"Progress: {progress.CurrentRecord} of {progress.TotalRecords} records ({progress.PercentComplete}%)";
            progressBar.Value = progress.PercentComplete;
        }

        private bool ValidateSettings()
        {
            if (string.IsNullOrWhiteSpace(txtLocalServer.Text) ||
                string.IsNullOrWhiteSpace(txtLocalDatabase.Text))
            {
                MessageBox.Show("Local database connection information is incomplete.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtRemoteServer.Text) ||
                string.IsNullOrWhiteSpace(txtRemoteDatabase.Text))
            {
                MessageBox.Show("Remote database connection information is incomplete.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void SetControlsEnabled(bool enabled)
        {
            // Enable/disable all controls except the sync button
            foreach (Control control in this.Controls)
            {
                if (control != btnStartSync)
                    control.Enabled = enabled;
            }

            foreach (Control control in panelLocalDb.Controls)
            {
                control.Enabled = enabled;
            }

            foreach (Control control in panelRemoteDb.Controls)
            {
                control.Enabled = enabled;
            }

            foreach (Control control in panelSync.Controls)
            {
                if (control != btnStartSync && control != btnViewFailedRecords)
                    control.Enabled = enabled;
            }

            btnViewFailedRecords.Enabled = enabled;
        }

        private void ShowSyncResults(SyncResult result)
        {
            var resultsForm = new SyncResultsForm(result, _logService);
            resultsForm.ShowDialog(this);

            // If there were errors, enable the View Failed Records button
            if (result.ErrorRecords > 0)
            {
                btnViewFailedRecords.Enabled = true;
                btnViewFailedRecords.ForeColor = Color.Red;
            }
        }

        private async void btnViewFailedRecords_Click(object sender, EventArgs e)
        {
            try
            {
                // Load failed records from file
                var failedRecords = await _customerDataService.LoadFailedRecordsAsync(_failedRecordsPath);

                if (failedRecords.Count == 0)
                {
                    MessageBox.Show(
                        "No failed records found.",
                        "Failed Records",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }

                // Show the failed records form
                var failedRecordsForm = new FailedRecordsForm(
                    failedRecords,
                    _customerDataService,
                    _dbService,
                    _appSettings);

                failedRecordsForm.ShowDialog(this);

                // Check if all records are resolved
                failedRecords = await _customerDataService.LoadFailedRecordsAsync(_failedRecordsPath);
                if (failedRecords.Count > 0 && failedRecords.TrueForAll(r => r.IsResolved))
                {
                    btnViewFailedRecords.ForeColor = Color.LightGreen;
                }
                else if (failedRecords.Count > 0)
                {
                    btnViewFailedRecords.ForeColor = Color.Red;
                }
                else
                {
                    btnViewFailedRecords.ForeColor = SystemColors.ControlText;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error loading failed records: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private async void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Ask user if they want to save settings
            if (DialogResult.Yes == MessageBox.Show(
                "Do you want to save your settings?",
                "Save Settings",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question))
            {
                try
                {
                    await _configService.SaveSettingsAsync(GetSettingsFromUI());
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving settings: {ex.Message}", "Settings Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}