using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SynchronizationApplication.Models;
using SynchronizationApplication.Services.Interfaces;

namespace SynchronizationApplication
{
    public partial class FailedAutoSyncForm : Form
    {
        // Changed from readonly to regular field to allow reassignment
        private List<EntityChange> _failedChanges;
        private readonly IChangeTrackingService _changeTrackingService;
        private readonly IDatabaseService _databaseService;
        private readonly AppSettings _appSettings;
        private CancellationTokenSource _cancellationTokenSource;
        private readonly EntityType _entityType;

        public FailedAutoSyncForm(
            List<EntityChange> failedChanges,
            IChangeTrackingService changeTrackingService,
            IDatabaseService databaseService,
            AppSettings appSettings,
            EntityType entityType)
        {
            InitializeComponent();

            _failedChanges = failedChanges;
            _changeTrackingService = changeTrackingService;
            _databaseService = databaseService;
            _appSettings = appSettings;
            _entityType = entityType;

            // Set form title based on entity type
            this.Text = $"Failed Auto-Sync {(_entityType == EntityType.Customer ? "Customer" : "User")} Records";

            // Update column header text
            colCustID.HeaderText = _entityType == EntityType.Customer ? "Customer ID" : "User ID";

            PopulateGrid();
            UpdateCounts();
        }

        private void PopulateGrid()
        {
            // Clear existing data
            gridFailedChanges.Rows.Clear();

            foreach (var change in _failedChanges)
            {
                string changeTypeStr = change.ChangeType.ToString();

                int rowIndex = gridFailedChanges.Rows.Add(
                    change.IsSelected,
                    change.LogID,
                    change.EntityID,
                    changeTypeStr,
                    change.ChangeTime,
                    change.Status,
                    change.ProcessedTime,
                    change.ErrorMessage ?? string.Empty
                );

                // Color-code the row based on status
                if (change.Status == "Failed")
                {
                    gridFailedChanges.Rows[rowIndex].DefaultCellStyle.BackColor = Color.MistyRose;
                }
                else if (change.Status == "Success")
                {
                    gridFailedChanges.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightGreen;
                }
                else if (change.Status == "Skipped")
                {
                    gridFailedChanges.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightYellow;
                }
            }
        }

        private void UpdateCounts()
        {
            int total = _failedChanges.Count;
            int selected = _failedChanges.Count(c => c.IsSelected);
            int inserted = _failedChanges.Count(c => c.ChangeType == ChangeType.Insert);
            int updated = _failedChanges.Count(c => c.ChangeType == ChangeType.Update);
            int deleted = _failedChanges.Count(c => c.ChangeType == ChangeType.Delete);

            lblTotalCount.Text = $"Total: {total}";
            lblSelectedCount.Text = $"Selected: {selected}";
            lblOperationsCount.Text = $"Operations: {inserted} inserts, {updated} updates, {deleted} deletes";
        }

        private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            bool isChecked = chkSelectAll.Checked;

            // Update data model
            foreach (var change in _failedChanges)
            {
                change.IsSelected = isChecked;
            }

            // Update UI
            foreach (DataGridViewRow row in gridFailedChanges.Rows)
            {
                row.Cells[0].Value = isChecked;
            }

            UpdateCounts();
        }

        private async void btnRetrySelected_Click(object sender, EventArgs e)
        {
            var selectedChanges = _failedChanges.Where(c => c.IsSelected).ToList();

            if (selectedChanges.Count == 0)
            {
                MessageBox.Show("No changes selected for retry.", "Retry", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            btnRetrySelected.Enabled = false;
            progressRetry.Visible = true;
            lblRetryStatus.Visible = true;

            try
            {
                string localConnectionString = _databaseService.BuildConnectionString(_appSettings.LocalDatabase);
                string remoteConnectionString = _databaseService.BuildConnectionString(_appSettings.RemoteDatabase);

                _cancellationTokenSource = new CancellationTokenSource();

                // Create progress reporter
                var progress = new Progress<SyncProgress>(p => OnProgressChanged(p));

                // Start the retry operation
                SyncResult result = await _changeTrackingService.RetryFailedChangesAsync(
                    localConnectionString,
                    remoteConnectionString,
                    selectedChanges,
                    _appSettings.SyncDeletes ?? true,
                    progress,
                    _cancellationTokenSource.Token);

                // Refresh the grid data - now safely getting new data
                _failedChanges = await _changeTrackingService.GetFailedChangesAsync(
                    localConnectionString,
                    _entityType,
                    _cancellationTokenSource.Token);

                PopulateGrid();
                UpdateCounts();

                // Show retry results
                MessageBox.Show(
                    $"Retry completed. {result.ProcessedRecords} of {selectedChanges.Count} changes were synchronized successfully." +
                    (result.ErrorRecords > 0 ? $"\n\n{result.ErrorRecords} changes failed." : ""),
                    "Retry Result",
                    MessageBoxButtons.OK,
                    result.ErrorRecords > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during retry: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
                btnRetrySelected.Enabled = true;
                progressRetry.Visible = false;
                lblRetryStatus.Visible = false;
            }
        }

        private void OnProgressChanged(SyncProgress progress)
        {
            // Ensure we're on the UI thread
            if (InvokeRequired)
            {
                Invoke(new Action<SyncProgress>(OnProgressChanged), progress);
                return;
            }

            lblRetryStatus.Text = progress.StatusMessage;

            if (progress.TotalRecords > 0)
            {
                progressRetry.Value = progress.PercentComplete;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            _cancellationTokenSource?.Cancel();
            Close();
        }

        private void gridFailedChanges_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Handle checkbox click
            if (e.ColumnIndex == 0 && e.RowIndex >= 0)
            {
                // Toggle the checkbox
                DataGridViewCheckBoxCell cell = (DataGridViewCheckBoxCell)gridFailedChanges.Rows[e.RowIndex].Cells[0];
                bool currentValue = Convert.ToBoolean(cell.Value);
                gridFailedChanges.Rows[e.RowIndex].Cells[0].Value = !currentValue;

                // Update the data model
                _failedChanges[e.RowIndex].IsSelected = !currentValue;

                // Update counts
                UpdateCounts();
            }
        }
    }
}