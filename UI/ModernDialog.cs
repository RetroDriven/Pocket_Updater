using Guna.UI2.WinForms;

namespace Pocket_Updater.UI
{
    internal static class ModernDialog
    {
        public static void ShowInfo(IWin32Window? owner, string title, string message) => Show(owner, title, message, false, ModernTheme.AccentHover);
        public static void ShowSuccess(IWin32Window? owner, string title, string message) => Show(owner, title, message, false, ModernTheme.Success);
        public static void ShowWarning(IWin32Window? owner, string title, string message) => Show(owner, title, message, false, ModernTheme.Warning);
        public static void ShowError(IWin32Window? owner, string title, string message) => Show(owner, title, message, false, ModernTheme.Danger);
        public static bool ShowConfirm(IWin32Window? owner, string title, string message) => Show(owner, title, message, true, ModernTheme.Warning) == DialogResult.Yes;

        public static void ShowDetails(IWin32Window? owner, string title, IEnumerable<string> items, Color? accent = null)
        {
            var values = (items ?? Enumerable.Empty<string>())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            using var form = new Form
            {
                Text = title,
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.CenterParent,
                Size = new Size(720, 520),
                MinimumSize = new Size(560, 400),
                BackColor = ModernTheme.AppBackground,
                ShowInTaskbar = false,
                MinimizeBox = false,
                MaximizeBox = false
            };

            var borderless = new Guna2BorderlessForm
            {
                ContainerControl = form,
                BorderRadius = 14,
                TransparentWhileDrag = false
            };

            var panel = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                FillColor = ModernTheme.Surface,
                BorderColor = ModernTheme.BorderStrong,
                BorderThickness = 1,
                BorderRadius = 14,
                Padding = new Padding(22)
            };
            form.Controls.Add(panel);

            Color marker = accent ?? ModernTheme.AccentHover;
            var icon = new Guna2Panel
            {
                Location = new Point(22, 20),
                Size = new Size(42, 42),
                BorderRadius = 11,
                FillColor = Color.FromArgb(38, marker)
            };
            icon.Controls.Add(new Guna2PictureBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                SizeMode = PictureBoxSizeMode.CenterImage,
                Image = UiIcons.Get(UiIcon.List, marker, 24)
            });
            panel.Controls.Add(icon);

            var titleLabel = new Label
            {
                Text = title,
                AutoSize = true,
                Location = new Point(78, 19),
                Font = new Font("Segoe UI", 15F, FontStyle.Bold, GraphicsUnit.Point),
                ForeColor = ModernTheme.TextPrimary,
                BackColor = Color.Transparent
            };
            panel.Controls.Add(titleLabel);

            var countLabel = new Label
            {
                Text = values.Count == 1 ? "1 item" : $"{values.Count} items",
                AutoSize = true,
                Location = new Point(80, 48),
                Font = ModernTheme.BodyFont,
                ForeColor = ModernTheme.TextSecondary,
                BackColor = Color.Transparent
            };
            panel.Controls.Add(countLabel);

            var closeBox = new Guna2ControlBox
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(form.ClientSize.Width - 48, 8),
                Size = new Size(34, 34),
                FillColor = Color.Transparent,
                IconColor = ModernTheme.TextMuted
            };
            closeBox.HoverState.FillColor = ModernTheme.Danger;
            panel.Controls.Add(closeBox);

            var details = new Guna2TextBox
            {
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                FillColor = Color.FromArgb(8, 14, 24),
                ForeColor = Color.FromArgb(205, 220, 242),
                BorderColor = ModernTheme.BorderStrong,
                BorderRadius = 10,
                Font = ModernTheme.MonoFont,
                Location = new Point(22, 82),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                Size = new Size(form.ClientSize.Width - 44, form.ClientSize.Height - 148),
                Text = values.Count == 0
                    ? "No items have been recorded for this category yet."
                    : string.Join(Environment.NewLine, values.Select((value, index) => $"{index + 1}. {value}"))
            };
            panel.Controls.Add(details);

            var close = new Guna2Button
            {
                Text = "Close",
                DialogResult = DialogResult.OK,
                Size = new Size(108, 38),
                BorderRadius = 9,
                FillColor = ModernTheme.Accent,
                ForeColor = ModernTheme.TextPrimary,
                Font = ModernTheme.BodyBoldFont,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                Location = new Point(form.ClientSize.Width - 130, form.ClientSize.Height - 54)
            };
            close.HoverState.FillColor = ModernTheme.AccentHover;
            panel.Controls.Add(close);
            form.AcceptButton = close;
            form.CancelButton = close;

            panel.Resize += (_, _) =>
            {
                closeBox.Location = new Point(panel.ClientSize.Width - 48, 8);
                details.Size = new Size(Math.Max(360, panel.ClientSize.Width - 44), Math.Max(240, panel.ClientSize.Height - 148));
                close.Location = new Point(panel.ClientSize.Width - close.Width - 22, panel.ClientSize.Height - close.Height - 16);
            };

            var drag = new Guna2DragControl { TargetControl = panel, UseTransparentDrag = true };
            if (owner == null) form.ShowDialog(); else form.ShowDialog(owner);
        }

        private static DialogResult Show(IWin32Window? owner, string title, string message, bool confirm, Color accent)
        {
            using var form = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.CenterParent,
                Size = new Size(480, 245),
                BackColor = ModernTheme.AppBackground,
                ShowInTaskbar = false,
                MinimizeBox = false,
                MaximizeBox = false
            };

            var borderless = new Guna2BorderlessForm
            {
                ContainerControl = form,
                BorderRadius = 14,
                TransparentWhileDrag = false
            };

            var panel = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                FillColor = ModernTheme.Surface,
                BorderColor = ModernTheme.BorderStrong,
                BorderThickness = 1,
                BorderRadius = 14,
                Padding = new Padding(22)
            };
            form.Controls.Add(panel);

            var icon = new Guna2Panel
            {
                Location = new Point(22, 20),
                Size = new Size(42, 42),
                BorderRadius = 11,
                FillColor = Color.FromArgb(38, accent)
            };
            icon.Controls.Add(new Guna2PictureBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                SizeMode = PictureBoxSizeMode.CenterImage,
                Image = UiIcons.Get(confirm ? UiIcon.About : (accent == ModernTheme.Danger ? UiIcon.Missing : UiIcon.Check), accent, 24)
            });
            panel.Controls.Add(icon);

            var titleLabel = new Label
            {
                Text = title,
                AutoSize = true,
                Location = new Point(78, 20),
                Font = new Font("Segoe UI", 14F, FontStyle.Bold, GraphicsUnit.Point),
                ForeColor = ModernTheme.TextPrimary,
                BackColor = Color.Transparent
            };
            panel.Controls.Add(titleLabel);

            var messageLabel = new Label
            {
                Text = message,
                Location = new Point(78, 55),
                Size = new Size(370, 100),
                Font = ModernTheme.BodyFont,
                ForeColor = ModernTheme.TextSecondary,
                BackColor = Color.Transparent
            };
            panel.Controls.Add(messageLabel);

            var close = new Guna2ControlBox
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(432, 8),
                Size = new Size(34, 34),
                FillColor = Color.Transparent,
                IconColor = ModernTheme.TextMuted
            };
            close.HoverState.FillColor = ModernTheme.Danger;
            panel.Controls.Add(close);

            Guna2Button MakeButton(string text, DialogResult result, bool primary)
            {
                var button = new Guna2Button
                {
                    Text = text,
                    DialogResult = result,
                    Size = new Size(108, 38),
                    BorderRadius = 9,
                    FillColor = primary ? ModernTheme.Accent : ModernTheme.SurfaceRaised,
                    BorderColor = ModernTheme.BorderStrong,
                    BorderThickness = primary ? 0 : 1,
                    ForeColor = ModernTheme.TextPrimary,
                    Font = ModernTheme.BodyBoldFont,
                    Anchor = AnchorStyles.Bottom | AnchorStyles.Right
                };
                button.HoverState.FillColor = primary ? ModernTheme.AccentHover : ModernTheme.SurfaceHover;
                return button;
            }

            if (confirm)
            {
                var no = MakeButton("Cancel", DialogResult.No, false);
                no.Location = new Point(236, 180);
                var yes = MakeButton("Continue", DialogResult.Yes, true);
                yes.Location = new Point(350, 180);
                panel.Controls.Add(no);
                panel.Controls.Add(yes);
                form.AcceptButton = yes;
                form.CancelButton = no;
            }
            else
            {
                var ok = MakeButton("OK", DialogResult.OK, true);
                ok.Location = new Point(350, 180);
                panel.Controls.Add(ok);
                form.AcceptButton = ok;
                form.CancelButton = ok;
            }

            var drag = new Guna2DragControl { TargetControl = panel, UseTransparentDrag = true };
            return owner == null ? form.ShowDialog() : form.ShowDialog(owner);
        }
    }
}
