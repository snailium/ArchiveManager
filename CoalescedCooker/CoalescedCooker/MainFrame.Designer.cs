namespace CoalescedCooker
{
    partial class CoalescedCooker
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CoalescedCooker));
            this.UncookButton = new System.Windows.Forms.Button();
            this.CookButton = new System.Windows.Forms.Button();
            this.ExitButton = new System.Windows.Forms.Button();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.SuspendLayout();
            // 
            // UncookButton
            // 
            this.UncookButton.Location = new System.Drawing.Point(21, 18);
            this.UncookButton.Name = "UncookButton";
            this.UncookButton.Size = new System.Drawing.Size(117, 46);
            this.UncookButton.TabIndex = 0;
            this.UncookButton.Text = "&Uncook";
            this.UncookButton.UseVisualStyleBackColor = true;
            this.UncookButton.Click += new System.EventHandler(this.UncookButton_Click);
            // 
            // CookButton
            // 
            this.CookButton.Location = new System.Drawing.Point(144, 18);
            this.CookButton.Name = "CookButton";
            this.CookButton.Size = new System.Drawing.Size(117, 46);
            this.CookButton.TabIndex = 1;
            this.CookButton.Text = "&Cook";
            this.CookButton.UseVisualStyleBackColor = true;
            this.CookButton.Click += new System.EventHandler(this.CookButton_Click);
            // 
            // ExitButton
            // 
            this.ExitButton.Location = new System.Drawing.Point(267, 18);
            this.ExitButton.Name = "ExitButton";
            this.ExitButton.Size = new System.Drawing.Size(117, 46);
            this.ExitButton.TabIndex = 2;
            this.ExitButton.Text = "E&xit";
            this.ExitButton.UseVisualStyleBackColor = true;
            this.ExitButton.Click += new System.EventHandler(this.ExitButton_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "Coalesced.ini";
            this.openFileDialog.ReadOnlyChecked = true;
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.FileName = "Coalesced.ini";
            // 
            // CoalescedCooker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(407, 83);
            this.Controls.Add(this.ExitButton);
            this.Controls.Add(this.CookButton);
            this.Controls.Add(this.UncookButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CoalescedCooker";
            this.ShowIcon = false;
            this.Text = "Coalesced Cooker 0.1 by Snailium";
            this.Load += new System.EventHandler(this.CoalescedCooker_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button UncookButton;
        private System.Windows.Forms.Button CookButton;
        private System.Windows.Forms.Button ExitButton;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
    }
}

