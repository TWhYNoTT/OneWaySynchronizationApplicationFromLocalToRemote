using System.Windows.Forms;

namespace SynchronizationApplication
{
    partial class FailedRecordsForm
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
            this.colCustID.HeaderText = "Entity ID";
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

        #endregion

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