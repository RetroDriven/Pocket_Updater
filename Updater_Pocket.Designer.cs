namespace Pocket_Updater
{
    partial class Updater_Pocket
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Updater_Pocket));
            this.updateCoresButton = new System.Windows.Forms.Button();
            this.infoTextBox = new System.Windows.Forms.TextBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.Button_Refresh = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // updateCoresButton
            // 
            this.updateCoresButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.updateCoresButton.Enabled = false;
            this.updateCoresButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.updateCoresButton.Location = new System.Drawing.Point(309, 21);
            this.updateCoresButton.Name = "updateCoresButton";
            this.updateCoresButton.Size = new System.Drawing.Size(137, 54);
            this.updateCoresButton.TabIndex = 1;
            this.updateCoresButton.Text = "Update Cores";
            this.updateCoresButton.UseVisualStyleBackColor = true;
            this.updateCoresButton.Click += new System.EventHandler(this.updateCoresButton_Click);
            // 
            // infoTextBox
            // 
            this.infoTextBox.Location = new System.Drawing.Point(12, 91);
            this.infoTextBox.Multiline = true;
            this.infoTextBox.Name = "infoTextBox";
            this.infoTextBox.ReadOnly = true;
            this.infoTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.infoTextBox.Size = new System.Drawing.Size(644, 584);
            this.infoTextBox.TabIndex = 2;
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(12, 33);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(182, 33);
            this.comboBox1.TabIndex = 3;
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
            this.Button_Refresh.Location = new System.Drawing.Point(200, 29);
            this.Button_Refresh.Name = "Button_Refresh";
            this.Button_Refresh.Size = new System.Drawing.Size(39, 38);
            this.Button_Refresh.TabIndex = 4;
            this.Button_Refresh.UseVisualStyleBackColor = true;
            this.Button_Refresh.Click += new System.EventHandler(this.Button_Refresh_Click);
            // 
            // Updater_Pocket
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(667, 691);
            this.Controls.Add(this.Button_Refresh);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.infoTextBox);
            this.Controls.Add(this.updateCoresButton);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.MaximizeBox = false;
            this.Name = "Updater_Pocket";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Updater to Pocket";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Button updateCoresButton;
        private TextBox infoTextBox;
        private ComboBox comboBox1;
        private Button Button_Refresh;
    }
}