namespace Pocket_Updater
{
    partial class Updater_PC
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
            this.Button_Download = new System.Windows.Forms.Button();
            this.updateCoresButton = new System.Windows.Forms.Button();
            this.infoTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // Button_Download
            // 
            this.Button_Download.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.Button_Download.Location = new System.Drawing.Point(46, 23);
            this.Button_Download.Name = "Button_Download";
            this.Button_Download.Size = new System.Drawing.Size(261, 122);
            this.Button_Download.TabIndex = 0;
            this.Button_Download.Text = "Download JSON File";
            this.Button_Download.UseVisualStyleBackColor = true;
            this.Button_Download.Click += new System.EventHandler(this.Button_Download_Click);
            // 
            // updateCoresButton
            // 
            this.updateCoresButton.Enabled = false;
            this.updateCoresButton.Location = new System.Drawing.Point(345, 34);
            this.updateCoresButton.Name = "updateCoresButton";
            this.updateCoresButton.Size = new System.Drawing.Size(263, 103);
            this.updateCoresButton.TabIndex = 1;
            this.updateCoresButton.Text = "Update Cores";
            this.updateCoresButton.UseVisualStyleBackColor = true;
            this.updateCoresButton.Click += new System.EventHandler(this.updateCoresButton_Click);
            // 
            // infoTextBox
            // 
            this.infoTextBox.Location = new System.Drawing.Point(12, 151);
            this.infoTextBox.Multiline = true;
            this.infoTextBox.Name = "infoTextBox";
            this.infoTextBox.ReadOnly = true;
            this.infoTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.infoTextBox.Size = new System.Drawing.Size(644, 445);
            this.infoTextBox.TabIndex = 2;
            // 
            // Updater_PC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(17F, 36F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(680, 608);
            this.Controls.Add(this.infoTextBox);
            this.Controls.Add(this.updateCoresButton);
            this.Controls.Add(this.Button_Download);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.Name = "Updater_PC";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Updater to PC";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button Button_Download;
        private Button updateCoresButton;
        private TextBox infoTextBox;
    }
}