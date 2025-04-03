using System.Windows.Forms;

namespace SynchronizationApplication
{
    partial class FailedAutoSyncForm
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
            this.gridFailedChanges = new System.Windows.Forms.DataGridView();
            this.colSelect = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colLogID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCustID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colChangeType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colChangeTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colProcessedTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colErrorMessage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblOperationsCount = new System.Windows.Forms.Label();
            this.lblSelectedCount = new System.Windows.Forms.Label();
            this.lblRetryStatus = new System.Windows.Forms.Label();
            this.progressRetry = new System.Windows.Forms.ProgressBar();
            this.lblTotalCount = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnRetrySelected = new System.Windows.Forms.Button();
            this.chkSelectAll = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.gridFailedChanges)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // gridFailedChanges
            // 
            this.gridFailedChanges.AllowUserToAddRows = false;
            this.gridFailedChanges.AllowUserToDeleteRows = false;
            this.gridFailedChanges.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridFailedChanges.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridFailedChanges.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridFailedChanges.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colSelect,
            this.colLogID,
            this.colCustID,
            this.colChangeType,
            this.colChangeTime,
            this.colStatus,
            this.colProcessedTime,
            this.colErrorMessage});
            this.gridFailedChanges.Location = new System.Drawing.Point(12, 41);
            this.gridFailedChanges.Name = "gridFailedChanges";
            this.gridFailedChanges.RowHeadersWidth = 51;
            this.gridFailedChanges.RowTemplate.Height = 29;
            this.gridFailedChanges.Size = new System.Drawing.Size(876, 309);
            this.gridFailedChanges.TabIndex = 0;
            this.gridFailedChanges.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridFailedChanges_CellContentClick);
            // 
            // colSelect
            // 
            this.colSelect.FillWeight = 30F;
            this.colSelect.HeaderText = "Select";
            this.colSelect.MinimumWidth = 6;
            this.colSelect.Name = "colSelect";
            // 
            // colLogID
            // 
            this.colLogID.FillWeight = 40F;
            this.colLogID.HeaderText = "Log ID";
            this.colLogID.MinimumWidth = 6;
            this.colLogID.Name = "colLogID";
            this.colLogID.ReadOnly = true;
            // 
            // colCustID
            // 
            this.colCustID.FillWeight = 50F;
            this.colCustID.HeaderText = "Entity ID";
            this.colCustID.MinimumWidth = 6;
            this.colCustID.Name = "colCustID";
            this.colCustID.ReadOnly = true;
            // 
            // colChangeType
            // 
            this.colChangeType.FillWeight = 60F;
            this.colChangeType.HeaderText = "Change Type";
            this.colChangeType.MinimumWidth = 6;
            this.colChangeType.Name = "colChangeType";
            this.colChangeType.ReadOnly = true;
            // 
            // colChangeTime
            // 
            this.colChangeTime.FillWeight = 80F;
            this.colChangeTime.HeaderText = "Change Time";
            this.colChangeTime.MinimumWidth = 6;
            this.colChangeTime.Name = "colChangeTime";
            this.colChangeTime.ReadOnly = true;
            // 
            // colStatus
            // 
            this.colStatus.FillWeight = 60F;
            this.colStatus.HeaderText = "Status";
            this.colStatus.MinimumWidth = 6;
            this.colStatus.Name = "colStatus";
            this.colStatus.ReadOnly = true;
            // 
            // colProcessedTime
            // 
            this.colProcessedTime.FillWeight = 80F;
            this.colProcessedTime.HeaderText = "Processed Time";
            this.colProcessedTime.MinimumWidth = 6;
            this.colProcessedTime.Name = "colProcessedTime";
            this.colProcessedTime.ReadOnly = true;
            // 
            // colErrorMessage
            // 
            this.colErrorMessage.FillWeight = 250F;
            this.colErrorMessage.HeaderText = "Error Message";
            this.colErrorMessage.MinimumWidth = 6;
            this.colErrorMessage.Name = "colErrorMessage";
            this.colErrorMessage.ReadOnly = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblOperationsCount);
            this.panel1.Controls.Add(this.lblSelectedCount);
            this.panel1.Controls.Add(this.lblRetryStatus);
            this.panel1.Controls.Add(this.progressRetry);
            this.panel1.Controls.Add(this.lblTotalCount);
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Controls.Add(this.btnRetrySelected);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 356);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(900, 94);
            this.panel1.TabIndex = 1;
            // 
            // lblOperationsCount
            // 
            this.lblOperationsCount.AutoSize = true;
            this.lblOperationsCount.Location = new System.Drawing.Point(400, 65);
            this.lblOperationsCount.Name = "lblOperationsCount";
            this.lblOperationsCount.Size = new System.Drawing.Size(86, 20);
            this.lblOperationsCount.TabIndex = 7;
            this.lblOperationsCount.Text = "Operations:";
            // 
            // lblSelectedCount
            // 
            this.lblSelectedCount.AutoSize = true;
            this.lblSelectedCount.Location = new System.Drawing.Point(213, 65);
            this.lblSelectedCount.Name = "lblSelectedCount";
            this.lblSelectedCount.Size = new System.Drawing.Size(82, 20);
            this.lblSelectedCount.TabIndex = 6;
            this.lblSelectedCount.Text = "Selected: 0";
            // 
            // lblRetryStatus
            // 
            this.lblRetryStatus.AutoSize = true;
            this.lblRetryStatus.Location = new System.Drawing.Point(162, 65);
            this.lblRetryStatus.Name = "lblRetryStatus";
            this.lblRetryStatus.Size = new System.Drawing.Size(95, 20);
            this.lblRetryStatus.TabIndex = 5;
            this.lblRetryStatus.Text = "Processing...";
            this.lblRetryStatus.Visible = false;
            // 
            // progressRetry
            // 
            this.progressRetry.Location = new System.Drawing.Point(162, 14);
            this.progressRetry.Name = "progressRetry";
            this.progressRetry.Size = new System.Drawing.Size(574, 29);
            this.progressRetry.TabIndex = 4;
            this.progressRetry.Visible = false;
            // 
            // lblTotalCount
            // 
            this.lblTotalCount.AutoSize = true;
            this.lblTotalCount.Location = new System.Drawing.Point(12, 65);
            this.lblTotalCount.Name = "lblTotalCount";
            this.lblTotalCount.Size = new System.Drawing.Size(58, 20);
            this.lblTotalCount.TabIndex = 2;
            this.lblTotalCount.Text = "Total: 0";
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(751, 53);
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
            // FailedAutoSyncForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 450);
            this.Controls.Add(this.chkSelectAll);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.gridFailedChanges);
            this.MinimumSize = new System.Drawing.Size(800, 400);
            this.Name = "FailedAutoSyncForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Failed Auto-Sync Records";
            ((System.ComponentModel.ISupportInitialize)(this.gridFailedChanges)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private DataGridView gridFailedChanges;
        private DataGridViewCheckBoxColumn colSelect;
        private DataGridViewTextBoxColumn colLogID;
        private DataGridViewTextBoxColumn colCustID;
        private DataGridViewTextBoxColumn colChangeType;
        private DataGridViewTextBoxColumn colChangeTime;
        private DataGridViewTextBoxColumn colStatus;
        private DataGridViewTextBoxColumn colProcessedTime;
        private DataGridViewTextBoxColumn colErrorMessage;
        private Panel panel1;
        private Button btnRetrySelected;
        private Button btnClose;
        private Label lblOperationsCount;
        private Label lblSelectedCount;
        private Label lblRetryStatus;
        private ProgressBar progressRetry;
        private Label lblTotalCount;
        private CheckBox chkSelectAll;
    }
}