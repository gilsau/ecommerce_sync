namespace DownloadSyncProducts
{
    partial class frmMain
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
            this.tcMain = new System.Windows.Forms.TabControl();
            this.tabDownload = new System.Windows.Forms.TabPage();
            this.btnDownloadAll = new System.Windows.Forms.Button();
            this.tabSyncCats = new System.Windows.Forms.TabPage();
            this.tabSyncProducts = new System.Windows.Forms.TabPage();
            this.txtFeedbackDwn = new System.Windows.Forms.TextBox();
            this.tcMain.SuspendLayout();
            this.tabDownload.SuspendLayout();
            this.SuspendLayout();
            // 
            // tcMain
            // 
            this.tcMain.Controls.Add(this.tabDownload);
            this.tcMain.Controls.Add(this.tabSyncCats);
            this.tcMain.Controls.Add(this.tabSyncProducts);
            this.tcMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcMain.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tcMain.Location = new System.Drawing.Point(0, 0);
            this.tcMain.Name = "tcMain";
            this.tcMain.SelectedIndex = 0;
            this.tcMain.Size = new System.Drawing.Size(781, 651);
            this.tcMain.TabIndex = 1;
            // 
            // tabDownload
            // 
            this.tabDownload.AutoScroll = true;
            this.tabDownload.Controls.Add(this.txtFeedbackDwn);
            this.tabDownload.Controls.Add(this.btnDownloadAll);
            this.tabDownload.Location = new System.Drawing.Point(4, 34);
            this.tabDownload.Name = "tabDownload";
            this.tabDownload.Padding = new System.Windows.Forms.Padding(3);
            this.tabDownload.Size = new System.Drawing.Size(773, 613);
            this.tabDownload.TabIndex = 0;
            this.tabDownload.Text = "Download";
            this.tabDownload.UseVisualStyleBackColor = true;
            // 
            // btnDownloadAll
            // 
            this.btnDownloadAll.Location = new System.Drawing.Point(28, 20);
            this.btnDownloadAll.Name = "btnDownloadAll";
            this.btnDownloadAll.Size = new System.Drawing.Size(164, 32);
            this.btnDownloadAll.TabIndex = 0;
            this.btnDownloadAll.Text = "Download All";
            this.btnDownloadAll.UseVisualStyleBackColor = true;
            this.btnDownloadAll.Click += new System.EventHandler(this.btnDownloadAll_Click);
            // 
            // tabSyncCats
            // 
            this.tabSyncCats.Location = new System.Drawing.Point(4, 34);
            this.tabSyncCats.Name = "tabSyncCats";
            this.tabSyncCats.Padding = new System.Windows.Forms.Padding(3);
            this.tabSyncCats.Size = new System.Drawing.Size(773, 613);
            this.tabSyncCats.TabIndex = 1;
            this.tabSyncCats.Text = "Sync Categories";
            this.tabSyncCats.UseVisualStyleBackColor = true;
            // 
            // tabSyncProducts
            // 
            this.tabSyncProducts.Location = new System.Drawing.Point(4, 34);
            this.tabSyncProducts.Name = "tabSyncProducts";
            this.tabSyncProducts.Padding = new System.Windows.Forms.Padding(3);
            this.tabSyncProducts.Size = new System.Drawing.Size(773, 613);
            this.tabSyncProducts.TabIndex = 2;
            this.tabSyncProducts.Text = "Sync Products";
            this.tabSyncProducts.UseVisualStyleBackColor = true;
            // 
            // txtFeedbackDwn
            // 
            this.txtFeedbackDwn.Dock = System.Windows.Forms.DockStyle.Right;
            this.txtFeedbackDwn.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.txtFeedbackDwn.Location = new System.Drawing.Point(670, 3);
            this.txtFeedbackDwn.Multiline = true;
            this.txtFeedbackDwn.Name = "txtFeedbackDwn";
            this.txtFeedbackDwn.Size = new System.Drawing.Size(100, 607);
            this.txtFeedbackDwn.TabIndex = 1;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(781, 651);
            this.Controls.Add(this.tcMain);
            this.Name = "frmMain";
            this.Text = "Download & Sync Products";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.tcMain.ResumeLayout(false);
            this.tabDownload.ResumeLayout(false);
            this.tabDownload.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TabControl tcMain;
        private System.Windows.Forms.TabPage tabSyncProducts;
        private System.Windows.Forms.TabPage tabDownload;
        private System.Windows.Forms.Button btnDownloadAll;
        private System.Windows.Forms.TabPage tabSyncCats;
        private System.Windows.Forms.TextBox txtFeedbackDwn;
    }
}

