namespace HACC_MDS_Joiner
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
            this.openCSVFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.btnSrc = new System.Windows.Forms.Button();
            this.btnSubsequent = new System.Windows.Forms.Button();
            this.btnDownload = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.rtbOutput = new System.Windows.Forms.RichTextBox();
            this.lblReset = new System.Windows.Forms.LinkLabel();
            this.saveCSVFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.SuspendLayout();
            // 
            // openCSVFileDialog
            // 
            this.openCSVFileDialog.Filter = "CSV files (*.csv)|*.csv";
            // 
            // btnSrc
            // 
            this.btnSrc.Location = new System.Drawing.Point(16, 68);
            this.btnSrc.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSrc.Name = "btnSrc";
            this.btnSrc.Size = new System.Drawing.Size(212, 28);
            this.btnSrc.TabIndex = 0;
            this.btnSrc.Text = "Open Primary File";
            this.btnSrc.UseVisualStyleBackColor = true;
            this.btnSrc.Click += new System.EventHandler(this.btnSrc_Click);
            // 
            // btnSubsequent
            // 
            this.btnSubsequent.Location = new System.Drawing.Point(16, 171);
            this.btnSubsequent.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSubsequent.Name = "btnSubsequent";
            this.btnSubsequent.Size = new System.Drawing.Size(212, 28);
            this.btnSubsequent.TabIndex = 1;
            this.btnSubsequent.Text = "Open Secondary File";
            this.btnSubsequent.UseVisualStyleBackColor = true;
            this.btnSubsequent.Click += new System.EventHandler(this.btnSubsequent_Click);
            // 
            // btnDownload
            // 
            this.btnDownload.Location = new System.Drawing.Point(16, 260);
            this.btnDownload.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(212, 28);
            this.btnDownload.TabIndex = 2;
            this.btnDownload.Text = "Save Joined File";
            this.btnDownload.UseVisualStyleBackColor = true;
            this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(16, 11);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Step 1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(16, 119);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Step 2";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(16, 224);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Step 3";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 32);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(274, 32);
            this.label4.TabIndex = 6;
            this.label4.Text = "Open the primary file that contains the correct \r\nagency and identification data." +
    "";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 135);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(299, 32);
            this.label5.TabIndex = 7;
            this.label5.Text = "Open secondary files that contain additional data.\r\nRepeat as manay times as ness" +
    "ecary.";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(16, 240);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(162, 16);
            this.label6.TabIndex = 8;
            this.label6.Text = "Download the resulting file";
            // 
            // rtbOutput
            // 
            this.rtbOutput.BackColor = System.Drawing.Color.White;
            this.rtbOutput.Location = new System.Drawing.Point(384, 15);
            this.rtbOutput.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rtbOutput.Name = "rtbOutput";
            this.rtbOutput.ReadOnly = true;
            this.rtbOutput.Size = new System.Drawing.Size(477, 272);
            this.rtbOutput.TabIndex = 9;
            this.rtbOutput.Text = "";
            this.rtbOutput.WordWrap = false;
            // 
            // lblReset
            // 
            this.lblReset.AutoSize = true;
            this.lblReset.Location = new System.Drawing.Point(236, 74);
            this.lblReset.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblReset.Name = "lblReset";
            this.lblReset.Size = new System.Drawing.Size(43, 16);
            this.lblReset.TabIndex = 10;
            this.lblReset.TabStop = true;
            this.lblReset.Text = "Reset";
            this.lblReset.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblReset_LinkClicked);
            // 
            // saveCSVFileDialog
            // 
            this.saveCSVFileDialog.Filter = "CSV files (*.csv)|*.csv";
            this.saveCSVFileDialog.Title = "Save HACC MDS Submission File";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(879, 303);
            this.Controls.Add(this.lblReset);
            this.Controls.Add(this.rtbOutput);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnDownload);
            this.Controls.Add(this.btnSubsequent);
            this.Controls.Add(this.btnSrc);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "frmMain";
            this.Text = "HACC MDS Joiner";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openCSVFileDialog;
        private System.Windows.Forms.Button btnSrc;
        private System.Windows.Forms.Button btnSubsequent;
        private System.Windows.Forms.Button btnDownload;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.RichTextBox rtbOutput;
        private System.Windows.Forms.LinkLabel lblReset;
        private System.Windows.Forms.SaveFileDialog saveCSVFileDialog;
    }
}

