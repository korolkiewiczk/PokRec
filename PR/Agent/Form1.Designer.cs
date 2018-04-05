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
            this.buttonNewPrj = new System.Windows.Forms.Button();
            this.buttonLoadPrj = new System.Windows.Forms.Button();
            this.buttonAddBoard = new System.Windows.Forms.Button();
            this.buttonBoards = new System.Windows.Forms.Button();
            this.buttonExit = new System.Windows.Forms.Button();
            this.labelCurrentPrj = new System.Windows.Forms.Label();
            this.textBoxMessage = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // buttonNewPrj
            // 
            this.buttonNewPrj.Location = new System.Drawing.Point(12, 53);
            this.buttonNewPrj.Name = "buttonNewPrj";
            this.buttonNewPrj.Size = new System.Drawing.Size(140, 40);
            this.buttonNewPrj.TabIndex = 0;
            this.buttonNewPrj.Text = "New project";
            this.buttonNewPrj.UseVisualStyleBackColor = true;
            this.buttonNewPrj.Click += new System.EventHandler(this.buttonNewPrj_Click);
            // 
            // buttonLoadPrj
            // 
            this.buttonLoadPrj.Location = new System.Drawing.Point(12, 97);
            this.buttonLoadPrj.Name = "buttonLoadPrj";
            this.buttonLoadPrj.Size = new System.Drawing.Size(140, 40);
            this.buttonLoadPrj.TabIndex = 1;
            this.buttonLoadPrj.Text = "Load project";
            this.buttonLoadPrj.UseVisualStyleBackColor = true;
            this.buttonLoadPrj.Click += new System.EventHandler(this.buttonLoadPrj_Click);
            // 
            // buttonAddBoard
            // 
            this.buttonAddBoard.Location = new System.Drawing.Point(12, 141);
            this.buttonAddBoard.Name = "buttonAddBoard";
            this.buttonAddBoard.Size = new System.Drawing.Size(140, 40);
            this.buttonAddBoard.TabIndex = 2;
            this.buttonAddBoard.Text = "Add board";
            this.buttonAddBoard.UseVisualStyleBackColor = true;
            this.buttonAddBoard.Click += new System.EventHandler(this.buttonAddBoard_Click);
            // 
            // buttonBoards
            // 
            this.buttonBoards.Location = new System.Drawing.Point(12, 187);
            this.buttonBoards.Name = "buttonBoards";
            this.buttonBoards.Size = new System.Drawing.Size(140, 40);
            this.buttonBoards.TabIndex = 4;
            this.buttonBoards.Text = "Boards";
            this.buttonBoards.UseVisualStyleBackColor = true;
            this.buttonBoards.Click += new System.EventHandler(this.buttonBoards_Click);
            // 
            // buttonExit
            // 
            this.buttonExit.Location = new System.Drawing.Point(12, 273);
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
            this.labelCurrentPrj.AutoSize = true;
            this.labelCurrentPrj.Location = new System.Drawing.Point(12, 18);
            this.labelCurrentPrj.Name = "labelCurrentPrj";
            this.labelCurrentPrj.Size = new System.Drawing.Size(59, 13);
            this.labelCurrentPrj.TabIndex = 6;
            this.labelCurrentPrj.Text = "Not loaded";
            // 
            // textBoxMessage
            // 
            this.textBoxMessage.Location = new System.Drawing.Point(12, 328);
            this.textBoxMessage.Multiline = true;
            this.textBoxMessage.Name = "textBoxMessage";
            this.textBoxMessage.ReadOnly = true;
            this.textBoxMessage.Size = new System.Drawing.Size(140, 98);
            this.textBoxMessage.TabIndex = 8;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(164, 465);
            this.Controls.Add(this.textBoxMessage);
            this.Controls.Add(this.labelCurrentPrj);
            this.Controls.Add(this.buttonExit);
            this.Controls.Add(this.buttonBoards);
            this.Controls.Add(this.buttonAddBoard);
            this.Controls.Add(this.buttonLoadPrj);
            this.Controls.Add(this.buttonNewPrj);
            this.Name = "Form1";
            this.Text = "PT";
            this.Load += new System.EventHandler(this.Form1_Load);
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
    }
}

