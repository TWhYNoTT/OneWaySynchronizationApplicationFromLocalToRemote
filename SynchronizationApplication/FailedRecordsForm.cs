using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using SynchronizationApplication.Models;
using SynchronizationApplication.Services;
using SynchronizationApplication.Services.Interfaces;

namespace SynchronizationApplication
{
    public partial class FailedRecordsForm : Form
    {
        private readonly List<FailedRecord> _failedRecords;
        private readonly ICustomerDataService _customerDataService;
        private readonly IUserDataService _userDataService;
        private readonly IDatabaseService _databaseService;
        private readonly AppSettings _appSettings;
        private readonly EntityType _entityType;

        public FailedRecordsForm(
            List<FailedRecord> failedRecords,
            ICustomerDataService customerDataService,
            IUserDataService userDataService,
            IDatabaseService databaseService,
            AppSettings appSettings,
            EntityType entityType)
        {
            InitializeComponent();

            _failedRecords = failedRecords;
            _customerDataService = customerDataService;
            _userDataService = userDataService;
            _databaseService = databaseService;
            _appSettings = appSettings;
            _entityType = entityType;

            this.Text = $"Failed {(_entityType == EntityType.Customer ? "Customer" : "User")} Records";

            PopulateGrid();
        }

        private void PopulateGrid()
        {
            // Clear existing data
            gridFailedRecords.Rows.Clear();

            // Update column header
            colCustID.HeaderText = _entityType == EntityType.Customer ? "Customer ID" : "User ID";

            foreach (var record in _failedRecords)
            {
                int rowIndex = gridFailedRecords.Rows.Add(
                    record.IsSelected,
                    record.EntityID,
                    record.ErrorMessage,
                    record.Timestamp,
                    record.IsResolved ? "Yes" : "No"
                );

                if (record.IsResolved)
                {
                    gridFailedRecords.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightGreen;
                }
            }

            // Update counts
            UpdateCounts();
        }

        private void UpdateCounts()
        {
            lblTotalCount.Text = $"Total Records: {_failedRecords.Count}";
            lblResolvedCount.Text = $"Resolved: {_failedRecords.Count(r => r.IsResolved)}";
            lblPendingCount.Text = $"Pending: {_failedRecords.Count(r => !r.IsResolved)}";
        }

        private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            bool isChecked = chkSelectAll.Checked;

            // Update data model
            foreach (var record in _failedRecords.Where(r => !r.IsResolved))
            {
                record.IsSelected = isChecked;
            }

            // Update UI
            foreach (DataGridViewRow row in gridFailedRecords.Rows)
            {
                if (row.Cells[4].Value.ToString() != "Yes") // Not resolved
                {
                    row.Cells[0].Value = isChecked;
                }
            }
        }

        private async void btnRetrySelected_Click(object sender, EventArgs e)
        {
            var selectedRecords = _failedRecords.Where(r => r.IsSelected && !r.IsResolved).ToList();

            if (selectedRecords.Count == 0)
            {
                MessageBox.Show("No records selected for retry.", "Retry", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            btnRetrySelected.Enabled = false;
            progressRetry.Visible = true;
            lblRetryStatus.Visible = true;

            try
            {
                string localConnectionString = _databaseService.BuildConnectionString(_appSettings.LocalDatabase);
                string remoteConnectionString = _databaseService.BuildConnectionString(_appSettings.RemoteDatabase);

                int successCount = 0;
                int totalToProcess = selectedRecords.Count;

                for (int i = 0; i < selectedRecords.Count; i++)
                {
                    var record = selectedRecords[i];
                    lblRetryStatus.Text = $"Retrying record {i + 1} of {totalToProcess}...";
                    progressRetry.Value = (int)((i + 1) / (double)totalToProcess * 100);

                    try
                    {
                        // Use the appropriate data service based on entity type
                        if (_entityType == EntityType.Customer)
                        {
                            await RetryCustomerRecord(localConnectionString, remoteConnectionString, record);
                        }
                        else // User
                        {
                            await RetryUserRecord(localConnectionString, remoteConnectionString, record);
                        }

                        // Mark as resolved
                        record.IsResolved = true;
                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        // Update error message
                        record.ErrorMessage = ex.Message;
                        record.Timestamp = DateTime.Now;
                    }

                    Application.DoEvents();
                }

                // Save updated failed records - this will only save unresolved records
                await FailedRecordsManager.SaveFailedRecordsAsync(_failedRecords);

                // Update UI
                PopulateGrid();

                // Show result
                MessageBox.Show(
                    $"Retry completed. {successCount} of {totalToProcess} records were synchronized successfully.",
                    "Retry Result",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during retry: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnRetrySelected.Enabled = true;
                progressRetry.Visible = false;
                lblRetryStatus.Visible = false;
            }
        }

        private async Task RetryCustomerRecord(string localConnectionString, string remoteConnectionString, FailedRecord record)
        {
            // Use a direct SQL approach instead of translating Customer to DataRow
            using (var localConnection = new SqlConnection(localConnectionString))
            {
                await localConnection.OpenAsync();

                // Get the data directly as a DataTable
                string selectSql = "SELECT * FROM Customers WHERE CustID = @CustID";
                using (var adapter = new SqlDataAdapter(selectSql, localConnection))
                {
                    adapter.SelectCommand.Parameters.AddWithValue("@CustID", record.EntityID);

                    var dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    if (dataTable.Rows.Count > 0)
                    {
                        var row = dataTable.Rows[0];

                        // Check if exists on remote database
                        bool exists = await _customerDataService.CustomerExistsAsync(remoteConnectionString, record.EntityID);

                        if (exists)
                        {
                            await _customerDataService.UpdateCustomerAsync(remoteConnectionString, row);
                        }
                        else
                        {
                            await _customerDataService.InsertCustomerAsync(remoteConnectionString, row);
                        }
                    }
                    else
                    {
                        throw new Exception($"Customer with ID {record.EntityID} not found in local database");
                    }
                }
            }
        }

        private async Task RetryUserRecord(string localConnectionString, string remoteConnectionString, FailedRecord record)
        {
            // Use a direct SQL approach instead of translating User to DataRow
            using (var localConnection = new SqlConnection(localConnectionString))
            {
                await localConnection.OpenAsync();

                // Get the data directly as a DataTable
                string selectSql = "SELECT * FROM Users WHERE UserID = @UserID";
                using (var adapter = new SqlDataAdapter(selectSql, localConnection))
                {
                    adapter.SelectCommand.Parameters.AddWithValue("@UserID", record.EntityID);

                    var dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    if (dataTable.Rows.Count > 0)
                    {
                        var row = dataTable.Rows[0];

                        // Check if exists on remote database
                        bool exists = await _userDataService.UserExistsAsync(remoteConnectionString, record.EntityID);

                        if (exists)
                        {
                            await _userDataService.UpdateUserAsync(remoteConnectionString, row);
                        }
                        else
                        {
                            await _userDataService.InsertUserAsync(remoteConnectionString, row);
                        }
                    }
                    else
                    {
                        throw new Exception($"User with ID {record.EntityID} not found in local database");
                    }
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void gridFailedRecords_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Handle checkbox click
            if (e.ColumnIndex == 0 && e.RowIndex >= 0)
            {
                // Toggle the checkbox
                DataGridViewCheckBoxCell cell = (DataGridViewCheckBoxCell)gridFailedRecords.Rows[e.RowIndex].Cells[0];
                bool currentValue = Convert.ToBoolean(cell.Value);
                gridFailedRecords.Rows[e.RowIndex].Cells[0].Value = !currentValue;

                // Update the data model
                _failedRecords[e.RowIndex].IsSelected = !currentValue;
            }
        }
    }
}