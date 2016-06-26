namespace ArchiveManager
{
    partial class ArchiveManager
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
            this.openButton = new System.Windows.Forms.Button();
            this.fileList = new System.Windows.Forms.CheckedListBox();
            this.archiveTypeLabel = new System.Windows.Forms.Label();
            this.archiveType = new System.Windows.Forms.Label();
            this.numFilesLabel = new System.Windows.Forms.Label();
            this.numFiles = new System.Windows.Forms.Label();
            this.extractButton = new System.Windows.Forms.Button();
            this.selectAllButton = new System.Windows.Forms.Button();
            this.deselectAllButton = new System.Windows.Forms.Button();
            this.removeButton = new System.Windows.Forms.Button();
            this.insertButton = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.saveButton = new System.Windows.Forms.Button();
            this.closeButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // openButton
            // 
            this.openButton.Location = new System.Drawing.Point(12, 12);
            this.openButton.Name = "openButton";
            this.openButton.Size = new System.Drawing.Size(87, 33);
            this.openButton.TabIndex = 0;
            this.openButton.Text = "&Open...";
            this.openButton.UseVisualStyleBackColor = true;
            this.openButton.Click += new System.EventHandler(this.openButton_Click);
            // 
            // fileList
            // 
            this.fileList.CheckOnClick = true;
            this.fileList.FormattingEnabled = true;
            this.fileList.Location = new System.Drawing.Point(12, 112);
            this.fileList.Name = "fileList";
            this.fileList.Size = new System.Drawing.Size(297, 308);
            this.fileList.TabIndex = 1;
            // 
            // archiveTypeLabel
            // 
            this.archiveTypeLabel.AutoSize = true;
            this.archiveTypeLabel.Location = new System.Drawing.Point(10, 61);
            this.archiveTypeLabel.Name = "archiveTypeLabel";
            this.archiveTypeLabel.Size = new System.Drawing.Size(89, 12);
            this.archiveTypeLabel.TabIndex = 2;
            this.archiveTypeLabel.Text = "Archieve Type:";
            // 
            // archiveType
            // 
            this.archiveType.AutoSize = true;
            this.archiveType.Location = new System.Drawing.Point(105, 61);
            this.archiveType.Name = "archiveType";
            this.archiveType.Size = new System.Drawing.Size(35, 12);
            this.archiveType.TabIndex = 3;
            this.archiveType.Text = "<N/A>";
            // 
            // numFilesLabel
            // 
            this.numFilesLabel.AutoSize = true;
            this.numFilesLabel.Location = new System.Drawing.Point(10, 82);
            this.numFilesLabel.Name = "numFilesLabel";
            this.numFilesLabel.Size = new System.Drawing.Size(101, 12);
            this.numFilesLabel.TabIndex = 4;
            this.numFilesLabel.Text = "Number of Files:";
            // 
            // numFiles
            // 
            this.numFiles.AutoSize = true;
            this.numFiles.Location = new System.Drawing.Point(117, 82);
            this.numFiles.Name = "numFiles";
            this.numFiles.Size = new System.Drawing.Size(35, 12);
            this.numFiles.TabIndex = 5;
            this.numFiles.Text = "<N/A>";
            // 
            // extractButton
            // 
            this.extractButton.Location = new System.Drawing.Point(12, 480);
            this.extractButton.Name = "extractButton";
            this.extractButton.Size = new System.Drawing.Size(87, 33);
            this.extractButton.TabIndex = 6;
            this.extractButton.Text = "&Extract";
            this.extractButton.UseVisualStyleBackColor = true;
            this.extractButton.Click += new System.EventHandler(this.extractButton_Click);
            // 
            // selectAllButton
            // 
            this.selectAllButton.Location = new System.Drawing.Point(116, 431);
            this.selectAllButton.Name = "selectAllButton";
            this.selectAllButton.Size = new System.Drawing.Size(87, 33);
            this.selectAllButton.TabIndex = 7;
            this.selectAllButton.Text = "Select &All";
            this.selectAllButton.UseVisualStyleBackColor = true;
            this.selectAllButton.Click += new System.EventHandler(this.selectAllButton_Click);
            // 
            // deselectAllButton
            // 
            this.deselectAllButton.Location = new System.Drawing.Point(222, 431);
            this.deselectAllButton.Name = "deselectAllButton";
            this.deselectAllButton.Size = new System.Drawing.Size(87, 33);
            this.deselectAllButton.TabIndex = 8;
            this.deselectAllButton.Text = "&Deselect All";
            this.deselectAllButton.UseVisualStyleBackColor = true;
            this.deselectAllButton.Click += new System.EventHandler(this.deselectAllButton_Click);
            // 
            // removeButton
            // 
            this.removeButton.Location = new System.Drawing.Point(116, 480);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(87, 33);
            this.removeButton.TabIndex = 9;
            this.removeButton.Text = "&Remove";
            this.removeButton.UseVisualStyleBackColor = true;
            // 
            // insertButton
            // 
            this.insertButton.Location = new System.Drawing.Point(222, 481);
            this.insertButton.Name = "insertButton";
            this.insertButton.Size = new System.Drawing.Size(87, 32);
            this.insertButton.TabIndex = 10;
            this.insertButton.Text = "&Insert";
            this.insertButton.UseVisualStyleBackColor = true;
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(116, 12);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(87, 33);
            this.saveButton.TabIndex = 11;
            this.saveButton.Text = "&Save...";
            this.saveButton.UseVisualStyleBackColor = true;
            // 
            // closeButton
            // 
            this.closeButton.Location = new System.Drawing.Point(222, 12);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(87, 33);
            this.closeButton.TabIndex = 12;
            this.closeButton.Text = "&Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // ArchiveManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(321, 525);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.insertButton);
            this.Controls.Add(this.removeButton);
            this.Controls.Add(this.deselectAllButton);
            this.Controls.Add(this.selectAllButton);
            this.Controls.Add(this.extractButton);
            this.Controls.Add(this.numFiles);
            this.Controls.Add(this.numFilesLabel);
            this.Controls.Add(this.archiveType);
            this.Controls.Add(this.archiveTypeLabel);
            this.Controls.Add(this.fileList);
            this.Controls.Add(this.openButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ArchiveManager";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Archive Manager 0.1 by Snailium";
            this.Load += new System.EventHandler(this.ArchiveManager_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button openButton;
        private System.Windows.Forms.CheckedListBox fileList;
        private System.Windows.Forms.Label archiveTypeLabel;
        private System.Windows.Forms.Label archiveType;
        private System.Windows.Forms.Label numFilesLabel;
        private System.Windows.Forms.Label numFiles;
        private System.Windows.Forms.Button extractButton;
        private System.Windows.Forms.Button selectAllButton;
        private System.Windows.Forms.Button deselectAllButton;
        private System.Windows.Forms.Button removeButton;
        private System.Windows.Forms.Button insertButton;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button closeButton;
    }
}

