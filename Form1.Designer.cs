namespace Pocket_Updater
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkForAppUpdatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Button_Pocket = new System.Windows.Forms.Button();
            this.Button_Cores = new System.Windows.Forms.Button();
            this.Button_Settings = new System.Windows.Forms.Button();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.linkLabel2 = new System.Windows.Forms.LinkLabel();
            this.linkLabel3 = new System.Windows.Forms.LinkLabel();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(40, 40);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(472, 33);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.checkForAppUpdatesToolStripMenuItem,
            this.updateLogToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(54, 29);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // checkForAppUpdatesToolStripMenuItem
            // 
            this.checkForAppUpdatesToolStripMenuItem.Name = "checkForAppUpdatesToolStripMenuItem";
            this.checkForAppUpdatesToolStripMenuItem.Size = new System.Drawing.Size(299, 34);
            this.checkForAppUpdatesToolStripMenuItem.Text = "Check for App Updates";
            this.checkForAppUpdatesToolStripMenuItem.Click += new System.EventHandler(this.checkForAppUpdatesToolStripMenuItem_Click);
            // 
            // updateLogToolStripMenuItem
            // 
            this.updateLogToolStripMenuItem.Name = "updateLogToolStripMenuItem";
            this.updateLogToolStripMenuItem.Size = new System.Drawing.Size(299, 34);
            this.updateLogToolStripMenuItem.Text = "Last Update Log";
            this.updateLogToolStripMenuItem.Click += new System.EventHandler(this.updateLogToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(299, 34);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // Button_Pocket
            // 
            this.Button_Pocket.BackColor = System.Drawing.Color.Transparent;
            this.Button_Pocket.BackgroundImage = global::Pocket_Updater.Properties.Resources.updater_to_pocket;
            this.Button_Pocket.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Button_Pocket.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Button_Pocket.FlatAppearance.BorderSize = 0;
            this.Button_Pocket.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Button_Pocket.Location = new System.Drawing.Point(111, 58);
            this.Button_Pocket.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Button_Pocket.Name = "Button_Pocket";
            this.Button_Pocket.Size = new System.Drawing.Size(70, 59);
            this.Button_Pocket.TabIndex = 2;
            this.Button_Pocket.UseVisualStyleBackColor = false;
            this.Button_Pocket.Click += new System.EventHandler(this.Button_Pocket_Click);
            // 
            // Button_Cores
            // 
            this.Button_Cores.BackColor = System.Drawing.Color.Transparent;
            this.Button_Cores.BackgroundImage = global::Pocket_Updater.Properties.Resources.manage_cores;
            this.Button_Cores.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Button_Cores.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Button_Cores.FlatAppearance.BorderSize = 0;
            this.Button_Cores.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Button_Cores.Location = new System.Drawing.Point(111, 130);
            this.Button_Cores.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Button_Cores.Name = "Button_Cores";
            this.Button_Cores.Size = new System.Drawing.Size(70, 59);
            this.Button_Cores.TabIndex = 13;
            this.Button_Cores.UseVisualStyleBackColor = false;
            this.Button_Cores.Click += new System.EventHandler(this.Button_Cores_Click);
            // 
            // Button_Settings
            // 
            this.Button_Settings.BackColor = System.Drawing.Color.Transparent;
            this.Button_Settings.BackgroundImage = global::Pocket_Updater.Properties.Resources.settings;
            this.Button_Settings.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Button_Settings.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Button_Settings.FlatAppearance.BorderSize = 0;
            this.Button_Settings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Button_Settings.Location = new System.Drawing.Point(111, 202);
            this.Button_Settings.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Button_Settings.Name = "Button_Settings";
            this.Button_Settings.Size = new System.Drawing.Size(70, 59);
            this.Button_Settings.TabIndex = 15;
            this.Button_Settings.UseVisualStyleBackColor = false;
            this.Button_Settings.Click += new System.EventHandler(this.Button_Settings_Click);
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.linkLabel1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.linkLabel1.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.linkLabel1.LinkColor = System.Drawing.Color.Black;
            this.linkLabel1.Location = new System.Drawing.Point(187, 72);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(149, 28);
            this.linkLabel1.TabIndex = 17;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Update Pocket";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // linkLabel2
            // 
            this.linkLabel2.AutoSize = true;
            this.linkLabel2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.linkLabel2.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.linkLabel2.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.linkLabel2.LinkColor = System.Drawing.Color.Black;
            this.linkLabel2.Location = new System.Drawing.Point(187, 144);
            this.linkLabel2.Name = "linkLabel2";
            this.linkLabel2.Size = new System.Drawing.Size(146, 28);
            this.linkLabel2.TabIndex = 18;
            this.linkLabel2.TabStop = true;
            this.linkLabel2.Text = "Manage Cores";
            this.linkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel2_LinkClicked);
            // 
            // linkLabel3
            // 
            this.linkLabel3.AutoSize = true;
            this.linkLabel3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.linkLabel3.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.linkLabel3.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.linkLabel3.LinkColor = System.Drawing.Color.Black;
            this.linkLabel3.Location = new System.Drawing.Point(187, 216);
            this.linkLabel3.Name = "linkLabel3";
            this.linkLabel3.Size = new System.Drawing.Size(89, 28);
            this.linkLabel3.TabIndex = 19;
            this.linkLabel3.TabStop = true;
            this.linkLabel3.Text = "Settings";
            this.linkLabel3.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel3_LinkClicked);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(472, 277);
            this.Controls.Add(this.linkLabel3);
            this.Controls.Add(this.linkLabel2);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.Button_Settings);
            this.Controls.Add(this.Button_Cores);
            this.Controls.Add(this.Button_Pocket);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Pocket Updater v1.3.2";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Button Button_Pocket;
        private Button Button_Cores;
        private Button Button_Settings;
        private ToolStripMenuItem updateLogToolStripMenuItem;
        private ToolStripMenuItem checkForAppUpdatesToolStripMenuItem;
        private LinkLabel linkLabel1;
        private LinkLabel linkLabel2;
        private LinkLabel linkLabel3;
    }
}
