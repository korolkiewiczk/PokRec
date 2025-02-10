using System.Windows.Forms;

namespace Agent.CfrSolver.Graphgen
{
    partial class GraphgenForm
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

        private TextBox txtConfigFile;
        private TextBox txtXmlFile;
        private NumericUpDown numIterations;
        private TextBox txtTableName;
        private CheckBox chkSilent;
        private Button btnGenConfig;
        private Button btnStart;
        private ProgressBar progressBar;
        private TextBox txtLog;

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Text = "CFR Solver";
            this.Size = new System.Drawing.Size(800, 600);

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 8,
                Padding = new Padding(10)
            };

            // Config File
            layout.Controls.Add(new Label { Text = "Config File:" }, 0, 0);
            txtConfigFile = new TextBox { Dock = DockStyle.Fill };
            var configPanel = new Panel { Dock = DockStyle.Fill };
            var btnBrowseConfig = new Button { Text = "Browse...", Width = 80 };
            btnBrowseConfig.Click += (s, e) => BrowseFile(txtConfigFile, "JSON files|*.json");
            configPanel.Controls.AddRange(new Control[] { txtConfigFile, btnBrowseConfig });
            layout.Controls.Add(configPanel, 1, 0);

            // XML File
            layout.Controls.Add(new Label { Text = "XML Output File:" }, 0, 1);
            txtXmlFile = new TextBox { Dock = DockStyle.Fill };
            var xmlPanel = new Panel { Dock = DockStyle.Fill };
            var btnBrowseXml = new Button { Text = "Browse...", Width = 80 };
            btnBrowseXml.Click += (s, e) => BrowseFile(txtXmlFile, "XML files|*.xml");
            xmlPanel.Controls.AddRange(new Control[] { txtXmlFile, btnBrowseXml });
            layout.Controls.Add(xmlPanel, 1, 1);

            // Iterations
            layout.Controls.Add(new Label { Text = "Iterations:" }, 0, 2);
            numIterations = new NumericUpDown { 
                Minimum = 1,
                Maximum = 1000000,
                Value = 5000,
                Dock = DockStyle.Fill
            };
            layout.Controls.Add(numIterations, 1, 2);

            // Table Name
            layout.Controls.Add(new Label { Text = "Table Name:" }, 0, 3);
            txtTableName = new TextBox { Text = "nodes1", Dock = DockStyle.Fill };
            layout.Controls.Add(txtTableName, 1, 3);

            // Silent Mode
            layout.Controls.Add(new Label { Text = "Silent Mode:" }, 0, 4);
            chkSilent = new CheckBox { Dock = DockStyle.Fill };
            layout.Controls.Add(chkSilent, 1, 4);

            // Buttons
            var buttonPanel = new FlowLayoutPanel { Dock = DockStyle.Fill };
            btnGenConfig = new Button { Text = "Generate Config", Width = 100 };
            btnStart = new Button { Text = "Start Processing", Width = 100 };
            btnGenConfig.Click += BtnGenConfig_Click;
            btnStart.Click += BtnStart_Click;
            buttonPanel.Controls.AddRange(new Control[] { btnGenConfig, btnStart });
            layout.Controls.Add(buttonPanel, 1, 5);

            // Progress Bar
            progressBar = new ProgressBar { Dock = DockStyle.Fill };
            layout.Controls.Add(progressBar, 1, 6);

            // Log TextBox
            txtLog = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Dock = DockStyle.Fill,
                Height = 200
            };
            layout.Controls.Add(txtLog, 1, 7);

            this.Controls.Add(layout);
        }

        #endregion
    }
} 