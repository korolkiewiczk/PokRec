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
            this.buttonRemoveBoard = new System.Windows.Forms.Button();
            this.buttonOptions = new System.Windows.Forms.Button();
            this.buttonExit = new System.Windows.Forms.Button();
            this.labelCurrentPrj = new System.Windows.Forms.Label();
            this.labelMessage = new System.Windows.Forms.Label();
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
            // buttonRemoveBoard
            // 
            this.buttonRemoveBoard.Location = new System.Drawing.Point(12, 185);
            this.buttonRemoveBoard.Name = "buttonRemoveBoard";
            this.buttonRemoveBoard.Size = new System.Drawing.Size(140, 40);
            this.buttonRemoveBoard.TabIndex = 3;
            this.buttonRemoveBoard.Text = "Remove board";
            this.buttonRemoveBoard.UseVisualStyleBackColor = true;
            // 
            // buttonOptions
            // 
            this.buttonOptions.Location = new System.Drawing.Point(12, 229);
            this.buttonOptions.Name = "buttonOptions";
            this.buttonOptions.Size = new System.Drawing.Size(140, 40);
            this.buttonOptions.TabIndex = 4;
            this.buttonOptions.Text = "Options";
            this.buttonOptions.UseVisualStyleBackColor = true;
            // 
            // buttonExit
            // 
            this.buttonExit.Location = new System.Drawing.Point(12, 273);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(140, 40);
            this.buttonExit.TabIndex = 5;
            this.buttonExit.Text = "Exit";
            this.buttonExit.UseVisualStyleBackColor = true;
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
            // labelMessage
            // 
            this.labelMessage.AutoSize = true;
            this.labelMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelMessage.ForeColor = System.Drawing.Color.Red;
            this.labelMessage.Location = new System.Drawing.Point(12, 332);
            this.labelMessage.Name = "labelMessage";
            this.labelMessage.Size = new System.Drawing.Size(63, 13);
            this.labelMessage.TabIndex = 7;
            this.labelMessage.Text = "Messsage";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(164, 366);
            this.Controls.Add(this.labelMessage);
            this.Controls.Add(this.labelCurrentPrj);
            this.Controls.Add(this.buttonExit);
            this.Controls.Add(this.buttonOptions);
            this.Controls.Add(this.buttonRemoveBoard);
            this.Controls.Add(this.buttonAddBoard);
            this.Controls.Add(this.buttonLoadPrj);
            this.Controls.Add(this.buttonNewPrj);
            this.Name = "Form1";
            this.Text = "PT";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonNewPrj;
        private System.Windows.Forms.Button buttonLoadPrj;
        private System.Windows.Forms.Button buttonAddBoard;
        private System.Windows.Forms.Button buttonRemoveBoard;
        private System.Windows.Forms.Button buttonOptions;
        private System.Windows.Forms.Button buttonExit;
        private System.Windows.Forms.Label labelCurrentPrj;
        private System.Windows.Forms.Label labelMessage;
    }
}

