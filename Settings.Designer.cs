namespace Pocket_Updater
{
    partial class Settings
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Settings));
            this.label1 = new System.Windows.Forms.Label();
            this.Button_Save = new System.Windows.Forms.Button();
            this.GitHub_Token = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.Core_Images = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip2 = new System.Windows.Forms.ToolTip(this.components);
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.Download_Assets = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.Download_Firmware = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.toolTip3 = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip4 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(126, 134);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(134, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "GitHub Token:";
            // 
            // Button_Save
            // 
            this.Button_Save.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.Button_Save.Location = new System.Drawing.Point(280, 224);
            this.Button_Save.Name = "Button_Save";
            this.Button_Save.Size = new System.Drawing.Size(112, 34);
            this.Button_Save.TabIndex = 4;
            this.Button_Save.Text = "Save";
            this.Button_Save.UseVisualStyleBackColor = true;
            this.Button_Save.Click += new System.EventHandler(this.Buttons_Save_Click);
            // 
            // GitHub_Token
            // 
            this.GitHub_Token.Location = new System.Drawing.Point(126, 162);
            this.GitHub_Token.Name = "GitHub_Token";
            this.GitHub_Token.Size = new System.Drawing.Size(395, 31);
            this.GitHub_Token.TabIndex = 3;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::Pocket_Updater.Properties.Resources.question;
            this.pictureBox1.Location = new System.Drawing.Point(527, 162);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(38, 31);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(59, 97);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(201, 25);
            this.label2.TabIndex = 4;
            this.label2.Text = "Preserve Core Images:";
            // 
            // Core_Images
            // 
            this.Core_Images.AutoSize = true;
            this.Core_Images.Location = new System.Drawing.Point(266, 101);
            this.Core_Images.Name = "Core_Images";
            this.Core_Images.Size = new System.Drawing.Size(22, 21);
            this.Core_Images.TabIndex = 2;
            this.Core_Images.UseVisualStyleBackColor = true;
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 50000;
            this.toolTip1.InitialDelay = 500;
            this.toolTip1.ReshowDelay = 100;
            this.toolTip1.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.toolTip1.ToolTipTitle = "GitHub Token";
            // 
            // toolTip2
            // 
            this.toolTip2.AutoPopDelay = 50000;
            this.toolTip2.InitialDelay = 500;
            this.toolTip2.ReshowDelay = 100;
            this.toolTip2.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.toolTip2.ToolTipTitle = "Preserve Core Images";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::Pocket_Updater.Properties.Resources.question;
            this.pictureBox2.Location = new System.Drawing.Point(294, 97);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(38, 31);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 6;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::Pocket_Updater.Properties.Resources.question;
            this.pictureBox3.Location = new System.Drawing.Point(294, 60);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(38, 31);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox3.TabIndex = 9;
            this.pictureBox3.TabStop = false;
            // 
            // Download_Assets
            // 
            this.Download_Assets.AutoSize = true;
            this.Download_Assets.Location = new System.Drawing.Point(266, 64);
            this.Download_Assets.Name = "Download_Assets";
            this.Download_Assets.Size = new System.Drawing.Size(22, 21);
            this.Download_Assets.TabIndex = 1;
            this.Download_Assets.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label3.Location = new System.Drawing.Point(61, 60);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(199, 25);
            this.label3.TabIndex = 7;
            this.label3.Text = "Download Roms/Bios:";
            // 
            // pictureBox4
            // 
            this.pictureBox4.Image = global::Pocket_Updater.Properties.Resources.question;
            this.pictureBox4.Location = new System.Drawing.Point(294, 23);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(38, 31);
            this.pictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox4.TabIndex = 12;
            this.pictureBox4.TabStop = false;
            // 
            // Download_Firmware
            // 
            this.Download_Firmware.AutoSize = true;
            this.Download_Firmware.Location = new System.Drawing.Point(266, 27);
            this.Download_Firmware.Name = "Download_Firmware";
            this.Download_Firmware.Size = new System.Drawing.Size(22, 21);
            this.Download_Firmware.TabIndex = 0;
            this.Download_Firmware.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label4.Location = new System.Drawing.Point(12, 23);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(248, 25);
            this.label4.TabIndex = 10;
            this.label4.Text = "Download Pocket Firmware:";
            // 
            // toolTip3
            // 
            this.toolTip3.AutoPopDelay = 50000;
            this.toolTip3.InitialDelay = 500;
            this.toolTip3.ReshowDelay = 100;
            this.toolTip3.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.toolTip3.ToolTipTitle = "Download Roms/Bios";
            // 
            // toolTip4
            // 
            this.toolTip4.AutoPopDelay = 50000;
            this.toolTip4.InitialDelay = 500;
            this.toolTip4.ReshowDelay = 100;
            this.toolTip4.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.toolTip4.ToolTipTitle = "Download Pocket Firmware";
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(662, 285);
            this.Controls.Add(this.pictureBox4);
            this.Controls.Add(this.Download_Firmware);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.Download_Assets);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.Core_Images);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.GitHub_Token);
            this.Controls.Add(this.Button_Save);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Settings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.Settings_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label label1;
        private Button Button_Save;
        private TextBox textBox1;
        private PictureBox pictureBox1;
        private Label label2;
        private CheckBox Core_Images;
        private ToolTip toolTip1;
        private ToolTip toolTip2;
        private PictureBox pictureBox2;
        private TextBox GitHub_Token;
        private PictureBox pictureBox3;
        private CheckBox Download_Assets;
        private Label label3;
        private PictureBox pictureBox4;
        private CheckBox Download_Firmware;
        private Label label4;
        private ToolTip toolTip3;
        private ToolTip toolTip4;
    }
}