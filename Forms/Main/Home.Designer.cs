namespace Pocket_Updater.Forms.Main
{
    partial class Home
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Home));
            this.fluentDesignFormContainer1 = new DevExpress.XtraBars.FluentDesignSystem.FluentDesignFormContainer();
            this.update_Pocket1 = new Pocket_Updater.Controls.Update_Pocket();
            this.accordionControl1 = new DevExpress.XtraBars.Navigation.AccordionControl();
            this.UpdatePocket = new DevExpress.XtraBars.Navigation.AccordionControlElement();
            this.ManageCores = new DevExpress.XtraBars.Navigation.AccordionControlElement();
            this.OrganizeCores = new DevExpress.XtraBars.Navigation.AccordionControlElement();
            this.Settings = new DevExpress.XtraBars.Navigation.AccordionControlElement();
            this.ViewLogs = new DevExpress.XtraBars.Navigation.AccordionControlElement();
            this.fluentDesignFormControl1 = new DevExpress.XtraBars.FluentDesignSystem.FluentDesignFormControl();
            this.fluentFormDefaultManager1 = new DevExpress.XtraBars.FluentDesignSystem.FluentFormDefaultManager(this.components);
            this.fluentDesignFormContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.accordionControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fluentDesignFormControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fluentFormDefaultManager1)).BeginInit();
            this.SuspendLayout();
            // 
            // fluentDesignFormContainer1
            // 
            this.fluentDesignFormContainer1.Controls.Add(this.update_Pocket1);
            this.fluentDesignFormContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fluentDesignFormContainer1.Location = new System.Drawing.Point(60, 39);
            this.fluentDesignFormContainer1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.fluentDesignFormContainer1.Name = "fluentDesignFormContainer1";
            this.fluentDesignFormContainer1.Size = new System.Drawing.Size(729, 551);
            this.fluentDesignFormContainer1.TabIndex = 0;
            // 
            // update_Pocket1
            // 
            this.update_Pocket1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.update_Pocket1.Location = new System.Drawing.Point(0, 0);
            this.update_Pocket1.Name = "update_Pocket1";
            this.update_Pocket1.Size = new System.Drawing.Size(729, 551);
            this.update_Pocket1.TabIndex = 0;
            this.update_Pocket1.Visible = false;
            // 
            // accordionControl1
            // 
            this.accordionControl1.Appearance.AccordionControl.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.accordionControl1.Appearance.Item.Hovered.BackColor2 = System.Drawing.Color.LightBlue;
            this.accordionControl1.Appearance.Item.Pressed.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.accordionControl1.Appearance.Item.Pressed.Options.UseFont = true;
            this.accordionControl1.Dock = System.Windows.Forms.DockStyle.Left;
            this.accordionControl1.Elements.AddRange(new DevExpress.XtraBars.Navigation.AccordionControlElement[] {
            this.UpdatePocket,
            this.ManageCores,
            this.OrganizeCores,
            this.Settings,
            this.ViewLogs});
            this.accordionControl1.Location = new System.Drawing.Point(0, 39);
            this.accordionControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.accordionControl1.Name = "accordionControl1";
            this.accordionControl1.OptionsMinimizing.State = DevExpress.XtraBars.Navigation.AccordionControlState.Minimized;
            this.accordionControl1.ScrollBarMode = DevExpress.XtraBars.Navigation.ScrollBarMode.AutoCollapse;
            this.accordionControl1.Size = new System.Drawing.Size(60, 551);
            this.accordionControl1.TabIndex = 1;
            this.accordionControl1.ViewType = DevExpress.XtraBars.Navigation.AccordionControlViewType.HamburgerMenu;
            // 
            // UpdatePocket
            // 
            this.UpdatePocket.Appearance.Default.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.UpdatePocket.Appearance.Default.Options.UseFont = true;
            this.UpdatePocket.Appearance.Disabled.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.UpdatePocket.Appearance.Disabled.Options.UseFont = true;
            this.UpdatePocket.Appearance.Hovered.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.UpdatePocket.Appearance.Hovered.Options.UseFont = true;
            this.UpdatePocket.Appearance.Normal.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.UpdatePocket.Appearance.Normal.Options.UseFont = true;
            this.UpdatePocket.Appearance.Pressed.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.UpdatePocket.Appearance.Pressed.Options.UseFont = true;
            this.UpdatePocket.HeaderTemplate.AddRange(new DevExpress.XtraBars.Navigation.HeaderElementInfo[] {
            new DevExpress.XtraBars.Navigation.HeaderElementInfo(DevExpress.XtraBars.Navigation.HeaderElementType.Image),
            new DevExpress.XtraBars.Navigation.HeaderElementInfo(DevExpress.XtraBars.Navigation.HeaderElementType.HeaderControl),
            new DevExpress.XtraBars.Navigation.HeaderElementInfo(DevExpress.XtraBars.Navigation.HeaderElementType.ContextButtons),
            new DevExpress.XtraBars.Navigation.HeaderElementInfo(DevExpress.XtraBars.Navigation.HeaderElementType.Text)});
            this.UpdatePocket.Hint = "Update Pocket";
            this.UpdatePocket.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("UpdatePocket.ImageOptions.Image")));
            this.UpdatePocket.Name = "UpdatePocket";
            this.UpdatePocket.Style = DevExpress.XtraBars.Navigation.ElementStyle.Item;
            this.UpdatePocket.Text = "Update Pocket";
            this.UpdatePocket.VisibleInFooter = false;
            this.UpdatePocket.Click += new System.EventHandler(this.UpdatePocket_Click);
            this.UpdatePocket.EnabledChanged += new System.EventHandler(this.UpdatePocket_EnabledChanged);
            // 
            // ManageCores
            // 
            this.ManageCores.Appearance.Default.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.ManageCores.Appearance.Default.Options.UseFont = true;
            this.ManageCores.Appearance.Disabled.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.ManageCores.Appearance.Disabled.Options.UseFont = true;
            this.ManageCores.Appearance.Hovered.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.ManageCores.Appearance.Hovered.Options.UseFont = true;
            this.ManageCores.Appearance.Normal.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.ManageCores.Appearance.Normal.Options.UseFont = true;
            this.ManageCores.Appearance.Pressed.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.ManageCores.Appearance.Pressed.Options.UseFont = true;
            this.ManageCores.Expanded = true;
            this.ManageCores.HeaderTemplate.AddRange(new DevExpress.XtraBars.Navigation.HeaderElementInfo[] {
            new DevExpress.XtraBars.Navigation.HeaderElementInfo(DevExpress.XtraBars.Navigation.HeaderElementType.Image),
            new DevExpress.XtraBars.Navigation.HeaderElementInfo(DevExpress.XtraBars.Navigation.HeaderElementType.HeaderControl),
            new DevExpress.XtraBars.Navigation.HeaderElementInfo(DevExpress.XtraBars.Navigation.HeaderElementType.ContextButtons),
            new DevExpress.XtraBars.Navigation.HeaderElementInfo(DevExpress.XtraBars.Navigation.HeaderElementType.Text)});
            this.ManageCores.Hint = "Manage Cores";
            this.ManageCores.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("ManageCores.ImageOptions.Image")));
            this.ManageCores.Name = "ManageCores";
            this.ManageCores.Style = DevExpress.XtraBars.Navigation.ElementStyle.Item;
            this.ManageCores.Text = "Manage Cores";
            this.ManageCores.VisibleInFooter = false;
            // 
            // OrganizeCores
            // 
            this.OrganizeCores.Appearance.Default.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.OrganizeCores.Appearance.Default.Options.UseFont = true;
            this.OrganizeCores.Appearance.Disabled.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.OrganizeCores.Appearance.Disabled.Options.UseFont = true;
            this.OrganizeCores.Appearance.Hovered.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.OrganizeCores.Appearance.Hovered.Options.UseFont = true;
            this.OrganizeCores.Appearance.Normal.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.OrganizeCores.Appearance.Normal.Options.UseFont = true;
            this.OrganizeCores.Appearance.Pressed.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.OrganizeCores.Appearance.Pressed.Options.UseFont = true;
            this.OrganizeCores.HeaderTemplate.AddRange(new DevExpress.XtraBars.Navigation.HeaderElementInfo[] {
            new DevExpress.XtraBars.Navigation.HeaderElementInfo(DevExpress.XtraBars.Navigation.HeaderElementType.Image),
            new DevExpress.XtraBars.Navigation.HeaderElementInfo(DevExpress.XtraBars.Navigation.HeaderElementType.HeaderControl),
            new DevExpress.XtraBars.Navigation.HeaderElementInfo(DevExpress.XtraBars.Navigation.HeaderElementType.ContextButtons),
            new DevExpress.XtraBars.Navigation.HeaderElementInfo(DevExpress.XtraBars.Navigation.HeaderElementType.Text)});
            this.OrganizeCores.Hint = "Organize Cores";
            this.OrganizeCores.ImageOptions.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.False;
            this.OrganizeCores.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("OrganizeCores.ImageOptions.Image")));
            this.OrganizeCores.Name = "OrganizeCores";
            this.OrganizeCores.Style = DevExpress.XtraBars.Navigation.ElementStyle.Item;
            this.OrganizeCores.Text = "Organize Cores";
            this.OrganizeCores.VisibleInFooter = false;
            // 
            // Settings
            // 
            this.Settings.Appearance.Default.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.Settings.Appearance.Default.Options.UseFont = true;
            this.Settings.Appearance.Disabled.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.Settings.Appearance.Disabled.Options.UseFont = true;
            this.Settings.Appearance.Hovered.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.Settings.Appearance.Hovered.Options.UseFont = true;
            this.Settings.Appearance.Normal.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.Settings.Appearance.Normal.Options.UseFont = true;
            this.Settings.Appearance.Pressed.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.Settings.Appearance.Pressed.Options.UseFont = true;
            this.Settings.HeaderTemplate.AddRange(new DevExpress.XtraBars.Navigation.HeaderElementInfo[] {
            new DevExpress.XtraBars.Navigation.HeaderElementInfo(DevExpress.XtraBars.Navigation.HeaderElementType.Image),
            new DevExpress.XtraBars.Navigation.HeaderElementInfo(DevExpress.XtraBars.Navigation.HeaderElementType.HeaderControl, DevExpress.XtraBars.Navigation.HeaderElementAlignment.Left),
            new DevExpress.XtraBars.Navigation.HeaderElementInfo(DevExpress.XtraBars.Navigation.HeaderElementType.ContextButtons),
            new DevExpress.XtraBars.Navigation.HeaderElementInfo(DevExpress.XtraBars.Navigation.HeaderElementType.Text)});
            this.Settings.Hint = "Settings";
            this.Settings.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("Settings.ImageOptions.Image")));
            this.Settings.Name = "Settings";
            this.Settings.Style = DevExpress.XtraBars.Navigation.ElementStyle.Item;
            this.Settings.Text = "Settings";
            // 
            // ViewLogs
            // 
            this.ViewLogs.Appearance.Default.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.ViewLogs.Appearance.Default.Options.UseFont = true;
            this.ViewLogs.Appearance.Disabled.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.ViewLogs.Appearance.Disabled.Options.UseFont = true;
            this.ViewLogs.Appearance.Hovered.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.ViewLogs.Appearance.Hovered.Options.UseFont = true;
            this.ViewLogs.Appearance.Normal.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.ViewLogs.Appearance.Normal.Options.UseFont = true;
            this.ViewLogs.Appearance.Pressed.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.ViewLogs.Appearance.Pressed.Options.UseFont = true;
            this.ViewLogs.Hint = "View Logs";
            this.ViewLogs.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("ViewLogs.ImageOptions.Image")));
            this.ViewLogs.Name = "ViewLogs";
            this.ViewLogs.Style = DevExpress.XtraBars.Navigation.ElementStyle.Item;
            this.ViewLogs.Text = "View Logs";
            // 
            // fluentDesignFormControl1
            // 
            this.fluentDesignFormControl1.FluentDesignForm = this;
            this.fluentDesignFormControl1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.fluentDesignFormControl1.Location = new System.Drawing.Point(0, 0);
            this.fluentDesignFormControl1.Manager = this.fluentFormDefaultManager1;
            this.fluentDesignFormControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.fluentDesignFormControl1.Name = "fluentDesignFormControl1";
            this.fluentDesignFormControl1.Size = new System.Drawing.Size(789, 39);
            this.fluentDesignFormControl1.TabIndex = 2;
            this.fluentDesignFormControl1.TabStop = false;
            // 
            // fluentFormDefaultManager1
            // 
            this.fluentFormDefaultManager1.Form = this;
            // 
            // Home
            // 
            this.Appearance.Options.UseFont = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(789, 590);
            this.ControlContainer = this.fluentDesignFormContainer1;
            this.Controls.Add(this.fluentDesignFormContainer1);
            this.Controls.Add(this.accordionControl1);
            this.Controls.Add(this.fluentDesignFormControl1);
            this.EnableAcrylicAccent = false;
            this.FluentDesignFormControl = this.fluentDesignFormControl1;
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.IconOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("Home.IconOptions.LargeImage")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Home";
            this.NavigationControl = this.accordionControl1;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Pocket Updater v1.4";
            this.fluentDesignFormContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.accordionControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fluentDesignFormControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fluentFormDefaultManager1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private DevExpress.XtraBars.FluentDesignSystem.FluentDesignFormContainer fluentDesignFormContainer1;
        private DevExpress.XtraBars.Navigation.AccordionControl accordionControl1;
        private DevExpress.XtraBars.FluentDesignSystem.FluentDesignFormControl fluentDesignFormControl1;
        private DevExpress.XtraBars.FluentDesignSystem.FluentFormDefaultManager fluentFormDefaultManager1;
        private DevExpress.XtraBars.Navigation.AccordionControlElement UpdatePocket;
        private DevExpress.XtraBars.Navigation.AccordionControlElement ManageCores;
        private DevExpress.XtraBars.Navigation.AccordionControlElement OrganizeCores;
        private DevExpress.XtraBars.Navigation.AccordionControlElement Settings;
        private DevExpress.XtraBars.Navigation.AccordionControlElement ViewLogs;
        private Controls.Update_Pocket update_Pocket1;
    }
}