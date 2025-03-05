using System.ComponentModel;
using System.Windows.Forms;

namespace Agent
{
    public partial class GraphgenForm : Form
    {
        private IContainer components = null;

        private System.Windows.Forms.TextBox txtConfigFile;
        private System.Windows.Forms.NumericUpDown numIterations;
        private System.Windows.Forms.TextBox txtTableName;
        private System.Windows.Forms.CheckBox chkIncludeDb;
        private System.Windows.Forms.Button btnGenConfig;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.TextBox txtLog;

        private System.Windows.Forms.Button btnBrowseConfig;

        private System.Windows.Forms.TabControl tabControlMain;
        private TabPage tabPageGeneral;
        private TabPage tabPageAdvanced;
        private System.Windows.Forms.GroupBox grpPossibleRaises;
        private System.Windows.Forms.DataGridView dgvPossibleRaises;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBets;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPhase;
        private System.Windows.Forms.TableLayoutPanel advConfigLayout;
        private System.Windows.Forms.NumericUpDown numReraiseAmount;
        private System.Windows.Forms.NumericUpDown numNumPlayers;
        private System.Windows.Forms.NumericUpDown numSbValue;
        private System.Windows.Forms.NumericUpDown numBbValue;
        private System.Windows.Forms.NumericUpDown numBankroll;
        private System.Windows.Forms.CheckBox chkRelativeBetting;
        
        public GraphgenForm()
        {
            InitializeComponent();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tabControlMain = new System.Windows.Forms.TabControl();
            tabPageGeneral = new System.Windows.Forms.TabPage();
            generalLayout = new System.Windows.Forms.TableLayoutPanel();
            lblConfigFile = new System.Windows.Forms.Label();
            configFlow = new System.Windows.Forms.FlowLayoutPanel();
            txtConfigFile = new System.Windows.Forms.TextBox();
            btnBrowseConfig = new System.Windows.Forms.Button();
            btnOpenConfig = new System.Windows.Forms.Button();
            btnGenConfig = new System.Windows.Forms.Button();
            xmlFlow = new System.Windows.Forms.FlowLayoutPanel();
            btnGenXml = new System.Windows.Forms.Button();
            lblIterations = new System.Windows.Forms.Label();
            numIterations = new System.Windows.Forms.NumericUpDown();
            lblTableName = new System.Windows.Forms.Label();
            txtTableName = new System.Windows.Forms.TextBox();
            lblIncludeDb = new System.Windows.Forms.Label();
            chkIncludeDb = new System.Windows.Forms.CheckBox();
            buttonPanel = new System.Windows.Forms.FlowLayoutPanel();
            btnStart = new System.Windows.Forms.Button();
            progressBar = new System.Windows.Forms.ProgressBar();
            txtLog = new System.Windows.Forms.TextBox();
            lblProgress = new System.Windows.Forms.Label();
            tabPageAdvanced = new System.Windows.Forms.TabPage();
            advancedLayout = new System.Windows.Forms.TableLayoutPanel();
            grpPossibleRaises = new System.Windows.Forms.GroupBox();
            dgvPossibleRaises = new System.Windows.Forms.DataGridView();
            colPhase = new System.Windows.Forms.DataGridViewTextBoxColumn();
            colBets = new System.Windows.Forms.DataGridViewTextBoxColumn();
            advConfigLayout = new System.Windows.Forms.TableLayoutPanel();
            lblReraiseAmount = new System.Windows.Forms.Label();
            numReraiseAmount = new System.Windows.Forms.NumericUpDown();
            lblNumPlayers = new System.Windows.Forms.Label();
            numNumPlayers = new System.Windows.Forms.NumericUpDown();
            lblSbValue = new System.Windows.Forms.Label();
            numSbValue = new System.Windows.Forms.NumericUpDown();
            lblBbValue = new System.Windows.Forms.Label();
            numBbValue = new System.Windows.Forms.NumericUpDown();
            lblBankroll = new System.Windows.Forms.Label();
            numBankroll = new System.Windows.Forms.NumericUpDown();
            lblRelativeBetting = new System.Windows.Forms.Label();
            chkRelativeBetting = new System.Windows.Forms.CheckBox();
            tabControlMain.SuspendLayout();
            tabPageGeneral.SuspendLayout();
            generalLayout.SuspendLayout();
            configFlow.SuspendLayout();
            xmlFlow.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize) numIterations).BeginInit();
            buttonPanel.SuspendLayout();
            tabPageAdvanced.SuspendLayout();
            advancedLayout.SuspendLayout();
            grpPossibleRaises.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize) dgvPossibleRaises).BeginInit();
            advConfigLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize) numReraiseAmount).BeginInit();
            ((System.ComponentModel.ISupportInitialize) numNumPlayers).BeginInit();
            ((System.ComponentModel.ISupportInitialize) numSbValue).BeginInit();
            ((System.ComponentModel.ISupportInitialize) numBbValue).BeginInit();
            ((System.ComponentModel.ISupportInitialize) numBankroll).BeginInit();
            SuspendLayout();
            // 
            // tabControlMain
            // 
            tabControlMain.Controls.Add(tabPageGeneral);
            tabControlMain.Controls.Add(tabPageAdvanced);
            tabControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            tabControlMain.Location = new System.Drawing.Point(0, 0);
            tabControlMain.Name = "tabControlMain";
            tabControlMain.SelectedIndex = 0;
            tabControlMain.Size = new System.Drawing.Size(800, 659);
            tabControlMain.TabIndex = 0;
            // 
            // tabPageGeneral
            // 
            tabPageGeneral.Controls.Add(generalLayout);
            tabPageGeneral.Location = new System.Drawing.Point(4, 29);
            tabPageGeneral.Name = "tabPageGeneral";
            tabPageGeneral.Size = new System.Drawing.Size(792, 626);
            tabPageGeneral.TabIndex = 0;
            tabPageGeneral.Text = "General";
            // 
            // generalLayout
            // 
            generalLayout.ColumnCount = 2;
            generalLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 133F));
            generalLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 8F));
            generalLayout.Controls.Add(lblConfigFile, 0, 0);
            generalLayout.Controls.Add(configFlow, 1, 0);
            generalLayout.Controls.Add(xmlFlow, 1, 1);
            generalLayout.Controls.Add(lblIterations, 0, 2);
            generalLayout.Controls.Add(numIterations, 1, 2);
            generalLayout.Controls.Add(lblTableName, 0, 3);
            generalLayout.Controls.Add(txtTableName, 1, 3);
            generalLayout.Controls.Add(lblIncludeDb, 0, 4);
            generalLayout.Controls.Add(chkIncludeDb, 1, 4);
            generalLayout.Controls.Add(buttonPanel, 1, 5);
            generalLayout.Controls.Add(progressBar, 1, 6);
            generalLayout.Controls.Add(txtLog, 1, 7);
            generalLayout.Controls.Add(lblProgress, 0, 6);
            generalLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            generalLayout.Location = new System.Drawing.Point(0, 0);
            generalLayout.Name = "generalLayout";
            generalLayout.Padding = new System.Windows.Forms.Padding(10);
            generalLayout.RowCount = 8;
            generalLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 44F));
            generalLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 44F));
            generalLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            generalLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            generalLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            generalLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 66F));
            generalLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 64F));
            generalLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 92F));
            generalLayout.Size = new System.Drawing.Size(792, 626);
            generalLayout.TabIndex = 0;
            // 
            // lblConfigFile
            // 
            lblConfigFile.AutoSize = true;
            lblConfigFile.Location = new System.Drawing.Point(13, 10);
            lblConfigFile.Name = "lblConfigFile";
            lblConfigFile.Size = new System.Drawing.Size(83, 40);
            lblConfigFile.TabIndex = 0;
            lblConfigFile.Text = "Config File:\r\n(optional)";
            // 
            // configFlow
            // 
            configFlow.Controls.Add(txtConfigFile);
            configFlow.Controls.Add(btnBrowseConfig);
            configFlow.Controls.Add(btnOpenConfig);
            configFlow.Controls.Add(btnGenConfig);
            configFlow.Dock = System.Windows.Forms.DockStyle.Fill;
            configFlow.Location = new System.Drawing.Point(146, 13);
            configFlow.Name = "configFlow";
            configFlow.Size = new System.Drawing.Size(633, 38);
            configFlow.TabIndex = 1;
            // 
            // txtConfigFile
            // 
            txtConfigFile.Dock = System.Windows.Forms.DockStyle.Fill;
            txtConfigFile.Location = new System.Drawing.Point(3, 3);
            txtConfigFile.Name = "txtConfigFile";
            txtConfigFile.ReadOnly = true;
            txtConfigFile.Size = new System.Drawing.Size(305, 27);
            txtConfigFile.TabIndex = 0;
            // 
            // btnBrowseConfig
            // 
            btnBrowseConfig.Location = new System.Drawing.Point(314, 3);
            btnBrowseConfig.Name = "btnBrowseConfig";
            btnBrowseConfig.Size = new System.Drawing.Size(100, 28);
            btnBrowseConfig.TabIndex = 1;
            btnBrowseConfig.Text = "Browse...";
            btnBrowseConfig.Click += btnBrowseConfig_Click;
            // 
            // btnOpenConfig
            // 
            btnOpenConfig.Location = new System.Drawing.Point(420, 3);
            btnOpenConfig.Name = "btnOpenConfig";
            btnOpenConfig.Size = new System.Drawing.Size(100, 28);
            btnOpenConfig.TabIndex = 2;
            btnOpenConfig.Text = "Settings";
            btnOpenConfig.Click += btnOpenConfig_Click;
            // 
            // btnGenConfig
            // 
            btnGenConfig.Anchor = ((System.Windows.Forms.AnchorStyles) (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right));
            btnGenConfig.Location = new System.Drawing.Point(526, 3);
            btnGenConfig.Name = "btnGenConfig";
            btnGenConfig.Size = new System.Drawing.Size(100, 28);
            btnGenConfig.TabIndex = 0;
            btnGenConfig.Text = "Save Config";
            btnGenConfig.Click += BtnGenConfig_Click;
            // 
            // xmlFlow
            // 
            xmlFlow.Controls.Add(btnGenXml);
            xmlFlow.Dock = System.Windows.Forms.DockStyle.Fill;
            xmlFlow.Location = new System.Drawing.Point(146, 57);
            xmlFlow.Name = "xmlFlow";
            xmlFlow.Size = new System.Drawing.Size(633, 38);
            xmlFlow.TabIndex = 3;
            // 
            // btnGenXml
            // 
            btnGenXml.Anchor = ((System.Windows.Forms.AnchorStyles) (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right));
            btnGenXml.Location = new System.Drawing.Point(3, 3);
            btnGenXml.Name = "btnGenXml";
            btnGenXml.Size = new System.Drawing.Size(630, 28);
            btnGenXml.TabIndex = 2;
            btnGenXml.Text = "Generate game tree XML";
            btnGenXml.Click += btnGenXml_Click;
            // 
            // lblIterations
            // 
            lblIterations.AutoSize = true;
            lblIterations.Location = new System.Drawing.Point(13, 98);
            lblIterations.Name = "lblIterations";
            lblIterations.Size = new System.Drawing.Size(74, 20);
            lblIterations.TabIndex = 4;
            lblIterations.Text = "Iterations:";
            // 
            // numIterations
            // 
            numIterations.Dock = System.Windows.Forms.DockStyle.Fill;
            numIterations.Location = new System.Drawing.Point(146, 101);
            numIterations.Maximum = new decimal(new int[] {1000000, 0, 0, 0});
            numIterations.Minimum = new decimal(new int[] {1, 0, 0, 0});
            numIterations.Name = "numIterations";
            numIterations.Size = new System.Drawing.Size(633, 27);
            numIterations.TabIndex = 5;
            numIterations.Value = new decimal(new int[] {5000, 0, 0, 0});
            // 
            // lblTableName
            // 
            lblTableName.AutoSize = true;
            lblTableName.Location = new System.Drawing.Point(13, 130);
            lblTableName.Name = "lblTableName";
            lblTableName.Size = new System.Drawing.Size(91, 20);
            lblTableName.TabIndex = 6;
            lblTableName.Text = "Name:";
            // 
            // txtTableName
            // 
            txtTableName.Dock = System.Windows.Forms.DockStyle.Fill;
            txtTableName.Location = new System.Drawing.Point(146, 133);
            txtTableName.Name = "txtTableName";
            txtTableName.Size = new System.Drawing.Size(633, 27);
            txtTableName.TabIndex = 7;
            txtTableName.Text = "data";
            // 
            // lblSilent
            // 
            lblIncludeDb.AutoSize = true;
            lblIncludeDb.Location = new System.Drawing.Point(13, 162);
            lblIncludeDb.Name = "lblIncludeDb";
            lblIncludeDb.Size = new System.Drawing.Size(92, 20);
            lblIncludeDb.TabIndex = 8;
            lblIncludeDb.Text = "Save nodes to SqlLiteDb";
            // 
            // chkSilent
            // 
            chkIncludeDb.Dock = System.Windows.Forms.DockStyle.Fill;
            chkIncludeDb.Location = new System.Drawing.Point(146, 165);
            chkIncludeDb.Name = "chkSilent";
            chkIncludeDb.Size = new System.Drawing.Size(633, 26);
            chkIncludeDb.TabIndex = 9;
            // 
            // buttonPanel
            // 
            buttonPanel.Controls.Add(btnStart);
            buttonPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            buttonPanel.Location = new System.Drawing.Point(146, 197);
            buttonPanel.Name = "buttonPanel";
            buttonPanel.Size = new System.Drawing.Size(633, 60);
            buttonPanel.TabIndex = 10;
            // 
            // btnStart
            // 
            btnStart.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            btnStart.Location = new System.Drawing.Point(0, 0);
            btnStart.Margin = new System.Windows.Forms.Padding(0);
            btnStart.Name = "btnStart";
            btnStart.Size = new System.Drawing.Size(630, 57);
            btnStart.TabIndex = 1;
            btnStart.Text = "Start Processing";
            btnStart.Click += BtnStart_Click;
            // 
            // progressBar
            // 
            progressBar.Dock = System.Windows.Forms.DockStyle.Fill;
            progressBar.Location = new System.Drawing.Point(146, 263);
            progressBar.Name = "progressBar";
            progressBar.Size = new System.Drawing.Size(633, 26);
            progressBar.TabIndex = 11;
            // 
            // txtLog
            // 
            txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            txtLog.Location = new System.Drawing.Point(146, 295);
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            txtLog.Size = new System.Drawing.Size(633, 318);
            txtLog.TabIndex = 12;
            // 
            // lblProgress
            // 
            lblProgress.AutoSize = true;
            lblProgress.Dock = System.Windows.Forms.DockStyle.Right;
            lblProgress.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            lblProgress.Location = new System.Drawing.Point(140, 260);
            lblProgress.Name = "lblProgress";
            lblProgress.Size = new System.Drawing.Size(0, 32);
            lblProgress.TabIndex = 13;
            lblProgress.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tabPageAdvanced
            // 
            tabPageAdvanced.Controls.Add(advancedLayout);
            tabPageAdvanced.Location = new System.Drawing.Point(4, 29);
            tabPageAdvanced.Name = "tabPageAdvanced";
            tabPageAdvanced.Size = new System.Drawing.Size(792, 626);
            tabPageAdvanced.TabIndex = 1;
            tabPageAdvanced.Text = "Betting settings";
            // 
            // advancedLayout
            // 
            advancedLayout.ColumnCount = 1;
            advancedLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            advancedLayout.Controls.Add(grpPossibleRaises, 0, 0);
            advancedLayout.Controls.Add(advConfigLayout, 0, 1);
            advancedLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            advancedLayout.Location = new System.Drawing.Point(0, 0);
            advancedLayout.Name = "advancedLayout";
            advancedLayout.Padding = new System.Windows.Forms.Padding(10);
            advancedLayout.RowCount = 2;
            advancedLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 243F));
            advancedLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 13F));
            advancedLayout.Size = new System.Drawing.Size(792, 626);
            advancedLayout.TabIndex = 0;
            // 
            // grpPossibleRaises
            // 
            grpPossibleRaises.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
            grpPossibleRaises.Controls.Add(dgvPossibleRaises);
            grpPossibleRaises.Location = new System.Drawing.Point(13, 13);
            grpPossibleRaises.Name = "grpPossibleRaises";
            grpPossibleRaises.Size = new System.Drawing.Size(766, 237);
            grpPossibleRaises.TabIndex = 0;
            grpPossibleRaises.TabStop = false;
            grpPossibleRaises.Text = "Possible Raises (comma separated)";
            // 
            // dgvPossibleRaises
            // 
            dgvPossibleRaises.AllowUserToAddRows = false;
            dgvPossibleRaises.AllowUserToDeleteRows = false;
            dgvPossibleRaises.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
            dgvPossibleRaises.AutoGenerateColumns = false;
            dgvPossibleRaises.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dgvPossibleRaises.ColumnHeadersHeight = 29;
            dgvPossibleRaises.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {colPhase, colBets});
            dgvPossibleRaises.Location = new System.Drawing.Point(6, 26);
            dgvPossibleRaises.Name = "dgvPossibleRaises";
            dgvPossibleRaises.RowHeadersVisible = false;
            dgvPossibleRaises.RowHeadersWidth = 51;
            dgvPossibleRaises.Size = new System.Drawing.Size(760, 205);
            dgvPossibleRaises.TabIndex = 0;
            // 
            // colPhase
            // 
            colPhase.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            colPhase.HeaderText = "Phase";
            colPhase.MinimumWidth = 6;
            colPhase.Name = "colPhase";
            colPhase.ReadOnly = true;
            colPhase.Width = 379;
            // 
            // colBets
            // 
            colBets.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            colBets.HeaderText = "Bets";
            colBets.MinimumWidth = 6;
            colBets.Name = "colBets";
            colBets.Width = 378;
            // 
            // advConfigLayout
            // 
            advConfigLayout.ColumnCount = 2;
            advConfigLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 178F));
            advConfigLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 160F));
            advConfigLayout.Controls.Add(lblReraiseAmount, 0, 0);
            advConfigLayout.Controls.Add(numReraiseAmount, 1, 0);
            advConfigLayout.Controls.Add(lblNumPlayers, 0, 1);
            advConfigLayout.Controls.Add(numNumPlayers, 1, 1);
            advConfigLayout.Controls.Add(lblSbValue, 0, 2);
            advConfigLayout.Controls.Add(numSbValue, 1, 2);
            advConfigLayout.Controls.Add(lblBbValue, 0, 3);
            advConfigLayout.Controls.Add(numBbValue, 1, 3);
            advConfigLayout.Controls.Add(lblBankroll, 0, 4);
            advConfigLayout.Controls.Add(numBankroll, 1, 4);
            advConfigLayout.Controls.Add(lblRelativeBetting, 0, 5);
            advConfigLayout.Controls.Add(chkRelativeBetting, 1, 5);
            advConfigLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            advConfigLayout.Location = new System.Drawing.Point(13, 256);
            advConfigLayout.Name = "advConfigLayout";
            advConfigLayout.Padding = new System.Windows.Forms.Padding(5);
            advConfigLayout.RowCount = 6;
            advConfigLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            advConfigLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            advConfigLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            advConfigLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            advConfigLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            advConfigLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            advConfigLayout.Size = new System.Drawing.Size(766, 357);
            advConfigLayout.TabIndex = 1;
            // 
            // lblReraiseAmount
            // 
            lblReraiseAmount.AutoSize = true;
            lblReraiseAmount.Location = new System.Drawing.Point(8, 5);
            lblReraiseAmount.Name = "lblReraiseAmount";
            lblReraiseAmount.Size = new System.Drawing.Size(117, 20);
            lblReraiseAmount.TabIndex = 0;
            lblReraiseAmount.Text = "Reraise Amount:";
            // 
            // numReraiseAmount
            // 
            numReraiseAmount.Dock = System.Windows.Forms.DockStyle.Fill;
            numReraiseAmount.Location = new System.Drawing.Point(186, 8);
            numReraiseAmount.Maximum = new decimal(new int[] {10, 0, 0, 0});
            numReraiseAmount.Name = "numReraiseAmount";
            numReraiseAmount.Size = new System.Drawing.Size(572, 27);
            numReraiseAmount.TabIndex = 1;
            numReraiseAmount.Value = new decimal(new int[] {2, 0, 0, 0});
            // 
            // lblNumPlayers
            // 
            lblNumPlayers.AutoSize = true;
            lblNumPlayers.Location = new System.Drawing.Point(8, 41);
            lblNumPlayers.Name = "lblNumPlayers";
            lblNumPlayers.Size = new System.Drawing.Size(94, 20);
            lblNumPlayers.TabIndex = 2;
            lblNumPlayers.Text = "Num Players:";
            // 
            // numNumPlayers
            // 
            numNumPlayers.Dock = System.Windows.Forms.DockStyle.Fill;
            numNumPlayers.Enabled = false;
            numNumPlayers.Location = new System.Drawing.Point(186, 44);
            numNumPlayers.Maximum = new decimal(new int[] {2, 0, 0, 0});
            numNumPlayers.Minimum = new decimal(new int[] {2, 0, 0, 0});
            numNumPlayers.Name = "numNumPlayers";
            numNumPlayers.Size = new System.Drawing.Size(572, 27);
            numNumPlayers.TabIndex = 3;
            numNumPlayers.Value = new decimal(new int[] {2, 0, 0, 0});
            // 
            // lblSbValue
            // 
            lblSbValue.AutoSize = true;
            lblSbValue.Location = new System.Drawing.Point(8, 77);
            lblSbValue.Name = "lblSbValue";
            lblSbValue.Size = new System.Drawing.Size(69, 20);
            lblSbValue.TabIndex = 4;
            lblSbValue.Text = "Sb Value:";
            // 
            // numSbValue
            // 
            numSbValue.Dock = System.Windows.Forms.DockStyle.Fill;
            numSbValue.Location = new System.Drawing.Point(186, 80);
            numSbValue.Name = "numSbValue";
            numSbValue.Size = new System.Drawing.Size(572, 27);
            numSbValue.TabIndex = 5;
            numSbValue.Value = new decimal(new int[] {1, 0, 0, 0});
            // 
            // lblBbValue
            // 
            lblBbValue.AutoSize = true;
            lblBbValue.Location = new System.Drawing.Point(8, 113);
            lblBbValue.Name = "lblBbValue";
            lblBbValue.Size = new System.Drawing.Size(70, 20);
            lblBbValue.TabIndex = 6;
            lblBbValue.Text = "Bb Value:";
            // 
            // numBbValue
            // 
            numBbValue.Dock = System.Windows.Forms.DockStyle.Fill;
            numBbValue.Location = new System.Drawing.Point(186, 116);
            numBbValue.Name = "numBbValue";
            numBbValue.Size = new System.Drawing.Size(572, 27);
            numBbValue.TabIndex = 7;
            numBbValue.Value = new decimal(new int[] {2, 0, 0, 0});
            // 
            // lblBankroll
            // 
            lblBankroll.AutoSize = true;
            lblBankroll.Location = new System.Drawing.Point(8, 149);
            lblBankroll.Name = "lblBankroll";
            lblBankroll.Size = new System.Drawing.Size(66, 20);
            lblBankroll.TabIndex = 8;
            lblBankroll.Text = "Bankroll:";
            // 
            // numBankroll
            // 
            numBankroll.Dock = System.Windows.Forms.DockStyle.Fill;
            numBankroll.Location = new System.Drawing.Point(186, 152);
            numBankroll.Maximum = new decimal(new int[] {100000, 0, 0, 0});
            numBankroll.Name = "numBankroll";
            numBankroll.Size = new System.Drawing.Size(572, 27);
            numBankroll.TabIndex = 9;
            numBankroll.Value = new decimal(new int[] {100, 0, 0, 0});
            // 
            // lblRelativeBetting
            // 
            lblRelativeBetting.AutoSize = true;
            lblRelativeBetting.Location = new System.Drawing.Point(8, 185);
            lblRelativeBetting.Name = "lblRelativeBetting";
            lblRelativeBetting.Size = new System.Drawing.Size(156, 80);
            lblRelativeBetting.TabIndex = 10;
            lblRelativeBetting.Text = "Relative Betting:\r\n(if checked, betting is percentage of current pot)";
            // 
            // chkRelativeBetting
            // 
            chkRelativeBetting.Checked = true;
            chkRelativeBetting.CheckState = System.Windows.Forms.CheckState.Checked;
            chkRelativeBetting.Dock = System.Windows.Forms.DockStyle.Top;
            chkRelativeBetting.Location = new System.Drawing.Point(186, 188);
            chkRelativeBetting.Name = "chkRelativeBetting";
            chkRelativeBetting.Size = new System.Drawing.Size(572, 26);
            chkRelativeBetting.TabIndex = 11;
            chkRelativeBetting.CheckedChanged += chkRelativeBetting_CheckedChanged;
            // 
            // GraphgenForm
            // 
            ClientSize = new System.Drawing.Size(800, 659);
            Controls.Add(tabControlMain);
            Text = "CFR Solver";
            Load += GraphgenForm_Load;
            tabControlMain.ResumeLayout(false);
            tabPageGeneral.ResumeLayout(false);
            generalLayout.ResumeLayout(false);
            generalLayout.PerformLayout();
            configFlow.ResumeLayout(false);
            configFlow.PerformLayout();
            xmlFlow.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize) numIterations).EndInit();
            buttonPanel.ResumeLayout(false);
            tabPageAdvanced.ResumeLayout(false);
            advancedLayout.ResumeLayout(false);
            grpPossibleRaises.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize) dgvPossibleRaises).EndInit();
            advConfigLayout.ResumeLayout(false);
            advConfigLayout.PerformLayout();
            ((System.ComponentModel.ISupportInitialize) numReraiseAmount).EndInit();
            ((System.ComponentModel.ISupportInitialize) numNumPlayers).EndInit();
            ((System.ComponentModel.ISupportInitialize) numSbValue).EndInit();
            ((System.ComponentModel.ISupportInitialize) numBbValue).EndInit();
            ((System.ComponentModel.ISupportInitialize) numBankroll).EndInit();
            ResumeLayout(false);
        }

        private System.Windows.Forms.Label lblProgress;

        private System.Windows.Forms.Button btnGenXml;

        private System.Windows.Forms.Button btnOpenConfig;

        private System.Windows.Forms.TableLayoutPanel generalLayout;
        private System.Windows.Forms.Label lblConfigFile;
        private System.Windows.Forms.FlowLayoutPanel configFlow;
        private System.Windows.Forms.FlowLayoutPanel xmlFlow;
        private System.Windows.Forms.Label lblIterations;
        private System.Windows.Forms.Label lblTableName;
        private System.Windows.Forms.Label lblIncludeDb;
        private System.Windows.Forms.FlowLayoutPanel buttonPanel;
        private System.Windows.Forms.Label lblReraiseAmount;
        private System.Windows.Forms.Label lblNumPlayers;
        private System.Windows.Forms.Label lblSbValue;
        private System.Windows.Forms.Label lblBbValue;
        private System.Windows.Forms.Label lblBankroll;
        private System.Windows.Forms.Label lblRelativeBetting;
        private System.Windows.Forms.TableLayoutPanel advancedLayout;

        #endregion
    }
}
