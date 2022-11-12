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
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewLogFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearLogFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Button_Pocket = new System.Windows.Forms.Button();
            this.Button_Cores = new System.Windows.Forms.Button();
            this.Button_Settings = new System.Windows.Forms.Button();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.linkLabel2 = new System.Windows.Forms.LinkLabel();
            this.linkLabel3 = new System.Windows.Forms.LinkLabel();
            this.linkLabel4 = new System.Windows.Forms.LinkLabel();
            this.Button_Organize = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(40, 40);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.logToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(562, 33);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.checkForAppUpdatesToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(54, 29);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // checkForAppUpdatesToolStripMenuItem
            // 
            this.checkForAppUpdatesToolStripMenuItem.Name = "checkForAppUpdatesToolStripMenuItem";
            this.checkForAppUpdatesToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.U)));
            this.checkForAppUpdatesToolStripMenuItem.Size = new System.Drawing.Size(362, 34);
            this.checkForAppUpdatesToolStripMenuItem.Text = "Check for App Updates";
            this.checkForAppUpdatesToolStripMenuItem.Click += new System.EventHandler(this.checkForAppUpdatesToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(362, 34);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // logToolStripMenuItem
            // 
            this.logToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewLogFileToolStripMenuItem,
            this.clearLogFileToolStripMenuItem});
            this.logToolStripMenuItem.Name = "logToolStripMenuItem";
            this.logToolStripMenuItem.Size = new System.Drawing.Size(58, 29);
            this.logToolStripMenuItem.Text = "Log";
            // 
            // viewLogFileToolStripMenuItem
            // 
            this.viewLogFileToolStripMenuItem.Name = "viewLogFileToolStripMenuItem";
            this.viewLogFileToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.viewLogFileToolStripMenuItem.Size = new System.Drawing.Size(281, 34);
            this.viewLogFileToolStripMenuItem.Text = "View Log File";
            this.viewLogFileToolStripMenuItem.Click += new System.EventHandler(this.viewLogFileToolStripMenuItem_Click);
            // 
            // clearLogFileToolStripMenuItem
            // 
            this.clearLogFileToolStripMenuItem.Name = "clearLogFileToolStripMenuItem";
            this.clearLogFileToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.clearLogFileToolStripMenuItem.Size = new System.Drawing.Size(281, 34);
            this.clearLogFileToolStripMenuItem.Text = "Clear Log File";
            this.clearLogFileToolStripMenuItem.Click += new System.EventHandler(this.clearLogFileToolStripMenuItem_Click);
            // 
            // Button_Pocket
            // 
            this.Button_Pocket.BackColor = System.Drawing.Color.Transparent;
            this.Button_Pocket.BackgroundImage = global::Pocket_Updater.Properties.Resources.updater_to_pocket;
            this.Button_Pocket.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Button_Pocket.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Button_Pocket.FlatAppearance.BorderSize = 0;
            this.Button_Pocket.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Button_Pocket.Location = new System.Drawing.Point(34, 63);
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
            this.Button_Cores.Location = new System.Drawing.Point(34, 142);
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
            this.Button_Settings.Location = new System.Drawing.Point(306, 63);
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
            this.linkLabel1.Location = new System.Drawing.Point(110, 77);
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
            this.linkLabel2.Location = new System.Drawing.Point(110, 156);
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
            this.linkLabel3.Location = new System.Drawing.Point(382, 77);
            this.linkLabel3.Name = "linkLabel3";
            this.linkLabel3.Size = new System.Drawing.Size(89, 28);
            this.linkLabel3.TabIndex = 19;
            this.linkLabel3.TabStop = true;
            this.linkLabel3.Text = "Settings";
            this.linkLabel3.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel3_LinkClicked);
            // 
            // linkLabel4
            // 
            this.linkLabel4.AutoSize = true;
            this.linkLabel4.Cursor = System.Windows.Forms.Cursors.Hand;
            this.linkLabel4.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.linkLabel4.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.linkLabel4.LinkColor = System.Drawing.Color.Black;
            this.linkLabel4.Location = new System.Drawing.Point(382, 156);
            this.linkLabel4.Name = "linkLabel4";
            this.linkLabel4.Size = new System.Drawing.Size(155, 28);
            this.linkLabel4.TabIndex = 21;
            this.linkLabel4.TabStop = true;
            this.linkLabel4.Text = "Organize Cores";
            this.linkLabel4.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel4_LinkClicked);
            // 
            // Button_Organize
            // 
            this.Button_Organize.BackColor = System.Drawing.Color.Transparent;
            this.Button_Organize.BackgroundImage = global::Pocket_Updater.Properties.Resources.organize;
            this.Button_Organize.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Button_Organize.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Button_Organize.FlatAppearance.BorderSize = 0;
            this.Button_Organize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Button_Organize.Location = new System.Drawing.Point(306, 142);
            this.Button_Organize.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Button_Organize.Name = "Button_Organize";
            this.Button_Organize.Size = new System.Drawing.Size(70, 59);
            this.Button_Organize.TabIndex = 20;
            this.Button_Organize.UseVisualStyleBackColor = false;
            this.Button_Organize.Click += new System.EventHandler(this.Button_Organize_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(562, 227);
            this.Controls.Add(this.linkLabel4);
            this.Controls.Add(this.Button_Organize);
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
            this.Text = "Pocket Updater v1.3.8";
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
        private ToolStripMenuItem checkForAppUpdatesToolStripMenuItem;
        private LinkLabel linkLabel1;
        private LinkLabel linkLabel2;
        private LinkLabel linkLabel3;
        private ToolStripMenuItem logToolStripMenuItem;
        private ToolStripMenuItem viewLogFileToolStripMenuItem;
        private ToolStripMenuItem clearLogFileToolStripMenuItem;
        private LinkLabel linkLabel4;
        private Button Button_Organize;
    }
}
