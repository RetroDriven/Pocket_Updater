using Guna.UI2.WinForms;
using Pocket_Updater.UI;
using System.Diagnostics;

namespace Pocket_Updater.Controls.Modern
{
    internal sealed class AboutPage : UserControl
    {
        public AboutPage()
        {
            Dock = DockStyle.Fill;
            BackColor = ModernTheme.AppBackground;
            Padding = new Padding(22);

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                BackColor = Color.Transparent,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 72));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            Controls.Add(root);

            root.Controls.Add(BuildHeading(), 0, 0);

            var contentHost = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Padding = Padding.Empty,
                Margin = Padding.Empty
            };

            var body = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 390,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.Transparent,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 59));
            body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 41));
            body.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            var info = BuildProjectCard();
            info.Margin = new Padding(0, 0, 8, 0);
            var credits = BuildCreditsCard();
            credits.Margin = new Padding(8, 0, 0, 0);

            body.Controls.Add(info, 0, 0);
            body.Controls.Add(credits, 1, 0);
            contentHost.Controls.Add(body);
            root.Controls.Add(contentHost, 0, 1);
        }

        private static Control BuildHeading()
        {
            var heading = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.Transparent,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            heading.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 58));
            heading.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            var tileHost = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
            tileHost.Controls.Add(IconTile(UiIcon.About, ModernTheme.AccentHover, new Point(0, 3)));

            var text = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                BackColor = Color.Transparent,
                Margin = Padding.Empty,
                Padding = new Padding(4, 0, 0, 0)
            };
            text.RowStyles.Add(new RowStyle(SizeType.Absolute, 39));
            text.RowStyles.Add(new RowStyle(SizeType.Absolute, 25));

            var title = NewLabel("About Pocket Updater", 23, FontStyle.Bold, ModernTheme.TextPrimary);
            title.Dock = DockStyle.Fill;
            title.TextAlign = ContentAlignment.BottomLeft;

            var subtitle = NewLabel("Open-source tools for keeping your Analogue Pocket library current.", 10, FontStyle.Regular, ModernTheme.TextSecondary);
            subtitle.Dock = DockStyle.Fill;
            subtitle.TextAlign = ContentAlignment.TopLeft;

            text.Controls.Add(title, 0, 0);
            text.Controls.Add(subtitle, 0, 1);
            heading.Controls.Add(tileHost, 0, 0);
            heading.Controls.Add(text, 1, 0);
            return heading;
        }

        private static Guna2Panel BuildProjectCard()
        {
            var card = Card();
            card.Padding = new Padding(24);

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 6,
                BackColor = Color.Transparent,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 1));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 68));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            var appTitle = NewLabel("Pocket Updater v2", 18, FontStyle.Bold, ModernTheme.TextPrimary);
            appTitle.Dock = DockStyle.Fill;
            appTitle.TextAlign = ContentAlignment.MiddleLeft;

            var description = NewLabel(
                "Keep openFPGA cores, Pocket firmware, required BIOS files, arcade ROMs, and community artwork packs current from one desktop app.",
                10.2F, FontStyle.Regular, ModernTheme.TextSecondary);
            description.Dock = DockStyle.Fill;
            description.TextAlign = ContentAlignment.TopLeft;

            var divider = new Panel { Dock = DockStyle.Fill, BackColor = ModernTheme.Border };

            var buttons = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 12, 0, 10),
                Margin = Padding.Empty
            };
            buttons.Controls.Add(LinkButton("GitHub", "https://github.com/RetroDriven/Pocket_Updater"));
            buttons.Controls.Add(LinkButton("Latest Release", "https://github.com/RetroDriven/Pocket_Updater/releases/latest"));
            buttons.Controls.Add(LinkButton("Read Me", "https://github.com/RetroDriven/Pocket_Updater/blob/master/README.md"));
            buttons.Controls.Add(LinkButton("Report Issue", "https://github.com/RetroDriven/Pocket_Updater/issues"));

            var sectionTitle = NewLabel("OPEN SOURCE & COMMUNITY", 8.5F, FontStyle.Bold, ModernTheme.TextMuted);
            sectionTitle.Dock = DockStyle.Fill;
            sectionTitle.TextAlign = ContentAlignment.BottomLeft;

            var note = NewLabel(
                "Pocket Updater is free and open source. It builds on community-maintained openFPGA tooling and inventory data so the Pocket ecosystem can stay easy to update and manage.",
                9.6F, FontStyle.Regular, ModernTheme.TextSecondary);
            note.Dock = DockStyle.Fill;
            note.TextAlign = ContentAlignment.TopLeft;

            layout.Controls.Add(appTitle, 0, 0);
            layout.Controls.Add(description, 0, 1);
            layout.Controls.Add(divider, 0, 2);
            layout.Controls.Add(buttons, 0, 3);
            layout.Controls.Add(sectionTitle, 0, 4);
            layout.Controls.Add(note, 0, 5);
            card.Controls.Add(layout);
            return card;
        }

        private static Guna2Panel BuildCreditsCard()
        {
            var card = Card();
            card.Padding = new Padding(20);

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                BackColor = Color.Transparent,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 112));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 12));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 112));

            var creditsTitle = NewLabel("Credits", 14, FontStyle.Bold, ModernTheme.TextPrimary);
            creditsTitle.Dock = DockStyle.Fill;
            creditsTitle.TextAlign = ContentAlignment.MiddleLeft;

            layout.Controls.Add(creditsTitle, 0, 0);
            layout.Controls.Add(CreditCard(
                "Matt Pannella",
                "Updater utility collaboration and core updater foundation.",
                "https://github.com/mattpannella/pocket-updater-utility"), 0, 1);
            layout.Controls.Add(CreditCard(
                "Josh Campbell",
                "openFPGA core inventory API creator/provider.",
                "https://github.com/openfpga-cores-inventory/analogue-pocket"), 0, 3);

            card.Controls.Add(layout);
            return card;
        }

        private static Guna2Panel CreditCard(string name, string description, string url)
        {
            var panel = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                BorderRadius = 11,
                FillColor = ModernTheme.SurfaceRaised,
                BorderColor = ModernTheme.Border,
                BorderThickness = 1,
                Padding = new Padding(16, 12, 14, 12),
                Margin = Padding.Empty
            };

            var grid = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                BackColor = Color.Transparent,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 92));
            grid.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            grid.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            var nameLabel = NewLabel(name, 11, FontStyle.Bold, ModernTheme.TextPrimary);
            nameLabel.Dock = DockStyle.Fill;
            nameLabel.TextAlign = ContentAlignment.MiddleLeft;

            var desc = NewLabel(description, 9.2F, FontStyle.Regular, ModernTheme.TextSecondary);
            desc.Dock = DockStyle.Fill;
            desc.TextAlign = ContentAlignment.TopLeft;

            var button = LinkButton("GitHub", url);
            button.Size = new Size(82, 34);
            button.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button.Margin = new Padding(0, 2, 0, 0);

            grid.Controls.Add(nameLabel, 0, 0);
            grid.SetColumnSpan(nameLabel, 2);
            grid.Controls.Add(desc, 0, 1);
            grid.Controls.Add(button, 1, 1);
            panel.Controls.Add(grid);
            return panel;
        }

        private static Guna2Button LinkButton(string text, string url)
        {
            var button = new Guna2Button
            {
                Text = text,
                Size = new Size(126, 38),
                BorderRadius = 9,
                FillColor = ModernTheme.SurfaceRaised,
                BorderColor = ModernTheme.BorderStrong,
                BorderThickness = 1,
                ForeColor = ModernTheme.TextPrimary,
                Font = ModernTheme.BodyBoldFont,
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 8, 0),
                TextAlign = HorizontalAlignment.Center
            };
            button.HoverState.FillColor = ModernTheme.SurfaceHover;
            button.Click += (_, _) => Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            return button;
        }

        private static Guna2Panel Card() => new()
        {
            Dock = DockStyle.Fill,
            FillColor = ModernTheme.Surface,
            BorderColor = ModernTheme.Border,
            BorderThickness = 1,
            BorderRadius = 14
        };

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
            BackColor = Color.Transparent,
            AutoEllipsis = false
        };
    }
}
