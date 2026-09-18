using Guna.UI2.WinForms;
using Pannella.Helpers;
using Pocket_Updater.Forms.Update_Progress;
using Pocket_Updater.UI;

namespace Pocket_Updater.Controls.Modern
{
    internal sealed class CoreLibrary : UserControl
    {
        private readonly FlowLayoutPanel _categoryPanel;
        private readonly Guna2TextBox _search;
        private readonly Guna2ComboBox _statusFilter;
        private readonly Guna2ComboBox _developerFilter;
        private readonly Guna2ComboBox _sortFilter;
        private readonly Guna2CheckBox _installedOnly;
        private readonly Guna2DataGridView _grid;
        private readonly Label _selectionLabel;
        private readonly Label _autoSaveLabel;
        private readonly System.Windows.Forms.Timer _savedStatusTimer;
        private readonly System.Windows.Forms.Timer _artPreviewTimer;
        private CoreArtworkPreviewPopup? _artPreviewPopup;
        private int _artPreviewRow = -1;
        private int _artPreviewColumn = -1;
        private readonly Label _sectionTitle;
        private readonly Label _title;
        private readonly Label _subtitle;
        private readonly Guna2Button _refreshButton;
        private readonly Guna2Button _updateSelected;
        private readonly Guna2Button _installMissing;
        private readonly Guna2Button _removeSelected;
        private readonly Label _allValue;
        private readonly Label _installedValue;
        private readonly Label _missingValue;
        private readonly Label _updatesValue;

        private LibrarySnapshot? _snapshot;
        private string _category = "All Cores";
        private bool _suppressPreferenceSave;

        public CoreLibrary()
        {
            Dock = DockStyle.Fill;
            BackColor = ModernTheme.AppBackground;
            Padding = new Padding(20);

            _savedStatusTimer = new System.Windows.Forms.Timer { Interval = 3000 };
            _savedStatusTimer.Tick += (_, _) =>
            {
                _savedStatusTimer.Stop();
                ShowAutoSaveIdleStatus();
            };

            _artPreviewTimer = new System.Windows.Forms.Timer { Interval = 275 };
            _artPreviewTimer.Tick += (_, _) =>
            {
                _artPreviewTimer.Stop();
                ShowQueuedArtworkPreview();
            };

            Disposed += (_, _) =>
            {
                _savedStatusTimer.Dispose();
                _artPreviewTimer.Dispose();
                if (_artPreviewPopup != null && !_artPreviewPopup.IsDisposed)
                    _artPreviewPopup.Close();
            };

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                BackColor = Color.Transparent
            };
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 68));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 108));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            Controls.Add(root);

            var heading = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1,
                BackColor = Color.Transparent,
                Margin = new Padding(0),
                Padding = new Padding(0)
            };
            heading.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 54));
            heading.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            heading.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 112));

            var headingIconHost = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                FillColor = Color.Transparent,
                BackColor = Color.Transparent,
                Margin = new Padding(0)
            };
            var headingIcon = IconTile(UiIcon.Core, ModernTheme.AccentHover, Point.Empty, 44);
            headingIcon.Anchor = AnchorStyles.Left;
            headingIcon.Location = new Point(0, 3);
            headingIconHost.Controls.Add(headingIcon);

            var headingText = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                FillColor = Color.Transparent,
                BackColor = Color.Transparent,
                Margin = new Padding(0),
                Padding = new Padding(0)
            };
            _title = Label("Cores", 23, FontStyle.Bold, ModernTheme.TextPrimary);
            _title.AutoSize = false;
            _title.Location = new Point(0, 3);
            _title.Size = new Size(520, 44);
            _title.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
            _title.TextAlign = ContentAlignment.MiddleLeft;
            _title.Padding = new Padding(0);

            _subtitle = Label(string.Empty, 10, FontStyle.Regular, ModernTheme.TextSecondary);
            _subtitle.Visible = false;
            headingText.Controls.Add(_title);
            headingText.Resize += (_, _) => _title.Width = Math.Max(0, headingText.ClientSize.Width);

            _refreshButton = Button("Refresh", 0, 0, 104, true);
            _refreshButton.FillColor = ModernTheme.Success;
            _refreshButton.BorderThickness = 0;
            _refreshButton.BorderColor = ModernTheme.Success;
            _refreshButton.ForeColor = Color.White;
            _refreshButton.Image = null;
            _refreshButton.TextOffset = Point.Empty;
            _refreshButton.HoverState.FillColor = Color.FromArgb(42, 184, 116);
            _refreshButton.PressedColor = Color.FromArgb(35, 158, 99);
            _refreshButton.Dock = DockStyle.Top;
            _refreshButton.Margin = new Padding(8, 8, 0, 0);
            _refreshButton.Click += async (_, _) => await RefreshAsync();

            heading.Controls.Add(headingIconHost, 0, 0);
            heading.Controls.Add(headingText, 1, 0);
            heading.Controls.Add(_refreshButton, 2, 0);
            root.Controls.Add(heading, 0, 0);

            var summary = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 1,
                Padding = new Padding(0, 0, 0, 12),
                BackColor = Color.Transparent
            };
            for (int i = 0; i < 4; i++) summary.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            summary.Controls.Add(SummaryCard("All Cores", UiIcon.Grid, out _allValue, ModernTheme.AccentHover, new Padding(0, 0, 7, 0)), 0, 0);
            summary.Controls.Add(SummaryCard("Installed", UiIcon.Check, out _installedValue, ModernTheme.Success, new Padding(7, 0, 7, 0)), 1, 0);
            summary.Controls.Add(SummaryCard("Missing", UiIcon.Missing, out _missingValue, ModernTheme.Danger, new Padding(7, 0, 7, 0)), 2, 0);
            summary.Controls.Add(SummaryCard("Updates Available", UiIcon.Update, out _updatesValue, Color.FromArgb(89, 161, 255), new Padding(7, 0, 0, 0)), 3, 0);
            root.Controls.Add(summary, 0, 1);

            var filters = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 6,
                RowCount = 1,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 0, 0, 10)
            };
            filters.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45));
            filters.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 185));
            filters.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 170));
            filters.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
            filters.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));
            filters.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 10));

            _search = new Guna2TextBox
            {
                Dock = DockStyle.Fill,
                Font = ModernTheme.BodyFont,
                FillColor = ModernTheme.SurfaceRaised,
                ForeColor = ModernTheme.TextPrimary,
                PlaceholderForeColor = ModernTheme.TextMuted,
                BorderColor = ModernTheme.BorderStrong,
                BorderRadius = 9,
                PlaceholderText = "Search cores by name, developer, or identifier…",
                Margin = new Padding(0, 0, 10, 0),
                IconLeft = UiIcons.Get(UiIcon.Search, Color.FromArgb(151, 174, 207), 17),
                IconLeftSize = new Size(17, 17),
                IconLeftOffset = new Point(8, 0)
            };
            _search.FocusedState.BorderColor = ModernTheme.Accent;
            _search.HoverState.BorderColor = ModernTheme.Accent;
            _search.TextChanged += (_, _) => ApplyFilters();

            _statusFilter = FilterCombo(new[] { "All Statuses", "Installed", "Missing", "Update Available" });
            _statusFilter.SelectedIndexChanged += (_, _) => ApplyFilters();
            _developerFilter = FilterCombo(new[] { "All Developers" });
            _developerFilter.SelectedIndexChanged += (_, _) => ApplyFilters();

            _sortFilter = FilterCombo(new[] { "Name (A-Z)", "Name (Z-A)" });
            _sortFilter.SelectedIndexChanged += (_, _) => ApplyFilters(_sortFilter.SelectedIndex == 1);

            _installedOnly = new Guna2CheckBox
            {
                Text = "Installed only",
                Dock = DockStyle.Fill,
                ForeColor = ModernTheme.TextSecondary,
                Font = ModernTheme.BodyFont,
                BackColor = Color.Transparent,
                Padding = new Padding(8, 0, 0, 0)
            };
            _installedOnly.CheckedState.FillColor = ModernTheme.Accent;
            _installedOnly.CheckedState.BorderColor = ModernTheme.Accent;
            _installedOnly.UncheckedState.BorderColor = ModernTheme.BorderStrong;
            _installedOnly.UncheckedState.FillColor = ModernTheme.SurfaceRaised;
            _installedOnly.CheckedChanged += (_, _) => ApplyFilters();

            filters.Controls.Add(_search, 0, 0);
            filters.Controls.Add(_statusFilter, 1, 0);
            filters.Controls.Add(_developerFilter, 2, 0);
            filters.Controls.Add(_sortFilter, 3, 0);
            filters.Controls.Add(_installedOnly, 4, 0);
            root.Controls.Add(filters, 0, 2);

            var content = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.Transparent
            };
            content.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 286));
            content.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            var categoryCard = Card();
            categoryCard.Margin = new Padding(0, 0, 12, 0);
            var categoryTitle = Label("Core Types", 11.5F, FontStyle.Bold, ModernTheme.TextPrimary);
            categoryTitle.Location = new Point(12, 8);
            categoryTitle.Size = new Size(224, 28);
            categoryTitle.AutoSize = false;
            categoryTitle.TextAlign = ContentAlignment.MiddleLeft;
            _categoryPanel = new FlowLayoutPanel
            {
                Location = new Point(8, 42),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = false,
                BackColor = Color.Transparent,
                Margin = new Padding(0),
                Size = new Size(260, 420)
            };
            categoryCard.Resize += (_, _) =>
            {
                _categoryPanel.Size = new Size(categoryCard.ClientSize.Width - 16, categoryCard.ClientSize.Height - 52);
                ResizeCategoryButtons();
            };
            _categoryPanel.Resize += (_, _) => ResizeCategoryButtons();
            categoryCard.Controls.Add(categoryTitle);
            categoryCard.Controls.Add(_categoryPanel);

            var right = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                FillColor = ModernTheme.Surface,
                BorderColor = ModernTheme.Border,
                BorderThickness = 1,
                BorderRadius = 14,
                Padding = new Padding(1)
            };
            var rightLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                BackColor = Color.Transparent,
                Padding = new Padding(12)
            };
            rightLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
            rightLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            rightLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 54));
            right.Controls.Add(rightLayout);

            var toolbar = new Guna2Panel { Dock = DockStyle.Fill, FillColor = Color.Transparent, BackColor = Color.Transparent };
            var toolbarLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1,
                BackColor = Color.Transparent,
                Margin = new Padding(0),
                Padding = new Padding(2, 0, 0, 0)
            };
            toolbarLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 190));
            toolbarLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            toolbarLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 132F));
            _sectionTitle = Label("All Cores", 13, FontStyle.Bold, ModernTheme.TextPrimary);
            _sectionTitle.Name = "SectionTitle";
            _sectionTitle.Dock = DockStyle.Fill;
            _sectionTitle.AutoSize = false;
            _sectionTitle.TextAlign = ContentAlignment.MiddleLeft;
            _sectionTitle.Padding = new Padding(6, 0, 0, 0);
            _selectionLabel = Label("0 enabled in view", 9, FontStyle.Regular, ModernTheme.TextSecondary);
            _selectionLabel.Dock = DockStyle.Fill;
            _selectionLabel.AutoSize = false;
            _selectionLabel.TextAlign = ContentAlignment.MiddleLeft;
            _selectionLabel.Padding = new Padding(4, 0, 0, 0);
            _autoSaveLabel = Label("Changes save automatically", 8.5F, FontStyle.Regular, ModernTheme.TextMuted);
            _autoSaveLabel.Dock = DockStyle.Fill;
            _autoSaveLabel.AutoSize = false;
            _autoSaveLabel.TextAlign = ContentAlignment.MiddleRight;
            _autoSaveLabel.Padding = new Padding(0, 0, 8, 0);

            toolbarLayout.Controls.Add(_sectionTitle, 0, 0);
            toolbarLayout.Controls.Add(_selectionLabel, 1, 0);
            toolbarLayout.Controls.Add(_autoSaveLabel, 2, 0);
            toolbar.Controls.Add(toolbarLayout);
            rightLayout.Controls.Add(toolbar, 0, 0);

            _grid = BuildGrid();
            var gridScrollHost = ModernScrollbars.WrapVertical(_grid, 10);
            rightLayout.Controls.Add(gridScrollHost, 0, 1);

            var actions = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 6,
                RowCount = 1,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 8, 0, 0),
                Margin = new Padding(0)
            };

            actions.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            actions.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 126F));
            actions.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 126F));
            actions.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 132F));
            actions.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140F));
            actions.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140F));
            var clear = Button("Disable Visible", 0, 0, 145, false);
            PrepareActionButton(clear, UiIcon.Missing, false);
            clear.Click += (_, _) => SetVisibleSelection(false);

            var selectVisible = Button("Enable Visible", 0, 0, 145, false);
            PrepareActionButton(selectVisible, UiIcon.Check, false);
            selectVisible.Click += (_, _) => SetVisibleSelection(true);

            _installMissing = Button("Install Missing", 0, 0, 148, false);
            PrepareActionButton(_installMissing, UiIcon.Download, false);
            _installMissing.Click += async (_, _) => await RunSelectedUpdateAsync(true);

            _updateSelected = Button("Update Checked", 0, 0, 158, true);
            PrepareActionButton(_updateSelected, UiIcon.Update, true);
            _updateSelected.Click += async (_, _) => await RunSelectedUpdateAsync(false);

            _removeSelected = Button("Uninstall Selected", 0, 0, 158, false);
            PrepareActionButton(_removeSelected, UiIcon.Trash, false);
            _removeSelected.BorderColor = ModernTheme.Danger;
            _removeSelected.ForeColor = Color.FromArgb(255, 184, 192);
            _removeSelected.Click += async (_, _) => await RemoveSelectedAsync();
            _grid.SelectionChanged += (_, _) => UpdateSelectionLabel();

            actions.Controls.Add(clear, 1, 0);
            actions.Controls.Add(selectVisible, 2, 0);
            actions.Controls.Add(_installMissing, 3, 0);
            actions.Controls.Add(_updateSelected, 4, 0);
            actions.Controls.Add(_removeSelected, 5, 0);
            rightLayout.Controls.Add(actions, 0, 2);

            content.Controls.Add(categoryCard, 0, 0);
            content.Controls.Add(right, 1, 0);
            root.Controls.Add(content, 0, 3);

            PocketTargetContext.TargetChanged += async (_, _) =>
            {
                if (Visible)
                    await RefreshAsync();
            };
        }

        public async Task RefreshAsync()
        {
            _refreshButton.Enabled = false;
            _refreshButton.Text = "Loading…";
            try
            {
                _snapshot = await LibraryInventoryService.LoadAsync(PocketTargetContext.SelectedPath);
                PopulateSnapshot();
            }
            catch (Exception ex)
            {
                _grid.Rows.Clear();
                _grid.Rows.Add(false, "Unable to load core inventory", ex.Message, "", "", "", "Error", "");
            }
            finally
            {
                _refreshButton.Text = "Refresh";
                _refreshButton.Enabled = true;
            }
        }

        private void PopulateSnapshot()
        {
            if (_snapshot == null) return;

            _allValue.Text = _snapshot.TotalCores.ToString();
            _installedValue.Text = _snapshot.InstalledCores.ToString();
            _missingValue.Text = _snapshot.MissingCores.ToString();
            _updatesValue.Text = _snapshot.CoreUpdates.ToString();

            _developerFilter.Items.Clear();
            _developerFilter.Items.Add("All Developers");
            foreach (string developer in _snapshot.Cores.Select(x => x.Developer).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(x => x))
                _developerFilter.Items.Add(developer);
            _developerFilter.SelectedIndex = 0;

            PopulateCategories();
            ApplyFilters();
        }

        private void PopulateCategories()
        {
            _categoryPanel.Controls.Clear();
            if (_snapshot == null) return;

            AddCategoryButton("All Cores", _snapshot.Cores);
            foreach (string category in new[] { "Arcade", "Console", "Computer", "Handheld", "Other" })
            {
                var items = _snapshot.Cores.Where(x => x.Category == category).ToList();
                if (items.Count > 0)
                    AddCategoryButton(category, items);
            }
        }

        private void AddCategoryButton(string category, IReadOnlyCollection<CoreLibraryItem> items)
        {
            int installed = items.Count(x => x.Status != CoreLibraryStatus.Missing);
            int enabled = items.Count(x => x.EnabledForUpdate);
            int updates = items.Count(x => x.Status == CoreLibraryStatus.UpdateAvailable);

            var card = new Guna2Panel
            {
                Name = "CategoryCard",
                Size = new Size(Math.Max(180, _categoryPanel.ClientSize.Width), 62),
                Margin = new Padding(0, 0, 0, 6),
                Padding = new Padding(6, 5, 6, 5),
                FillColor = category == _category ? ModernTheme.AccentSubtle : ModernTheme.SurfaceRaised,
                BorderColor = category == _category ? ModernTheme.Accent : ModernTheme.Border,
                BorderThickness = 1,
                BorderRadius = 10,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand,
                Tag = category
            };

            var layout = new TableLayoutPanel
            {
                Name = "CategoryLayout",
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 3,
                BackColor = Color.Transparent,
                Margin = new Padding(0),
                Padding = new Padding(0),
                Cursor = Cursors.Hand
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 34));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 16));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 15));

            var icon = new Guna2PictureBox
            {
                Name = "CategoryIcon",
                Dock = DockStyle.Fill,
                Margin = new Padding(0),
                BackColor = Color.Transparent,
                SizeMode = PictureBoxSizeMode.CenterImage,
                Image = UiIcons.Get(CategoryIcon(category), Color.FromArgb(177, 205, 244), 26),
                Cursor = Cursors.Hand
            };
            layout.Controls.Add(icon, 0, 0);
            layout.SetRowSpan(icon, 2);

            var textStack = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                BackColor = Color.Transparent,
                Margin = new Padding(0),
                Padding = new Padding(0, 3, 0, 0),
                Cursor = Cursors.Hand
            };
            textStack.RowStyles.Add(new RowStyle(SizeType.Absolute, 18));
            textStack.RowStyles.Add(new RowStyle(SizeType.Absolute, 15));
            textStack.RowStyles.Add(new RowStyle(SizeType.Absolute, 15));

            var title = Label(category, 9.4F, FontStyle.Bold, ModernTheme.TextPrimary);
            title.Name = "CategoryTitle";
            title.Dock = DockStyle.Fill;
            title.AutoSize = false;
            title.Margin = new Padding(0);
            title.TextAlign = ContentAlignment.MiddleLeft;
            title.Padding = new Padding(0);
            title.Cursor = Cursors.Hand;

            var stats1 = Label($"{items.Count} total  •  {installed} installed", 8.2F, FontStyle.Regular, ModernTheme.TextSecondary);
            stats1.Name = "CategoryStats1";
            stats1.Dock = DockStyle.Fill;
            stats1.AutoSize = false;
            stats1.Margin = new Padding(0);
            stats1.TextAlign = ContentAlignment.MiddleLeft;
            stats1.Padding = new Padding(0);
            stats1.Cursor = Cursors.Hand;

            var stats2 = Label($"{enabled} enabled  •  {updates} updates", 8.2F, FontStyle.Regular, ModernTheme.TextSecondary);
            stats2.Name = "CategoryStats2";
            stats2.Dock = DockStyle.Fill;
            stats2.AutoSize = false;
            stats2.Margin = new Padding(0);
            stats2.TextAlign = ContentAlignment.MiddleLeft;
            stats2.Padding = new Padding(0);
            stats2.Cursor = Cursors.Hand;

            textStack.Controls.Add(title, 0, 0);
            textStack.Controls.Add(stats1, 0, 1);
            textStack.Controls.Add(stats2, 0, 2);
            layout.Controls.Add(textStack, 1, 0);
            layout.SetRowSpan(textStack, 3);

            void SelectCategory()
            {
                _category = category;
                foreach (Control control in _categoryPanel.Controls)
                {
                    if (control is Guna2Panel categoryCard && categoryCard.Name == "CategoryCard")
                    {
                        bool active = string.Equals(categoryCard.Tag as string, _category, StringComparison.OrdinalIgnoreCase);
                        SetCategoryCardActive(categoryCard, active);
                    }
                }
                ApplyFilters();
            }

            card.Click += (_, _) => SelectCategory();
            layout.Click += (_, _) => SelectCategory();
            icon.Click += (_, _) => SelectCategory();
            title.Click += (_, _) => SelectCategory();
            stats1.Click += (_, _) => SelectCategory();
            stats2.Click += (_, _) => SelectCategory();

            card.Controls.Add(layout);
            _categoryPanel.Controls.Add(card);
        }

        private static void SetCategoryCardActive(Guna2Panel card, bool active)
        {
            card.FillColor = active ? ModernTheme.AccentSubtle : ModernTheme.SurfaceRaised;
            card.BorderColor = active ? ModernTheme.Accent : ModernTheme.Border;
        }

        private void ResizeCategoryButtons()
        {
            if (_categoryPanel == null) return;
            int width = Math.Max(180, _categoryPanel.ClientSize.Width);
            foreach (Control control in _categoryPanel.Controls)
            {
                if (control is Guna2Panel card && card.Name == "CategoryCard")
                    card.Width = width;
            }
        }

        private void RefreshCategoryCounts()
        {
            if (_snapshot == null) return;

            foreach (Control control in _categoryPanel.Controls)
            {
                if (control is not Guna2Panel card || card.Name != "CategoryCard" || card.Tag is not string category) continue;
                IReadOnlyCollection<CoreLibraryItem> items = category == "All Cores"
                    ? _snapshot.Cores
                    : _snapshot.Cores.Where(x => x.Category == category).ToList();

                int installed = items.Count(x => x.Status != CoreLibraryStatus.Missing);
                int enabled = items.Count(x => x.EnabledForUpdate);
                int updates = items.Count(x => x.Status == CoreLibraryStatus.UpdateAvailable);

                if (card.Controls.Find("CategoryStats1", true).FirstOrDefault() is Label stats1)
                    stats1.Text = $"{items.Count} total  •  {installed} installed";
                if (card.Controls.Find("CategoryStats2", true).FirstOrDefault() is Label stats2)
                    stats2.Text = $"{enabled} enabled  •  {updates} updates";
            }
        }

        private void ApplyFilters(bool descending = false)
        {
            if (_snapshot == null) return;

            string search = _search.Text.Trim();
            string status = _statusFilter.SelectedItem?.ToString() ?? "All Statuses";
            string developer = _developerFilter.SelectedItem?.ToString() ?? "All Developers";

            IEnumerable<CoreLibraryItem> query = _snapshot.Cores;
            if (_category != "All Cores") query = query.Where(x => x.Category == _category);
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x => x.Name.Contains(search, StringComparison.OrdinalIgnoreCase)
                    || x.Developer.Contains(search, StringComparison.OrdinalIgnoreCase)
                    || x.Identifier.Contains(search, StringComparison.OrdinalIgnoreCase));
            }
            if (developer != "All Developers") query = query.Where(x => x.Developer == developer);
            if (_installedOnly.Checked) query = query.Where(x => x.Status != CoreLibraryStatus.Missing);
            query = status switch
            {
                "Installed" => query.Where(x => x.Status == CoreLibraryStatus.Installed),
                "Missing" => query.Where(x => x.Status == CoreLibraryStatus.Missing),
                "Update Available" => query.Where(x => x.Status == CoreLibraryStatus.UpdateAvailable),
                _ => query
            };
            query = descending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name);

            HideArtworkPreview();
            _suppressPreferenceSave = true;
            try
            {
                _grid.Rows.Clear();
                foreach (CoreLibraryItem item in query)
                {
                    string statusText = item.Status switch
                    {
                        CoreLibraryStatus.Installed => "Installed",
                        CoreLibraryStatus.UpdateAvailable => "Update Available",
                        _ => "Not Installed"
                    };
                    string assets = item.RequiredAssets == 0 ? "—" : $"{item.PresentAssets}/{item.RequiredAssets}";
                    int index = _grid.Rows.Add(item.EnabledForUpdate, item.Name, item.Developer, item.Category,
                        string.IsNullOrWhiteSpace(item.InstalledVersion) ? "—" : item.InstalledVersion,
                        string.IsNullOrWhiteSpace(item.LatestVersion) ? "—" : item.LatestVersion,
                        statusText, assets);
                    _grid.Rows[index].Tag = item;
                }
            }
            finally
            {
                _suppressPreferenceSave = false;
            }

            _sectionTitle.Text = _category;
            UpdateSelectionLabel();
        }

        private Guna2DataGridView BuildGrid()
        {
            var grid = new Guna2DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                MultiSelect = true,
                ReadOnly = false,
                BackgroundColor = ModernTheme.Surface,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                GridColor = ModernTheme.Border,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ScrollBars = ScrollBars.Vertical
            };
            grid.Columns.Add(new DataGridViewCheckBoxColumn { Name = "Enabled", HeaderText = "Enabled", FillWeight = 10, MinimumWidth = 74, SortMode = DataGridViewColumnSortMode.NotSortable });
            grid.Columns.Add(new DataGridViewLinkColumn
            {
                Name = "CoreName",
                HeaderText = "Core Name",
                FillWeight = 32,
                ReadOnly = true,
                TrackVisitedState = false,
                LinkColor = ModernTheme.AccentHover,
                ActiveLinkColor = Color.White,
                VisitedLinkColor = ModernTheme.AccentHover,
                LinkBehavior = LinkBehavior.HoverUnderline
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Developer", HeaderText = "Developer", FillWeight = 17, ReadOnly = true });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Category", HeaderText = "Type", FillWeight = 13, ReadOnly = true });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Installed", HeaderText = "Installed", FillWeight = 13, ReadOnly = true });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Latest", HeaderText = "Latest", FillWeight = 13, ReadOnly = true });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Status", HeaderText = "Status", FillWeight = 19, MinimumWidth = 112, ReadOnly = true });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Assets", HeaderText = "Assets", FillWeight = 10, ReadOnly = true });
            ModernTheme.StyleDataGrid(grid);
            grid.Columns["Enabled"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            grid.Columns["Enabled"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            grid.RowTemplate.Height = 35;
            grid.CurrentCellDirtyStateChanged += (_, _) =>
            {
                if (grid.IsCurrentCellDirty) grid.CommitEdit(DataGridViewDataErrorContexts.Commit);
            };
            grid.CellValueChanged += (_, e) =>
            {
                if (e.RowIndex < 0 || e.ColumnIndex != 0 || _suppressPreferenceSave) return;
                if (grid.Rows[e.RowIndex].Tag is not CoreLibraryItem item) return;

                bool enabled = grid.Rows[e.RowIndex].Cells[0].Value is bool value && value;
                SaveCorePreference(item, enabled);
                UpdateSelectionLabel();
            };
            grid.CellContentClick += (_, e) =>
            {
                if (e.RowIndex < 0 || grid.Columns[e.ColumnIndex].Name != "CoreName") return;
                if (grid.Rows[e.RowIndex].Tag is not CoreLibraryItem item || string.IsNullOrWhiteSpace(item.RepositoryUrl)) return;

                HideArtworkPreview();
                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = item.RepositoryUrl,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    ModernDialog.ShowError(FindForm(), "Unable to open GitHub", ex.Message);
                }
            };
            grid.ShowCellToolTips = false;
            grid.CellMouseEnter += (_, e) =>
            {
                if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

                if (grid.Rows[e.RowIndex].Tag is CoreLibraryItem item)
                {
                    grid.Cursor = grid.Columns[e.ColumnIndex].Name == "CoreName"
                        && !string.IsNullOrWhiteSpace(item.RepositoryUrl)
                            ? Cursors.Hand
                            : Cursors.Default;
                    QueueArtworkPreview(e.RowIndex, e.ColumnIndex);
                }
            };
            grid.CellMouseLeave += (_, _) => grid.Cursor = Cursors.Default;
            grid.Scroll += (_, _) => HideArtworkPreview();
            grid.MouseLeave += (_, _) => HideArtworkPreview();
            grid.CellFormatting += (_, e) =>
            {
                if (e.RowIndex < 0 || grid.Columns[e.ColumnIndex].Name != "Status" || e.Value == null) return;
                string value = e.Value.ToString() ?? string.Empty;
                e.CellStyle.ForeColor = value switch
                {
                    "Installed" => ModernTheme.Success,
                    "Update Available" => Color.FromArgb(92, 166, 255),
                    _ => Color.FromArgb(255, 133, 148)
                };
                e.CellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            };
            return grid;
        }

        private void QueueArtworkPreview(int rowIndex, int columnIndex)
        {

            if (_artPreviewRow == rowIndex)
            {
                _artPreviewColumn = columnIndex;
                return;
            }

            _artPreviewTimer.Stop();
            _artPreviewPopup?.Hide();
            _artPreviewRow = rowIndex;
            _artPreviewColumn = columnIndex;
            _artPreviewTimer.Start();
        }

        private void ShowQueuedArtworkPreview()
        {
            if (_snapshot == null || _artPreviewRow < 0 || _artPreviewColumn < 0
                || _artPreviewRow >= _grid.Rows.Count || _artPreviewColumn >= _grid.Columns.Count)
                return;

            Point client = _grid.PointToClient(Cursor.Position);
            DataGridView.HitTestInfo hit = _grid.HitTest(client.X, client.Y);
            if (hit.RowIndex != _artPreviewRow)
                return;
            if (_grid.Rows[_artPreviewRow].Tag is not CoreLibraryItem item)
                return;

            PlatformArtworkPreview? preview = PlatformArtworkPreviewService.TryLoad(item, _snapshot.TargetPath);
            if (preview == null)
                return;

            Rectangle row = _grid.GetRowDisplayRectangle(_artPreviewRow, false);
            Point screenPoint = _grid.PointToScreen(row.Location);
            Rectangle anchor = new(screenPoint, row.Size);

            _artPreviewPopup ??= new CoreArtworkPreviewPopup();
            _artPreviewPopup.ShowPreview(_grid, item, preview, anchor);
        }

        private void HideArtworkPreview()
        {
            _artPreviewTimer.Stop();
            _artPreviewRow = -1;
            _artPreviewColumn = -1;
            if (_artPreviewPopup != null && !_artPreviewPopup.IsDisposed)
                _artPreviewPopup.Hide();
        }

        private List<CoreLibraryItem> SelectedItems(bool missingOnly = false)
        {
            var selected = new List<CoreLibraryItem>();
            foreach (DataGridViewRow row in _grid.Rows)
            {
                bool isChecked = row.Cells[0].Value is bool value && value;
                if (!isChecked || row.Tag is not CoreLibraryItem item) continue;
                if (!missingOnly || item.Status == CoreLibraryStatus.Missing) selected.Add(item);
            }
            return selected;
        }

        private async Task RunSelectedUpdateAsync(bool missingOnly)
        {
            var items = SelectedItems(missingOnly);
            if (items.Count == 0 && missingOnly)
            {
                items = _grid.Rows.Cast<DataGridViewRow>()
                    .Select(r => r.Tag as CoreLibraryItem)
                    .Where(x => x?.Status == CoreLibraryStatus.Missing)
                    .Cast<CoreLibraryItem>()
                    .ToList();
            }
            if (items.Count == 0)
            {
                ModernDialog.ShowInfo(FindForm(), "Pocket Updater", "Check one or more cores first. Checked cores are saved automatically.");
                return;
            }

            using var progress = new ModernUpdateProgressForm(PocketTargetContext.SelectedPath, items.Select(x => x.Identifier).ToArray());
            progress.ShowDialog(FindForm());
            await RefreshAsync();
            LibraryChangeNotifier.Notify();
        }

        private async Task RemoveSelectedAsync()
        {

            var items = _grid.SelectedRows
                .Cast<DataGridViewRow>()
                .Select(row => row.Tag as CoreLibraryItem)
                .Where(item => item != null && item.Status != CoreLibraryStatus.Missing)
                .Cast<CoreLibraryItem>()
                .DistinctBy(item => item.Identifier)
                .ToList();

            if (items.Count == 0)
            {
                ModernDialog.ShowInfo(FindForm(), "Pocket Updater",
                    "Select one or more installed rows first. The Enabled checkboxes only control which cores participate in updates.");
                return;
            }

            if (!ModernDialog.ShowConfirm(FindForm(), "Uninstall Cores",
                    $"Uninstall {items.Count} selected core(s) from the selected Pocket?\n\nCore files and core-specific assets will be removed. Shared ROM / BIOS files are preserved."))
                return;

            try
            {
                ServiceHelper.Initialize(PocketTargetContext.SelectedPath, Directory.GetCurrentDirectory(), forceReload: true);
                await Task.Run(() =>
                {
                    foreach (var item in items)
                        ServiceHelper.CoresService.Uninstall(item.Identifier, item.Core.platform_id, true);
                });
                ModernDialog.ShowInfo(FindForm(), "Pocket Updater", $"Uninstalled {items.Count} core(s) and their core-specific assets.");
                await RefreshAsync();
                LibraryChangeNotifier.Notify();
            }
            catch (Exception ex)
            {
                ModernDialog.ShowError(FindForm(), "Unable to uninstall cores", ex.Message);
            }
        }

        private void SetVisibleSelection(bool enabled)
        {
            if (_snapshot == null) return;

            var changed = new List<CoreLibraryItem>();
            _suppressPreferenceSave = true;
            try
            {
                foreach (DataGridViewRow row in _grid.Rows)
                {
                    if (row.Tag is not CoreLibraryItem item) continue;
                    if (item.EnabledForUpdate == enabled)
                    {
                        row.Cells[0].Value = enabled;
                        continue;
                    }

                    item.EnabledForUpdate = enabled;
                    row.Cells[0].Value = enabled;
                    changed.Add(item);
                }
            }
            finally
            {
                _suppressPreferenceSave = false;
            }

            if (changed.Count > 0)
            {
                foreach (CoreLibraryItem item in changed)
                {
                    if (enabled) ServiceHelper.SettingsService.EnableCore(item.Identifier);
                    else ServiceHelper.SettingsService.DisableCore(item.Identifier);
                }
                ServiceHelper.SettingsService.Save();
                ShowSavedStatus();
                RefreshCategoryCounts();
            }

            UpdateSelectionLabel();
        }

        private void SaveCorePreference(CoreLibraryItem item, bool enabled)
        {
            item.EnabledForUpdate = enabled;
            if (enabled) ServiceHelper.SettingsService.EnableCore(item.Identifier);
            else ServiceHelper.SettingsService.DisableCore(item.Identifier);
            ServiceHelper.SettingsService.Save();
            ShowSavedStatus();
            RefreshCategoryCounts();
        }

        private void ShowSavedStatus()
        {
            _autoSaveLabel.Text = "Changes Saved";
            _autoSaveLabel.ForeColor = ModernTheme.Success;
            _savedStatusTimer.Stop();
            _savedStatusTimer.Start();
        }

        private void ShowAutoSaveIdleStatus()
        {
            _autoSaveLabel.Text = "Changes save automatically";
            _autoSaveLabel.ForeColor = ModernTheme.TextMuted;
        }

        private void UpdateSelectionLabel()
        {
            int enabledCount = _grid.Rows.Cast<DataGridViewRow>().Count(r => r.Cells[0].Value is bool value && value);
            int installedSelected = _grid.SelectedRows.Cast<DataGridViewRow>().Count(r =>
                r.Tag is CoreLibraryItem item && item.Status != CoreLibraryStatus.Missing);

            _selectionLabel.Text = $"{enabledCount} enabled in view";
            _updateSelected.Enabled = enabledCount > 0;
            _removeSelected.Enabled = installedSelected > 0;
        }

        private static Guna2Panel SummaryCard(string text, UiIcon icon, out Label value, Color valueColor, Padding margin)
        {
            var card = Card();
            card.Margin = margin;
            card.Padding = new Padding(14, 12, 14, 10);

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                BackColor = Color.Transparent,
                Margin = new Padding(0),
                Padding = new Padding(0)
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 48));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 27));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            var iconHost = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                FillColor = Color.FromArgb(38, valueColor),
                BorderRadius = 10,
                BackColor = Color.Transparent,
                Margin = new Padding(0, 0, 10, 0)
            };
            iconHost.Controls.Add(new Guna2PictureBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                SizeMode = PictureBoxSizeMode.CenterImage,
                Image = UiIcons.Get(icon, valueColor, 21)
            });
            layout.Controls.Add(iconHost, 0, 0);
            layout.SetRowSpan(iconHost, 2);

            var caption = Label(text, 9.5F, FontStyle.Bold, ModernTheme.TextSecondary);
            caption.Dock = DockStyle.Fill;
            caption.AutoSize = false;
            caption.Margin = new Padding(0);
            caption.TextAlign = ContentAlignment.BottomLeft;

            value = Label("—", 22, FontStyle.Bold, valueColor);
            value.Dock = DockStyle.Fill;
            value.AutoSize = false;
            value.Margin = new Padding(0);
            value.TextAlign = ContentAlignment.TopLeft;

            layout.Controls.Add(caption, 1, 0);
            layout.Controls.Add(value, 1, 1);
            card.Controls.Add(layout);
            return card;
        }

        private static UiIcon CategoryIcon(string category) => category switch
        {
            "Arcade" => UiIcon.Arcade,
            "Console" => UiIcon.Console,
            "Computer" => UiIcon.Computer,
            "Handheld" => UiIcon.Handheld,
            "Other" => UiIcon.Grid,
            _ => UiIcon.Grid
        };

        private static Guna2Panel IconTile(UiIcon icon, Color color, Point location, int size = 44)
        {
            var tile = new Guna2Panel
            {
                Location = location,
                Size = new Size(size, size),
                BorderRadius = 11,
                FillColor = Color.FromArgb(38, color),
                BackColor = Color.Transparent
            };
            tile.Controls.Add(new Guna2PictureBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                SizeMode = PictureBoxSizeMode.CenterImage,
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

        private static void PrepareActionButton(Guna2Button button, UiIcon icon, bool primary)
        {

            button.Dock = DockStyle.Fill;
            button.Margin = new Padding(5, 0, 0, 0);
            button.Image = null;
            button.TextAlign = HorizontalAlignment.Center;
            button.TextOffset = Point.Empty;
            button.Padding = new Padding(0);
            button.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
        }

        private static Guna2Panel Card() => new()
        {
            Dock = DockStyle.Fill,
            FillColor = ModernTheme.Surface,
            BorderColor = ModernTheme.Border,
            BorderThickness = 1,
            BorderRadius = 14
        };

        private static Label Label(string text, float size, FontStyle style, Color color) => new()
        {
            Text = text,
            Font = new Font("Segoe UI", size, style, GraphicsUnit.Point),
            ForeColor = color,
            BackColor = Color.Transparent
        };

        private static Guna2ComboBox FilterCombo(IEnumerable<string> items)
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

        private static Guna2Button Button(string text, int x, int y, int width, bool primary)
        {
            var button = new Guna2Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(width, 36),
                BorderRadius = 9,
                FillColor = primary ? ModernTheme.Accent : ModernTheme.SurfaceRaised,
                ForeColor = ModernTheme.TextPrimary,
                Font = ModernTheme.BodyBoldFont,
                BorderThickness = primary ? 0 : 1,
                BorderColor = ModernTheme.BorderStrong,
                Cursor = Cursors.Hand,
                Margin = new Padding(8, 0, 0, 0)
            };
            button.HoverState.FillColor = primary ? ModernTheme.AccentHover : ModernTheme.SurfaceHover;
            return button;
        }
    }
}
