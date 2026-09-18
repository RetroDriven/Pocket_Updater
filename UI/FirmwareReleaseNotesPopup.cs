using System.Diagnostics;
using System.Drawing.Drawing2D;
using Guna.UI2.WinForms;

namespace Pocket_Updater.UI
{

    internal sealed class FirmwareReleaseNotesPopup : Form
    {
        private const int CornerRadius = 12;
        private const int HorizontalPadding = 20;
        private const int ContentWidth = 860;
        private const int FooterHeight = 28;
        private const int BottomPadding = 16;
        private const int NotesTop = 46;
        private const int NotesFooterGap = 12;
        private const int MaxNotesViewportHeight = 300;
        private static readonly Color PopupBackground = Color.FromArgb(8, 13, 23);
        private static readonly Color PopupBorder = Color.FromArgb(54, 74, 102);

        private readonly Guna2Panel _card;
        private readonly Label _title;
        private readonly Panel _notesHost;
        private readonly Label _notes;
        private readonly LinkLabel _releaseLink;
        private string _releaseUrl = string.Empty;
        private int _notesContentHeight = 58;

        public event EventHandler? PreviewMouseEntered;
        public event EventHandler? PreviewMouseLeft;

        public FirmwareReleaseNotesPopup()
        {
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            ShowInTaskbar = false;
            TopMost = true;
            BackColor = PopupBackground;
            Padding = Padding.Empty;
            Size = new Size(900, 240);

            Resize += (_, _) =>
            {
                LayoutContents();
                ApplyRoundedRegion();
            };
            Shown += (_, _) =>
            {
                LayoutContents();
                ApplyRoundedRegion();
            };

            _card = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                FillColor = PopupBackground,
                BorderColor = PopupBorder,
                BorderThickness = 1,
                BorderRadius = CornerRadius,
                BackColor = PopupBackground
            };
            Controls.Add(_card);

            _title = new Label
            {
                Location = new Point(HorizontalPadding, 15),
                Size = new Size(ContentWidth, 24),
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold, GraphicsUnit.Point),
                ForeColor = ModernTheme.AccentHover,
                BackColor = Color.Transparent,
                AutoEllipsis = true
            };
            _card.Controls.Add(_title);

            _notesHost = new Panel
            {
                Location = new Point(HorizontalPadding, NotesTop),
                Size = new Size(ContentWidth, 120),
                BackColor = Color.Transparent,
                AutoScroll = true,
                TabStop = false
            };
            _card.Controls.Add(_notesHost);

            _notes = new Label
            {
                Location = Point.Empty,
                Size = new Size(ContentWidth - SystemInformation.VerticalScrollBarWidth - 4, 58),
                Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point),
                ForeColor = ModernTheme.TextPrimary,
                BackColor = Color.Transparent,
                AutoSize = false
            };
            _notesHost.Controls.Add(_notes);

            _releaseLink = new LinkLabel
            {
                Location = new Point(HorizontalPadding, 190),
                Size = new Size(ContentWidth, FooterHeight),
                Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point),
                ForeColor = ModernTheme.AccentHover,
                LinkColor = ModernTheme.AccentHover,
                ActiveLinkColor = Color.White,
                VisitedLinkColor = ModernTheme.AccentHover,
                BackColor = Color.Transparent,
                AutoEllipsis = true,
                TextAlign = ContentAlignment.MiddleLeft,
                Cursor = Cursors.Hand,
                TabStop = false
            };
            _releaseLink.LinkClicked += (_, _) => OpenReleasePage();
            _card.Controls.Add(_releaseLink);

            WireHover(this);
        }

        protected override bool ShowWithoutActivation => true;

        public void ShowNotes(Control anchor, string title, string notes, string releaseUrl)
        {
            _title.Text = string.IsNullOrWhiteSpace(title) ? "Pocket Firmware Release Notes" : title;
            _notes.Text = string.IsNullOrWhiteSpace(notes)
                ? "No release notes were provided for this firmware release."
                : notes.Trim();

            _releaseUrl = releaseUrl ?? string.Empty;
            _releaseLink.Text = string.IsNullOrWhiteSpace(_releaseUrl)
                ? "Official release page unavailable"
                : "Official release: " + _releaseUrl;
            _releaseLink.LinkArea = string.IsNullOrWhiteSpace(_releaseUrl)
                ? new LinkArea(0, 0)
                : new LinkArea("Official release: ".Length, _releaseUrl.Length);
            _releaseLink.Enabled = !string.IsNullOrWhiteSpace(_releaseUrl);
            _releaseLink.Visible = true;

            int notesWidth = Math.Max(200, ContentWidth - SystemInformation.VerticalScrollBarWidth - 6);
            Size measured = TextRenderer.MeasureText(
                _notes.Text,
                _notes.Font,
                new Size(notesWidth, 2000),
                TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl | TextFormatFlags.NoPadding);

            _notesContentHeight = Math.Max(58, measured.Height + 8);
            int notesViewportHeight = Math.Min(_notesContentHeight, MaxNotesViewportHeight);

            Width = ContentWidth + (HorizontalPadding * 2);
            Height = Math.Clamp(
                NotesTop + notesViewportHeight + NotesFooterGap + FooterHeight + BottomPadding,
                180,
                NotesTop + MaxNotesViewportHeight + NotesFooterGap + FooterHeight + BottomPadding);

            LayoutContents();
            ApplyRoundedRegion();

            Rectangle anchorScreen = anchor.RectangleToScreen(anchor.ClientRectangle);
            Rectangle work = Screen.FromRectangle(anchorScreen).WorkingArea;

            int x = anchorScreen.Left;
            int y = anchorScreen.Bottom + 8;

            if (x + Width > work.Right - 8)
                x = work.Right - Width - 8;
            if (x < work.Left + 8)
                x = work.Left + 8;
            if (y + Height > work.Bottom - 8)
                y = anchorScreen.Top - Height - 8;
            if (y < work.Top + 8)
                y = work.Top + 8;

            Location = new Point(x, y);

            if (!Visible)
            {
                Form? owner = anchor.FindForm();
                if (owner != null)
                    Show(owner);
                else
                    Show();
            }
            else
            {
                Invalidate(true);
            }
        }

        private void LayoutContents()
        {
            if (_card == null || _title == null || _notesHost == null || _notes == null || _releaseLink == null)
                return;

            int contentWidth = Math.Max(200, ClientSize.Width - (HorizontalPadding * 2));
            _title.Width = contentWidth;

            _releaseLink.Left = HorizontalPadding;
            _releaseLink.Width = contentWidth;
            _releaseLink.Height = FooterHeight;
            _releaseLink.Top = Math.Max(NotesTop + 60, ClientSize.Height - BottomPadding - FooterHeight);

            _notesHost.Left = HorizontalPadding;
            _notesHost.Top = NotesTop;
            _notesHost.Width = contentWidth;
            _notesHost.Height = Math.Max(58, _releaseLink.Top - NotesFooterGap - NotesTop);

            int notesWidth = Math.Max(160, _notesHost.ClientSize.Width - SystemInformation.VerticalScrollBarWidth - 6);
            _notes.Location = Point.Empty;
            _notes.Size = new Size(notesWidth, _notesContentHeight);
            _notesHost.AutoScrollMinSize = new Size(0, _notesContentHeight);

            _notesHost.AutoScrollPosition = Point.Empty;
            _releaseLink.BringToFront();
        }

        private void ApplyRoundedRegion()
        {
            if (ClientSize.Width <= 1 || ClientSize.Height <= 1)
                return;

            int diameter = CornerRadius * 2;
            using var path = new GraphicsPath();
            path.StartFigure();
            path.AddArc(0, 0, diameter, diameter, 180, 90);
            path.AddArc(ClientSize.Width - diameter - 1, 0, diameter, diameter, 270, 90);
            path.AddArc(ClientSize.Width - diameter - 1, ClientSize.Height - diameter - 1, diameter, diameter, 0, 90);
            path.AddArc(0, ClientSize.Height - diameter - 1, diameter, diameter, 90, 90);
            path.CloseFigure();

            Region? oldRegion = Region;
            Region = new Region(path);
            oldRegion?.Dispose();
        }

        private void OpenReleasePage()
        {
            if (string.IsNullOrWhiteSpace(_releaseUrl))
                return;

            try
            {
                Process.Start(new ProcessStartInfo(_releaseUrl) { UseShellExecute = true });
            }
            catch
            {

            }
        }

        private void WireHover(Control control)
        {
            control.MouseEnter += (_, _) => PreviewMouseEntered?.Invoke(this, EventArgs.Empty);
            control.MouseLeave += (_, _) => PreviewMouseLeft?.Invoke(this, EventArgs.Empty);
            foreach (Control child in control.Controls)
                WireHover(child);
        }
    }
}
