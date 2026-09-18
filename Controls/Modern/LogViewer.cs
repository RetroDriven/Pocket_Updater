using Guna.UI2.WinForms;
using Pocket_Updater.UI;

namespace Pocket_Updater.Controls.Modern
{
    internal sealed class LogViewer : UserControl
    {
        private readonly Guna2TextBox _log;
        private readonly Guna2Button _clearButton;
        private readonly Guna2Button _refreshButton;

        public LogViewer()
        {
            Dock = DockStyle.Fill;
            BackColor = ModernTheme.AppBackground;
            Padding = new Padding(22);

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                BackColor = Color.Transparent
            };
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 76));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            Controls.Add(root);

            var heading = new Guna2Panel { Dock = DockStyle.Fill, FillColor = Color.Transparent, BackColor = Color.Transparent };
            heading.Controls.Add(IconTile(UiIcon.Logs, ModernTheme.AccentHover, new Point(0, 1)));

            var title = NewLabel("Logs & Activity", 23, FontStyle.Bold, ModernTheme.TextPrimary);
            title.Location = new Point(58, 0);
            title.AutoSize = true;
            var subtitle = NewLabel("View updater activity and diagnostic output.", 10, FontStyle.Regular, ModernTheme.TextSecondary);
            subtitle.Location = new Point(60, 40);
            subtitle.AutoSize = true;

            _refreshButton = NewButton("Refresh", 104, false);
            _refreshButton.FillColor = ModernTheme.Success;
            _refreshButton.BorderThickness = 0;
            _refreshButton.BorderColor = ModernTheme.Success;
            _refreshButton.ForeColor = Color.White;
            _refreshButton.Image = null;
            _refreshButton.TextOffset = Point.Empty;
            _refreshButton.HoverState.FillColor = Color.FromArgb(42, 184, 116);
            _refreshButton.PressedColor = Color.FromArgb(35, 158, 99);
            _refreshButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _refreshButton.Location = new Point(heading.Width - 224, 8);
            _refreshButton.Click += (_, _) => RefreshLog();

            _clearButton = NewButton("Clear Logs", 112, false);
            _clearButton.Image = UiIcons.Get(UiIcon.Missing, Color.FromArgb(255, 150, 164), 17);
            _clearButton.ImageSize = new Size(17, 17);
            _clearButton.ImageAlign = HorizontalAlignment.Left;
            _clearButton.ImageOffset = new Point(8, 0);
            _clearButton.TextOffset = new Point(10, 0);
            _clearButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _clearButton.Location = new Point(heading.Width - 112, 8);
            _clearButton.Click += (_, _) => ClearLog();

            heading.Resize += (_, _) =>
            {
                _clearButton.Left = heading.ClientSize.Width - _clearButton.Width;
                _refreshButton.Left = _clearButton.Left - _refreshButton.Width - 8;
            };

            heading.Controls.Add(title);
            heading.Controls.Add(subtitle);
            heading.Controls.Add(_refreshButton);
            heading.Controls.Add(_clearButton);
            root.Controls.Add(heading, 0, 0);

            var card = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                FillColor = ModernTheme.Surface,
                BorderColor = ModernTheme.Border,
                BorderThickness = 1,
                BorderRadius = 14,
                Padding = new Padding(14)
            };
            _log = new Guna2TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                FillColor = Color.FromArgb(7, 13, 23),
                ForeColor = Color.FromArgb(205, 220, 240),
                BorderColor = ModernTheme.BorderStrong,
                BorderRadius = 10,
                Font = ModernTheme.MonoFont
            };
            card.Controls.Add(_log);
            root.Controls.Add(card, 0, 1);

            VisibleChanged += (_, _) => { if (Visible) RefreshLog(); };
            RefreshLog();
        }

        public void RefreshLog()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Pocket_Updater_Log.txt");
            if (File.Exists(path))
            {
                _log.Text = File.ReadAllText(path);
                _log.SelectionStart = _log.Text.Length;
                _log.ScrollToCaret();
                _clearButton.Enabled = true;
            }
            else
            {
                _log.Text = "No updater log has been created yet.";
                _clearButton.Enabled = false;
            }
        }

        private void ClearLog()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Pocket_Updater_Log.txt");
            if (!File.Exists(path))
            {
                ModernDialog.ShowInfo(FindForm(), "Logs", "No log file was found.");
                RefreshLog();
                return;
            }

            if (!ModernDialog.ShowConfirm(FindForm(), "Clear Logs", "Clear the current updater log?"))
                return;

            try
            {
                File.Delete(path);
                RefreshLog();
            }
            catch (Exception ex)
            {
                ModernDialog.ShowError(FindForm(), "Unable to clear logs", ex.Message);
            }
        }

        private static Guna2Panel IconTile(UiIcon icon, Color color, Point location)
        {
            var tile = new Guna2Panel
            {
                Location = location,
                Size = new Size(44, 44),
                BorderRadius = 11,
                FillColor = Color.FromArgb(38, color),
                BackColor = Color.Transparent
            };
            tile.Controls.Add(new Guna2PictureBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                SizeMode = PictureBoxSizeMode.CenterImage,
                Image = UiIcons.Get(icon, color, 26)
            });
            return tile;
        }

        private static Label NewLabel(string text, float size, FontStyle style, Color color) => new()
        {
            Text = text,
            Font = new Font("Segoe UI", size, style, GraphicsUnit.Point),
            ForeColor = color,
            BackColor = Color.Transparent
        };

        private static Guna2Button NewButton(string text, int width, bool primary)
        {
            var button = new Guna2Button
            {
                Text = text,
                Size = new Size(width, 36),
                BorderRadius = 9,
                FillColor = primary ? ModernTheme.Accent : ModernTheme.SurfaceRaised,
                ForeColor = ModernTheme.TextPrimary,
                Font = ModernTheme.BodyBoldFont,
                BorderThickness = primary ? 0 : 1,
                BorderColor = ModernTheme.BorderStrong,
                Cursor = Cursors.Hand
            };
            button.HoverState.FillColor = primary ? ModernTheme.AccentHover : ModernTheme.SurfaceHover;
            return button;
        }
    }
}
