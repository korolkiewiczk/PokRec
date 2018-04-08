namespace Agent
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            this.buttonNewPrj = new System.Windows.Forms.Button();
            this.buttonLoadPrj = new System.Windows.Forms.Button();
            this.buttonAddBoard = new System.Windows.Forms.Button();
            this.buttonBoards = new System.Windows.Forms.Button();
            this.buttonExit = new System.Windows.Forms.Button();
            this.labelCurrentPrj = new System.Windows.Forms.Label();
            this.textBoxMessage = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.pageDesign = new System.Windows.Forms.TabPage();
            this.tabPlay = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.numericInterval = new System.Windows.Forms.NumericUpDown();
            this.buttonShowGame = new System.Windows.Forms.Button();
            this.buttonStop = new System.Windows.Forms.Button();
            this.buttonStart = new System.Windows.Forms.Button();
            this.tabSettings = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.numericSavedImagesPerBoard = new System.Windows.Forms.NumericUpDown();
            this.timerGame = new System.Windows.Forms.Timer(this.components);
            this.buttonTakeShot = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.pageDesign.SuspendLayout();
            this.tabPlay.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericInterval)).BeginInit();
            this.tabSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericSavedImagesPerBoard)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonNewPrj
            // 
            this.buttonNewPrj.Location = new System.Drawing.Point(4, 11);
            this.buttonNewPrj.Name = "buttonNewPrj";
            this.buttonNewPrj.Size = new System.Drawing.Size(140, 40);
            this.buttonNewPrj.TabIndex = 0;
            this.buttonNewPrj.Text = "New project";
            this.buttonNewPrj.UseVisualStyleBackColor = true;
            this.buttonNewPrj.Click += new System.EventHandler(this.buttonNewPrj_Click);
            // 
            // buttonLoadPrj
            // 
            this.buttonLoadPrj.Location = new System.Drawing.Point(4, 55);
            this.buttonLoadPrj.Name = "buttonLoadPrj";
            this.buttonLoadPrj.Size = new System.Drawing.Size(140, 40);
            this.buttonLoadPrj.TabIndex = 1;
            this.buttonLoadPrj.Text = "Load project";
            this.buttonLoadPrj.UseVisualStyleBackColor = true;
            this.buttonLoadPrj.Click += new System.EventHandler(this.buttonLoadPrj_Click);
            // 
            // buttonAddBoard
            // 
            this.buttonAddBoard.Location = new System.Drawing.Point(4, 99);
            this.buttonAddBoard.Name = "buttonAddBoard";
            this.buttonAddBoard.Size = new System.Drawing.Size(140, 40);
            this.buttonAddBoard.TabIndex = 2;
            this.buttonAddBoard.Text = "Add board";
            this.buttonAddBoard.UseVisualStyleBackColor = true;
            this.buttonAddBoard.Click += new System.EventHandler(this.buttonAddBoard_Click);
            // 
            // buttonBoards
            // 
            this.buttonBoards.Location = new System.Drawing.Point(4, 145);
            this.buttonBoards.Name = "buttonBoards";
            this.buttonBoards.Size = new System.Drawing.Size(140, 40);
            this.buttonBoards.TabIndex = 4;
            this.buttonBoards.Text = "Boards";
            this.buttonBoards.UseVisualStyleBackColor = true;
            this.buttonBoards.Click += new System.EventHandler(this.buttonBoards_Click);
            // 
            // buttonExit
            // 
            this.buttonExit.Location = new System.Drawing.Point(11, 273);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(140, 40);
            this.buttonExit.TabIndex = 5;
            this.buttonExit.Text = "Exit";
            this.buttonExit.UseVisualStyleBackColor = true;
            this.buttonExit.Click += new System.EventHandler(this.buttonExit_Click);
            // 
            // labelCurrentPrj
            // 
            this.labelCurrentPrj.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelCurrentPrj.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelCurrentPrj.Location = new System.Drawing.Point(7, 9);
            this.labelCurrentPrj.Name = "labelCurrentPrj";
            this.labelCurrentPrj.Size = new System.Drawing.Size(150, 22);
            this.labelCurrentPrj.TabIndex = 6;
            this.labelCurrentPrj.Text = "Not loaded";
            this.labelCurrentPrj.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // textBoxMessage
            // 
            this.textBoxMessage.Location = new System.Drawing.Point(12, 328);
            this.textBoxMessage.Multiline = true;
            this.textBoxMessage.Name = "textBoxMessage";
            this.textBoxMessage.ReadOnly = true;
            this.textBoxMessage.Size = new System.Drawing.Size(139, 98);
            this.textBoxMessage.TabIndex = 8;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.pageDesign);
            this.tabControl1.Controls.Add(this.tabPlay);
            this.tabControl1.Controls.Add(this.tabSettings);
            this.tabControl1.Location = new System.Drawing.Point(3, 34);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(158, 233);
            this.tabControl1.TabIndex = 9;
            // 
            // pageDesign
            // 
            this.pageDesign.BackColor = System.Drawing.Color.Transparent;
            this.pageDesign.Controls.Add(this.buttonBoards);
            this.pageDesign.Controls.Add(this.buttonNewPrj);
            this.pageDesign.Controls.Add(this.buttonLoadPrj);
            this.pageDesign.Controls.Add(this.buttonAddBoard);
            this.pageDesign.Location = new System.Drawing.Point(4, 22);
            this.pageDesign.Name = "pageDesign";
            this.pageDesign.Padding = new System.Windows.Forms.Padding(3);
            this.pageDesign.Size = new System.Drawing.Size(150, 207);
            this.pageDesign.TabIndex = 0;
            this.pageDesign.Text = "Design";
            // 
            // tabPlay
            // 
            this.tabPlay.BackColor = System.Drawing.Color.Transparent;
            this.tabPlay.Controls.Add(this.buttonTakeShot);
            this.tabPlay.Controls.Add(this.label1);
            this.tabPlay.Controls.Add(this.numericInterval);
            this.tabPlay.Controls.Add(this.buttonShowGame);
            this.tabPlay.Controls.Add(this.buttonStop);
            this.tabPlay.Controls.Add(this.buttonStart);
            this.tabPlay.Location = new System.Drawing.Point(4, 22);
            this.tabPlay.Name = "tabPlay";
            this.tabPlay.Padding = new System.Windows.Forms.Padding(3);
            this.tabPlay.Size = new System.Drawing.Size(150, 207);
            this.tabPlay.TabIndex = 1;
            this.tabPlay.Text = "Play";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(129, 137);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(20, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "ms";
            // 
            // numericInterval
            // 
            this.numericInterval.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericInterval.Location = new System.Drawing.Point(76, 133);
            this.numericInterval.Maximum = new decimal(new int[] {
            4000,
            0,
            0,
            0});
            this.numericInterval.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericInterval.Name = "numericInterval";
            this.numericInterval.Size = new System.Drawing.Size(47, 20);
            this.numericInterval.TabIndex = 4;
            this.numericInterval.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericInterval.ValueChanged += new System.EventHandler(this.numericInterval_ValueChanged);
            // 
            // buttonShowGame
            // 
            this.buttonShowGame.Location = new System.Drawing.Point(4, 55);
            this.buttonShowGame.Name = "buttonShowGame";
            this.buttonShowGame.Size = new System.Drawing.Size(140, 40);
            this.buttonShowGame.TabIndex = 3;
            this.buttonShowGame.Text = "Show game";
            this.buttonShowGame.UseVisualStyleBackColor = true;
            this.buttonShowGame.Click += new System.EventHandler(this.buttonShowGame_Click);
            // 
            // buttonStop
            // 
            this.buttonStop.Location = new System.Drawing.Point(3, 161);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(140, 40);
            this.buttonStop.TabIndex = 2;
            this.buttonStop.Text = "Stop";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(4, 11);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(140, 40);
            this.buttonStart.TabIndex = 1;
            this.buttonStart.Text = "Start";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // tabSettings
            // 
            this.tabSettings.BackColor = System.Drawing.Color.Transparent;
            this.tabSettings.Controls.Add(this.label2);
            this.tabSettings.Controls.Add(this.numericSavedImagesPerBoard);
            this.tabSettings.Location = new System.Drawing.Point(4, 22);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.Size = new System.Drawing.Size(150, 207);
            this.tabSettings.TabIndex = 2;
            this.tabSettings.Text = "Settings";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(122, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Saved images per board";
            // 
            // numericSavedImagesPerBoard
            // 
            this.numericSavedImagesPerBoard.Location = new System.Drawing.Point(8, 29);
            this.numericSavedImagesPerBoard.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericSavedImagesPerBoard.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericSavedImagesPerBoard.Name = "numericSavedImagesPerBoard";
            this.numericSavedImagesPerBoard.Size = new System.Drawing.Size(58, 20);
            this.numericSavedImagesPerBoard.TabIndex = 0;
            this.numericSavedImagesPerBoard.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericSavedImagesPerBoard.ValueChanged += new System.EventHandler(this.numericSavedImagesPerBoard_ValueChanged);
            // 
            // timerGame
            // 
            this.timerGame.Tick += new System.EventHandler(this.timerGame_Tick);
            // 
            // buttonTakeShot
            // 
            this.buttonTakeShot.Location = new System.Drawing.Point(4, 132);
            this.buttonTakeShot.Name = "buttonTakeShot";
            this.buttonTakeShot.Size = new System.Drawing.Size(63, 23);
            this.buttonTakeShot.TabIndex = 6;
            this.buttonTakeShot.Text = "Take shot";
            this.buttonTakeShot.UseVisualStyleBackColor = true;
            this.buttonTakeShot.Click += new System.EventHandler(this.buttonTakeShot_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(163, 438);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.textBoxMessage);
            this.Controls.Add(this.labelCurrentPrj);
            this.Controls.Add(this.buttonExit);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "PT";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.pageDesign.ResumeLayout(false);
            this.tabPlay.ResumeLayout(false);
            this.tabPlay.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericInterval)).EndInit();
            this.tabSettings.ResumeLayout(false);
            this.tabSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericSavedImagesPerBoard)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonNewPrj;
        private System.Windows.Forms.Button buttonLoadPrj;
        private System.Windows.Forms.Button buttonAddBoard;
        private System.Windows.Forms.Button buttonBoards;
        private System.Windows.Forms.Button buttonExit;
        private System.Windows.Forms.Label labelCurrentPrj;
        private System.Windows.Forms.TextBox textBoxMessage;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage pageDesign;
        private System.Windows.Forms.TabPage tabPlay;
        private System.Windows.Forms.Button buttonShowGame;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericInterval;
        private System.Windows.Forms.Timer timerGame;
        private System.Windows.Forms.TabPage tabSettings;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericSavedImagesPerBoard;
        private System.Windows.Forms.Button buttonTakeShot;
    }
}

