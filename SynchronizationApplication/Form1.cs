using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SynchronizationApplication.Models;
using SynchronizationApplication.Services;
using SynchronizationApplication.Services.Interfaces;
using SynchronizationApplication.Services.Providers;

namespace SynchronizationApplication
{
    public partial class MainForm : Form
    {
        private readonly IConfigurationService _configService;
        private readonly IDatabaseService _dbService;
        private readonly ICustomerDataService _customerDataService;
        private readonly IUserDataService _userDataService;
        private readonly ISynchronizationService _syncService;
        private readonly ILogService _logService;
        private readonly ITriggerService _triggerService;
        private readonly IChangeTrackingService _changeTrackingService;
        private readonly IStartupService _startupService;

        private CancellationTokenSource? _customerSyncCancellationTokenSource;
        private CancellationTokenSource? _userSyncCancellationTokenSource;
        private CancellationTokenSource? _autoSyncCancellationTokenSource;
        private AppSettings _appSettings = new AppSettings();
        private bool _isAutoSyncRunning = false;
        private DateTime _lastAutoSyncTime = DateTime.MinValue;
        private bool _isCustomerSyncInProgress = false;
        private bool _isUserSyncInProgress = false;

        // Delegate for updating UI from background threads
        private delegate void UpdateUIDelegate(Action action);

        public MainForm(
            IConfigurationService configService,
            IDatabaseService dbService,
            ICustomerDataService customerDataService,
            IUserDataService userDataService,
            ISynchronizationService syncService,
            ILogService logService,
            ITriggerService triggerService,
            IChangeTrackingService changeTrackingService,
            IStartupService startupService)
        {
            InitializeComponent();

            // Initialize services
            _configService = configService;
            _dbService = dbService;
            _customerDataService = customerDataService;
            _userDataService = userDataService;
            _syncService = syncService;
            _logService = logService;
            _triggerService = triggerService;
            _changeTrackingService = changeTrackingService;
            _startupService = startupService;

            // Initialize form
            InitializeForm();

            // Initialize database provider dropdowns
            InitializeDatabaseProviders();
        }

        private void InitializeForm()
        {
            try
            {
                // First initialize the system tray manager before anything else
                InitializeSystemTrayManager();

                // Then continue with other form initializations
                txtLocalServer.MinimumSize = new Size(200, txtLocalServer.Height);
                txtRemoteServer.MinimumSize = new Size(200, txtRemoteServer.Height);

                // Set the application icon
                try
                {
                    Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

                    // If the icon changes, update the tray icon too
                    if (_systemTrayManager != null)
                    {
                        _systemTrayManager.SetTrayIcon(Icon);
                    }
                }
                catch
                {
                    // Use default icon if extraction fails
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing application UI: {ex.Message}", "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                // Use Task.Run to run async operations without blocking the UI thread
                Task.Run(async () =>
                {
                    try
                    {
                        // Load settings
                        await LoadSettingsAsync();

                        // All other operations should handle their own UI updates
                        await CheckForFailedRecordsAsync();
                        await CheckForIncompleteSyncAsync();
                        await UpdateAutoSyncStatusCounts();

                        // Handle UI operations on the UI thread
                        this.BeginInvoke(new Action(() =>
                        {
                            try
                            {
                                // Apply startup settings
                                if (StartMinimized || _appSettings.StartMinimized)
                                {
                                    this.WindowState = FormWindowState.Minimized;

                                    if (_appSettings.MinimizeToTray)
                                    {
                                        HideToTray();
                                    }
                                }

                                // Start auto sync if configured
                                if (_appSettings.AutoSyncOnStartup)
                                {
                                    StartAutoSyncAsync();
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Error applying startup settings: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }));
                    }
                    catch (Exception ex)
                    {
                        this.BeginInvoke(new Action(() =>
                        {
                            MessageBox.Show($"Error initializing application: {ex.Message}", "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }));
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Critical error during startup: {ex.Message}", "Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            // Make sure system tray manager is initialized before minimizing
            InitializeSystemTrayManager();

            // Apply startup settings immediately when the form is shown

                this.WindowState = FormWindowState.Minimized;

                
                
                    // Hide to system tray
                    HideToTray();
                
            
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (_appSettings.MinimizeToTray && WindowState == FormWindowState.Minimized)
            {
                HideToTray();
            }
        }

        private void HideToTray()
        {
            // Hide the form
            Hide();

            // Show the application in the system tray (with null check)
            if (_systemTrayManager != null)
            {
                _systemTrayManager.ShowInTray();
            }
            else
            {
                // Log or handle the case where system tray manager isn't initialized
                Console.WriteLine("Warning: System tray manager was not initialized before hiding to tray");

                // Attempt to initialize it if needed
                InitializeSystemTrayManager();
            }
        }

        private void InitializeSystemTrayManager()
        {
            try
            {
                if (_systemTrayManager == null)
                {
                    // Initialize the system tray manager
                    _systemTrayManager = new SystemTrayManager(
                        this,
                        Icon ?? SystemIcons.Application, // Use system icon as fallback
                        "Database Synchronization");

                    Console.WriteLine("System tray manager initialized");
                }
            }
            catch (Exception ex)
            {
                // Just log the error - don't crash the application
                Console.WriteLine($"Error initializing system tray manager: {ex.Message}");
            }
        }

        private void btnHideToTray_Click(object sender, EventArgs e)
        {
            HideToTray();
        }

        private void InitializeDatabaseProviders()
        {
            // Populate the DB provider dropdown lists
            cboLocalDbType.Items.Clear();
            cboLocalDbType.Items.Add("SQL Server");
            cboLocalDbType.Items.Add("MySQL");
            cboLocalDbType.SelectedIndex = 0;

            cboRemoteDbType.Items.Clear();
            cboRemoteDbType.Items.Add("SQL Server");
            cboRemoteDbType.Items.Add("MySQL");
            cboRemoteDbType.SelectedIndex = 0;

            cboLocalDbType.SelectedIndexChanged += (s, e) =>
            {
                UpdateLocalConnectionHelp();
            };

            cboRemoteDbType.SelectedIndexChanged += (s, e) =>
            {
                UpdateRemoteConnectionHelp();
            };

            UpdateLocalConnectionHelp();
            UpdateRemoteConnectionHelp();
        }

        private void UpdateLocalConnectionHelp()
        {
            if (cboLocalDbType.SelectedIndex == 0) // SQL Server
            {
                lblLocalHelp.Text = "";
            }
            else // MySQL
            {
                lblLocalHelp.Text = "";
            }
        }

        private void UpdateRemoteConnectionHelp()
        {
            if (cboRemoteDbType.SelectedIndex == 0) // SQL Server
            {
                lblRemoteHelp.Text = "";
            }
            else // MySQL
            {
                lblRemoteHelp.Text = "";
            }
        }

        private async Task LoadSettingsAsync()
        {
            try
            {
                // Load settings (this operation doesn't touch UI)
                _appSettings = await _configService.LoadSettingsAsync();

                // Update UI with loaded settings - must be on UI thread
                UpdateUI(() => {
                    // Update connection controls
                    txtLocalServer.Text = _appSettings.LocalDatabase.Server;
                    txtLocalDatabase.Text = _appSettings.LocalDatabase.Database;
                    txtLocalUsername.Text = _appSettings.LocalDatabase.Username;
                    txtLocalPassword.Text = _appSettings.LocalDatabase.Password;
                    cboLocalDbType.SelectedIndex = (int)_appSettings.LocalDatabase.ProviderType;

                    txtRemoteServer.Text = _appSettings.RemoteDatabase.Server;
                    txtRemoteDatabase.Text = _appSettings.RemoteDatabase.Database;
                    txtRemoteUsername.Text = _appSettings.RemoteDatabase.Username;
                    txtRemotePassword.Text = _appSettings.RemoteDatabase.Password;
                    cboRemoteDbType.SelectedIndex = (int)_appSettings.RemoteDatabase.ProviderType;

                    // Update sync settings
                    chkTruncateBeforeCustomerSync.Checked = _appSettings.TruncateBeforeSync;
                    chkTruncateBeforeUserSync.Checked = _appSettings.TruncateBeforeSync;

                    // Update auto-sync settings if available
                    if (_appSettings.AutoSyncInterval > 0)
                    {
                        numSyncInterval.Value = _appSettings.AutoSyncInterval;
                    }

                    if (_appSettings.SyncDeletes.HasValue)
                    {
                        chkSyncDeletes.Checked = _appSettings.SyncDeletes.Value;
                    }

                    // Update app settings
                    chkRunAtStartup.Checked = _startupService.IsApplicationInStartup();
                    chkMinimizeToTray.Checked = _appSettings.MinimizeToTray;
                    chkStartMinimized.Checked = _appSettings.StartMinimized;
                    chkAutoSyncOnStartup.Checked = _appSettings.AutoSyncOnStartup;
                    chkSyncCustomers.Checked = _appSettings.SyncCustomers;
                    chkSyncUsers.Checked = _appSettings.SyncUsers;
                });

                // Refresh record counts - this operation will handle its own UI updates
                await RefreshRecordCountsAsync();
            }
            catch (Exception ex)
            {
                UpdateUI(() => {
                    MessageBox.Show($"Error loading settings: {ex.Message}", "Settings Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                });
            }
        }



        private async Task RefreshRecordCountsAsync()
        {
            try
            {
                // Update UI to show counting is in progress - USING UPDATEUI HELPER
                UpdateUI(() => {
                    lblLocalCustomerCount.Text = "Local Customers: (Counting...)";
                    lblRemoteCustomerCount.Text = "Remote Customers: (Counting...)";
                    lblLocalUserCount.Text = "Local Users: (Counting...)";
                    lblRemoteUserCount.Text = "Remote Users: (Counting...)";
                    btnRefreshCounts.Enabled = false;
                });

                // Build connection strings
                string localConnectionString = _dbService.BuildConnectionString(_appSettings.LocalDatabase);
                string remoteConnectionString = _dbService.BuildConnectionString(_appSettings.RemoteDatabase);

                // Get local customer count
                int localCustomerCount = 0;
                bool localCustomerSuccess = false;
                try
                {
                    localCustomerCount = await _customerDataService.GetTotalCustomerCountAsync(localConnectionString);
                    localCustomerSuccess = true;
                }
                catch
                {
                    localCustomerSuccess = false;
                }

                // Update UI with local customer count - USING UPDATEUI HELPER
                UpdateUI(() => {
                    lblLocalCustomerCount.Text = localCustomerSuccess ?
                        $"Local Customers: {localCustomerCount:N0}" :
                        "Local Customers: (Error)";
                });

                // Get remote customer count
                int remoteCustomerCount = 0;
                bool remoteCustomerSuccess = false;
                try
                {
                    remoteCustomerCount = await _customerDataService.GetTotalCustomerCountAsync(remoteConnectionString);
                    remoteCustomerSuccess = true;
                }
                catch
                {
                    remoteCustomerSuccess = false;
                }

                // Update UI with remote customer count - USING UPDATEUI HELPER
                UpdateUI(() => {
                    lblRemoteCustomerCount.Text = remoteCustomerSuccess ?
                        $"Remote Customers: {remoteCustomerCount:N0}" :
                        "Remote Customers: (Error)";
                });

                // Get local user count
                int localUserCount = 0;
                bool localUserSuccess = false;
                try
                {
                    localUserCount = await _userDataService.GetTotalUserCountAsync(localConnectionString);
                    localUserSuccess = true;
                }
                catch
                {
                    localUserSuccess = false;
                }

                // Update UI with local user count - USING UPDATEUI HELPER
                UpdateUI(() => {
                    lblLocalUserCount.Text = localUserSuccess ?
                        $"Local Users: {localUserCount:N0}" :
                        "Local Users: (Error)";
                });

                // Get remote user count
                int remoteUserCount = 0;
                bool remoteUserSuccess = false;
                try
                {
                    remoteUserCount = await _userDataService.GetTotalUserCountAsync(remoteConnectionString);
                    remoteUserSuccess = true;
                }
                catch
                {
                    remoteUserSuccess = false;
                }

                // Update UI with remote user count - USING UPDATEUI HELPER
                UpdateUI(() => {
                    lblRemoteUserCount.Text = remoteUserSuccess ?
                        $"Remote Users: {remoteUserCount:N0}" :
                        "Remote Users: (Error)";
                });

                // Update connection status indicators
                await TestConnectionsAsync();
            }
            catch (Exception ex)
            {
                UpdateUI(() => {
                    MessageBox.Show($"Error refreshing record counts: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                });
            }
            finally
            {
                UpdateUI(() => {
                    btnRefreshCounts.Enabled = true;
                });
            }
        }

        // Also fix the TestConnectionsAsync method to use UpdateUI
        private async Task TestConnectionsAsync()
        {
            try
            {
                var settings = GetSettingsFromUI();
                var (localSuccess, remoteSuccess) = await _syncService.TestConnectionsAsync(settings);

                // Update the connection status indicators - USING UPDATEUI HELPER
                UpdateUI(() => {
                    pnlLocalStatus.BackColor = localSuccess ? Color.LightGreen : Color.Red;
                    pnlRemoteStatus.BackColor = remoteSuccess ? Color.LightGreen : Color.Red;
                });
            }
            catch
            {
                // If there's an error, set indicators to red - USING UPDATEUI HELPER
                UpdateUI(() => {
                    pnlLocalStatus.BackColor = Color.Red;
                    pnlRemoteStatus.BackColor = Color.Red;
                });
            }
        }
        private AppSettings GetSettingsFromUI()
        {
            var settings = new AppSettings
            {
                LocalDatabase = new DatabaseSettings
                {
                    Server = txtLocalServer.Text,
                    Database = txtLocalDatabase.Text,
                    Username = txtLocalUsername.Text,
                    Password = txtLocalPassword.Text,
                    ProviderType = (DatabaseProviderType)cboLocalDbType.SelectedIndex
                },
                RemoteDatabase = new DatabaseSettings
                {
                    Server = txtRemoteServer.Text,
                    Database = txtRemoteDatabase.Text,
                    Username = txtRemoteUsername.Text,
                    Password = txtRemotePassword.Text,
                    ProviderType = (DatabaseProviderType)cboRemoteDbType.SelectedIndex
                },
                TruncateBeforeSync = chkTruncateBeforeCustomerSync.Checked,
                AutoSyncInterval = (int)numSyncInterval.Value,
                SyncDeletes = chkSyncDeletes.Checked,
                RunAtStartup = chkRunAtStartup.Checked,
                MinimizeToTray = chkMinimizeToTray.Checked,
                StartMinimized = chkStartMinimized.Checked,
                AutoSyncOnStartup = chkAutoSyncOnStartup.Checked,
                SyncCustomers = chkSyncCustomers.Checked,
                SyncUsers = chkSyncUsers.Checked
            };

            return settings;
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
            // Disable test buttons during test
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
                    pnlLocalStatus.BackColor = success ? Color.Green : Color.Red;
                }
                else
                {
                    pnlRemoteStatus.BackColor = success ? Color.Green : Color.Red;
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
                    pnlLocalStatus.BackColor = Color.Red;
                }
                else
                {
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

        private async void btnStartCustomerSync_Click(object sender, EventArgs e)
        {
            await StartSyncAsync(EntityType.Customer, false);
        }

        private async void btnStartUserSync_Click(object sender, EventArgs e)
        {
            await StartSyncAsync(EntityType.User, false);
        }

        private async void btnResumeCustomerSync_Click(object sender, EventArgs e)
        {
            await StartSyncAsync(EntityType.Customer, true);
        }

        private async void btnResumeUserSync_Click(object sender, EventArgs e)
        {
            await StartSyncAsync(EntityType.User, true);
        }

        private async void btnResetCustomerSync_Click(object sender, EventArgs e)
        {
            await ResetSyncStateAsync(EntityType.Customer);
        }

        private async void btnResetUserSync_Click(object sender, EventArgs e)
        {
            await ResetSyncStateAsync(EntityType.User);
        }

        private async Task ResetSyncStateAsync(EntityType entityType)
        {
            string entityTypeName = entityType == EntityType.Customer ? "customer" : "user";

            if (DialogResult.Yes == MessageBox.Show(
                $"Are you sure you want to reset the {entityTypeName} sync state? This will clear the resume point.",
                $"Reset {entityTypeName} Sync State",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question))
            {
                try
                {
                    await _syncService.ResetSyncStateAsync(entityType);

                    // Update resume tab status
                    if (entityType == EntityType.Customer)
                    {
                        lblCustomerResumeStatus.Text = "Sync state reset. No incomplete sync found.";
                        lblCustomerResumeStatus.ForeColor = SystemColors.ControlText;
                        btnResumeCustomerSync.Enabled = false;
                    }
                    else // User
                    {
                        lblUserResumeStatus.Text = "Sync state reset. No incomplete sync found.";
                        lblUserResumeStatus.ForeColor = SystemColors.ControlText;
                        btnResumeUserSync.Enabled = false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error resetting sync state: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async Task StartSyncAsync(EntityType entityType, bool resumeSync)
        {
            // Validate settings in any case
            if (!ValidateSettings())
                return;

            // Check which progress controls to use based on entity type and sync type
            ProgressBar progressBar;
            Label lblProgress;
            Label lblStatus;
            Button btnSync;
            CheckBox chkTruncate;
            CancellationTokenSource cancellationTokenSource;
            bool isInProgress;

            if (entityType == EntityType.Customer)
            {
                if (resumeSync)
                {
                    progressBar = progressBarCustomerResume;
                    lblProgress = lblProgressCustomerResume;
                    lblStatus = lblCustomerResumeStatus;
                    btnSync = btnResumeCustomerSync;
                }
                else
                {
                    progressBar = progressBarCustomerSync;
                    lblProgress = lblProgressCustomerSync;
                    lblStatus = lblCustomerSyncStatus;
                    btnSync = btnStartCustomerSync;
                }
                chkTruncate = chkTruncateBeforeCustomerSync;
                cancellationTokenSource = _customerSyncCancellationTokenSource;
                isInProgress = _isCustomerSyncInProgress;
            }
            else // User
            {
                if (resumeSync)
                {
                    progressBar = progressBarUserResume;
                    lblProgress = lblProgressUserResume;
                    lblStatus = lblUserResumeStatus;
                    btnSync = btnResumeUserSync;
                }
                else
                {
                    progressBar = progressBarUserSync;
                    lblProgress = lblProgressUserSync;
                    lblStatus = lblUserSyncStatus;
                    btnSync = btnStartUserSync;
                }
                chkTruncate = chkTruncateBeforeUserSync;
                cancellationTokenSource = _userSyncCancellationTokenSource;
                isInProgress = _isUserSyncInProgress;
            }

            // If cancellation is in progress, stop it
            if (cancellationTokenSource != null)
            {
                if (entityType == EntityType.Customer)
                {
                    _customerSyncCancellationTokenSource.Cancel();
                    btnSync.Text = resumeSync ? "Resume Sync" : "Start Sync";
                }
                else // User
                {
                    _userSyncCancellationTokenSource.Cancel();
                    btnSync.Text = resumeSync ? "Resume Sync" : "Start Sync";
                }
                return;
            }

            // Save settings
            _appSettings = GetSettingsFromUI();
            await _configService.SaveSettingsAsync(_appSettings);

            // Reset UI
            progressBar.Value = 0;
            lblStatus.Text = "Testing connections...";
            lblProgress.Text = "Progress: 0%";

            if (entityType == EntityType.Customer)
            {
                _isCustomerSyncInProgress = true;
            }
            else // User
            {
                _isUserSyncInProgress = true;
            }

            // Test connections first
            try
            {
                var (localSuccess, remoteSuccess) = await _syncService.TestConnectionsAsync(_appSettings);

                // Update status indicators
                pnlLocalStatus.BackColor = localSuccess ? Color.Green : Color.Red;
                pnlRemoteStatus.BackColor = remoteSuccess ? Color.Green : Color.Red;

                if (!localSuccess)
                {
                    MessageBox.Show(
                        "Cannot connect to local database. Please check your settings.",
                        "Connection Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    if (entityType == EntityType.Customer)
                    {
                        _isCustomerSyncInProgress = false;
                    }
                    else // User
                    {
                        _isUserSyncInProgress = false;
                    }
                    return;
                }

                if (!remoteSuccess)
                {
                    MessageBox.Show(
                        "Cannot connect to remote database. Please check your settings.",
                        "Connection Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    if (entityType == EntityType.Customer)
                    {
                        _isCustomerSyncInProgress = false;
                    }
                    else // User
                    {
                        _isUserSyncInProgress = false;
                    }
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

                if (entityType == EntityType.Customer)
                {
                    _isCustomerSyncInProgress = false;
                }
                else // User
                {
                    _isUserSyncInProgress = false;
                }
                return;
            }

            // Disable UI during sync
            SetControlsEnabled(false);
            btnSync.Text = "Cancel Sync";
            btnSync.ForeColor = Color.Red;

            // Create cancellation token
            if (entityType == EntityType.Customer)
            {
                _customerSyncCancellationTokenSource = new CancellationTokenSource();
                cancellationTokenSource = _customerSyncCancellationTokenSource;
            }
            else // User
            {
                _userSyncCancellationTokenSource = new CancellationTokenSource();
                cancellationTokenSource = _userSyncCancellationTokenSource;
            }

            try
            {
                // Create progress reporter
                var progress = new Progress<SyncProgress>(p => OnSyncProgressChanged(p, progressBar, lblProgress, lblStatus));

                // Save truncate setting based on the specific checkbox
                _appSettings.TruncateBeforeSync = chkTruncate.Checked;
                await _configService.SaveSettingsAsync(_appSettings);

                // Start sync
                SyncResult result = await _syncService.SynchronizeAsync(
                    _appSettings,
                    entityType,
                    progress,
                    resumeSync,
                    cancellationTokenSource.Token);

                // Show results
                ShowSyncResults(result);

                // Update resume button state
                await CheckForIncompleteSyncAsync();

                // Update failed records button state
                await CheckForFailedRecordsAsync();

                // Refresh record counts after sync
                await RefreshRecordCountsAsync();

                // Update last sync time
                _lastAutoSyncTime = DateTime.Now;
                UpdateLastSyncTimeDisplay();

                // Update auto-sync status counts
                await UpdateAutoSyncStatusCounts();
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
                if (entityType == EntityType.Customer)
                {
                    _customerSyncCancellationTokenSource?.Dispose();
                    _customerSyncCancellationTokenSource = null;
                    _isCustomerSyncInProgress = false;
                }
                else // User
                {
                    _userSyncCancellationTokenSource?.Dispose();
                    _userSyncCancellationTokenSource = null;
                    _isUserSyncInProgress = false;
                }

                // Reset UI
                btnSync.Text = resumeSync ? "Resume Sync" : "Start Sync";
                btnSync.ForeColor = resumeSync ? Color.Blue : Color.Green;
                SetControlsEnabled(true);
            }
        }

        private void OnSyncProgressChanged(SyncProgress progress, ProgressBar progressBar, Label lblProgress, Label lblStatus)
        {
            // Ensure we update the UI on the UI thread
            UpdateUI(() =>
            {
                lblStatus.Text = progress.StatusMessage;
                lblProgress.Text = $"Progress: {progress.CurrentRecord} of {progress.TotalRecords} records ({progress.PercentComplete}%)";
                progressBar.Value = progress.PercentComplete;
                statusLabel.Text = progress.StatusMessage;

                // Update tray tooltip
                _systemTrayManager.UpdateSyncStatus($"{progress.StatusMessage} - {progress.PercentComplete}%");
            });
        }

        private bool ValidateSettings()
        {
            if (string.IsNullOrWhiteSpace(txtLocalServer.Text) ||
                string.IsNullOrWhiteSpace(txtLocalDatabase.Text))
            {
                MessageBox.Show("Local database connection information is incomplete.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tabControl.SelectedTab = tabConnections;
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtRemoteServer.Text) ||
                string.IsNullOrWhiteSpace(txtRemoteDatabase.Text))
            {
                MessageBox.Show("Remote database connection information is incomplete.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tabControl.SelectedTab = tabConnections;
                return false;
            }

            return true;
        }

        private void SetControlsEnabled(bool enabled)
        {
            // Safely update UI elements
            UpdateUI(() =>
            {
                // Enable/disable tabs
                tabControl.Enabled = enabled;

                // Always keep exit button enabled
                btnExit.Enabled = true;
                btnHideToTray.Enabled = true;

                // If auto-sync is running, ensure its controls remain enabled
                if (_isAutoSyncRunning)
                {
                    btnToggleAutoSync.Enabled = true;
                }
            });
        }

        private void ShowSyncResults(SyncResult result)
        {
            UpdateUI(() =>
            {
                var resultsForm = new SyncResultsForm(result, _logService);
                resultsForm.ShowDialog(this);
            });
        }

        private async Task CheckForFailedRecordsAsync()
        {
            try
            {
                bool hasFailedCustomerRecords = await FailedRecordsManager.HasFailedRecordsAsync(EntityType.Customer);
                bool hasFailedUserRecords = await FailedRecordsManager.HasFailedRecordsAsync(EntityType.User);

                // Update customer tab appearance
                UpdateUI(() =>
                {
                    if (hasFailedCustomerRecords)
                    {
                        lblCustomerFailedStatus.Text = "Failed customer records found. You can retry them.";
                        lblCustomerFailedStatus.ForeColor = Color.Red;
                        btnRetryFailedCustomers.Enabled = true;
                    }
                    else
                    {
                        lblCustomerFailedStatus.Text = "No failed customer records found.";
                        lblCustomerFailedStatus.ForeColor = SystemColors.ControlText;
                        btnRetryFailedCustomers.Enabled = false;
                    }
                });

                // Update user tab appearance
                UpdateUI(() =>
                {
                    if (hasFailedUserRecords)
                    {
                        lblUserFailedStatus.Text = "Failed user records found. You can retry them.";
                        lblUserFailedStatus.ForeColor = Color.Red;
                        btnRetryFailedUsers.Enabled = true;
                    }
                    else
                    {
                        lblUserFailedStatus.Text = "No failed user records found.";
                        lblUserFailedStatus.ForeColor = SystemColors.ControlText;
                        btnRetryFailedUsers.Enabled = false;
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error checking for failed records: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task CheckForIncompleteSyncAsync()
        {
            try
            {
                bool hasIncompleteCustomerSync = await _syncService.HasIncompleteSyncAsync(EntityType.Customer);
                bool hasIncompleteUserSync = await _syncService.HasIncompleteSyncAsync(EntityType.User);

                // Update Customer resume tab appearance
                UpdateUI(() =>
                {
                    if (hasIncompleteCustomerSync)
                    {
                        lblCustomerResumeStatus.Text = "Incomplete customer sync detected. You can resume where you left off.";
                        lblCustomerResumeStatus.ForeColor = Color.Blue;
                        btnResumeCustomerSync.Enabled = true;
                    }
                    else
                    {
                        lblCustomerResumeStatus.Text = "No incomplete customer sync found.";
                        lblCustomerResumeStatus.ForeColor = SystemColors.ControlText;
                        btnResumeCustomerSync.Enabled = false;
                    }
                });

                // Update User resume tab appearance
                UpdateUI(() =>
                {
                    if (hasIncompleteUserSync)
                    {
                        lblUserResumeStatus.Text = "Incomplete user sync detected. You can resume where you left off.";
                        lblUserResumeStatus.ForeColor = Color.Blue;
                        btnResumeUserSync.Enabled = true;
                    }
                    else
                    {
                        lblUserResumeStatus.Text = "No incomplete user sync found.";
                        lblUserResumeStatus.ForeColor = SystemColors.ControlText;
                        btnResumeUserSync.Enabled = false;
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error checking for incomplete sync: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnRetryFailedCustomers_Click(object sender, EventArgs e)
        {
            await RetryFailedRecordsAsync(EntityType.Customer);
        }

        private async void btnRetryFailedUsers_Click(object sender, EventArgs e)
        {
            await RetryFailedRecordsAsync(EntityType.User);
        }

        private async Task RetryFailedRecordsAsync(EntityType entityType)
        {
            try
            {
                // Load failed records from file
                var failedRecords = await FailedRecordsManager.LoadFailedRecordsAsync(entityType);

                if (failedRecords.Count == 0)
                {
                    MessageBox.Show(
                        $"No failed {(entityType == EntityType.Customer ? "customer" : "user")} records found.",
                        "Failed Records",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }

                // Show the failed records form
                var failedRecordsForm = new FailedRecordsForm(
                    failedRecords,
                    _customerDataService,
                    _userDataService,
                    _dbService,
                    _appSettings,
                    entityType);

                failedRecordsForm.ShowDialog(this);

                // Update button status
                await CheckForFailedRecordsAsync();

                // Refresh record counts after retry
                await RefreshRecordCountsAsync();
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

        #region Auto Sync Implementation

        private async void btnToggleAutoSync_Click(object sender, EventArgs e)
        {
            if (_isAutoSyncRunning)
            {
                // Stop auto sync
                StopAutoSync();
            }
            else
            {
                await StartAutoSyncAsync();
            }
        }

        private async Task StartAutoSyncAsync()
        {
            // Start auto sync - first validate settings and connections
            if (!ValidateSettings())
            {
                return;
            }

            // Save settings
            _appSettings = GetSettingsFromUI();
            await _configService.SaveSettingsAsync(_appSettings);

            // Test database connections
            try
            {
                var (localSuccess, remoteSuccess) = await _syncService.TestConnectionsAsync(_appSettings);

                if (!localSuccess || !remoteSuccess)
                {
                    MessageBox.Show(
                        "Cannot start auto sync because database connections failed. Please check your settings.",
                        "Connection Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                // Check if at least one entity type is selected
                if (!_appSettings.SyncCustomers && !_appSettings.SyncUsers)
                {
                    MessageBox.Show(
                        "Please select at least one entity type to synchronize (Customers or Users).",
                        "Configuration Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                // Check if customer triggers are installed if customers sync is enabled
                if (_appSettings.SyncCustomers)
                {
                    string connectionString = _dbService.BuildConnectionString(_appSettings.LocalDatabase);
                    bool customerTriggersExist = await _triggerService.DoTriggersExistAsync(connectionString, EntityType.Customer);

                    if (!customerTriggersExist)
                    {
                        DialogResult result = MessageBox.Show(
                            "Auto sync requires change tracking triggers to be installed on the Customers table. " +
                            "Would you like to install them now?",
                            "Triggers Required",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            bool created = await CreateTriggersAsync(EntityType.Customer);
                            if (!created)
                            {
                                return; // Failed to create triggers
                            }
                        }
                        else
                        {
                            return; // User chose not to install triggers
                        }
                    }

                    // Ensure the table has status tracking columns
                    await _triggerService.EnsureStatusColumnsExistAsync(connectionString, EntityType.Customer);
                }

                // Check if user triggers are installed if users sync is enabled
                if (_appSettings.SyncUsers)
                {
                    string connectionString = _dbService.BuildConnectionString(_appSettings.LocalDatabase);
                    bool userTriggersExist = await _triggerService.DoTriggersExistAsync(connectionString, EntityType.User);

                    if (!userTriggersExist)
                    {
                        DialogResult result = MessageBox.Show(
                            "Auto sync requires change tracking triggers to be installed on the Users table. " +
                            "Would you like to install them now?",
                            "Triggers Required",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            bool created = await CreateTriggersAsync(EntityType.User);
                            if (!created)
                            {
                                return; // Failed to create triggers
                            }
                        }
                        else
                        {
                            return; // User chose not to install triggers
                        }
                    }

                    // Ensure the table has status tracking columns
                    await _triggerService.EnsureStatusColumnsExistAsync(connectionString, EntityType.User);
                }

                // All checks passed, start the auto sync
                StartAutoSync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error testing connections: {ex.Message}",
                    "Connection Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void StartAutoSync()
        {
            _isAutoSyncRunning = true;

            // Set the timer interval based on user settings
            timerAutoSync.Interval = (int)numSyncInterval.Value * 1000;

            // Start the timer
            timerAutoSync.Start();

            // Update UI
            btnToggleAutoSync.Text = "Stop Auto Sync";
            btnToggleAutoSync.ForeColor = Color.Red;
            lblAutoSyncStatus.Text = "Auto Sync: Running";
            statusLabel.Text = "Auto Sync: Running";
            _systemTrayManager.UpdateSyncStatus("Auto Sync: Running");

            // Disable controls that shouldn't be modified during auto sync
            numSyncInterval.Enabled = false;
            chkSyncDeletes.Enabled = false;
            chkSyncCustomers.Enabled = false;
            chkSyncUsers.Enabled = false;

            // Add a log entry
            AddAutoSyncLogEntry("Auto sync started");

            // Immediately do an initial sync
            timerAutoSync_Tick(this, EventArgs.Empty);
        }

        private void StopAutoSync()
        {
            timerAutoSync.Stop();
            _isAutoSyncRunning = false;

            // Cancel any running auto sync task
            if (_autoSyncCancellationTokenSource != null)
            {
                _autoSyncCancellationTokenSource.Cancel();
                _autoSyncCancellationTokenSource.Dispose();
                _autoSyncCancellationTokenSource = null;
            }

            // Update UI
            btnToggleAutoSync.Text = "Start Auto Sync";
            btnToggleAutoSync.ForeColor = Color.Green;
            lblAutoSyncStatus.Text = "Auto Sync: Stopped";
            statusLabel.Text = "Ready";
            _systemTrayManager.UpdateSyncStatus("Auto Sync: Stopped");
            progressAutoSync.Value = 0;

            // Re-enable controls
            numSyncInterval.Enabled = true;
            chkSyncDeletes.Enabled = true;
            chkSyncCustomers.Enabled = true;
            chkSyncUsers.Enabled = true;

            // Add a log entry
            AddAutoSyncLogEntry("Auto sync stopped");
        }

        private async void timerAutoSync_Tick(object sender, EventArgs e)
        {
            // Don't start a new sync if one is already in progress
            if (_isCustomerSyncInProgress || _isUserSyncInProgress || _autoSyncCancellationTokenSource != null)
            {
                return;
            }

            // Create a cancellation token
            _autoSyncCancellationTokenSource = new CancellationTokenSource();

            try
            {
                // Update UI
                progressAutoSync.Value = 0;
                lblAutoSyncStatus.Text = "Auto Sync: Syncing...";
                statusLabel.Text = "Auto Sync: Checking for changes...";
                _systemTrayManager.UpdateSyncStatus("Auto Sync: Checking for changes...");

                // Create a progress reporter
                var progress = new Progress<SyncProgress>(p => OnAutoSyncProgressChanged(p));

                // Add log entry
                AddAutoSyncLogEntry("Checking for changes...");

                // Get connection strings
                string localConnectionString = _dbService.BuildConnectionString(_appSettings.LocalDatabase);
                string remoteConnectionString = _dbService.BuildConnectionString(_appSettings.RemoteDatabase);

                // Process customer changes if enabled
                if (_appSettings.SyncCustomers)
                {
                    await ProcessAutoSyncChangesAsync(
                        EntityType.Customer,
                        localConnectionString,
                        remoteConnectionString,
                        progress,
                        _autoSyncCancellationTokenSource.Token);
                }

                // Process user changes if enabled
                if (_appSettings.SyncUsers)
                {
                    await ProcessAutoSyncChangesAsync(
                        EntityType.User,
                        localConnectionString,
                        remoteConnectionString,
                        progress,
                        _autoSyncCancellationTokenSource.Token);
                }

                // Update the last sync time
                _lastAutoSyncTime = DateTime.Now;
                UpdateLastSyncTimeDisplay();

                // Update record counts
                await RefreshRecordCountsAsync();

                // Update auto-sync status counts
                await UpdateAutoSyncStatusCounts();

                // Final status update
                lblAutoSyncStatus.Text = "Auto Sync: Completed";
                statusLabel.Text = "Auto Sync: Completed";
                _systemTrayManager.UpdateSyncStatus("Auto Sync: Completed");
                AddAutoSyncLogEntry("Auto sync cycle completed");
            }
            catch (Exception ex)
            {
                // Don't show error if it was due to cancellation
                if (_autoSyncCancellationTokenSource?.Token.IsCancellationRequested == true)
                {
                    AddAutoSyncLogEntry("Auto sync was cancelled");
                }
                else
                {
                    AddAutoSyncLogEntry($"Error during auto sync: {ex.Message}");

                    // Show notification
                    if (WindowState == FormWindowState.Minimized || !this.Visible || !tabAutoSync.Visible)
                    {
                        _systemTrayManager.ShowBalloonTip(
                            "Auto Sync Error",
                            ex.Message,
                            ToolTipIcon.Error,
                            10000);
                    }
                }

                lblAutoSyncStatus.Text = "Auto Sync: Error";
                statusLabel.Text = "Auto Sync: Error";
                _systemTrayManager.UpdateSyncStatus("Auto Sync: Error");
            }
            finally
            {
                // Clean up
                _autoSyncCancellationTokenSource?.Dispose();
                _autoSyncCancellationTokenSource = null;
            }
        }

        private async Task ProcessAutoSyncChangesAsync(
      EntityType entityType,
      string localConnectionString,
      string remoteConnectionString,
      IProgress<SyncProgress> progress,
      CancellationToken cancellationToken)
        {
            // Get list of changes since last sync
            var changes = await _changeTrackingService.GetChangesAsync(
                localConnectionString,
                entityType,
                _lastAutoSyncTime,
                cancellationToken);

            if (changes.Count == 0)
            {
               string entityLabel = entityType == EntityType.Customer ? "customer" : "user";
AddAutoSyncLogEntry($"No {entityLabel} changes detected.");
                AddAutoSyncLogEntry($"No {entityLabel} changes detected.");
                return;
            }

            // Log the changes
            string entityTypeName = entityType == EntityType.Customer ? "customer" : "user";
            AddAutoSyncLogEntry($"Detected {changes.Count} {entityTypeName} changes: " +
                               $"{changes.Count(c => c.ChangeType == ChangeType.Insert)} inserts, " +
                               $"{changes.Count(c => c.ChangeType == ChangeType.Update)} updates, " +
                               $"{changes.Count(c => c.ChangeType == ChangeType.Delete)} deletes");

            // Set status
            if (entityType == EntityType.Customer)
            {
                _isCustomerSyncInProgress = true;
            }
            else // User
            {
                _isUserSyncInProgress = true;
            }

            try
            {
                // Process the changes
                SyncResult result = await _changeTrackingService.SynchronizeChangesAsync(
                    localConnectionString,
                    remoteConnectionString,
                    changes,
                    _appSettings.SyncDeletes ?? true,
                    progress,
                    cancellationToken);

                // Log the results
                AddAutoSyncLogEntry($"{entityTypeName} sync completed: {result.ProcessedRecords} processed, " +
                                   $"{result.AddedRecords} added, {result.UpdatedRecords} updated, " +
                                   $"{result.SkippedRecords} skipped, {result.ErrorRecords} errors");

                if (result.ErrorRecords > 0)
                {
                    // Log the error messages
                    foreach (var error in result.ErrorMessages)
                    {
                        AddAutoSyncLogEntry($"Error: {error}");
                    }

                    // Show notification
                    if (WindowState == FormWindowState.Minimized || !this.Visible || !tabAutoSync.Visible)
                    {
                        _systemTrayManager.ShowBalloonTip(
                            "Auto Sync Errors",
                            $"{result.ErrorRecords} errors occurred during {entityTypeName} sync. Check the Auto Sync log for details.",
                            ToolTipIcon.Warning,
                            10000);
                    }
                }
            }
            finally
            {
                // Reset status
                if (entityType == EntityType.Customer)
                {
                    _isCustomerSyncInProgress = false;
                }
                else // User
                {
                    _isUserSyncInProgress = false;
                }
            }
        }

        private void OnAutoSyncProgressChanged(SyncProgress progress)
        {
            // Update the UI on the UI thread
            UpdateUI(() =>
            {
                string entityName = progress.EntityType == EntityType.Customer ? "Customer" : "User";
                lblAutoSyncStatus.Text = $"Auto Sync: {entityName} - {progress.StatusMessage}";
                statusLabel.Text = $"Auto Sync: {entityName} - {progress.StatusMessage}";
                _systemTrayManager.UpdateSyncStatus($"Auto Sync: {entityName} - {progress.StatusMessage}");

                if (progress.TotalRecords > 0)
                {
                    progressAutoSync.Value = progress.PercentComplete;
                }
            });
        }

        private void AddAutoSyncLogEntry(string message)
        {
            UpdateUI(() =>
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                txtAutoSyncLog.AppendText($"[{timestamp}] {message}{Environment.NewLine}");

                // Auto-scroll to the end
                txtAutoSyncLog.SelectionStart = txtAutoSyncLog.Text.Length;
                txtAutoSyncLog.ScrollToCaret();
            });
        }

        private void UpdateLastSyncTimeDisplay()
        {
            UpdateUI(() =>
            {
                if (_lastAutoSyncTime != DateTime.MinValue)
                {
                    lblLastSync.Text = $"Last Sync: {_lastAutoSyncTime:yyyy-MM-dd HH:mm:ss}";
                }
                else
                {
                    lblLastSync.Text = "Last Sync: Never";
                }
            });
        }

        private async void btnViewFailedAutoSync_Click(object sender, EventArgs e)
        {
            try
            {
                // Get current selected entity type
                EntityType entityType = rbCustomer.Checked ? EntityType.Customer : EntityType.User;

                // Save current settings
                _appSettings = GetSettingsFromUI();
                await _configService.SaveSettingsAsync(_appSettings);

                string localConnectionString = _dbService.BuildConnectionString(_appSettings.LocalDatabase);

                // Get failed changes
                var failedChanges = await _changeTrackingService.GetFailedChangesAsync(localConnectionString, entityType);

                if (failedChanges.Count == 0)
                {
                    MessageBox.Show(
                        $"No failed auto-sync {(entityType == EntityType.Customer ? "customer" : "user")} records found.",
                        "Failed Auto-Sync Records",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }

                // Show the failed auto-sync form
                var failedAutoSyncForm = new FailedAutoSyncForm(
                    failedChanges,
                    _changeTrackingService,
                    _dbService,
                    _appSettings,
                    entityType);

                failedAutoSyncForm.ShowDialog(this);

                // Update UI - refresh status counts on the auto-sync tab
                await UpdateAutoSyncStatusCounts();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error loading failed auto-sync records: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private async Task UpdateAutoSyncStatusCounts()
        {
            try
            {
                string connectionString = _dbService.BuildConnectionString(_appSettings.LocalDatabase);
                bool customerTableExists = await _triggerService.DoesChangeTrackingTableExistAsync(connectionString, EntityType.Customer);
                bool userTableExists = await _triggerService.DoesChangeTrackingTableExistAsync(connectionString, EntityType.User);

                if (!customerTableExists && !userTableExists)
                {
                    lblAutoSyncStatusCounts.Text = "Status: Change tracking not set up";
                    btnViewFailedAutoSync.Enabled = false;
                    return;
                }

                int customerFailedCount = 0;
                int userFailedCount = 0;

                // Get Customer status counts if available
                if (customerTableExists)
                {
                    var customerCounts = await GetEntityStatusCountsDataAsync(connectionString, EntityType.Customer);
                    customerFailedCount = customerCounts.failedCount;
                }

                // Get User status counts if available
                if (userTableExists)
                {
                    var userCounts = await GetEntityStatusCountsDataAsync(connectionString, EntityType.User);
                    userFailedCount = userCounts.failedCount;
                }

                // Update the overall status
                int totalFailedCount = customerFailedCount + userFailedCount;
                lblAutoSyncStatusCounts.Text = $"Status: {totalFailedCount} failed changes detected";

                // Enable/disable the view failed button
                btnViewFailedAutoSync.Enabled = totalFailedCount > 0;

                // Update the failed button text
                btnViewFailedAutoSync.Text = totalFailedCount > 0
                    ? $"View Failed Records ({totalFailedCount})"
                    : "No Failed Records";
            }
            catch (Exception ex)
            {
                lblAutoSyncStatusCounts.Text = $"Status: Error getting counts: {ex.Message}";
                btnViewFailedAutoSync.Enabled = false;
            }
        }

        // Use this helper method that returns a tuple instead of using out parameters
        private async Task<(int pendingCount, int successCount, int failedCount, int skippedCount)>
            GetEntityStatusCountsDataAsync(string connectionString, EntityType entityType)
        {
            int pendingCount = 0;
            int successCount = 0;
            int failedCount = 0;
            int skippedCount = 0;

            var provider = _dbService.GetProviderForConnection(connectionString);
            string tableName = provider.GetChangeTrackingTableName(entityType);

            using (var connection = provider.CreateConnection(connectionString))
            {
                await connection.OpenAsync();

                string sql = $@"
            SELECT Status, COUNT(*) as Count
            FROM [dbo].[{tableName}]
            GROUP BY Status
            ORDER BY Status";

                using (var command = provider.CreateCommand(sql, connection))
                {
                    try
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                string status = reader.GetString(0);
                                int count = reader.GetInt32(1);

                                switch (status.ToLower())
                                {
                                    case "pending":
                                        pendingCount = count;
                                        break;
                                    case "success":
                                        successCount = count;
                                        break;
                                    case "failed":
                                        failedCount = count;
                                        break;
                                    case "skipped":
                                        skippedCount = count;
                                        break;
                                }
                            }
                        }
                    }
                    catch
                    {
                        // Status columns might not exist yet
                        await _triggerService.EnsureStatusColumnsExistAsync(connectionString, entityType);
                    }
                }
            }

            return (pendingCount, successCount, failedCount, skippedCount);
        }

      
        #endregion

        #region Trigger Management

        private async void btnCheckTriggers_Click(object sender, EventArgs e)
        {
            await CheckTriggersAsync();
        }

        private async Task<bool> CheckTriggersAsync()
        {
            try
            {
                EntityType entityType = rbCustomer.Checked ? EntityType.Customer : EntityType.User;
                string entityTypeName = entityType == EntityType.Customer ? "customer" : "user";

                // Save current settings
                _appSettings = GetSettingsFromUI();
                await _configService.SaveSettingsAsync(_appSettings);

                string connectionString = _dbService.BuildConnectionString(_appSettings.LocalDatabase);
                bool triggersExist = await _triggerService.DoTriggersExistAsync(connectionString, entityType);
                bool tableExists = await _triggerService.DoesChangeTrackingTableExistAsync(connectionString, entityType);

                // Update UI
                UpdateUI(() =>
                {
                    if (triggersExist && tableExists)
                    {
                        lblTriggerStatus.Text = $"{entityTypeName} change tracking triggers are properly installed.";
                        lblTriggerStatus.ForeColor = Color.Green;
                    }
                    else if (!tableExists)
                    {
                        lblTriggerStatus.Text = $"{entityTypeName} change tracking table not found. Click 'Create Triggers' to install.";
                        lblTriggerStatus.ForeColor = Color.Red;
                    }
                    else
                    {
                        lblTriggerStatus.Text = $"Some {entityTypeName} triggers are missing. Click 'Create Triggers' to install.";
                        lblTriggerStatus.ForeColor = Color.Orange;
                    }

                    // Enable/disable buttons based on trigger state
                    btnCreateTriggers.Enabled = !triggersExist || !tableExists;
                    btnRemoveTriggers.Enabled = triggersExist || tableExists;
                });

                // Get and display trigger details
                var details = await _triggerService.GetTriggerDetailsAsync(connectionString, entityType);
                txtTriggerDetails.Text = string.Join(Environment.NewLine, details);

                return triggersExist && tableExists;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error checking triggers: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private async void btnCreateTriggers_Click(object sender, EventArgs e)
        {
            EntityType entityType = rbCustomer.Checked ? EntityType.Customer : EntityType.User;
            await CreateTriggersAsync(entityType);
        }

        private async Task<bool> CreateTriggersAsync(EntityType entityType)
        {
            try
            {
                string entityTypeName = entityType == EntityType.Customer ? "customer" : "user";

                // Save current settings
                _appSettings = GetSettingsFromUI();
                await _configService.SaveSettingsAsync(_appSettings);

                string connectionString = _dbService.BuildConnectionString(_appSettings.LocalDatabase);

                // Test connection first
                bool connSuccess = await _dbService.TestConnectionAsync(_appSettings.LocalDatabase);
                if (!connSuccess)
                {
                    MessageBox.Show(
                        "Cannot connect to local database. Please check your settings.",
                        "Connection Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return false;
                }

                // Create the triggers
                await _triggerService.CreateTriggersAsync(connectionString, entityType);

                // Check if creation was successful
                bool success = await CheckTriggersAsync();

                if (success)
                {
                    MessageBox.Show(
                        $"{entityTypeName} change tracking triggers have been successfully installed.",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }

                return success;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating triggers: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private async void btnRemoveTriggers_Click(object sender, EventArgs e)
        {
            EntityType entityType = rbCustomer.Checked ? EntityType.Customer : EntityType.User;
            string entityTypeName = entityType == EntityType.Customer ? "customer" : "user";

            if (DialogResult.Yes == MessageBox.Show(
                $"Are you sure you want to remove all {entityTypeName} change tracking triggers? This will disable auto sync functionality for {entityTypeName}s.",
                "Remove Triggers",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning))
            {
                try
                {
                    // Save current settings
                    _appSettings = GetSettingsFromUI();
                    await _configService.SaveSettingsAsync(_appSettings);

                    string connectionString = _dbService.BuildConnectionString(_appSettings.LocalDatabase);

                    // Test connection first
                    bool connSuccess = await _dbService.TestConnectionAsync(_appSettings.LocalDatabase);
                    if (!connSuccess)
                    {
                        MessageBox.Show(
                            "Cannot connect to local database. Please check your settings.",
                            "Connection Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }

                    // Stop auto sync if it's running
                    if (_isAutoSyncRunning)
                    {
                        StopAutoSync();
                    }

                    // Remove the triggers
                    await _triggerService.RemoveTriggersAsync(connectionString, entityType);

                    // Verify removal
                    await CheckTriggersAsync();

                    MessageBox.Show(
                        $"{entityTypeName} change tracking triggers have been successfully removed.",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error removing triggers: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void rbEntityType_CheckedChanged(object sender, EventArgs e)
        {
            CheckTriggersAsync().ConfigureAwait(false);
        }

        #endregion

        private async void btnRefreshCounts_Click(object sender, EventArgs e)
        {
            await RefreshRecordCountsAsync();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private async void btnSaveSettings_Click(object sender, EventArgs e)
        {
            try
            {
                // Get settings from UI
                _appSettings = GetSettingsFromUI();

                // Update startup registry
                if (_appSettings.RunAtStartup)
                {
                    _startupService.AddApplicationToStartup();
                }
                else
                {
                    _startupService.RemoveApplicationFromStartup();
                }

                // Save settings
                await _configService.SaveSettingsAsync(_appSettings);

                MessageBox.Show(
                    "Settings saved successfully.",
                    "Save Settings",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving settings: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Check if we should minimize to tray instead of closing
            if (e.CloseReason == CloseReason.UserClosing && _appSettings.MinimizeToTray)
            {
                e.Cancel = true;
                HideToTray();
                return;
            }

            // Stop auto sync if it's running
            if (_isAutoSyncRunning)
            {
                StopAutoSync();
            }

            // Cancel any running sync operations
            if (_customerSyncCancellationTokenSource != null)
            {
                _customerSyncCancellationTokenSource.Cancel();
            }

            if (_userSyncCancellationTokenSource != null)
            {
                _userSyncCancellationTokenSource.Cancel();
            }

            // Save settings
            try
            {
                await _configService.SaveSettingsAsync(GetSettingsFromUI());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving settings: {ex.Message}", "Settings Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Method used by the system tray menu to trigger sync
        internal void TriggerSyncFromTray()
        {
            if (!_isAutoSyncRunning)
            {
                StartAutoSyncAsync().ConfigureAwait(false);
            }
            else
            {
                // Manually trigger a sync cycle
                timerAutoSync_Tick(this, EventArgs.Empty);
            }
        }

        // Helper method to safely update UI from any thread
        private void UpdateUI(Action action)
        {
            if (InvokeRequired)
            {
                Invoke(new UpdateUIDelegate(UpdateUI), action);
            }
            else
            {
                action();
            }
        }
    }
}