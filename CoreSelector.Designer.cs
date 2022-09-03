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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CoreSelector));
            this.coresList = new System.Windows.Forms.CheckedListBox();
            this.Button_Save = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // coresList
            // 
            this.coresList.FormattingEnabled = true;
            this.coresList.Location = new System.Drawing.Point(11, 22);
            this.coresList.Margin = new System.Windows.Forms.Padding(2);
            this.coresList.Name = "coresList";
            this.coresList.Size = new System.Drawing.Size(456, 508);
            this.coresList.TabIndex = 0;
            // 
            // Button_Save
            // 
            this.Button_Save.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.Button_Save.Location = new System.Drawing.Point(183, 551);
            this.Button_Save.Margin = new System.Windows.Forms.Padding(2);
            this.Button_Save.Name = "Button_Save";
            this.Button_Save.Size = new System.Drawing.Size(111, 35);
            this.Button_Save.TabIndex = 1;
            this.Button_Save.Text = "Save";
            this.Button_Save.UseVisualStyleBackColor = true;
            this.Button_Save.Click += new System.EventHandler(this.button1_Click);
            // 
            // CoreSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(506, 612);
            this.Controls.Add(this.Button_Save);
            this.Controls.Add(this.coresList);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.Name = "CoreSelector";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Manage Cores";
            this.Load += new System.EventHandler(this.CoreSelector_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private CheckedListBox coresList;
        private Button Button_Save;
        private CheckBox checkBox1;
    }
}