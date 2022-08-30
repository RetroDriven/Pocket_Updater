namespace Pocket_Updater
{
    partial class CoreSelector
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
            this.coresList = new System.Windows.Forms.CheckedListBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // coresList
            // 
            this.coresList.FormattingEnabled = true;
            this.coresList.Location = new System.Drawing.Point(320, 49);
            this.coresList.Name = "coresList";
            this.coresList.Size = new System.Drawing.Size(772, 840);
            this.coresList.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(1158, 892);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(188, 58);
            this.button1.TabIndex = 1;
            this.button1.Text = "save";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // CoreSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(17F, 41F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1415, 1003);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.coresList);
            this.Name = "CoreSelector";
            this.Text = "CoreSelector";
            this.Load += new System.EventHandler(this.CoreSelector_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CheckedListBox coresList;
        private Button button1;
        private CheckBox checkBox1;
    }
}