using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using SynchronizationApplication.Models;
using SynchronizationApplication.Services.Interfaces;

namespace SynchronizationApplication
{
    public partial class FailedRecordsForm : Form
    {
        private readonly List<FailedRecord> _failedRecords;
        private readonly ICustomerDataService _customerDataService;
        private readonly IDatabaseService _databaseService;
        private readonly AppSettings _appSettings;

        public FailedRecordsForm(
            List<FailedRecord> failedRecords,
            ICustomerDataService customerDataService,
            IDatabaseService databaseService,
            AppSettings appSettings)
        {
            InitializeComponent();

            _failedRecords = failedRecords;
            _customerDataService = customerDataService;
            _databaseService = databaseService;
            _appSettings = appSettings;

            PopulateGrid();
        }

        private void PopulateGrid()
        {
            // Clear existing data
            gridFailedRecords.Rows.Clear();

            foreach (var record in _failedRecords)
            {
                int rowIndex = gridFailedRecords.Rows.Add(
                    record.IsSelected,
                    record.CustID,
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
                        // Use a direct SQL approach instead of translating Customer to DataRow
                        using (var localConnection = new SqlConnection(localConnectionString))
                        {
                            await localConnection.OpenAsync();

                            // Get the data directly as a DataTable
                            string selectSql = "SELECT * FROM Customers WHERE CustID = @CustID";
                            using (var adapter = new SqlDataAdapter(selectSql, localConnection))
                            {
                                adapter.SelectCommand.Parameters.AddWithValue("@CustID", record.CustID);

                                var dataTable = new DataTable();
                                adapter.Fill(dataTable);

                                if (dataTable.Rows.Count > 0)
                                {
                                    var row = dataTable.Rows[0];

                                    // Check if exists on remote database
                                    bool exists = await _customerDataService.CustomerExistsAsync(remoteConnectionString, record.CustID);

                                    if (exists)
                                    {
                                        await _customerDataService.UpdateCustomerAsync(remoteConnectionString, row);
                                    }
                                    else
                                    {
                                        await _customerDataService.InsertCustomerAsync(remoteConnectionString, row);
                                    }

                                    // Mark as resolved
                                    record.IsResolved = true;
                                    successCount++;
                                }
                                else
                                {
                                    record.ErrorMessage = $"Customer with ID {record.CustID} not found in local database";
                                    record.Timestamp = DateTime.Now;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Update error message
                        record.ErrorMessage = ex.Message;
                        record.Timestamp = DateTime.Now;
                    }

                    Application.DoEvents();
                }

                // Save updated failed records
                await _customerDataService.SaveFailedRecordsAsync(
                    System.IO.Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                        "CustomerSync",
                        "failed_records.csv"),
                    _failedRecords);

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



        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void InitializeComponent()
        {
            this.gridFailedRecords = new System.Windows.Forms.DataGridView();
            this.colSelect = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colCustID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colError = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTimestamp = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colResolved = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblRetryStatus = new System.Windows.Forms.Label();
            this.progressRetry = new System.Windows.Forms.ProgressBar();
            this.lblPendingCount = new System.Windows.Forms.Label();
            this.lblResolvedCount = new System.Windows.Forms.Label();
            this.lblTotalCount = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnRetrySelected = new System.Windows.Forms.Button();
            this.chkSelectAll = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.gridFailedRecords)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // gridFailedRecords
            // 
            this.gridFailedRecords.AllowUserToAddRows = false;
            this.gridFailedRecords.AllowUserToDeleteRows = false;
            this.gridFailedRecords.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridFailedRecords.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridFailedRecords.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridFailedRecords.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colSelect,
            this.colCustID,
            this.colError,
            this.colTimestamp,
            this.colResolved});
            this.gridFailedRecords.Location = new System.Drawing.Point(12, 41);
            this.gridFailedRecords.Name = "gridFailedRecords";
            this.gridFailedRecords.RowHeadersWidth = 51;
            this.gridFailedRecords.RowTemplate.Height = 29;
            this.gridFailedRecords.Size = new System.Drawing.Size(776, 309);
            this.gridFailedRecords.TabIndex = 0;
            this.gridFailedRecords.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridFailedRecords_CellContentClick);
            // 
            // colSelect
            // 
            this.colSelect.FillWeight = 30F;
            this.colSelect.HeaderText = "Select";
            this.colSelect.MinimumWidth = 6;
            this.colSelect.Name = "colSelect";
            // 
            // colCustID
            // 
            this.colCustID.FillWeight = 50F;
            this.colCustID.HeaderText = "Customer ID";
            this.colCustID.MinimumWidth = 6;
            this.colCustID.Name = "colCustID";
            this.colCustID.ReadOnly = true;
            // 
            // colError
            // 
            this.colError.FillWeight = 250F;
            this.colError.HeaderText = "Error Message";
            this.colError.MinimumWidth = 6;
            this.colError.Name = "colError";
            this.colError.ReadOnly = true;
            // 
            // colTimestamp
            // 
            this.colTimestamp.FillWeight = 80F;
            this.colTimestamp.HeaderText = "Timestamp";
            this.colTimestamp.MinimumWidth = 6;
            this.colTimestamp.Name = "colTimestamp";
            this.colTimestamp.ReadOnly = true;
            // 
            // colResolved
            // 
            this.colResolved.FillWeight = 40F;
            this.colResolved.HeaderText = "Resolved";
            this.colResolved.MinimumWidth = 6;
            this.colResolved.Name = "colResolved";
            this.colResolved.ReadOnly = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblRetryStatus);
            this.panel1.Controls.Add(this.progressRetry);
            this.panel1.Controls.Add(this.lblPendingCount);
            this.panel1.Controls.Add(this.lblResolvedCount);
            this.panel1.Controls.Add(this.lblTotalCount);
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Controls.Add(this.btnRetrySelected);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 356);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(800, 94);
            this.panel1.TabIndex = 1;
            // 
            // lblRetryStatus
            // 
            this.lblRetryStatus.AutoSize = true;
            this.lblRetryStatus.Location = new System.Drawing.Point(162, 65);
            this.lblRetryStatus.Name = "lblRetryStatus";
            this.lblRetryStatus.Size = new System.Drawing.Size(95, 20);
            this.lblRetryStatus.TabIndex = 6;
            this.lblRetryStatus.Text = "Processing...";
            this.lblRetryStatus.Visible = false;
            // 
            // progressRetry
            // 
            this.progressRetry.Location = new System.Drawing.Point(162, 14);
            this.progressRetry.Name = "progressRetry";
            this.progressRetry.Size = new System.Drawing.Size(474, 29);
            this.progressRetry.TabIndex = 5;
            this.progressRetry.Visible = false;
            // 
            // lblPendingCount
            // 
            this.lblPendingCount.AutoSize = true;
            this.lblPendingCount.Location = new System.Drawing.Point(309, 65);
            this.lblPendingCount.Name = "lblPendingCount";
            this.lblPendingCount.Size = new System.Drawing.Size(79, 20);
            this.lblPendingCount.TabIndex = 4;
            this.lblPendingCount.Text = "Pending: 0";
            // 
            // lblResolvedCount
            // 
            this.lblResolvedCount.AutoSize = true;
            this.lblResolvedCount.Location = new System.Drawing.Point(213, 65);
            this.lblResolvedCount.Name = "lblResolvedCount";
            this.lblResolvedCount.Size = new System.Drawing.Size(84, 20);
            this.lblResolvedCount.TabIndex = 3;
            this.lblResolvedCount.Text = "Resolved: 0";
            // 
            // lblTotalCount
            // 
            this.lblTotalCount.AutoSize = true;
            this.lblTotalCount.Location = new System.Drawing.Point(12, 65);
            this.lblTotalCount.Name = "lblTotalCount";
            this.lblTotalCount.Size = new System.Drawing.Size(109, 20);
            this.lblTotalCount.TabIndex = 2;
            this.lblTotalCount.Text = "Total Records: 0";
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(651, 53);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(137, 29);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnRetrySelected
            // 
            this.btnRetrySelected.Location = new System.Drawing.Point(12, 14);
            this.btnRetrySelected.Name = "btnRetrySelected";
            this.btnRetrySelected.Size = new System.Drawing.Size(137, 29);
            this.btnRetrySelected.TabIndex = 0;
            this.btnRetrySelected.Text = "Retry Selected";
            this.btnRetrySelected.UseVisualStyleBackColor = true;
            this.btnRetrySelected.Click += new System.EventHandler(this.btnRetrySelected_Click);
            // 
            // chkSelectAll
            // 
            this.chkSelectAll.AutoSize = true;
            this.chkSelectAll.Location = new System.Drawing.Point(12, 12);
            this.chkSelectAll.Name = "chkSelectAll";
            this.chkSelectAll.Size = new System.Drawing.Size(99, 24);
            this.chkSelectAll.TabIndex = 2;
            this.chkSelectAll.Text = "Select All";
            this.chkSelectAll.UseVisualStyleBackColor = true;
            this.chkSelectAll.CheckedChanged += new System.EventHandler(this.chkSelectAll_CheckedChanged);
            // 
            // FailedRecordsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.chkSelectAll);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.gridFailedRecords);
            this.Name = "FailedRecordsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Failed Records";
            ((System.ComponentModel.ISupportInitialize)(this.gridFailedRecords)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

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

        private DataGridView gridFailedRecords;
        private DataGridViewCheckBoxColumn colSelect;
        private DataGridViewTextBoxColumn colCustID;
        private DataGridViewTextBoxColumn colError;
        private DataGridViewTextBoxColumn colTimestamp;
        private DataGridViewTextBoxColumn colResolved;
        private Panel panel1;
        private Button btnRetrySelected;
        private Button btnClose;
        private Label lblPendingCount;
        private Label lblResolvedCount;
        private Label lblTotalCount;
        private Label lblRetryStatus;
        private ProgressBar progressRetry;
        private CheckBox chkSelectAll;
    }
}