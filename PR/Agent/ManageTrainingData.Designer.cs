namespace Agent
{
    partial class ManageTrainingData
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.ListBox listBoxFiles;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Button btnRefresh;

        // GroupBox for Config with TableLayoutPanel
        private System.Windows.Forms.GroupBox groupBoxConfig;
        private System.Windows.Forms.TableLayoutPanel generalLayout;
        private System.Windows.Forms.Label lblNumPlayers;
        private System.Windows.Forms.TextBox textBoxNumPlayers;
        private System.Windows.Forms.Label lblSbValue;
        private System.Windows.Forms.TextBox textBoxSbValue;
        private System.Windows.Forms.Label lblBbValue;
        private System.Windows.Forms.TextBox textBoxBbValue;
        private System.Windows.Forms.Label lblBankroll;
        private System.Windows.Forms.TextBox textBoxBankroll;
        private System.Windows.Forms.Label lblReraiseAmount;
        private System.Windows.Forms.TextBox textBoxReraiseAmount;
        private System.Windows.Forms.Label lblRelativeBetting;
        private System.Windows.Forms.TextBox textBoxRelativeBetting;
        private System.Windows.Forms.Label lblPossibleRaises;
        private System.Windows.Forms.TextBox textBoxPossibleRaises;

        // GroupBox for Metadata with TableLayoutPanel
        private System.Windows.Forms.GroupBox groupBoxMetadata;
        private System.Windows.Forms.TableLayoutPanel metaLayout;
        private System.Windows.Forms.Label lblIterations;
        private System.Windows.Forms.TextBox textBoxIterations;

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
            listBoxFiles = new System.Windows.Forms.ListBox();
            btnOpen = new System.Windows.Forms.Button();
            btnRefresh = new System.Windows.Forms.Button();
            groupBoxConfig = new System.Windows.Forms.GroupBox();
            generalLayout = new System.Windows.Forms.TableLayoutPanel();
            lblNumPlayers = new System.Windows.Forms.Label();
            textBoxNumPlayers = new System.Windows.Forms.TextBox();
            lblSbValue = new System.Windows.Forms.Label();
            textBoxSbValue = new System.Windows.Forms.TextBox();
            lblBbValue = new System.Windows.Forms.Label();
            textBoxBbValue = new System.Windows.Forms.TextBox();
            lblBankroll = new System.Windows.Forms.Label();
            textBoxBankroll = new System.Windows.Forms.TextBox();
            lblReraiseAmount = new System.Windows.Forms.Label();
            textBoxReraiseAmount = new System.Windows.Forms.TextBox();
            lblRelativeBetting = new System.Windows.Forms.Label();
            textBoxRelativeBetting = new System.Windows.Forms.TextBox();
            lblPossibleRaises = new System.Windows.Forms.Label();
            textBoxPossibleRaises = new System.Windows.Forms.TextBox();
            groupBoxMetadata = new System.Windows.Forms.GroupBox();
            metaLayout = new System.Windows.Forms.TableLayoutPanel();
            lblIterations = new System.Windows.Forms.Label();
            textBoxIterations = new System.Windows.Forms.TextBox();
            groupBoxConfig.SuspendLayout();
            generalLayout.SuspendLayout();
            groupBoxMetadata.SuspendLayout();
            metaLayout.SuspendLayout();
            SuspendLayout();
            // 
            // listBoxFiles
            // 
            listBoxFiles.FormattingEnabled = true;
            listBoxFiles.Location = new System.Drawing.Point(12, 12);
            listBoxFiles.Name = "listBoxFiles";
            listBoxFiles.Size = new System.Drawing.Size(300, 384);
            listBoxFiles.TabIndex = 0;
            listBoxFiles.SelectedIndexChanged += listBoxFiles_SelectedIndexChanged;
            // 
            // btnOpen
            // 
            btnOpen.Location = new System.Drawing.Point(170, 411);
            btnOpen.Name = "btnOpen";
            btnOpen.Size = new System.Drawing.Size(142, 32);
            btnOpen.TabIndex = 1;
            btnOpen.Text = "Open";
            btnOpen.UseVisualStyleBackColor = true;
            btnOpen.Click += btnOpen_Click;
            // 
            // btnRefresh
            // 
            btnRefresh.Location = new System.Drawing.Point(12, 411);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new System.Drawing.Size(142, 32);
            btnRefresh.TabIndex = 2;
            btnRefresh.Text = "Refresh";
            btnRefresh.UseVisualStyleBackColor = true;
            btnRefresh.Click += btnRefresh_Click;
            // 
            // groupBoxConfig
            // 
            groupBoxConfig.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
            groupBoxConfig.Controls.Add(generalLayout);
            groupBoxConfig.Location = new System.Drawing.Point(330, 12);
            groupBoxConfig.Name = "groupBoxConfig";
            groupBoxConfig.Size = new System.Drawing.Size(582, 277);
            groupBoxConfig.TabIndex = 3;
            groupBoxConfig.TabStop = false;
            groupBoxConfig.Text = "Config";
            // 
            // generalLayout
            // 
            generalLayout.ColumnCount = 2;
            generalLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            generalLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            generalLayout.Controls.Add(lblNumPlayers, 0, 0);
            generalLayout.Controls.Add(textBoxNumPlayers, 1, 0);
            generalLayout.Controls.Add(lblSbValue, 0, 1);
            generalLayout.Controls.Add(textBoxSbValue, 1, 1);
            generalLayout.Controls.Add(lblBbValue, 0, 2);
            generalLayout.Controls.Add(textBoxBbValue, 1, 2);
            generalLayout.Controls.Add(lblBankroll, 0, 3);
            generalLayout.Controls.Add(textBoxBankroll, 1, 3);
            generalLayout.Controls.Add(lblReraiseAmount, 0, 4);
            generalLayout.Controls.Add(textBoxReraiseAmount, 1, 4);
            generalLayout.Controls.Add(lblRelativeBetting, 0, 5);
            generalLayout.Controls.Add(textBoxRelativeBetting, 1, 5);
            generalLayout.Controls.Add(lblPossibleRaises, 0, 6);
            generalLayout.Controls.Add(textBoxPossibleRaises, 1, 6);
            generalLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            generalLayout.Location = new System.Drawing.Point(3, 23);
            generalLayout.Name = "generalLayout";
            generalLayout.RowCount = 7;
            generalLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28F));
            generalLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28F));
            generalLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28F));
            generalLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28F));
            generalLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28F));
            generalLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28F));
            generalLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28F));
            generalLayout.Size = new System.Drawing.Size(576, 251);
            generalLayout.TabIndex = 0;
            // 
            // lblNumPlayers
            // 
            lblNumPlayers.AutoSize = true;
            lblNumPlayers.Dock = System.Windows.Forms.DockStyle.Fill;
            lblNumPlayers.Location = new System.Drawing.Point(3, 0);
            lblNumPlayers.Name = "lblNumPlayers";
            lblNumPlayers.Size = new System.Drawing.Size(224, 35);
            lblNumPlayers.TabIndex = 0;
            lblNumPlayers.Text = "Num Players:";
            lblNumPlayers.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBoxNumPlayers
            // 
            textBoxNumPlayers.Dock = System.Windows.Forms.DockStyle.Fill;
            textBoxNumPlayers.Location = new System.Drawing.Point(233, 3);
            textBoxNumPlayers.Name = "textBoxNumPlayers";
            textBoxNumPlayers.ReadOnly = true;
            textBoxNumPlayers.Size = new System.Drawing.Size(340, 27);
            textBoxNumPlayers.TabIndex = 1;
            // 
            // lblSbValue
            // 
            lblSbValue.AutoSize = true;
            lblSbValue.Dock = System.Windows.Forms.DockStyle.Fill;
            lblSbValue.Location = new System.Drawing.Point(3, 35);
            lblSbValue.Name = "lblSbValue";
            lblSbValue.Size = new System.Drawing.Size(224, 35);
            lblSbValue.TabIndex = 2;
            lblSbValue.Text = "Small Blind:";
            lblSbValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBoxSbValue
            // 
            textBoxSbValue.Dock = System.Windows.Forms.DockStyle.Fill;
            textBoxSbValue.Location = new System.Drawing.Point(233, 38);
            textBoxSbValue.Name = "textBoxSbValue";
            textBoxSbValue.ReadOnly = true;
            textBoxSbValue.Size = new System.Drawing.Size(340, 27);
            textBoxSbValue.TabIndex = 3;
            // 
            // lblBbValue
            // 
            lblBbValue.AutoSize = true;
            lblBbValue.Dock = System.Windows.Forms.DockStyle.Fill;
            lblBbValue.Location = new System.Drawing.Point(3, 70);
            lblBbValue.Name = "lblBbValue";
            lblBbValue.Size = new System.Drawing.Size(224, 35);
            lblBbValue.TabIndex = 4;
            lblBbValue.Text = "Big Blind:";
            lblBbValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBoxBbValue
            // 
            textBoxBbValue.Dock = System.Windows.Forms.DockStyle.Fill;
            textBoxBbValue.Location = new System.Drawing.Point(233, 73);
            textBoxBbValue.Name = "textBoxBbValue";
            textBoxBbValue.ReadOnly = true;
            textBoxBbValue.Size = new System.Drawing.Size(340, 27);
            textBoxBbValue.TabIndex = 5;
            // 
            // lblBankroll
            // 
            lblBankroll.AutoSize = true;
            lblBankroll.Dock = System.Windows.Forms.DockStyle.Fill;
            lblBankroll.Location = new System.Drawing.Point(3, 105);
            lblBankroll.Name = "lblBankroll";
            lblBankroll.Size = new System.Drawing.Size(224, 35);
            lblBankroll.TabIndex = 6;
            lblBankroll.Text = "Bankroll:";
            lblBankroll.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBoxBankroll
            // 
            textBoxBankroll.Dock = System.Windows.Forms.DockStyle.Fill;
            textBoxBankroll.Location = new System.Drawing.Point(233, 108);
            textBoxBankroll.Name = "textBoxBankroll";
            textBoxBankroll.ReadOnly = true;
            textBoxBankroll.Size = new System.Drawing.Size(340, 27);
            textBoxBankroll.TabIndex = 7;
            // 
            // lblReraiseAmount
            // 
            lblReraiseAmount.AutoSize = true;
            lblReraiseAmount.Dock = System.Windows.Forms.DockStyle.Fill;
            lblReraiseAmount.Location = new System.Drawing.Point(3, 140);
            lblReraiseAmount.Name = "lblReraiseAmount";
            lblReraiseAmount.Size = new System.Drawing.Size(224, 35);
            lblReraiseAmount.TabIndex = 8;
            lblReraiseAmount.Text = "Reraise Amt:";
            lblReraiseAmount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBoxReraiseAmount
            // 
            textBoxReraiseAmount.Dock = System.Windows.Forms.DockStyle.Fill;
            textBoxReraiseAmount.Location = new System.Drawing.Point(233, 143);
            textBoxReraiseAmount.Name = "textBoxReraiseAmount";
            textBoxReraiseAmount.ReadOnly = true;
            textBoxReraiseAmount.Size = new System.Drawing.Size(340, 27);
            textBoxReraiseAmount.TabIndex = 9;
            // 
            // lblRelativeBetting
            // 
            lblRelativeBetting.AutoSize = true;
            lblRelativeBetting.Dock = System.Windows.Forms.DockStyle.Fill;
            lblRelativeBetting.Location = new System.Drawing.Point(3, 175);
            lblRelativeBetting.Name = "lblRelativeBetting";
            lblRelativeBetting.Size = new System.Drawing.Size(224, 35);
            lblRelativeBetting.TabIndex = 10;
            lblRelativeBetting.Text = "Relative Betting:";
            lblRelativeBetting.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBoxRelativeBetting
            // 
            textBoxRelativeBetting.Dock = System.Windows.Forms.DockStyle.Fill;
            textBoxRelativeBetting.Location = new System.Drawing.Point(233, 178);
            textBoxRelativeBetting.Name = "textBoxRelativeBetting";
            textBoxRelativeBetting.ReadOnly = true;
            textBoxRelativeBetting.Size = new System.Drawing.Size(340, 27);
            textBoxRelativeBetting.TabIndex = 11;
            // 
            // lblPossibleRaises
            // 
            lblPossibleRaises.AutoSize = true;
            lblPossibleRaises.Dock = System.Windows.Forms.DockStyle.Fill;
            lblPossibleRaises.Location = new System.Drawing.Point(3, 210);
            lblPossibleRaises.Name = "lblPossibleRaises";
            lblPossibleRaises.Size = new System.Drawing.Size(224, 41);
            lblPossibleRaises.TabIndex = 12;
            lblPossibleRaises.Text = "Possible Raises:";
            lblPossibleRaises.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBoxPossibleRaises
            // 
            textBoxPossibleRaises.Dock = System.Windows.Forms.DockStyle.Fill;
            textBoxPossibleRaises.Location = new System.Drawing.Point(233, 213);
            textBoxPossibleRaises.Name = "textBoxPossibleRaises";
            textBoxPossibleRaises.ReadOnly = true;
            textBoxPossibleRaises.Size = new System.Drawing.Size(340, 27);
            textBoxPossibleRaises.TabIndex = 13;
            // 
            // groupBoxMetadata
            // 
            groupBoxMetadata.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
            groupBoxMetadata.Controls.Add(metaLayout);
            groupBoxMetadata.Location = new System.Drawing.Point(330, 317);
            groupBoxMetadata.Name = "groupBoxMetadata";
            groupBoxMetadata.Size = new System.Drawing.Size(582, 79);
            groupBoxMetadata.TabIndex = 4;
            groupBoxMetadata.TabStop = false;
            groupBoxMetadata.Text = "Metadata";
            // 
            // metaLayout
            // 
            metaLayout.ColumnCount = 2;
            metaLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            metaLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            metaLayout.Controls.Add(lblIterations, 0, 0);
            metaLayout.Controls.Add(textBoxIterations, 1, 0);
            metaLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            metaLayout.Location = new System.Drawing.Point(3, 23);
            metaLayout.Name = "metaLayout";
            metaLayout.RowCount = 1;
            metaLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            metaLayout.Size = new System.Drawing.Size(576, 53);
            metaLayout.TabIndex = 0;
            // 
            // lblIterations
            // 
            lblIterations.AutoSize = true;
            lblIterations.Dock = System.Windows.Forms.DockStyle.Fill;
            lblIterations.Location = new System.Drawing.Point(3, 0);
            lblIterations.Name = "lblIterations";
            lblIterations.Size = new System.Drawing.Size(224, 53);
            lblIterations.TabIndex = 0;
            lblIterations.Text = "Iterations:";
            lblIterations.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBoxIterations
            // 
            textBoxIterations.Dock = System.Windows.Forms.DockStyle.Fill;
            textBoxIterations.Location = new System.Drawing.Point(233, 3);
            textBoxIterations.Name = "textBoxIterations";
            textBoxIterations.ReadOnly = true;
            textBoxIterations.Size = new System.Drawing.Size(340, 27);
            textBoxIterations.TabIndex = 1;
            // 
            // ManageTrainingData
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(932, 457);
            Controls.Add(groupBoxMetadata);
            Controls.Add(groupBoxConfig);
            Controls.Add(btnRefresh);
            Controls.Add(btnOpen);
            Controls.Add(listBoxFiles);
            Text = "Manage Training Data";
            Load += ManageTrainingData_Load;
            groupBoxConfig.ResumeLayout(false);
            generalLayout.ResumeLayout(false);
            generalLayout.PerformLayout();
            groupBoxMetadata.ResumeLayout(false);
            metaLayout.ResumeLayout(false);
            metaLayout.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
    }
}
