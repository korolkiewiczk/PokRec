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
            buttonNewPrj = new System.Windows.Forms.Button();
            buttonLoadPrj = new System.Windows.Forms.Button();
            buttonAddBoard = new System.Windows.Forms.Button();
            buttonBoards = new System.Windows.Forms.Button();
            buttonExit = new System.Windows.Forms.Button();
            labelCurrentPrj = new System.Windows.Forms.Label();
            textBoxMessage = new System.Windows.Forms.TextBox();
            tabControl1 = new System.Windows.Forms.TabControl();
            pageDesign = new System.Windows.Forms.TabPage();
            tabPlay = new System.Windows.Forms.TabPage();
            buttonShowGame = new System.Windows.Forms.Button();
            buttonStop = new System.Windows.Forms.Button();
            buttonStart = new System.Windows.Forms.Button();
            buttonFixWindow = new System.Windows.Forms.Button();
            tabSettings = new System.Windows.Forms.TabPage();
            tabControl1.SuspendLayout();
            pageDesign.SuspendLayout();
            tabPlay.SuspendLayout();
            SuspendLayout();
            // 
            // buttonNewPrj
            // 
            buttonNewPrj.Location = new System.Drawing.Point(5, 17);
            buttonNewPrj.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            buttonNewPrj.Name = "buttonNewPrj";
            buttonNewPrj.Size = new System.Drawing.Size(187, 62);
            buttonNewPrj.TabIndex = 0;
            buttonNewPrj.Text = "New project";
            buttonNewPrj.UseVisualStyleBackColor = true;
            buttonNewPrj.Click += buttonNewPrj_Click;
            // 
            // buttonLoadPrj
            // 
            buttonLoadPrj.Location = new System.Drawing.Point(5, 85);
            buttonLoadPrj.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            buttonLoadPrj.Name = "buttonLoadPrj";
            buttonLoadPrj.Size = new System.Drawing.Size(187, 62);
            buttonLoadPrj.TabIndex = 1;
            buttonLoadPrj.Text = "Load project";
            buttonLoadPrj.UseVisualStyleBackColor = true;
            buttonLoadPrj.Click += buttonLoadPrj_Click;
            // 
            // buttonAddBoard
            // 
            buttonAddBoard.Location = new System.Drawing.Point(5, 152);
            buttonAddBoard.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            buttonAddBoard.Name = "buttonAddBoard";
            buttonAddBoard.Size = new System.Drawing.Size(187, 62);
            buttonAddBoard.TabIndex = 2;
            buttonAddBoard.Text = "Add board";
            buttonAddBoard.UseVisualStyleBackColor = true;
            buttonAddBoard.Click += buttonAddBoard_Click;
            // 
            // buttonBoards
            // 
            buttonBoards.Location = new System.Drawing.Point(5, 223);
            buttonBoards.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            buttonBoards.Name = "buttonBoards";
            buttonBoards.Size = new System.Drawing.Size(187, 62);
            buttonBoards.TabIndex = 4;
            buttonBoards.Text = "Boards";
            buttonBoards.UseVisualStyleBackColor = true;
            buttonBoards.Click += buttonBoards_Click;
            // 
            // buttonExit
            // 
            buttonExit.Location = new System.Drawing.Point(15, 420);
            buttonExit.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            buttonExit.Name = "buttonExit";
            buttonExit.Size = new System.Drawing.Size(187, 62);
            buttonExit.TabIndex = 5;
            buttonExit.Text = "Exit";
            buttonExit.UseVisualStyleBackColor = true;
            buttonExit.Click += buttonExit_Click;
            // 
            // labelCurrentPrj
            // 
            labelCurrentPrj.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
            labelCurrentPrj.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte) 238));
            labelCurrentPrj.Location = new System.Drawing.Point(9, 14);
            labelCurrentPrj.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            labelCurrentPrj.Name = "labelCurrentPrj";
            labelCurrentPrj.Size = new System.Drawing.Size(200, 34);
            labelCurrentPrj.TabIndex = 6;
            labelCurrentPrj.Text = "Not loaded";
            labelCurrentPrj.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // textBoxMessage
            // 
            textBoxMessage.Location = new System.Drawing.Point(16, 505);
            textBoxMessage.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            textBoxMessage.Multiline = true;
            textBoxMessage.Name = "textBoxMessage";
            textBoxMessage.ReadOnly = true;
            textBoxMessage.Size = new System.Drawing.Size(184, 149);
            textBoxMessage.TabIndex = 8;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(pageDesign);
            tabControl1.Controls.Add(tabPlay);
            tabControl1.Controls.Add(tabSettings);
            tabControl1.Location = new System.Drawing.Point(4, 52);
            tabControl1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new System.Drawing.Size(211, 358);
            tabControl1.TabIndex = 9;
            // 
            // pageDesign
            // 
            pageDesign.BackColor = System.Drawing.Color.Transparent;
            pageDesign.Controls.Add(buttonBoards);
            pageDesign.Controls.Add(buttonNewPrj);
            pageDesign.Controls.Add(buttonLoadPrj);
            pageDesign.Controls.Add(buttonAddBoard);
            pageDesign.Location = new System.Drawing.Point(4, 29);
            pageDesign.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            pageDesign.Name = "pageDesign";
            pageDesign.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            pageDesign.Size = new System.Drawing.Size(203, 325);
            pageDesign.TabIndex = 0;
            pageDesign.Text = "Design";
            // 
            // tabPlay
            // 
            tabPlay.BackColor = System.Drawing.Color.Transparent;
            tabPlay.Controls.Add(buttonShowGame);
            tabPlay.Controls.Add(buttonStop);
            tabPlay.Controls.Add(buttonStart);
            tabPlay.Controls.Add(buttonFixWindow);
            tabPlay.Location = new System.Drawing.Point(4, 29);
            tabPlay.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            tabPlay.Name = "tabPlay";
            tabPlay.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            tabPlay.Size = new System.Drawing.Size(203, 325);
            tabPlay.TabIndex = 1;
            tabPlay.Text = "Play";
            // 
            // buttonShowGame
            // 
            buttonShowGame.Location = new System.Drawing.Point(5, 85);
            buttonShowGame.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            buttonShowGame.Name = "buttonShowGame";
            buttonShowGame.Size = new System.Drawing.Size(187, 62);
            buttonShowGame.TabIndex = 3;
            buttonShowGame.Text = "Show game";
            buttonShowGame.UseVisualStyleBackColor = true;
            buttonShowGame.Click += buttonShowGame_Click;
            // 
            // buttonStop
            // 
            buttonStop.Location = new System.Drawing.Point(5, 227);
            buttonStop.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            buttonStop.Name = "buttonStop";
            buttonStop.Size = new System.Drawing.Size(187, 62);
            buttonStop.TabIndex = 2;
            buttonStop.Text = "Stop";
            buttonStop.UseVisualStyleBackColor = true;
            buttonStop.Click += buttonStop_Click;
            // 
            // buttonStart
            // 
            buttonStart.Location = new System.Drawing.Point(5, 17);
            buttonStart.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            buttonStart.Name = "buttonStart";
            buttonStart.Size = new System.Drawing.Size(187, 62);
            buttonStart.TabIndex = 1;
            buttonStart.Text = "Start";
            buttonStart.UseVisualStyleBackColor = true;
            buttonStart.Click += buttonStart_Click;
            // 
            // buttonFixWindow
            // 
            buttonFixWindow.Location = new System.Drawing.Point(5, 155);
            buttonFixWindow.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            buttonFixWindow.Name = "buttonFixWindow";
            buttonFixWindow.Size = new System.Drawing.Size(187, 62);
            buttonFixWindow.TabIndex = 7;
            buttonFixWindow.Text = "Fix window";
            buttonFixWindow.UseVisualStyleBackColor = true;
            buttonFixWindow.Click += buttonFixWindow_Click;
            // 
            // tabSettings
            // 
            tabSettings.BackColor = System.Drawing.Color.Transparent;
            tabSettings.Location = new System.Drawing.Point(4, 29);
            tabSettings.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            tabSettings.Name = "tabSettings";
            tabSettings.Size = new System.Drawing.Size(203, 325);
            tabSettings.TabIndex = 2;
            tabSettings.Text = "Settings";
            // 
            // Form1
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(217, 674);
            Controls.Add(tabControl1);
            Controls.Add(textBoxMessage);
            Controls.Add(labelCurrentPrj);
            Controls.Add(buttonExit);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            MaximizeBox = false;
            Text = "PT";
            Load += Form1_Load;
            tabControl1.ResumeLayout(false);
            pageDesign.ResumeLayout(false);
            tabPlay.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
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
        private System.Windows.Forms.TabPage tabSettings;
        private System.Windows.Forms.Button buttonFixWindow;
    }
}

