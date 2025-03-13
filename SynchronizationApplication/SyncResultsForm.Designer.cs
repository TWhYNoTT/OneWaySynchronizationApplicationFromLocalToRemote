namespace SynchronizationApplication
{
    partial class SyncResultsForm
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
            this.panelSummary = new System.Windows.Forms.Panel();
            this.lblErrorRecords = new System.Windows.Forms.Label();
            this.lblSkippedRecords = new System.Windows.Forms.Label();
            this.lblUpdatedRecords = new System.Windows.Forms.Label();
            this.lblAddedRecords = new System.Windows.Forms.Label();
            this.lblProcessedRecords = new System.Windows.Forms.Label();
            this.lblTotalRecords = new System.Windows.Forms.Label();
            this.lblSummary = new System.Windows.Forms.Label();
            this.picResult = new System.Windows.Forms.PictureBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtErrors = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnSaveLog = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.panelSummary.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picResult)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelSummary
            // 
            this.panelSummary.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelSummary.Controls.Add(this.lblErrorRecords);
            this.panelSummary.Controls.Add(this.lblSkippedRecords);
            this.panelSummary.Controls.Add(this.lblUpdatedRecords);
            this.panelSummary.Controls.Add(this.lblAddedRecords);
            this.panelSummary.Controls.Add(this.lblProcessedRecords);
            this.panelSummary.Controls.Add(this.lblTotalRecords);
            this.panelSummary.Controls.Add(this.lblSummary);
            this.panelSummary.Controls.Add(this.picResult);
            this.panelSummary.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelSummary.Location = new System.Drawing.Point(0, 0);
            this.panelSummary.Name = "panelSummary";
            this.panelSummary.Size = new System.Drawing.Size(582, 162);
            this.panelSummary.TabIndex = 0;
            // 
            // lblErrorRecords
            // 
            this.lblErrorRecords.AutoSize = true;
            this.lblErrorRecords.Location = new System.Drawing.Point(256, 130);
            this.lblErrorRecords.Name = "lblErrorRecords";
            this.lblErrorRecords.Size = new System.Drawing.Size(144, 20);
            this.lblErrorRecords.TabIndex = 7;
            this.lblErrorRecords.Text = "Records with Errors:";
            // 
            // lblSkippedRecords
            // 
            this.lblSkippedRecords.AutoSize = true;
            this.lblSkippedRecords.Location = new System.Drawing.Point(256, 110);
            this.lblSkippedRecords.Name = "lblSkippedRecords";
            this.lblSkippedRecords.Size = new System.Drawing.Size(128, 20);
            this.lblSkippedRecords.TabIndex = 6;
            this.lblSkippedRecords.Text = "Skipped Records:";
            // 
            // lblUpdatedRecords
            // 
            this.lblUpdatedRecords.AutoSize = true;
            this.lblUpdatedRecords.Location = new System.Drawing.Point(256, 90);
            this.lblUpdatedRecords.Name = "lblUpdatedRecords";
            this.lblUpdatedRecords.Size = new System.Drawing.Size(136, 20);
            this.lblUpdatedRecords.TabIndex = 5;
            this.lblUpdatedRecords.Text = "Updated Records:";
            // 
            // lblAddedRecords
            // 
            this.lblAddedRecords.AutoSize = true;
            this.lblAddedRecords.Location = new System.Drawing.Point(256, 70);
            this.lblAddedRecords.Name = "lblAddedRecords";
            this.lblAddedRecords.Size = new System.Drawing.Size(122, 20);
            this.lblAddedRecords.TabIndex = 4;
            this.lblAddedRecords.Text = "Added Records:";
            // 
            // lblProcessedRecords
            // 
            this.lblProcessedRecords.AutoSize = true;
            this.lblProcessedRecords.Location = new System.Drawing.Point(256, 50);
            this.lblProcessedRecords.Name = "lblProcessedRecords";
            this.lblProcessedRecords.Size = new System.Drawing.Size(143, 20);
            this.lblProcessedRecords.TabIndex = 3;
            this.lblProcessedRecords.Text = "Processed Records:";
            // 
            // lblTotalRecords
            // 
            this.lblTotalRecords.AutoSize = true;
            this.lblTotalRecords.Location = new System.Drawing.Point(256, 30);
            this.lblTotalRecords.Name = "lblTotalRecords";
            this.lblTotalRecords.Size = new System.Drawing.Size(108, 20);
            this.lblTotalRecords.TabIndex = 2;
            this.lblTotalRecords.Text = "Total Records:";
            // 
            // lblSummary
            // 
            this.lblSummary.AutoSize = true;
            this.lblSummary.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblSummary.Location = new System.Drawing.Point(75, 14);
            this.lblSummary.Name = "lblSummary";
            this.lblSummary.Size = new System.Drawing.Size(275, 23);
            this.lblSummary.TabIndex = 1;
            this.lblSummary.Text = "Synchronization Complete Status";
            // 
            // picResult
            // 
            this.picResult.Location = new System.Drawing.Point(13, 14);
            this.picResult.Name = "picResult";
            this.picResult.Size = new System.Drawing.Size(56, 56);
            this.picResult.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picResult.TabIndex = 0;
            this.picResult.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtErrors);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 162);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(582, 183);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Error Details";
            // 
            // txtErrors
            // 
            this.txtErrors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtErrors.Location = new System.Drawing.Point(3, 23);
            this.txtErrors.Multiline = true;
            this.txtErrors.Name = "txtErrors";
            this.txtErrors.ReadOnly = true;
            this.txtErrors.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtErrors.Size = new System.Drawing.Size(576, 157);
            this.txtErrors.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnSaveLog);
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 345);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(582, 53);
            this.panel1.TabIndex = 2;
            // 
            // btnSaveLog
            // 
            this.btnSaveLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSaveLog.Location = new System.Drawing.Point(13, 11);
            this.btnSaveLog.Name = "btnSaveLog";
            this.btnSaveLog.Size = new System.Drawing.Size(137, 29);
            this.btnSaveLog.TabIndex = 1;
            this.btnSaveLog.Text = "Save Log";
            this.btnSaveLog.UseVisualStyleBackColor = true;
            this.btnSaveLog.Click += new System.EventHandler(this.btnSaveLog_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(433, 11);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(137, 29);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // SyncResultsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(582, 398);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panelSummary);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SyncResultsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Synchronization Results";
            this.panelSummary.ResumeLayout(false);
            this.panelSummary.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picResult)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Panel panelSummary;
        private PictureBox picResult;
        private Label lblSummary;
        private GroupBox groupBox1;
        private Panel panel1;
        private Button btnClose;
        private TextBox txtErrors;
        private Label lblErrorRecords;
        private Label lblSkippedRecords;
        private Label lblUpdatedRecords;
        private Label lblAddedRecords;
        private Label lblProcessedRecords;
        private Label lblTotalRecords;
        private Button btnSaveLog;
    }
}