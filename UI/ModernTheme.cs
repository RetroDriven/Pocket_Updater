using Guna.UI2.WinForms;
using System.Drawing;
using System.Windows.Forms;

namespace Pocket_Updater.UI
{

    internal static class ModernTheme
    {
        public static readonly Color AppBackground = Color.FromArgb(10, 15, 26);
        public static readonly Color TopBar = Color.FromArgb(12, 18, 31);
        public static readonly Color Sidebar = Color.FromArgb(14, 22, 38);
        public static readonly Color Surface = Color.FromArgb(17, 25, 42);
        public static readonly Color SurfaceRaised = Color.FromArgb(22, 32, 52);
        public static readonly Color SurfaceHover = Color.FromArgb(28, 41, 65);
        public static readonly Color Border = Color.FromArgb(43, 58, 82);
        public static readonly Color BorderStrong = Color.FromArgb(58, 76, 103);
        public static readonly Color Accent = Color.FromArgb(47, 128, 237);
        public static readonly Color AccentHover = Color.FromArgb(67, 145, 247);
        public static readonly Color AccentPressed = Color.FromArgb(34, 104, 205);
        public static readonly Color AccentSubtle = Color.FromArgb(25, 62, 111);
        public static readonly Color TextPrimary = Color.FromArgb(244, 247, 252);
        public static readonly Color TextSecondary = Color.FromArgb(166, 178, 198);
        public static readonly Color TextMuted = Color.FromArgb(119, 135, 160);
        public static readonly Color Success = Color.FromArgb(50, 205, 126);
        public static readonly Color Danger = Color.FromArgb(239, 83, 102);
        public static readonly Color Warning = Color.FromArgb(247, 181, 72);

        public static readonly Font BodyFont = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
        public static readonly Font BodyBoldFont = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
        public static readonly Font TitleFont = new Font("Segoe UI", 16F, FontStyle.Bold, GraphicsUnit.Point);
        public static readonly Font SectionFont = new Font("Segoe UI", 13F, FontStyle.Bold, GraphicsUnit.Point);
        public static readonly Font MonoFont = new Font("Consolas", 10F, FontStyle.Regular, GraphicsUnit.Point);

        public static void Apply(Control root)
        {
            root.SuspendLayout();
            ApplyRecursive(root);
            root.ResumeLayout(true);
        }

        public static void ApplyPageChrome(Control root)
        {
            root.BackColor = AppBackground;
            root.ForeColor = TextPrimary;
            root.Padding = new Padding(18);

            foreach (Control control in GetAllControls(root))
            {
                if (control.Name == "Panel_Top" && control is Guna2Panel top)
                {
                    top.AutoSize = false;
                    top.Height = 52;
                    top.FillColor = Color.Transparent;
                    top.BackColor = Color.Transparent;
                    top.BorderThickness = 0;
                    top.Padding = new Padding(2, 0, 0, 0);
                }

                if (control.Name == "Panel_Bottom" && control is Guna2GradientPanel bottom)
                {
                    bottom.FillColor = Surface;
                    bottom.FillColor2 = Surface;
                    bottom.BorderColor = Border;
                    bottom.BorderThickness = 1;
                    bottom.BorderRadius = 14;
                }

                if (control.Name == "panel1" && control.Parent == root)
                {
                    control.BackColor = Surface;
                    control.Padding = new Padding(1);
                }
            }
        }

        public static void StyleDataGrid(DataGridView grid)
        {
            grid.EnableHeadersVisualStyles = false;
            grid.BackgroundColor = Surface;
            grid.BorderStyle = BorderStyle.None;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.GridColor = Border;
            grid.RowHeadersVisible = false;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            grid.ColumnHeadersHeight = 46;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            grid.RowTemplate.Height = 36;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            grid.ColumnHeadersDefaultCellStyle.BackColor = SurfaceRaised;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = TextPrimary;
            grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = SurfaceRaised;
            grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = TextPrimary;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
            grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 0, 8, 0);

            grid.DefaultCellStyle.BackColor = Surface;
            grid.DefaultCellStyle.ForeColor = TextSecondary;
            grid.DefaultCellStyle.SelectionBackColor = AccentSubtle;
            grid.DefaultCellStyle.SelectionForeColor = TextPrimary;
            grid.DefaultCellStyle.Font = BodyFont;
            grid.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            grid.DefaultCellStyle.Padding = new Padding(8, 2, 8, 2);

            grid.RowsDefaultCellStyle.BackColor = Surface;
            grid.RowsDefaultCellStyle.ForeColor = TextSecondary;
            grid.RowsDefaultCellStyle.SelectionBackColor = AccentSubtle;
            grid.RowsDefaultCellStyle.SelectionForeColor = TextPrimary;

            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(19, 28, 47);
            grid.AlternatingRowsDefaultCellStyle.ForeColor = TextSecondary;
            grid.AlternatingRowsDefaultCellStyle.SelectionBackColor = AccentSubtle;
            grid.AlternatingRowsDefaultCellStyle.SelectionForeColor = TextPrimary;

            foreach (DataGridViewColumn column in grid.Columns)
            {
                column.SortMode = column is DataGridViewButtonColumn || column is DataGridViewCheckBoxColumn
                    ? DataGridViewColumnSortMode.NotSortable
                    : column.SortMode;

                if (column is DataGridViewButtonColumn buttonColumn)
                {
                    buttonColumn.FlatStyle = FlatStyle.Flat;
                    buttonColumn.DefaultCellStyle.BackColor = Accent;
                    buttonColumn.DefaultCellStyle.ForeColor = Color.White;
                    buttonColumn.DefaultCellStyle.SelectionBackColor = AccentHover;
                    buttonColumn.DefaultCellStyle.SelectionForeColor = Color.White;
                    buttonColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    buttonColumn.DefaultCellStyle.Padding = new Padding(7, 5, 7, 5);
                }
                else if (column is DataGridViewLinkColumn linkColumn)
                {
                    linkColumn.LinkColor = Color.FromArgb(112, 172, 255);
                    linkColumn.ActiveLinkColor = Color.FromArgb(151, 196, 255);
                    linkColumn.VisitedLinkColor = Color.FromArgb(112, 172, 255);
                }
            }
        }

        private static void ApplyRecursive(Control control)
        {
            switch (control)
            {
                case Form form:
                    form.BackColor = AppBackground;
                    form.ForeColor = TextPrimary;
                    form.Font = BodyFont;
                    break;

                case Guna2GradientPanel gradientPanel:
                    gradientPanel.FillColor = Surface;
                    gradientPanel.FillColor2 = Surface;
                    gradientPanel.BorderColor = Border;
                    gradientPanel.BorderThickness = 0;
                    gradientPanel.BackColor = Color.Transparent;
                    break;

                case Guna2TextBox textBox:
                    textBox.FillColor = SurfaceRaised;
                    textBox.BorderColor = BorderStrong;
                    textBox.BorderRadius = 9;
                    textBox.ForeColor = TextPrimary;
                    textBox.PlaceholderForeColor = TextMuted;
                    textBox.Font = BodyFont;
                    textBox.FocusedState.BorderColor = Accent;
                    textBox.HoverState.BorderColor = Accent;
                    break;

                case Guna2Panel gunaPanel:
                    gunaPanel.FillColor = Color.Transparent;
                    gunaPanel.BackColor = Color.Transparent;
                    gunaPanel.BorderColor = Border;
                    break;

                case FlowLayoutPanel flow:
                    flow.BackColor = Color.Transparent;
                    break;

                case TableLayoutPanel table:
                    table.BackColor = Color.Transparent;
                    break;

                case Panel panel:
                    panel.BackColor = Color.Transparent;
                    break;

                case Guna2CircleButton circleButton:
                    circleButton.FillColor = SurfaceRaised;
                    circleButton.ForeColor = TextPrimary;
                    circleButton.Font = BodyBoldFont;
                    circleButton.HoverState.FillColor = SurfaceHover;
                    break;

                case Guna2Button button:
                    StyleButton(button);
                    break;

                case Guna2ComboBox comboBox:
                    comboBox.FillColor = SurfaceRaised;
                    comboBox.BorderColor = BorderStrong;
                    comboBox.BorderRadius = 9;
                    comboBox.ForeColor = TextPrimary;
                    comboBox.Font = BodyFont;
                    comboBox.FocusedColor = Color.Transparent;
                    comboBox.FocusedState.BorderColor = Accent;
                    comboBox.ItemsAppearance.SelectedBackColor = AccentSubtle;
                    comboBox.ItemsAppearance.SelectedForeColor = TextPrimary;
                    break;

                case Guna2ToggleSwitch toggle:
                    toggle.CheckedState.BorderColor = Accent;
                    toggle.CheckedState.FillColor = Accent;
                    toggle.CheckedState.InnerBorderColor = Color.White;
                    toggle.CheckedState.InnerColor = Color.White;
                    toggle.UncheckedState.BorderColor = BorderStrong;
                    toggle.UncheckedState.FillColor = Color.FromArgb(67, 79, 98);
                    toggle.UncheckedState.InnerBorderColor = Color.White;
                    toggle.UncheckedState.InnerColor = Color.White;
                    break;

                case Guna2Separator separator:
                    separator.FillColor = Border;
                    break;

                case Guna2ProgressBar progress:
                    progress.FillColor = SurfaceRaised;
                    progress.ProgressColor = Accent;
                    progress.ProgressColor2 = Accent;
                    break;

                case Guna2DataGridView dataGrid:
                    StyleDataGrid(dataGrid);
                    break;

                case Guna2VScrollBar vScroll:
                    vScroll.FillColor = Surface;
                    vScroll.ThumbColor = BorderStrong;
                    vScroll.BorderColor = Border;
                    break;

                case Guna2HScrollBar hScroll:
                    hScroll.FillColor = Surface;
                    hScroll.ThumbColor = BorderStrong;
                    hScroll.BorderColor = Border;
                    break;

                case DataGridView dataGrid:
                    StyleDataGrid(dataGrid);
                    break;

                case TextBox standardTextBox:
                    standardTextBox.BackColor = SurfaceRaised;
                    standardTextBox.ForeColor = TextPrimary;
                    standardTextBox.BorderStyle = BorderStyle.FixedSingle;
                    standardTextBox.Font = BodyFont;
                    break;

                case UserControl userControl:

                    userControl.BackColor = AppBackground;
                    userControl.ForeColor = TextPrimary;
                    break;

                case Label label:
                    label.ForeColor = label.Enabled ? TextPrimary : TextMuted;
                    if (label.Font.Size <= 10.5F && !label.Font.Bold)
                        label.Font = BodyFont;
                    break;
            }

            foreach (Control child in control.Controls)
                ApplyRecursive(child);
        }

        private static void StyleButton(Guna2Button button)
        {
            button.AutoRoundedCorners = false;
            button.BorderRadius = 10;
            button.BorderThickness = 0;
            button.FillColor = Accent;
            button.ForeColor = Color.White;
            button.Font = BodyBoldFont;
            button.HoverState.FillColor = AccentHover;
            button.PressedColor = AccentPressed;
            button.DisabledState.FillColor = Color.FromArgb(53, 64, 82);
            button.DisabledState.ForeColor = Color.FromArgb(123, 137, 158);
            button.Cursor = Cursors.Hand;

            if (button.ButtonMode == Guna.UI2.WinForms.Enums.ButtonMode.RadioButton)
            {
                button.FillColor = Color.Transparent;
                button.HoverState.FillColor = SurfaceHover;
                button.CheckedState.FillColor = Accent;
                button.CheckedState.ForeColor = Color.White;
            }
        }

        private static IEnumerable<Control> GetAllControls(Control parent)
        {
            foreach (Control child in parent.Controls)
            {
                yield return child;
                foreach (Control grandChild in GetAllControls(child))
                    yield return grandChild;
            }
        }
    }
}
