using System.Drawing.Drawing2D;
using Guna.UI2.WinForms;

namespace Pocket_Updater.UI
{

    internal sealed class AssetPackArtworkPreviewPopup : Form
    {
        private const int CornerRadius = 12;
        private static readonly Color PopupBackground = Color.FromArgb(8, 13, 23);
        private static readonly Color PopupBorder = Color.FromArgb(54, 74, 102);
        private readonly Guna2PictureBox[] _images;
        private readonly Label _title;
        private readonly Label _source;

        public AssetPackArtworkPreviewPopup()
        {
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            ShowInTaskbar = false;
            TopMost = true;
            BackColor = PopupBackground;
            Size = new Size(900, 180);

            Resize += (_, _) => ApplyRoundedRegion();
            Shown += (_, _) => ApplyRoundedRegion();

            var borderless = new Guna2BorderlessForm
            {
                ContainerControl = this,
                BorderRadius = CornerRadius,
                TransparentWhileDrag = false
            };

            var card = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                FillColor = PopupBackground,
                BorderColor = PopupBorder,
                BorderThickness = 1,
                BorderRadius = 12,
                Padding = new Padding(12),
                BackColor = PopupBackground
            };
            Controls.Add(card);

            _images = new Guna2PictureBox[3];
            for (int i = 0; i < _images.Length; i++)
            {
                _images[i] = new Guna2PictureBox
                {
                    Location = new Point(12 + (i * 292), 12),
                    Size = new Size(280, 100),
                    BackColor = Color.Black,
                    BorderRadius = 8,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Visible = false
                };
                card.Controls.Add(_images[i]);
            }

            _title = new Label
            {
                Location = new Point(14, 124),
                Size = new Size(870, 28),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point),
                ForeColor = ModernTheme.TextPrimary,
                BackColor = Color.Transparent,
                AutoEllipsis = true
            };
            card.Controls.Add(_title);

            _source = new Label
            {
                Visible = false,
                AutoSize = false,
                Size = Size.Empty
            };
            card.Controls.Add(_source);
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

        protected override bool ShowWithoutActivation => true;

        protected override CreateParams CreateParams
        {
            get
            {
                const int WsExNoActivate = 0x08000000;
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= WsExNoActivate;
                return cp;
            }
        }

        public void ShowPreview(
            Control owner,
            string ownerName,
            string repository,
            string variant,
            IReadOnlyList<Bitmap> images,
            Rectangle anchorScreen)
        {
            for (int i = 0; i < _images.Length; i++)
            {
                Bitmap? image = i < images.Count ? images[i] : null;
                _images[i].Image = image;
                _images[i].Visible = image != null;
            }

            string displayVariant = string.IsNullOrWhiteSpace(variant) ? "Default" : variant;
            _title.Text = $"{ownerName} • {displayVariant}";
            _source.Text = string.Empty;

            Rectangle work = Screen.FromRectangle(anchorScreen).WorkingArea;
            int x = anchorScreen.Left;
            int y = anchorScreen.Top - Height - 8;
            if (y < work.Top + 8)
                y = anchorScreen.Bottom + 8;
            if (x + Width > work.Right - 8)
                x = work.Right - Width - 8;
            if (x < work.Left + 8)
                x = work.Left + 8;
            if (y + Height > work.Bottom - 8)
                y = Math.Max(work.Top + 8, work.Bottom - Height - 8);

            Location = new Point(x, y);
            if (!Visible)
            {
                Form? ownerForm = owner.FindForm();
                if (ownerForm != null)
                    Show(ownerForm);
                else
                    Show();
            }
            else
            {
                Invalidate();
            }
        }
    }
}
