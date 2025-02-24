using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Agent
{
    partial class PokerStrategyForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;
        
        // Main layout container.
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        
        // Left panel controls.
        private Panel leftPanel;
        private System.Windows.Forms.GroupBox groupBoxCards;
        private System.Windows.Forms.TableLayoutPanel cardLayoutPanel;
        private Label lblPlayerHand;
        private System.Windows.Forms.FlowLayoutPanel panelPlayerHand;
        private ComboBox comboPlayerHand1;
        private ComboBox comboPlayerHand2;
        private System.Windows.Forms.Label lblFlop;
        private System.Windows.Forms.FlowLayoutPanel panelFlop;
        private System.Windows.Forms.ComboBox comboFlop1;
        private System.Windows.Forms.ComboBox comboFlop2;
        private System.Windows.Forms.ComboBox comboFlop3;
        private System.Windows.Forms.Label lblTurn;
        private System.Windows.Forms.ComboBox comboTurn;
        private System.Windows.Forms.Label lblRiver;
        private System.Windows.Forms.ComboBox comboRiver;
        private System.Windows.Forms.TextBox txtActions;
        
        private System.Windows.Forms.GroupBox groupBoxActions;
        private System.Windows.Forms.Button btnCalculate;
        
        private System.Windows.Forms.GroupBox groupBoxState;
        private System.Windows.Forms.Label lblState;
        
        // Right panel controls.
        private Panel rightPanel;
        private System.Windows.Forms.DataGridView dataGridViewStrategy;

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
            tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            leftPanel = new System.Windows.Forms.Panel();
            groupBoxState = new System.Windows.Forms.GroupBox();
            lblState = new System.Windows.Forms.Label();
            groupBoxCards = new System.Windows.Forms.GroupBox();
            cardLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            lblPlayerHand = new System.Windows.Forms.Label();
            panelPlayerHand = new System.Windows.Forms.FlowLayoutPanel();
            comboPlayerHand1 = new System.Windows.Forms.ComboBox();
            comboPlayerHand2 = new System.Windows.Forms.ComboBox();
            lblFlop = new System.Windows.Forms.Label();
            panelFlop = new System.Windows.Forms.FlowLayoutPanel();
            comboFlop1 = new System.Windows.Forms.ComboBox();
            comboFlop2 = new System.Windows.Forms.ComboBox();
            comboFlop3 = new System.Windows.Forms.ComboBox();
            lblTurn = new System.Windows.Forms.Label();
            comboTurn = new System.Windows.Forms.ComboBox();
            lblRiver = new System.Windows.Forms.Label();
            comboRiver = new System.Windows.Forms.ComboBox();
            groupBoxActions = new System.Windows.Forms.GroupBox();
            txtActions = new System.Windows.Forms.TextBox();
            btnCalculate = new System.Windows.Forms.Button();
            rightPanel = new System.Windows.Forms.Panel();
            dataGridViewStrategy = new System.Windows.Forms.DataGridView();
            tableLayoutPanel.SuspendLayout();
            leftPanel.SuspendLayout();
            groupBoxState.SuspendLayout();
            groupBoxCards.SuspendLayout();
            cardLayoutPanel.SuspendLayout();
            panelPlayerHand.SuspendLayout();
            panelFlop.SuspendLayout();
            groupBoxActions.SuspendLayout();
            rightPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize) dataGridViewStrategy).BeginInit();
            SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            tableLayoutPanel.ColumnCount = 2;
            tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel.Controls.Add(leftPanel, 0, 0);
            tableLayoutPanel.Controls.Add(rightPanel, 1, 0);
            tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 1;
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            tableLayoutPanel.Size = new System.Drawing.Size(800, 700);
            tableLayoutPanel.TabIndex = 0;
            // 
            // leftPanel
            // 
            leftPanel.Controls.Add(groupBoxState);
            leftPanel.Controls.Add(groupBoxCards);
            leftPanel.Controls.Add(groupBoxActions);
            leftPanel.Controls.Add(btnCalculate);
            leftPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            leftPanel.Location = new System.Drawing.Point(3, 3);
            leftPanel.Name = "leftPanel";
            leftPanel.Size = new System.Drawing.Size(394, 594);
            leftPanel.TabIndex = 0;
            // 
            // groupBoxState
            // 
            groupBoxState.Controls.Add(lblState);
            groupBoxState.Dock = System.Windows.Forms.DockStyle.Fill;
            groupBoxState.Location = new System.Drawing.Point(0, 319);
            groupBoxState.Name = "groupBoxState";
            groupBoxState.Size = new System.Drawing.Size(394, 275);
            groupBoxState.TabIndex = 0;
            groupBoxState.TabStop = false;
            groupBoxState.Text = "State";
            // 
            // lblState
            // 
            lblState.Dock = System.Windows.Forms.DockStyle.Fill;
            lblState.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte) 238));
            lblState.Location = new System.Drawing.Point(3, 23);
            lblState.Name = "lblState";
            lblState.Size = new System.Drawing.Size(388, 249);
            lblState.TabIndex = 0;
            // 
            // groupBoxCards
            // 
            groupBoxCards.Controls.Add(cardLayoutPanel);
            groupBoxCards.Dock = System.Windows.Forms.DockStyle.Top;
            groupBoxCards.Location = new System.Drawing.Point(0, 119);
            groupBoxCards.Name = "groupBoxCards";
            groupBoxCards.Size = new System.Drawing.Size(394, 170);
            groupBoxCards.TabIndex = 0;
            groupBoxCards.TabStop = false;
            groupBoxCards.Text = "Card Setup";
            // 
            // cardLayoutPanel
            // 
            cardLayoutPanel.ColumnCount = 2;
            cardLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            cardLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            cardLayoutPanel.Controls.Add(lblPlayerHand, 0, 0);
            cardLayoutPanel.Controls.Add(panelPlayerHand, 1, 0);
            cardLayoutPanel.Controls.Add(lblFlop, 0, 1);
            cardLayoutPanel.Controls.Add(panelFlop, 1, 1);
            cardLayoutPanel.Controls.Add(lblTurn, 0, 2);
            cardLayoutPanel.Controls.Add(comboTurn, 1, 2);
            cardLayoutPanel.Controls.Add(lblRiver, 0, 3);
            cardLayoutPanel.Controls.Add(comboRiver, 1, 3);
            cardLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            cardLayoutPanel.Location = new System.Drawing.Point(3, 23);
            cardLayoutPanel.Name = "cardLayoutPanel";
            cardLayoutPanel.RowCount = 4;
            cardLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            cardLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            cardLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            cardLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            cardLayoutPanel.Size = new System.Drawing.Size(388, 174);
            cardLayoutPanel.TabIndex = 0;
            // 
            // lblPlayerHand
            // 
            lblPlayerHand.Dock = System.Windows.Forms.DockStyle.Fill;
            lblPlayerHand.Location = new System.Drawing.Point(3, 0);
            lblPlayerHand.Name = "lblPlayerHand";
            lblPlayerHand.Size = new System.Drawing.Size(149, 32);
            lblPlayerHand.TabIndex = 0;
            lblPlayerHand.Text = "Player Hand:";
            // 
            // panelPlayerHand
            // 
            panelPlayerHand.Controls.Add(comboPlayerHand1);
            panelPlayerHand.Controls.Add(comboPlayerHand2);
            panelPlayerHand.Dock = System.Windows.Forms.DockStyle.Fill;
            panelPlayerHand.Location = new System.Drawing.Point(158, 3);
            panelPlayerHand.Name = "panelPlayerHand";
            panelPlayerHand.Size = new System.Drawing.Size(227, 26);
            panelPlayerHand.TabIndex = 1;
            // 
            // comboPlayerHand1
            // 
            comboPlayerHand1.Location = new System.Drawing.Point(3, 3);
            comboPlayerHand1.Name = "comboPlayerHand1";
            comboPlayerHand1.Size = new System.Drawing.Size(60, 28);
            comboPlayerHand1.TabIndex = 0;
            // 
            // comboPlayerHand2
            // 
            comboPlayerHand2.Location = new System.Drawing.Point(69, 3);
            comboPlayerHand2.Name = "comboPlayerHand2";
            comboPlayerHand2.Size = new System.Drawing.Size(60, 28);
            comboPlayerHand2.TabIndex = 1;
            // 
            // lblFlop
            // 
            lblFlop.Dock = System.Windows.Forms.DockStyle.Fill;
            lblFlop.Location = new System.Drawing.Point(3, 32);
            lblFlop.Name = "lblFlop";
            lblFlop.Size = new System.Drawing.Size(149, 32);
            lblFlop.TabIndex = 2;
            lblFlop.Text = "Flop:";
            // 
            // panelFlop
            // 
            panelFlop.Controls.Add(comboFlop1);
            panelFlop.Controls.Add(comboFlop2);
            panelFlop.Controls.Add(comboFlop3);
            panelFlop.Dock = System.Windows.Forms.DockStyle.Fill;
            panelFlop.Location = new System.Drawing.Point(158, 35);
            panelFlop.Name = "panelFlop";
            panelFlop.Size = new System.Drawing.Size(227, 26);
            panelFlop.TabIndex = 3;
            // 
            // comboFlop1
            // 
            comboFlop1.Location = new System.Drawing.Point(3, 3);
            comboFlop1.Name = "comboFlop1";
            comboFlop1.Size = new System.Drawing.Size(60, 28);
            comboFlop1.TabIndex = 0;
            // 
            // comboFlop2
            // 
            comboFlop2.Location = new System.Drawing.Point(69, 3);
            comboFlop2.Name = "comboFlop2";
            comboFlop2.Size = new System.Drawing.Size(60, 28);
            comboFlop2.TabIndex = 1;
            // 
            // comboFlop3
            // 
            comboFlop3.Location = new System.Drawing.Point(135, 3);
            comboFlop3.Name = "comboFlop3";
            comboFlop3.Size = new System.Drawing.Size(60, 28);
            comboFlop3.TabIndex = 2;
            // 
            // lblTurn
            // 
            lblTurn.Dock = System.Windows.Forms.DockStyle.Fill;
            lblTurn.Location = new System.Drawing.Point(3, 64);
            lblTurn.Name = "lblTurn";
            lblTurn.Size = new System.Drawing.Size(149, 32);
            lblTurn.TabIndex = 4;
            lblTurn.Text = "Turn:";
            // 
            // comboTurn
            // 
            comboTurn.Location = new System.Drawing.Point(158, 67);
            comboTurn.Name = "comboTurn";
            comboTurn.Size = new System.Drawing.Size(60, 28);
            comboTurn.TabIndex = 5;
            // 
            // lblRiver
            // 
            lblRiver.Dock = System.Windows.Forms.DockStyle.Fill;
            lblRiver.Location = new System.Drawing.Point(3, 96);
            lblRiver.Name = "lblRiver";
            lblRiver.Size = new System.Drawing.Size(149, 78);
            lblRiver.TabIndex = 6;
            lblRiver.Text = "River:";
            // 
            // comboRiver
            // 
            comboRiver.Location = new System.Drawing.Point(158, 99);
            comboRiver.Name = "comboRiver";
            comboRiver.Size = new System.Drawing.Size(60, 28);
            comboRiver.TabIndex = 7;
            // 
            // groupBoxActions
            // 
            groupBoxActions.Controls.Add(txtActions);
            groupBoxActions.Dock = System.Windows.Forms.DockStyle.Top;
            groupBoxActions.Location = new System.Drawing.Point(0, 58);
            groupBoxActions.Name = "groupBoxActions";
            groupBoxActions.Size = new System.Drawing.Size(394, 61);
            groupBoxActions.TabIndex = 1;
            groupBoxActions.TabStop = false;
            groupBoxActions.Text = "Action Sequence";
            // 
            // txtActions
            // 
            txtActions.Location = new System.Drawing.Point(6, 26);
            txtActions.Name = "txtActions";
            txtActions.Size = new System.Drawing.Size(382, 27);
            txtActions.TabIndex = 0;
            // 
            // btnCalculate
            // 
            btnCalculate.Dock = System.Windows.Forms.DockStyle.Top;
            btnCalculate.Location = new System.Drawing.Point(0, 0);
            btnCalculate.Name = "btnCalculate";
            btnCalculate.Size = new System.Drawing.Size(394, 58);
            btnCalculate.TabIndex = 2;
            btnCalculate.Text = "Calculate Strategy";
            btnCalculate.Click += btnCalculate_Click;
            // 
            // rightPanel
            // 
            rightPanel.Controls.Add(dataGridViewStrategy);
            rightPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            rightPanel.Location = new System.Drawing.Point(403, 3);
            rightPanel.Name = "rightPanel";
            rightPanel.Size = new System.Drawing.Size(394, 594);
            rightPanel.TabIndex = 1;
            // 
            // dataGridViewStrategy
            // 
            dataGridViewStrategy.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewStrategy.ColumnHeadersHeight = 29;
            dataGridViewStrategy.ColumnCount = 2;
            dataGridViewStrategy.Dock = System.Windows.Forms.DockStyle.Fill;
            dataGridViewStrategy.Location = new System.Drawing.Point(0, 0);
            dataGridViewStrategy.Name = "dataGridViewStrategy";
            dataGridViewStrategy.RowHeadersWidth = 51;
            dataGridViewStrategy.Size = new System.Drawing.Size(394, 594);
            dataGridViewStrategy.TabIndex = 0;
            dataGridViewStrategy.CellDoubleClick += dataGridViewStrategy_CellDoubleClick;
            // 
            // PokerStrategyForm
            // 
            ClientSize = new System.Drawing.Size(800, 700);
            Controls.Add(tableLayoutPanel);
            Text = "Poker Strategy CFR Explorer";
            tableLayoutPanel.ResumeLayout(false);
            leftPanel.ResumeLayout(false);
            groupBoxState.ResumeLayout(false);
            groupBoxCards.ResumeLayout(false);
            cardLayoutPanel.ResumeLayout(false);
            panelPlayerHand.ResumeLayout(false);
            panelFlop.ResumeLayout(false);
            groupBoxActions.ResumeLayout(false);
            groupBoxActions.PerformLayout();
            rightPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize) dataGridViewStrategy).EndInit();
            ResumeLayout(false);
        }

        #endregion
    }
}
