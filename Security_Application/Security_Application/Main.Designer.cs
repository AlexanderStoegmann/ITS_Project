namespace Security_Application
{
    partial class Main
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnCreateAsymmetric = new System.Windows.Forms.Button();
            this.btnExportPublicRSAKey = new System.Windows.Forms.Button();
            this.btnEncryptOneFile = new System.Windows.Forms.Button();
            this.btnDecryptOneFile = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.openFileDialog2 = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(125, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "RSA-Key Handling";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 98);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 17);
            this.label2.TabIndex = 1;
            this.label2.Text = "File Handling";
            // 
            // btnCreateAsymmetric
            // 
            this.btnCreateAsymmetric.Location = new System.Drawing.Point(18, 42);
            this.btnCreateAsymmetric.Name = "btnCreateAsymmetric";
            this.btnCreateAsymmetric.Size = new System.Drawing.Size(160, 30);
            this.btnCreateAsymmetric.TabIndex = 2;
            this.btnCreateAsymmetric.Text = "Create Asymmetric RSA-Keys";
            this.btnCreateAsymmetric.UseVisualStyleBackColor = true;
            this.btnCreateAsymmetric.Click += new System.EventHandler(this.btnCreateAsymmetric_Click);
            // 
            // btnExportPublicRSAKey
            // 
            this.btnExportPublicRSAKey.Location = new System.Drawing.Point(225, 42);
            this.btnExportPublicRSAKey.Name = "btnExportPublicRSAKey";
            this.btnExportPublicRSAKey.Size = new System.Drawing.Size(160, 30);
            this.btnExportPublicRSAKey.TabIndex = 3;
            this.btnExportPublicRSAKey.Text = "Export public RSA-Key";
            this.btnExportPublicRSAKey.UseVisualStyleBackColor = true;
            this.btnExportPublicRSAKey.Click += new System.EventHandler(this.btnExportPublicRSAKey_Click);
            // 
            // btnEncryptOneFile
            // 
            this.btnEncryptOneFile.Location = new System.Drawing.Point(18, 127);
            this.btnEncryptOneFile.Name = "btnEncryptOneFile";
            this.btnEncryptOneFile.Size = new System.Drawing.Size(160, 30);
            this.btnEncryptOneFile.TabIndex = 4;
            this.btnEncryptOneFile.Text = "Encrypt One File";
            this.btnEncryptOneFile.UseVisualStyleBackColor = true;
            this.btnEncryptOneFile.Click += new System.EventHandler(this.btnEncryptOneFile_Click);
            // 
            // btnDecryptOneFile
            // 
            this.btnDecryptOneFile.Location = new System.Drawing.Point(225, 126);
            this.btnDecryptOneFile.Name = "btnDecryptOneFile";
            this.btnDecryptOneFile.Size = new System.Drawing.Size(160, 31);
            this.btnDecryptOneFile.TabIndex = 5;
            this.btnDecryptOneFile.Text = "Decrypt One File";
            this.btnDecryptOneFile.UseVisualStyleBackColor = true;
            this.btnDecryptOneFile.Click += new System.EventHandler(this.btnDecryptOneFile_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(18, 191);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(160, 30);
            this.btnClear.TabIndex = 6;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(225, 191);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(160, 30);
            this.btnExit.TabIndex = 7;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // openFileDialog2
            // 
            this.openFileDialog2.FileName = "openFileDialog2";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(408, 239);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnDecryptOneFile);
            this.Controls.Add(this.btnEncryptOneFile);
            this.Controls.Add(this.btnExportPublicRSAKey);
            this.Controls.Add(this.btnCreateAsymmetric);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Main";
            this.Text = "RSA Encryption & Decryption";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnCreateAsymmetric;
        private System.Windows.Forms.Button btnExportPublicRSAKey;
        private System.Windows.Forms.Button btnEncryptOneFile;
        private System.Windows.Forms.Button btnDecryptOneFile;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog2;
    }
}

