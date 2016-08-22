namespace imgcrypt
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.label1 = new System.Windows.Forms.Label();
            this.file = new System.Windows.Forms.TextBox();
            this.pickFile = new System.Windows.Forms.Button();
            this.outputImage = new System.Windows.Forms.PictureBox();
            this.doWork = new System.Windows.Forms.Button();
            this.saveImg = new System.Windows.Forms.Button();
            this.loadImg = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.outputImage)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Select a File:";
            // 
            // file
            // 
            this.file.Location = new System.Drawing.Point(12, 25);
            this.file.Name = "file";
            this.file.ReadOnly = true;
            this.file.Size = new System.Drawing.Size(196, 20);
            this.file.TabIndex = 1;
            this.file.Text = "...";
            this.file.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // pickFile
            // 
            this.pickFile.Location = new System.Drawing.Point(214, 23);
            this.pickFile.Name = "pickFile";
            this.pickFile.Size = new System.Drawing.Size(75, 23);
            this.pickFile.TabIndex = 2;
            this.pickFile.Text = "Browse";
            this.pickFile.UseVisualStyleBackColor = true;
            this.pickFile.Click += new System.EventHandler(this.pickFile_Click);
            // 
            // outputImage
            // 
            this.outputImage.Location = new System.Drawing.Point(12, 51);
            this.outputImage.Name = "outputImage";
            this.outputImage.Size = new System.Drawing.Size(270, 136);
            this.outputImage.TabIndex = 3;
            this.outputImage.TabStop = false;
            // 
            // doWork
            // 
            this.doWork.Location = new System.Drawing.Point(12, 193);
            this.doWork.Name = "doWork";
            this.doWork.Size = new System.Drawing.Size(75, 23);
            this.doWork.TabIndex = 4;
            this.doWork.Text = "Pixelate";
            this.doWork.UseVisualStyleBackColor = true;
            this.doWork.Click += new System.EventHandler(this.doWork_Click);
            // 
            // saveImg
            // 
            this.saveImg.Location = new System.Drawing.Point(93, 193);
            this.saveImg.Name = "saveImg";
            this.saveImg.Size = new System.Drawing.Size(75, 23);
            this.saveImg.TabIndex = 5;
            this.saveImg.Text = "Save Image";
            this.saveImg.UseVisualStyleBackColor = true;
            this.saveImg.Click += new System.EventHandler(this.saveImg_Click);
            // 
            // loadImg
            // 
            this.loadImg.Location = new System.Drawing.Point(207, 193);
            this.loadImg.Name = "loadImg";
            this.loadImg.Size = new System.Drawing.Size(75, 23);
            this.loadImg.TabIndex = 6;
            this.loadImg.Text = "Load Image";
            this.loadImg.UseVisualStyleBackColor = true;
            this.loadImg.Click += new System.EventHandler(this.loadImg_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(294, 226);
            this.Controls.Add(this.loadImg);
            this.Controls.Add(this.saveImg);
            this.Controls.Add(this.doWork);
            this.Controls.Add(this.outputImage);
            this.Controls.Add(this.pickFile);
            this.Controls.Add(this.file);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Text = "imgcrypt v0.1";
            ((System.ComponentModel.ISupportInitialize)(this.outputImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox file;
        private System.Windows.Forms.Button pickFile;
        private System.Windows.Forms.PictureBox outputImage;
        private System.Windows.Forms.Button doWork;
        private System.Windows.Forms.Button saveImg;
        private System.Windows.Forms.Button loadImg;
    }
}

