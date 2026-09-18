using Guna.UI2.WinForms;
using Pocket_Updater.Forms.Update_Progress;
using Pocket_Updater.UI;

namespace Pocket_Updater.Controls.Modern
{
    internal sealed class RomBiosLibrary : UserControl
    {
        private readonly Label _total;
        private readonly Label _installed;
        private readonly Label _missing;
        private readonly Label _coverage;
        private readonly Guna2ProgressBar _coverageBar;
        private readonly Guna2TextBox _search;
        private readonly Guna2ComboBox _typeFilter;
        private readonly Guna2ComboBox _statusFilter;
        private readonly Guna2DataGridView _grid;
        private readonly Guna2Button _downloadMissing;
        private readonly Guna2Button _refresh;
        private LibrarySnapshot? _snapshot;

        public RomBiosLibrary()
        {
            Dock = DockStyle.Fill;
            BackColor = ModernTheme.AppBackground;
            Padding = new Padding(20);

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                BackColor = Color.Transparent
            };
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 76));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 116));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            Controls.Add(root);

            var heading = new Guna2Panel { Dock = DockStyle.Fill, FillColor = Color.Transparent, BackColor = Color.Transparent };
            var title = Label("ROMs / BIOS", 23, FontStyle.Bold, ModernTheme.TextPrimary);
            title.Location = new Point(58, 0);
            title.AutoSize = true;
            var subtitle = Label("See the files required by your installed cores and exactly what is present or missing.", 10, FontStyle.Regular, ModernTheme.TextSecondary);
            subtitle.Location = new Point(60, 40);
            subtitle.AutoSize = true;
            _refresh = Button("Refresh", 104, false);
            _refresh.FillColor = ModernTheme.Success;
            _refresh.BorderThickness = 0;
            _refresh.BorderColor = ModernTheme.Success;
            _refresh.ForeColor = Color.White;
            _refresh.Image = null;
            _refresh.TextOffset = Point.Empty;
            _refresh.HoverState.FillColor = Color.FromArgb(42, 184, 116);
            _refresh.PressedColor = Color.FromArgb(35, 158, 99);
            _refresh.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _refresh.Click += async (_, _) => await RefreshAsync();
            _downloadMissing = Button("Download Missing", 172, true);
            _downloadMissing.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _downloadMissing.Click += async (_, _) => await DownloadMissingAsync();
            heading.Resize += (_, _) =>
            {
                _downloadMissing.Location = new Point(heading.ClientSize.Width - _downloadMissing.Width, 8);
                _refresh.Location = new Point(_downloadMissing.Left - _refresh.Width - 10, 8);
            };
            heading.Controls.Add(IconTile(UiIcon.RomBios, Color.FromArgb(165, 112, 255), new Point(0, 1)));
            heading.Controls.Add(title);
            heading.Controls.Add(subtitle);
            heading.Controls.Add(_refresh);
            heading.Controls.Add(_downloadMissing);
            root.Controls.Add(heading, 0, 0);

            var summary = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 1,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 0, 0, 12)
            };
            for (int i = 0; i < 4; i++) summary.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            summary.Controls.Add(MetricCard("Total Required", UiIcon.Grid, out _total, ModernTheme.AccentHover, new Padding(0, 0, 7, 0)), 0, 0);
            summary.Controls.Add(MetricCard("Installed", UiIcon.Check, out _installed, ModernTheme.Success, new Padding(7, 0, 7, 0)), 1, 0);
            summary.Controls.Add(MetricCard("Missing", UiIcon.Missing, out _missing, ModernTheme.Danger, new Padding(7, 0, 7, 0)), 2, 0);

            var coverageCard = Card();
            coverageCard.Margin = new Padding(7, 0, 0, 0);
            var coverageLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                BackColor = Color.Transparent,
                Padding = new Padding(18, 14, 18, 14)
            };
            coverageLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 78));
            coverageLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            coverageLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
            coverageLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            var coverageCaption = Label("Coverage", 11F, FontStyle.Bold, ModernTheme.TextPrimary);
            coverageCaption.Dock = DockStyle.Fill;
            coverageCaption.TextAlign = ContentAlignment.MiddleLeft;
            _coverage = Label("—", 24F, FontStyle.Bold, ModernTheme.TextPrimary);
            _coverage.Dock = DockStyle.Fill;
            _coverage.TextAlign = ContentAlignment.MiddleLeft;
            _coverageBar = new Guna2ProgressBar
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(12, 18, 4, 14),
                BorderRadius = 5,
                FillColor = ModernTheme.SurfaceRaised,
                ProgressColor = ModernTheme.Success,
                ProgressColor2 = ModernTheme.Accent
            };
            coverageLayout.Controls.Add(coverageCaption, 0, 0);
            coverageLayout.SetColumnSpan(coverageCaption, 2);
            coverageLayout.Controls.Add(_coverage, 0, 1);
            coverageLayout.Controls.Add(_coverageBar, 1, 1);
            coverageCard.Controls.Add(coverageLayout);
            summary.Controls.Add(coverageCard, 3, 0);
            root.Controls.Add(summary, 0, 1);

            var filters = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 1,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 0, 0, 10)
            };
            filters.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            filters.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 170));
            filters.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 170));
            filters.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140));

            _search = new Guna2TextBox
            {
                Dock = DockStyle.Fill,
                FillColor = ModernTheme.SurfaceRaised,
                ForeColor = ModernTheme.TextPrimary,
                PlaceholderForeColor = ModernTheme.TextMuted,
                BorderColor = ModernTheme.BorderStrong,
                BorderRadius = 9,
                Font = ModernTheme.BodyFont,
                PlaceholderText = "Search systems, core identifiers, or files…",
                Margin = new Padding(0, 0, 10, 0),
                IconLeft = UiIcons.Get(UiIcon.Search, Color.FromArgb(151, 174, 207), 17),
                IconLeftSize = new Size(17, 17),
                IconLeftOffset = new Point(8, 0)
            };
            _search.FocusedState.BorderColor = ModernTheme.Accent;
            _search.HoverState.BorderColor = ModernTheme.Accent;
            _search.TextChanged += (_, _) => ApplyFilters();
            _typeFilter = Combo(new[] { "All Types", "BIOS", "ROM", "ROM / Data" });
            _typeFilter.SelectedIndexChanged += (_, _) => ApplyFilters();
            _statusFilter = Combo(new[] { "All Statuses", "Complete", "Partial", "Missing" });
            _statusFilter.SelectedIndexChanged += (_, _) => ApplyFilters();
            var selectMissing = Button("Select Missing", 138, false);
            selectMissing.Dock = DockStyle.Fill;
            selectMissing.Margin = new Padding(0);
            selectMissing.Click += (_, _) => SelectMissing();
            filters.Controls.Add(_search, 0, 0);
            filters.Controls.Add(_typeFilter, 1, 0);
            filters.Controls.Add(_statusFilter, 2, 0);
            filters.Controls.Add(selectMissing, 3, 0);
            root.Controls.Add(filters, 0, 2);

            var card = Card();
            card.Padding = new Padding(1);
            _grid = BuildGrid();
            card.Controls.Add(ModernScrollbars.WrapVertical(_grid, 10));
            root.Controls.Add(card, 0, 3);

            PocketTargetContext.TargetChanged += async (_, _) =>
            {
                if (Visible)
                    await RefreshAsync();
            };
        }

        public async Task RefreshAsync()
        {
            _refresh.Enabled = false;
            _downloadMissing.Enabled = false;
            _refresh.Text = "Loading…";
            try
            {
                _snapshot = await LibraryInventoryService.LoadAsync(PocketTargetContext.SelectedPath);
                _total.Text = _snapshot.RequiredAssets.ToString();
                _installed.Text = _snapshot.InstalledAssets.ToString();
                _missing.Text = _snapshot.MissingAssets.ToString();
                int percent = _snapshot.RequiredAssets == 0 ? 0 : (int)Math.Round(_snapshot.InstalledAssets * 100d / _snapshot.RequiredAssets);
                _coverage.Text = $"{percent}%";
                _coverageBar.Value = Math.Max(0, Math.Min(100, percent));
                ApplyFilters();
            }
            catch (Exception ex)
            {
                _grid.Rows.Clear();
                _grid.Rows.Add(false, "Unable to scan ROM / BIOS requirements", "", "0", "0", "0", "Error", ex.Message);
            }
            finally
            {
                _refresh.Text = "Refresh";
                _refresh.Enabled = true;
                _downloadMissing.Enabled = _snapshot?.MissingAssets > 0;
            }
        }

        private void ApplyFilters()
        {
            if (_snapshot == null) return;
            string search = _search.Text.Trim();
            string type = _typeFilter.SelectedItem?.ToString() ?? "All Types";
            string status = _statusFilter.SelectedItem?.ToString() ?? "All Statuses";

            IEnumerable<PlatformAssetSummary> query = _snapshot.AssetSummaries;
            if (!string.IsNullOrWhiteSpace(search))
            {
                var coreIds = _snapshot.Assets.Where(a => a.FileName.Contains(search, StringComparison.OrdinalIgnoreCase))
                    .Select(a => a.CoreIdentifier).ToHashSet(StringComparer.OrdinalIgnoreCase);
                query = query.Where(x => x.Platform.Contains(search, StringComparison.OrdinalIgnoreCase)
                    || x.CoreIdentifier.Contains(search, StringComparison.OrdinalIgnoreCase)
                    || coreIds.Contains(x.CoreIdentifier));
            }
            if (type != "All Types") query = query.Where(x => x.DominantType == type);
            query = status switch
            {
                "Complete" => query.Where(x => x.Missing == 0),
                "Missing" => query.Where(x => x.Installed == 0 && x.Missing > 0),
                "Partial" => query.Where(x => x.Installed > 0 && x.Missing > 0),
                _ => query
            };

            _grid.Rows.Clear();
            foreach (var item in query.OrderBy(x => x.Platform))
            {
                string statusText = item.Missing == 0 ? "Complete" : item.Installed == 0 ? "Missing" : "Partial";
                int index = _grid.Rows.Add(false, item.Platform, item.DominantType, item.Installed, item.Required, item.Missing, statusText, item.CoreIdentifier);
                _grid.Rows[index].Tag = item;
            }
        }

        private async Task DownloadMissingAsync()
        {
            if (_snapshot == null) return;

            var checkedIds = _grid.Rows.Cast<DataGridViewRow>()
                .Where(r => r.Cells[0].Value is bool value && value)
                .Select(r => (r.Tag as PlatformAssetSummary)?.CoreIdentifier)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Cast<string>()
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (checkedIds.Count == 0)
            {
                checkedIds = _snapshot.Assets.Where(x => !x.Present)
                    .Select(x => x.CoreIdentifier)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();
            }

            if (checkedIds.Count == 0)
            {
                ModernDialog.ShowInfo(FindForm(), "Pocket Updater", "No missing ROM / BIOS requirements were found.");
                return;
            }

            using var progress = new ModernUpdateProgressForm(PocketTargetContext.SelectedPath, checkedIds.ToArray());
            progress.ShowDialog(FindForm());
            await RefreshAsync();
            LibraryChangeNotifier.Notify();
        }

        private void SelectMissing()
        {
            foreach (DataGridViewRow row in _grid.Rows)
            {
                if (row.Tag is PlatformAssetSummary item)
                    row.Cells[0].Value = item.Missing > 0;
            }
        }

        private static Guna2DataGridView BuildGrid()
        {
            var grid = new Guna2DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                MultiSelect = false,
                BackgroundColor = ModernTheme.Surface,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                GridColor = ModernTheme.Border,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            grid.Columns.Add(new DataGridViewCheckBoxColumn { Name = "Selected", HeaderText = "", FillWeight = 7, SortMode = DataGridViewColumnSortMode.NotSortable });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Platform", HeaderText = "System / Platform", FillWeight = 30, ReadOnly = true });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Type", HeaderText = "Type", FillWeight = 14, ReadOnly = true });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Installed", HeaderText = "Installed", FillWeight = 12, ReadOnly = true });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Required", HeaderText = "Required", FillWeight = 12, ReadOnly = true });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Missing", HeaderText = "Missing", FillWeight = 12, ReadOnly = true });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Status", HeaderText = "Status", FillWeight = 16, ReadOnly = true });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Core", HeaderText = "Core", FillWeight = 28, ReadOnly = true });
            ModernTheme.StyleDataGrid(grid);
            grid.CellFormatting += (_, e) =>
            {
                if (e.RowIndex < 0 || grid.Columns[e.ColumnIndex].Name != "Status" || e.Value == null) return;
                string value = e.Value.ToString() ?? string.Empty;
                e.CellStyle.ForeColor = value switch
                {
                    "Complete" => ModernTheme.Success,
                    "Partial" => ModernTheme.Warning,
                    _ => ModernTheme.Danger
                };
                e.CellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            };
            return grid;
        }

        private static Guna2Panel MetricCard(string captionText, UiIcon icon, out Label value, Color color, Padding margin)
        {
            var card = Card();
            card.Margin = margin;

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                BackColor = Color.Transparent,
                Padding = new Padding(14, 12, 16, 12)
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 52));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 29));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            var iconHost = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Margin = new Padding(0)
            };
            var tile = IconTile(icon, color, new Point(0, 6), 38);
            iconHost.Controls.Add(tile);
            layout.Controls.Add(iconHost, 0, 0);
            layout.SetRowSpan(iconHost, 2);

            var caption = Label(captionText, 11F, FontStyle.Bold, ModernTheme.TextPrimary);
            caption.Dock = DockStyle.Fill;
            caption.TextAlign = ContentAlignment.BottomLeft;
            caption.Margin = new Padding(0);

            value = Label("—", 24F, FontStyle.Bold, color);
            value.Dock = DockStyle.Fill;
            value.TextAlign = ContentAlignment.TopLeft;
            value.Margin = new Padding(0);

            layout.Controls.Add(caption, 1, 0);
            layout.Controls.Add(value, 1, 1);
            card.Controls.Add(layout);
            return card;
        }

        private static Guna2Panel IconTile(UiIcon icon, Color color, Point location, int size = 44)
        {
            var tile = new Guna2Panel
            {
                Location = location, Size = new Size(size, size), BorderRadius = 11,
                FillColor = Color.FromArgb(38, color), BackColor = Color.Transparent
            };
            tile.Controls.Add(new Guna2PictureBox
            {
                Dock = DockStyle.Fill, BackColor = Color.Transparent, SizeMode = PictureBoxSizeMode.CenterImage,
                Image = UiIcons.Get(icon, color, Math.Max(20, size - 16))
            });
            return tile;
        }

        private static void SetButtonIcon(Guna2Button button, UiIcon icon, bool primary)
        {
            button.Image = UiIcons.Get(icon, primary ? Color.White : Color.FromArgb(190, 208, 234), 18);
            button.ImageSize = new Size(18, 18);
            button.ImageAlign = HorizontalAlignment.Left;
            button.ImageOffset = new Point(8, 0);
            button.TextOffset = new Point(10, 0);
        }

        private static Guna2Panel Card() => new()
        {
            Dock = DockStyle.Fill,
            FillColor = ModernTheme.Surface,
            BorderColor = ModernTheme.Border,
            BorderThickness = 1,
            BorderRadius = 14,
            BackColor = Color.Transparent
        };

        private static Label Label(string text, float size, FontStyle style, Color color) => new()
        {
            Text = text,
            Font = new Font("Segoe UI", size, style, GraphicsUnit.Point),
            ForeColor = color,
            BackColor = Color.Transparent
        };

        private static Guna2ComboBox Combo(IEnumerable<string> items)
        {
            var combo = new Guna2ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList,
                FillColor = ModernTheme.SurfaceRaised,
                ForeColor = ModernTheme.TextPrimary,
                BorderColor = ModernTheme.BorderStrong,
                BorderRadius = 9,
                Font = ModernTheme.BodyFont,
                Margin = new Padding(0, 0, 10, 0),
                DrawMode = DrawMode.OwnerDrawFixed,
                ItemHeight = 30
            };
            combo.FocusedState.BorderColor = ModernTheme.Accent;
            combo.ItemsAppearance.BackColor = ModernTheme.SurfaceRaised;
            combo.ItemsAppearance.ForeColor = ModernTheme.TextPrimary;
            combo.ItemsAppearance.SelectedBackColor = ModernTheme.AccentSubtle;
            combo.ItemsAppearance.SelectedForeColor = ModernTheme.TextPrimary;
            combo.Items.AddRange(items.Cast<object>().ToArray());
            combo.SelectedIndex = 0;
            return combo;
        }

        private static Guna2Button Button(string text, int width, bool primary)
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
