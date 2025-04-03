using SynchronizationApplication.Models;
using SynchronizationApplication.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SynchronizationApplication
{
    public partial class SyncResultsForm : Form
    {
        private readonly SyncResult _result;
        private readonly ILogService _logService;

        public SyncResultsForm(SyncResult result, ILogService logService)
        {
            InitializeComponent();

            _result = result;
            _logService = logService;

            PopulateUI();
        }

        private void PopulateUI()
        {
            string entityName = _result.EntityType == EntityType.Customer ? "Customer" : "User";
            this.Text = $"{entityName} Synchronization Results";

            lblTotalRecords.Text = $"Total Records: {_result.TotalRecords}";
            lblProcessedRecords.Text = $"Processed Records: {_result.ProcessedRecords}";
            lblAddedRecords.Text = $"Added Records: {_result.AddedRecords}";
            lblUpdatedRecords.Text = $"Updated Records: {_result.UpdatedRecords}";
            lblSkippedRecords.Text = $"Skipped Records: {_result.SkippedRecords}";
            lblErrorRecords.Text = $"Records with Errors: {_result.ErrorRecords}";

            // Populate error list
            if (_result.ErrorMessages.Count > 0)
            {
                txtErrors.Text = string.Join(Environment.NewLine, _result.ErrorMessages);
                txtErrors.SelectionStart = 0;
                txtErrors.SelectionLength = 0;
            }
            else
            {
                txtErrors.Text = "No errors occurred during synchronization.";
            }

            // Generate a summary message and set icon
            if (_result.ErrorRecords == 0)
            {
                if (_result.ProcessedRecords == _result.TotalRecords)
                {
                    lblSummary.Text = $"All {entityName.ToLower()} records were successfully synchronized.";
                    picResult.Image = SystemIcons.Information.ToBitmap();
                }
                else
                {
                    lblSummary.Text = $"{entityName} synchronization completed with warnings.";
                    picResult.Image = SystemIcons.Warning.ToBitmap();
                }
            }
            else
            {
                lblSummary.Text = $"{entityName} synchronization completed with errors.";
                picResult.Image = SystemIcons.Error.ToBitmap();
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private async void btnSaveLog_Click(object sender, EventArgs e)
        {
            var saveDialog = new SaveFileDialog
            {
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                DefaultExt = "txt",
                FileName = $"{_result.EntityType}_Sync_Log_{DateTime.Now:yyyyMMdd_HHmmss}"
            };

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    await _logService.SaveLogAsync(saveDialog.FileName, _result);
                    MessageBox.Show("Log file saved successfully.", "Save Log", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving log file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}