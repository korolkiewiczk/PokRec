namespace Agent
{
    partial class ManageBoards
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
            btnDelete = new System.Windows.Forms.Button();
            btnEdit = new System.Windows.Forms.Button();
            listBox1 = new System.Windows.Forms.ListBox();
            btnShow = new System.Windows.Forms.Button();
            btnRefresh = new System.Windows.Forms.Button();
            btnSave = new System.Windows.Forms.Button();
            btnOpenFolder = new System.Windows.Forms.Button();
            btnWinPos = new System.Windows.Forms.Button();
            btnSetSize = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // btnDelete
            // 
            btnDelete.Location = new System.Drawing.Point(336, 154);
            btnDelete.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new System.Drawing.Size(124, 35);
            btnDelete.TabIndex = 1;
            btnDelete.Text = "Delete";
            btnDelete.UseVisualStyleBackColor = true;
            btnDelete.Click += btnDelete_Click;
            // 
            // btnEdit
            // 
            btnEdit.Location = new System.Drawing.Point(336, 109);
            btnEdit.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            btnEdit.Name = "btnEdit";
            btnEdit.Size = new System.Drawing.Size(124, 35);
            btnEdit.TabIndex = 2;
            btnEdit.Text = "Edit";
            btnEdit.UseVisualStyleBackColor = true;
            btnEdit.Click += btnEdit_Click;
            // 
            // listBox1
            // 
            listBox1.AllowDrop = true;
            listBox1.FormattingEnabled = true;
            listBox1.Location = new System.Drawing.Point(17, 20);
            listBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            listBox1.Name = "listBox1";
            listBox1.Size = new System.Drawing.Size(309, 364);
            listBox1.TabIndex = 3;
            listBox1.DragDrop += listBox1_DragDrop;
            listBox1.DragEnter += listBox1_DragEnter;
            // 
            // btnShow
            // 
            btnShow.Location = new System.Drawing.Point(336, 20);
            btnShow.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            btnShow.Name = "btnShow";
            btnShow.Size = new System.Drawing.Size(124, 35);
            btnShow.TabIndex = 4;
            btnShow.Text = "Show";
            btnShow.UseVisualStyleBackColor = true;
            btnShow.Click += btnShow_Click;
            // 
            // btnRefresh
            // 
            btnRefresh.Location = new System.Drawing.Point(336, 65);
            btnRefresh.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new System.Drawing.Size(124, 35);
            btnRefresh.TabIndex = 5;
            btnRefresh.Text = "Refresh";
            btnRefresh.UseVisualStyleBackColor = true;
            btnRefresh.Click += btnRefresh_Click;
            // 
            // btnSave
            // 
            btnSave.Location = new System.Drawing.Point(336, 349);
            btnSave.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            btnSave.Name = "btnSave";
            btnSave.Size = new System.Drawing.Size(124, 35);
            btnSave.TabIndex = 6;
            btnSave.Text = "Save";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // btnOpenFolder
            // 
            btnOpenFolder.Location = new System.Drawing.Point(336, 198);
            btnOpenFolder.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            btnOpenFolder.Name = "btnOpenFolder";
            btnOpenFolder.Size = new System.Drawing.Size(124, 35);
            btnOpenFolder.TabIndex = 7;
            btnOpenFolder.Text = "Open folder";
            btnOpenFolder.UseVisualStyleBackColor = true;
            btnOpenFolder.Click += btnOpenFolder_Click;
            // 
            // btnWinPos
            // 
            btnWinPos.Location = new System.Drawing.Point(336, 243);
            btnWinPos.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            btnWinPos.Name = "btnWinPos";
            btnWinPos.Size = new System.Drawing.Size(124, 35);
            btnWinPos.TabIndex = 8;
            btnWinPos.Text = "New position";
            btnWinPos.UseVisualStyleBackColor = true;
            btnWinPos.Click += btnWinPos_Click;
            // 
            // btnSetSize
            // 
            btnSetSize.Location = new System.Drawing.Point(336, 288);
            btnSetSize.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            btnSetSize.Name = "btnSetSize";
            btnSetSize.Size = new System.Drawing.Size(124, 35);
            btnSetSize.TabIndex = 9;
            btnSetSize.Text = "Fix window";
            btnSetSize.UseVisualStyleBackColor = true;
            btnSetSize.Click += btnSetSize_Click;
            // 
            // ManageBoards
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(476, 412);
            Controls.Add(btnSetSize);
            Controls.Add(btnWinPos);
            Controls.Add(btnOpenFolder);
            Controls.Add(btnSave);
            Controls.Add(btnRefresh);
            Controls.Add(btnShow);
            Controls.Add(listBox1);
            Controls.Add(btnEdit);
            Controls.Add(btnDelete);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            Text = "ManageBoards";
            FormClosing += ManageBoards_FormClosing;
            Load += ManageBoards_Load;
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button btnShow;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnOpenFolder;
        private System.Windows.Forms.Button btnWinPos;
        private System.Windows.Forms.Button btnSetSize;
    }
}