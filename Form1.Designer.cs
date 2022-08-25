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
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.Button_Log = new System.Windows.Forms.Button();
            this.Button_Pocket = new System.Windows.Forms.Button();
            this.Button_PC = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(40, 40);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(482, 33);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(54, 29);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(141, 34);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(78, 29);
            this.aboutToolStripMenuItem.Text = "About";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label4.Location = new System.Drawing.Point(152, 77);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(143, 24);
            this.label4.TabIndex = 9;
            this.label4.Text = "Updater to PC";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(152, 163);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(181, 24);
            this.label1.TabIndex = 10;
            this.label1.Text = "Updater to Pocket";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(152, 245);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(119, 24);
            this.label2.TabIndex = 11;
            this.label2.Text = "Update Log";
            // 
            // Button_Log
            // 
            this.Button_Log.AutoSize = true;
            this.Button_Log.BackColor = System.Drawing.Color.Transparent;
            this.Button_Log.BackgroundImage = global::Pocket_Updater.Properties.Resources.update_log;
            this.Button_Log.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Button_Log.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Button_Log.FlatAppearance.BorderSize = 0;
            this.Button_Log.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Button_Log.Location = new System.Drawing.Point(76, 228);
            this.Button_Log.Name = "Button_Log";
            this.Button_Log.Size = new System.Drawing.Size(70, 59);
            this.Button_Log.TabIndex = 3;
            this.Button_Log.UseVisualStyleBackColor = false;
            this.Button_Log.Click += new System.EventHandler(this.Button_Log_Click);
            // 
            // Button_Pocket
            // 
            this.Button_Pocket.BackColor = System.Drawing.Color.Transparent;
            this.Button_Pocket.BackgroundImage = global::Pocket_Updater.Properties.Resources.updater_to_pocket;
            this.Button_Pocket.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Button_Pocket.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Button_Pocket.FlatAppearance.BorderSize = 0;
            this.Button_Pocket.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Button_Pocket.Location = new System.Drawing.Point(76, 146);
            this.Button_Pocket.Name = "Button_Pocket";
            this.Button_Pocket.Size = new System.Drawing.Size(70, 59);
            this.Button_Pocket.TabIndex = 2;
            this.Button_Pocket.UseVisualStyleBackColor = false;
            // 
            // Button_PC
            // 
            this.Button_PC.BackColor = System.Drawing.Color.Transparent;
            this.Button_PC.BackgroundImage = global::Pocket_Updater.Properties.Resources.updater_to_pc;
            this.Button_PC.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Button_PC.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Button_PC.FlatAppearance.BorderSize = 0;
            this.Button_PC.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Button_PC.Location = new System.Drawing.Point(76, 60);
            this.Button_PC.Name = "Button_PC";
            this.Button_PC.Size = new System.Drawing.Size(70, 59);
            this.Button_PC.TabIndex = 1;
            this.Button_PC.UseVisualStyleBackColor = false;
            this.Button_PC.Click += new System.EventHandler(this.Button_PC_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(482, 319);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.Button_Log);
            this.Controls.Add(this.Button_Pocket);
            this.Controls.Add(this.Button_PC);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Pocket Cores Updater v1.0";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.Button Button_PC;
        private System.Windows.Forms.Button Button_Pocket;
        private System.Windows.Forms.Button Button_Log;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}