namespace Pocket_Updater
{
    partial class Update_Pocket
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Update_Pocket));
            this.Button_Removable = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.Button_Refresh = new System.Windows.Forms.Button();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // Button_Removable
            // 
            this.Button_Removable.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Button_Removable.Enabled = false;
            this.Button_Removable.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.Button_Removable.Location = new System.Drawing.Point(413, 25);
            this.Button_Removable.Name = "Button_Removable";
            this.Button_Removable.Size = new System.Drawing.Size(142, 46);
            this.Button_Removable.TabIndex = 1;
            this.Button_Removable.Text = "Update Cores";
            this.Button_Removable.UseVisualStyleBackColor = true;
            this.Button_Removable.Click += new System.EventHandler(this.updateCoresButton_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(198, 78);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(209, 33);
            this.comboBox1.TabIndex = 3;
            this.comboBox1.Visible = false;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // Button_Refresh
            // 
            this.Button_Refresh.BackColor = System.Drawing.Color.Transparent;
            this.Button_Refresh.BackgroundImage = global::Pocket_Updater.Properties.Resources.refresh;
            this.Button_Refresh.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Button_Refresh.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Button_Refresh.FlatAppearance.BorderSize = 0;
            this.Button_Refresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Button_Refresh.Location = new System.Drawing.Point(413, 78);
            this.Button_Refresh.Name = "Button_Refresh";
            this.Button_Refresh.Size = new System.Drawing.Size(39, 38);
            this.Button_Refresh.TabIndex = 4;
            this.Button_Refresh.UseVisualStyleBackColor = true;
            this.Button_Refresh.Visible = false;
            this.Button_Refresh.Click += new System.EventHandler(this.Button_Refresh_Click);
            // 
            // comboBox2
            // 
            this.comboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Items.AddRange(new object[] {
            "Current Directory",
            "Removable Storage"});
            this.comboBox2.Location = new System.Drawing.Point(175, 33);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(232, 33);
            this.comboBox2.TabIndex = 5;
            this.comboBox2.SelectedIndexChanged += new System.EventHandler(this.comboBox2_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(12, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(157, 25);
            this.label1.TabIndex = 6;
            this.label1.Text = "Update Location:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(12, 81);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(180, 25);
            this.label2.TabIndex = 7;
            this.label2.Text = "Pocket Drive Letter:";
            this.label2.Visible = false;
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 50000;
            this.toolTip1.InitialDelay = 500;
            this.toolTip1.ReshowDelay = 100;
            this.toolTip1.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.toolTip1.ToolTipTitle = "Refresh";
            // 
            // Update_Pocket
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(667, 134);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBox2);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.Button_Refresh);
            this.Controls.Add(this.Button_Removable);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.MaximizeBox = false;
            this.Name = "Update_Pocket";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Update Pocket";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Button Button_Removable;
        private ComboBox comboBox1;
        private Button Button_Refresh;
        private ComboBox comboBox2;
        private Label label1;
        private Label label2;
        private ToolTip toolTip1;
    }
}