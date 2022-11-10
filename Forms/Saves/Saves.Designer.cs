namespace Pocket_Updater.Forms.Saves
{
    partial class Saves
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Saves));
            this.Backup_Saves = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.Button_Browse = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.Button_Refresh = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.Pocket_Drive = new System.Windows.Forms.ComboBox();
            this.Button_Sync = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.SuspendLayout();
            // 
            // Backup_Saves
            // 
            this.Backup_Saves.AutoSize = true;
            this.Backup_Saves.Checked = true;
            this.Backup_Saves.CheckState = System.Windows.Forms.CheckState.Checked;
            this.Backup_Saves.Location = new System.Drawing.Point(232, 123);
            this.Backup_Saves.Name = "Backup_Saves";
            this.Backup_Saves.Size = new System.Drawing.Size(22, 21);
            this.Backup_Saves.TabIndex = 28;
            this.Backup_Saves.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label4.Location = new System.Drawing.Point(93, 119);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(133, 25);
            this.label4.TabIndex = 27;
            this.label4.Text = "Backup Saves:";
            // 
            // Button_Browse
            // 
            this.Button_Browse.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Button_Browse.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.Button_Browse.Location = new System.Drawing.Point(447, 66);
            this.Button_Browse.Name = "Button_Browse";
            this.Button_Browse.Size = new System.Drawing.Size(108, 38);
            this.Button_Browse.TabIndex = 26;
            this.Button_Browse.Text = "Browse";
            this.Button_Browse.UseVisualStyleBackColor = true;
            this.Button_Browse.Click += new System.EventHandler(this.Button_Browse_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(232, 70);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(209, 31);
            this.textBox1.TabIndex = 25;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label3.Location = new System.Drawing.Point(31, 73);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(195, 25);
            this.label3.TabIndex = 24;
            this.label3.Text = "Local/Network Saves:";
            // 
            // Button_Refresh
            // 
            this.Button_Refresh.BackColor = System.Drawing.Color.Transparent;
            this.Button_Refresh.BackgroundImage = global::Pocket_Updater.Properties.Resources.refresh;
            this.Button_Refresh.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Button_Refresh.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Button_Refresh.FlatAppearance.BorderSize = 0;
            this.Button_Refresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Button_Refresh.Location = new System.Drawing.Point(447, 18);
            this.Button_Refresh.Name = "Button_Refresh";
            this.Button_Refresh.Size = new System.Drawing.Size(39, 38);
            this.Button_Refresh.TabIndex = 23;
            this.Button_Refresh.UseVisualStyleBackColor = true;
            this.Button_Refresh.Click += new System.EventHandler(this.Button_Refresh_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(46, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(180, 25);
            this.label2.TabIndex = 22;
            this.label2.Text = "Pocket Drive Letter:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(46, 73);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 25);
            this.label1.TabIndex = 21;
            // 
            // Pocket_Drive
            // 
            this.Pocket_Drive.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Pocket_Drive.FormattingEnabled = true;
            this.Pocket_Drive.Location = new System.Drawing.Point(232, 22);
            this.Pocket_Drive.Name = "Pocket_Drive";
            this.Pocket_Drive.Size = new System.Drawing.Size(209, 33);
            this.Pocket_Drive.TabIndex = 20;
            this.Pocket_Drive.SelectedIndexChanged += new System.EventHandler(this.Pocket_Drive_SelectedIndexChanged);
            // 
            // Button_Sync
            // 
            this.Button_Sync.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Button_Sync.Enabled = false;
            this.Button_Sync.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.Button_Sync.Location = new System.Drawing.Point(232, 175);
            this.Button_Sync.Name = "Button_Sync";
            this.Button_Sync.Size = new System.Drawing.Size(142, 46);
            this.Button_Sync.TabIndex = 19;
            this.Button_Sync.Text = "Sync Saves";
            this.Button_Sync.UseVisualStyleBackColor = true;
            this.Button_Sync.Click += new System.EventHandler(this.Button_Sync_Click);
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 50000;
            this.toolTip1.InitialDelay = 500;
            this.toolTip1.ReshowDelay = 100;
            this.toolTip1.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.toolTip1.ToolTipTitle = "Refresh";
            // 
            // Saves
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(614, 252);
            this.Controls.Add(this.Backup_Saves);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.Button_Browse);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.Button_Refresh);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Pocket_Drive);
            this.Controls.Add(this.Button_Sync);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Saves";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Save File Sync";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CheckBox Backup_Saves;
        private Label label4;
        private Button Button_Browse;
        private TextBox textBox1;
        private Label label3;
        private Button Button_Refresh;
        private Label label2;
        private Label label1;
        private ComboBox Pocket_Drive;
        private Button Button_Sync;
        private ToolTip toolTip1;
        private FolderBrowserDialog folderBrowserDialog1;
    }
}