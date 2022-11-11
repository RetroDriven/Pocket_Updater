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
            this.checkBox_All = new System.Windows.Forms.CheckBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.CoreName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CoreAuthor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Enabled = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // coresList
            // 
            this.coresList.CheckOnClick = true;
            this.coresList.ColumnWidth = 335;
            this.coresList.FormattingEnabled = true;
            this.coresList.HorizontalScrollbar = true;
            this.coresList.Location = new System.Drawing.Point(11, 54);
            this.coresList.Margin = new System.Windows.Forms.Padding(2);
            this.coresList.MultiColumn = true;
            this.coresList.Name = "coresList";
            this.coresList.Size = new System.Drawing.Size(119, 144);
            this.coresList.Sorted = true;
            this.coresList.TabIndex = 0;
            this.coresList.TabStop = false;
            // 
            // Button_Save
            // 
            this.Button_Save.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.Button_Save.Location = new System.Drawing.Point(475, 623);
            this.Button_Save.Margin = new System.Windows.Forms.Padding(2);
            this.Button_Save.Name = "Button_Save";
            this.Button_Save.Size = new System.Drawing.Size(111, 45);
            this.Button_Save.TabIndex = 1;
            this.Button_Save.Text = "Save";
            this.Button_Save.UseVisualStyleBackColor = true;
            this.Button_Save.Click += new System.EventHandler(this.button1_Click);
            // 
            // checkBox_All
            // 
            this.checkBox_All.Checked = true;
            this.checkBox_All.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_All.Location = new System.Drawing.Point(14, 9);
            this.checkBox_All.Name = "checkBox_All";
            this.checkBox_All.Size = new System.Drawing.Size(116, 29);
            this.checkBox_All.TabIndex = 2;
            this.checkBox_All.Text = "Select All";
            this.checkBox_All.UseVisualStyleBackColor = true;
            this.checkBox_All.CheckedChanged += new System.EventHandler(this.checkBox_All_CheckedChanged);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Raised;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.CoreName,
            this.CoreAuthor,
            this.Enabled});
            this.dataGridView1.Location = new System.Drawing.Point(166, 12);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 62;
            this.dataGridView1.RowTemplate.Height = 33;
            this.dataGridView1.Size = new System.Drawing.Size(882, 606);
            this.dataGridView1.TabIndex = 3;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // CoreName
            // 
            this.CoreName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.CoreName.HeaderText = "Core Name";
            this.CoreName.MinimumWidth = 8;
            this.CoreName.Name = "CoreName";
            this.CoreName.ReadOnly = true;
            // 
            // CoreAuthor
            // 
            this.CoreAuthor.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.CoreAuthor.HeaderText = "Core Author";
            this.CoreAuthor.MinimumWidth = 8;
            this.CoreAuthor.Name = "CoreAuthor";
            this.CoreAuthor.ReadOnly = true;
            // 
            // Enabled
            // 
            this.Enabled.HeaderText = "Enabled";
            this.Enabled.MinimumWidth = 8;
            this.Enabled.Name = "Enabled";
            this.Enabled.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Enabled.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Enabled.Width = 150;
            // 
            // CoreSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1060, 679);
            this.Controls.Add(this.coresList);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.checkBox_All);
            this.Controls.Add(this.Button_Save);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.Name = "CoreSelector";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Manage Cores";
            this.Load += new System.EventHandler(this.CoreSelector_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private CheckedListBox coresList;
        private Button Button_Save;
        private CheckBox checkBox1;
        private CheckBox checkBox_All;
        private DataGridView dataGridView1;
        private DataGridViewTextBoxColumn CoreName;
        private DataGridViewTextBoxColumn CoreAuthor;
        private DataGridViewCheckBoxColumn Enabled;
    }
}