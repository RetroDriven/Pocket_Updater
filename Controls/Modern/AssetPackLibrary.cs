using System.Diagnostics;
using Guna.UI2.WinForms;
using Pannella.Helpers;
using Pannella.Models;
using Pannella.Services;
using Pocket_Updater.UI;
using Pocket_Updater.Forms.Asset_Pack_Progress;

namespace Pocket_Updater.Controls.Modern
{
    internal enum AssetPackLibraryStatus
    {
        Installed,
        Missing,
        UpdateAvailable
    }

    internal sealed class AssetPackLibraryItem
    {
        public PlatformImagePack Pack { get; init; } = null!;
        public string Owner => Pack.owner ?? string.Empty;
        public string Repository => Pack.repository ?? string.Empty;
        public string Variant => string.IsNullOrWhiteSpace(Pack.variant) ? "Default" : Pack.variant;
        public string InstalledVersion { get; init; } = string.Empty;
        public string LatestVersion { get; init; } = string.Empty;
        public int InstalledFiles { get; init; }
        public AssetPackLibraryStatus Status { get; init; }
        public bool Selected { get; set; }
        public string Key => $"{Owner}/{Repository}/{Variant}";
    }

    internal sealed class AssetPackLibrary : UserControl
    {
        private readonly FlowLayoutPanel _creatorPanel;
        private readonly Guna2TextBox _search;
        private readonly Guna2ComboBox _statusFilter;
        private readonly Guna2ComboBox _variantFilter;
        private readonly Guna2ComboBox _sortFilter;
        private readonly Guna2CheckBox _installedOnly;
        private readonly Guna2DataGridView _grid;
        private readonly Label _sectionTitle;
        private readonly Label _selectionLabel;
        private readonly Guna2Button _refreshButton;
        private readonly Guna2Button _installSelected;
        private readonly Guna2Button _updateSelected;
        private readonly Guna2Button _uninstallSelected;
        private readonly Label _allValue;
        private readonly Label _installedValue;
        private readonly Label _missingValue;
        private readonly Label _updatesValue;
        private readonly System.Windows.Forms.Timer _artPreviewTimer;
        private AssetPackArtworkPreviewPopup? _artPreviewPopup;
        private int _artPreviewRow = -1;
        private int _artPreviewColumn = -1;
        private int _artPreviewRequest;

        private List<AssetPackLibraryItem> _items = new();
        private string _creator = "All Packs";
        private bool _creatorInitialized;
        private bool _suppressSelectionChange;

        public AssetPackLibrary()
        {
            Dock = DockStyle.Fill;
            BackColor = ModernTheme.AppBackground;
            Padding = new Padding(20);

            _artPreviewTimer = new System.Windows.Forms.Timer { Interval = 300 };
            _artPreviewTimer.Tick += async (_, _) =>
            {
                _artPreviewTimer.Stop();
                await ShowQueuedArtworkPreviewAsync();
            };
            Disposed += (_, _) =>
            {
                _artPreviewTimer.Dispose();
                if (_artPreviewPopup != null && !_artPreviewPopup.IsDisposed)
                    _artPreviewPopup.Close();
            };

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                BackColor = Color.Transparent,
                Margin = new Padding(0),
                Padding = new Padding(0)
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
            var headingIcon = IconTile(UiIcon.AssetPack, Color.FromArgb(81, 194, 255), Point.Empty, 44);
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
            var title = Label("Asset Image Packs", 23, FontStyle.Bold, ModernTheme.TextPrimary);
            title.AutoSize = false;
            title.Location = new Point(0, 3);
            title.Size = new Size(520, 44);
            title.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
            title.TextAlign = ContentAlignment.MiddleLeft;
            title.Padding = new Padding(0);
            headingText.Controls.Add(title);
            headingText.Resize += (_, _) => title.Width = Math.Max(0, headingText.ClientSize.Width);

            _refreshButton = Button("Refresh", 104, true);
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
                BackColor = Color.Transparent,
                Margin = new Padding(0)
            };
            for (int i = 0; i < 4; i++)
                summary.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));

            summary.Controls.Add(SummaryCard("All Packs", UiIcon.Package, out _allValue, ModernTheme.AccentHover, new Padding(0, 0, 7, 0)), 0, 0);
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
                Padding = new Padding(0, 0, 0, 10),
                Margin = new Padding(0)
            };
            filters.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45F));
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
                PlaceholderText = "Search packs by creator, repository, or variant…",
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
            _variantFilter = FilterCombo(new[] { "All Variants" });
            _variantFilter.SelectedIndexChanged += (_, _) => ApplyFilters();
            _sortFilter = FilterCombo(new[] { "Creator (A-Z)", "Creator (Z-A)" });
            _sortFilter.SelectedIndexChanged += (_, _) => ApplyFilters();

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
            filters.Controls.Add(_variantFilter, 2, 0);
            filters.Controls.Add(_sortFilter, 3, 0);
            filters.Controls.Add(_installedOnly, 4, 0);
            root.Controls.Add(filters, 0, 2);

            var content = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.Transparent,
                Margin = new Padding(0)
            };
            content.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 286));
            content.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            var creatorCard = Card();
            creatorCard.Margin = new Padding(0, 0, 12, 0);
            var creatorTitle = Label("Creators", 11.5F, FontStyle.Bold, ModernTheme.TextPrimary);
            creatorTitle.Location = new Point(12, 8);
            creatorTitle.Size = new Size(224, 28);
            creatorTitle.AutoSize = false;
            creatorTitle.TextAlign = ContentAlignment.MiddleLeft;

            _creatorPanel = new FlowLayoutPanel
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
            creatorCard.Resize += (_, _) =>
            {
                _creatorPanel.Size = new Size(creatorCard.ClientSize.Width - 16, creatorCard.ClientSize.Height - 52);
                ResizeCreatorCards();
            };
            _creatorPanel.Resize += (_, _) => ResizeCreatorCards();
            creatorCard.Controls.Add(creatorTitle);
            creatorCard.Controls.Add(_creatorPanel);

            var right = Card();
            right.Margin = new Padding(0);
            right.Padding = new Padding(1);
            var rightLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                BackColor = Color.Transparent,
                Padding = new Padding(12),
                Margin = new Padding(0)
            };
            rightLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
            rightLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            rightLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 54));
            right.Controls.Add(rightLayout);

            var toolbarLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.Transparent,
                Margin = new Padding(0),
                Padding = new Padding(2, 0, 0, 0)
            };
            toolbarLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 190));
            toolbarLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            _sectionTitle = Label("All Packs", 13, FontStyle.Bold, ModernTheme.TextPrimary);
            _sectionTitle.Dock = DockStyle.Fill;
            _sectionTitle.AutoSize = false;
            _sectionTitle.TextAlign = ContentAlignment.MiddleLeft;
            _sectionTitle.Padding = new Padding(6, 0, 0, 0);
            _sectionTitle.Margin = new Padding(0);

            _selectionLabel = Label("0 selected • installed versions are tracked by Pocket Updater", 9, FontStyle.Regular, ModernTheme.TextSecondary);
            _selectionLabel.Dock = DockStyle.Fill;
            _selectionLabel.AutoSize = false;
            _selectionLabel.TextAlign = ContentAlignment.MiddleLeft;
            _selectionLabel.Padding = new Padding(4, 0, 0, 0);
            _selectionLabel.Margin = new Padding(0);

            toolbarLayout.Controls.Add(_sectionTitle, 0, 0);
            toolbarLayout.Controls.Add(_selectionLabel, 1, 0);
            rightLayout.Controls.Add(toolbarLayout, 0, 0);

            _grid = BuildGrid();
            rightLayout.Controls.Add(ModernScrollbars.WrapVertical(_grid, 10), 0, 1);

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
            actions.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 148F));

            var clearSelection = Button("Clear Selection", 0, false);
            PrepareActionButton(clearSelection);
            clearSelection.Click += (_, _) => SetVisibleSelection(false, clearAll: true);

            var selectVisible = Button("Select Visible", 0, false);
            PrepareActionButton(selectVisible);
            selectVisible.Click += (_, _) => SetVisibleSelection(true, clearAll: false);

            _installSelected = Button("Install Selected", 0, false);
            PrepareActionButton(_installSelected);
            _installSelected.Click += async (_, _) => await InstallSelectedAsync();

            _updateSelected = Button("Update Selected", 0, true);
            PrepareActionButton(_updateSelected);
            _updateSelected.Click += async (_, _) => await UpdateSelectedAsync();

            _uninstallSelected = Button("Uninstall Selected", 0, false);
            PrepareActionButton(_uninstallSelected);
            _uninstallSelected.BorderColor = ModernTheme.Danger;
            _uninstallSelected.ForeColor = Color.FromArgb(255, 184, 192);
            _uninstallSelected.Click += async (_, _) => await UninstallSelectedAsync();

            actions.Controls.Add(clearSelection, 1, 0);
            actions.Controls.Add(selectVisible, 2, 0);
            actions.Controls.Add(_installSelected, 3, 0);
            actions.Controls.Add(_updateSelected, 4, 0);
            actions.Controls.Add(_uninstallSelected, 5, 0);
            rightLayout.Controls.Add(actions, 0, 2);

            content.Controls.Add(creatorCard, 0, 0);
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
            _selectionLabel.Text = "Loading image-pack versions from GitHub releases…";

            var selectedKeys = new HashSet<string>(_items.Where(item => item.Selected).Select(item => item.Key), StringComparer.OrdinalIgnoreCase);

            try
            {
                string target = PocketTargetContext.SelectedPath;
                _items = await Task.Run(() => LoadItems(target, selectedKeys));
                PopulateSnapshot();
            }
            catch (Exception ex)
            {
                _items.Clear();
                _grid.Rows.Clear();
                _grid.Rows.Add(false, "Unable to load image packs", ex.Message, "", "", "", "Error", "");
                _allValue.Text = "0";
                _installedValue.Text = "0";
                _missingValue.Text = "0";
                _updatesValue.Text = "0";
                _selectionLabel.Text = "Unable to load image-pack inventory.";
            }
            finally
            {
                _refreshButton.Text = "Refresh";
                _refreshButton.Enabled = true;
            }
        }

        private static List<AssetPackLibraryItem> LoadItems(string target, HashSet<string> selectedKeys)
        {
            ServiceHelper.Initialize(target, Directory.GetCurrentDirectory(), forceReload: true);
            PlatformImagePacksService service = ServiceHelper.PlatformImagePacksService;
            List<PlatformImagePack> packs = service.List?.ToList() ?? new List<PlatformImagePack>();
            List<InstalledImagePackRecord> installed = service.GetInstalledPacks();

            var latestByRepo = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var repo in packs
                         .Where(pack => !string.IsNullOrWhiteSpace(pack.owner) && !string.IsNullOrWhiteSpace(pack.repository))
                         .Select(pack => new { Owner = pack.owner, Repository = pack.repository })
                         .DistinctBy(repo => $"{repo.Owner}/{repo.Repository}"))
            {
                string key = $"{repo.Owner}/{repo.Repository}";
                try
                {
                    latestByRepo[key] = service.GetLatestVersion(repo.Owner, repo.Repository);
                }
                catch
                {
                    latestByRepo[key] = string.Empty;
                }
            }

            var result = new List<AssetPackLibraryItem>();
            foreach (PlatformImagePack pack in packs)
            {
                string variant = NormalizeVariant(pack.variant);
                InstalledImagePackRecord record = installed.FirstOrDefault(candidate =>
                    string.Equals(candidate.owner ?? string.Empty, pack.owner ?? string.Empty, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(candidate.repository ?? string.Empty, pack.repository ?? string.Empty, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(NormalizeVariant(candidate.variant), variant, StringComparison.OrdinalIgnoreCase));

                latestByRepo.TryGetValue($"{pack.owner}/{pack.repository}", out string latest);
                string installedVersion = record?.version ?? string.Empty;
                AssetPackLibraryStatus status = record == null
                    ? AssetPackLibraryStatus.Missing
                    : IsUpdateAvailable(installedVersion, latest)
                        ? AssetPackLibraryStatus.UpdateAvailable
                        : AssetPackLibraryStatus.Installed;

                var item = new AssetPackLibraryItem
                {
                    Pack = pack,
                    InstalledVersion = installedVersion,
                    LatestVersion = latest ?? string.Empty,
                    InstalledFiles = record?.files?.Count ?? 0,
                    Status = status
                };
                item.Selected = selectedKeys.Contains(item.Key);
                result.Add(item);
            }

            return result;
        }

        private void PopulateSnapshot()
        {
            _allValue.Text = _items.Count.ToString();
            _installedValue.Text = _items.Count(item => item.Status != AssetPackLibraryStatus.Missing).ToString();
            _missingValue.Text = _items.Count(item => item.Status == AssetPackLibraryStatus.Missing).ToString();
            _updatesValue.Text = _items.Count(item => item.Status == AssetPackLibraryStatus.UpdateAvailable).ToString();

            _variantFilter.Items.Clear();
            _variantFilter.Items.Add("All Variants");
            foreach (string variant in _items.Select(item => item.Variant).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(value => value))
                _variantFilter.Items.Add(variant);
            _variantFilter.SelectedIndex = 0;

            if (!_creatorInitialized)
            {
                if (_items.Any(item => string.Equals(item.Owner, "dyreschlock", StringComparison.OrdinalIgnoreCase)))
                    _creator = "dyreschlock";
                _creatorInitialized = true;
            }

            PopulateCreators();
            ApplyFilters();
        }

        private void PopulateCreators()
        {
            _creatorPanel.SuspendLayout();
            try
            {
                _creatorPanel.Controls.Clear();
                AddCreatorCard("All Packs", _items);
                foreach (string creator in _items.Select(item => item.Owner)
                             .Where(value => !string.IsNullOrWhiteSpace(value))
                             .Distinct(StringComparer.OrdinalIgnoreCase)
                             .OrderBy(value => value))
                {
                    AddCreatorCard(creator, _items.Where(item => string.Equals(item.Owner, creator, StringComparison.OrdinalIgnoreCase)).ToList());
                }
            }
            finally
            {
                _creatorPanel.ResumeLayout();
                ResizeCreatorCards();
            }
        }

        private void AddCreatorCard(string creator, IReadOnlyCollection<AssetPackLibraryItem> items)
        {
            int installed = items.Count(item => item.Status != AssetPackLibraryStatus.Missing);
            int missing = items.Count(item => item.Status == AssetPackLibraryStatus.Missing);
            int updates = items.Count(item => item.Status == AssetPackLibraryStatus.UpdateAvailable);

            var card = new Guna2Panel
            {
                Name = "CreatorCard",
                Size = new Size(Math.Max(180, _creatorPanel.ClientSize.Width - 2), 62),
                Margin = new Padding(0, 0, 0, 6),
                Padding = new Padding(6, 5, 6, 5),
                FillColor = string.Equals(creator, _creator, StringComparison.OrdinalIgnoreCase) ? ModernTheme.AccentSubtle : ModernTheme.SurfaceRaised,
                BorderColor = string.Equals(creator, _creator, StringComparison.OrdinalIgnoreCase) ? ModernTheme.Accent : ModernTheme.Border,
                BorderThickness = 1,
                BorderRadius = 10,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand,
                Tag = creator
            };

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.Transparent,
                Margin = new Padding(0),
                Padding = new Padding(0),
                Cursor = Cursors.Hand
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 34));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            var icon = new Guna2PictureBox
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0),
                BackColor = Color.Transparent,
                SizeMode = PictureBoxSizeMode.CenterImage,
                Image = UiIcons.Get(creator == "All Packs" ? UiIcon.Grid : UiIcon.Package, Color.FromArgb(177, 205, 244), 25),
                Cursor = Cursors.Hand
            };

            var text = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                BackColor = Color.Transparent,
                Margin = new Padding(0),
                Padding = new Padding(0, 3, 0, 0),
                Cursor = Cursors.Hand
            };
            text.RowStyles.Add(new RowStyle(SizeType.Absolute, 18));
            text.RowStyles.Add(new RowStyle(SizeType.Absolute, 15));
            text.RowStyles.Add(new RowStyle(SizeType.Absolute, 15));

            var title = Label(creator, 9.4F, FontStyle.Bold, ModernTheme.TextPrimary);
            title.Dock = DockStyle.Fill;
            title.AutoSize = false;
            title.TextAlign = ContentAlignment.MiddleLeft;
            title.Margin = new Padding(0);
            title.Cursor = Cursors.Hand;

            var stats1 = Label($"{items.Count} total  •  {installed} installed", 8.2F, FontStyle.Regular, ModernTheme.TextSecondary);
            stats1.Name = "CreatorStats1";
            stats1.Dock = DockStyle.Fill;
            stats1.AutoSize = false;
            stats1.TextAlign = ContentAlignment.MiddleLeft;
            stats1.Margin = new Padding(0);
            stats1.Cursor = Cursors.Hand;

            var stats2 = Label($"{missing} missing  •  {updates} updates", 8.2F, FontStyle.Regular, ModernTheme.TextSecondary);
            stats2.Name = "CreatorStats2";
            stats2.Dock = DockStyle.Fill;
            stats2.AutoSize = false;
            stats2.TextAlign = ContentAlignment.MiddleLeft;
            stats2.Margin = new Padding(0);
            stats2.Cursor = Cursors.Hand;

            text.Controls.Add(title, 0, 0);
            text.Controls.Add(stats1, 0, 1);
            text.Controls.Add(stats2, 0, 2);
            layout.Controls.Add(icon, 0, 0);
            layout.Controls.Add(text, 1, 0);
            card.Controls.Add(layout);

            void SelectCreator()
            {
                _creator = creator;
                foreach (Control control in _creatorPanel.Controls)
                {
                    if (control is Guna2Panel creatorCard && creatorCard.Name == "CreatorCard")
                    {
                        bool active = string.Equals(creatorCard.Tag as string, _creator, StringComparison.OrdinalIgnoreCase);
                        creatorCard.FillColor = active ? ModernTheme.AccentSubtle : ModernTheme.SurfaceRaised;
                        creatorCard.BorderColor = active ? ModernTheme.Accent : ModernTheme.Border;
                    }
                }
                ApplyFilters();
            }

            card.Click += (_, _) => SelectCreator();
            layout.Click += (_, _) => SelectCreator();
            icon.Click += (_, _) => SelectCreator();
            text.Click += (_, _) => SelectCreator();
            title.Click += (_, _) => SelectCreator();
            stats1.Click += (_, _) => SelectCreator();
            stats2.Click += (_, _) => SelectCreator();
            _creatorPanel.Controls.Add(card);
        }

        private void ResizeCreatorCards()
        {
            int width = Math.Max(180, _creatorPanel.ClientSize.Width - (_creatorPanel.VerticalScroll.Visible ? SystemInformation.VerticalScrollBarWidth : 0) - 2);
            foreach (Control control in _creatorPanel.Controls)
                if (control is Guna2Panel card && card.Name == "CreatorCard")
                    card.Width = width;
        }

        private void ApplyFilters()
        {
            string search = _search.Text.Trim();
            string status = _statusFilter.SelectedItem?.ToString() ?? "All Statuses";
            string variant = _variantFilter.SelectedItem?.ToString() ?? "All Variants";

            IEnumerable<AssetPackLibraryItem> query = _items;
            if (_creator != "All Packs")
                query = query.Where(item => string.Equals(item.Owner, _creator, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(item => item.Owner.Contains(search, StringComparison.OrdinalIgnoreCase)
                    || item.Repository.Contains(search, StringComparison.OrdinalIgnoreCase)
                    || item.Variant.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            if (status != "All Statuses")
            {
                query = status switch
                {
                    "Installed" => query.Where(item => item.Status == AssetPackLibraryStatus.Installed),
                    "Missing" => query.Where(item => item.Status == AssetPackLibraryStatus.Missing),
                    "Update Available" => query.Where(item => item.Status == AssetPackLibraryStatus.UpdateAvailable),
                    _ => query
                };
            }

            if (variant != "All Variants")
                query = query.Where(item => string.Equals(item.Variant, variant, StringComparison.OrdinalIgnoreCase));

            if (_installedOnly.Checked)
                query = query.Where(item => item.Status != AssetPackLibraryStatus.Missing);

            query = _sortFilter.SelectedIndex == 1
                ? query.OrderByDescending(item => item.Owner).ThenByDescending(item => item.Variant)
                : query.OrderBy(item => item.Owner).ThenBy(item => item.Variant);

            _suppressSelectionChange = true;
            try
            {
                _grid.Rows.Clear();
                foreach (AssetPackLibraryItem item in query)
                {
                    string statusText = item.Status switch
                    {
                        AssetPackLibraryStatus.Installed => "Installed",
                        AssetPackLibraryStatus.UpdateAvailable => "Update Available",
                        _ => "Not Installed"
                    };

                    int rowIndex = _grid.Rows.Add(
                        item.Selected,
                        item.Owner,
                        item.Variant,
                        item.Repository,
                        string.IsNullOrWhiteSpace(item.InstalledVersion) ? "—" : item.InstalledVersion,
                        string.IsNullOrWhiteSpace(item.LatestVersion) ? "—" : item.LatestVersion,
                        statusText,
                        item.InstalledFiles == 0 ? "—" : item.InstalledFiles.ToString());
                    _grid.Rows[rowIndex].Tag = item;
                    if (!string.IsNullOrWhiteSpace(item.Owner))
                    {
                        _grid.Rows[rowIndex].Cells["Creator"].ToolTipText =
                            $"Open https://github.com/{item.Owner}";
                    }
                    if (!string.IsNullOrWhiteSpace(item.Owner) && !string.IsNullOrWhiteSpace(item.Repository))
                    {
                        _grid.Rows[rowIndex].Cells["Repository"].ToolTipText =
                            $"Open https://github.com/{item.Owner}/{item.Repository}";
                    }
                }
            }
            finally
            {
                _suppressSelectionChange = false;
            }

            _sectionTitle.Text = _creator;
            UpdateSelectionState();
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

            grid.Columns.Add(new DataGridViewCheckBoxColumn { Name = "Selected", HeaderText = "Select", FillWeight = 9, MinimumWidth = 64, SortMode = DataGridViewColumnSortMode.NotSortable });
            grid.Columns.Add(new DataGridViewLinkColumn
            {
                Name = "Creator",
                HeaderText = "Creator",
                FillWeight = 18,
                ReadOnly = true,
                LinkColor = Color.FromArgb(115, 178, 255),
                ActiveLinkColor = ModernTheme.AccentHover,
                VisitedLinkColor = Color.FromArgb(115, 178, 255),
                TrackVisitedState = false,
                LinkBehavior = LinkBehavior.HoverUnderline
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Variant", HeaderText = "Variant", FillWeight = 17, ReadOnly = true });
            grid.Columns.Add(new DataGridViewLinkColumn
            {
                Name = "Repository",
                HeaderText = "Repository",
                FillWeight = 27,
                ReadOnly = true,
                LinkColor = Color.FromArgb(115, 178, 255),
                ActiveLinkColor = ModernTheme.AccentHover,
                VisitedLinkColor = Color.FromArgb(115, 178, 255),
                TrackVisitedState = false,
                LinkBehavior = LinkBehavior.HoverUnderline
            });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Installed", HeaderText = "Installed", FillWeight = 13, ReadOnly = true });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Latest", HeaderText = "Latest", FillWeight = 13, ReadOnly = true });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Status", HeaderText = "Status", FillWeight = 19, MinimumWidth = 112, ReadOnly = true });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Files", HeaderText = "Files", FillWeight = 9, MinimumWidth = 62, ReadOnly = true });

            ModernTheme.StyleDataGrid(grid);
            grid.Columns["Selected"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            grid.Columns["Selected"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            grid.RowTemplate.Height = 35;

            grid.CurrentCellDirtyStateChanged += (_, _) =>
            {
                if (grid.IsCurrentCellDirty)
                    grid.CommitEdit(DataGridViewDataErrorContexts.Commit);
            };
            grid.CellValueChanged += (_, e) =>
            {
                if (e.RowIndex < 0 || e.ColumnIndex != 0 || _suppressSelectionChange) return;
                if (grid.Rows[e.RowIndex].Tag is not AssetPackLibraryItem item) return;
                item.Selected = grid.Rows[e.RowIndex].Cells[0].Value is bool value && value;
                UpdateSelectionState();
            };
            grid.CellContentClick += (_, e) =>
            {
                if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
                if (grid.Rows[e.RowIndex].Tag is not AssetPackLibraryItem item) return;

                string columnName = grid.Columns[e.ColumnIndex].Name;
                string url;
                if (string.Equals(columnName, "Creator", StringComparison.Ordinal))
                {
                    if (string.IsNullOrWhiteSpace(item.Owner)) return;
                    url = $"https://github.com/{item.Owner}";
                }
                else if (string.Equals(columnName, "Repository", StringComparison.Ordinal))
                {
                    if (string.IsNullOrWhiteSpace(item.Owner) || string.IsNullOrWhiteSpace(item.Repository)) return;
                    url = $"https://github.com/{item.Owner}/{item.Repository}";
                }
                else
                {
                    return;
                }

                try
                {
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }
                catch (Exception ex)
                {
                    ModernDialog.ShowError(FindForm(), "Unable to open GitHub", ex.Message);
                }
            };
            grid.CellMouseEnter += (_, e) =>
            {
                if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
                string columnName = grid.Columns[e.ColumnIndex].Name;
                grid.Cursor = string.Equals(columnName, "Creator", StringComparison.Ordinal)
                    || string.Equals(columnName, "Repository", StringComparison.Ordinal)
                        ? Cursors.Hand
                        : Cursors.Default;

                QueueArtworkPreview(e.RowIndex, e.ColumnIndex);
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
            _artPreviewRequest++;
            _artPreviewRow = rowIndex;
            _artPreviewColumn = columnIndex;
            _artPreviewTimer.Start();
        }

        private async Task ShowQueuedArtworkPreviewAsync()
        {
            if (_artPreviewRow < 0 || _artPreviewColumn < 0
                || _artPreviewRow >= _grid.Rows.Count || _artPreviewColumn >= _grid.Columns.Count)
                return;

            int rowIndex = _artPreviewRow;
            int request = _artPreviewRequest;

            Point client = _grid.PointToClient(Cursor.Position);
            DataGridView.HitTestInfo hit = _grid.HitTest(client.X, client.Y);
            if (hit.RowIndex != rowIndex)
                return;
            if (_grid.Rows[rowIndex].Tag is not AssetPackLibraryItem item)
                return;

            string owner = item.Owner;
            string repository = item.Repository;
            string variant = item.Pack.variant ?? string.Empty;
            string displayVariant = item.Variant;

            IReadOnlyList<Bitmap> images = await Task.Run(() =>
                PlatformArtworkPreviewService.TryLoadRandomPackImages(owner, repository, variant, 3));
            if (images.Count == 0 || IsDisposed || request != _artPreviewRequest || rowIndex != _artPreviewRow)
                return;

            client = _grid.PointToClient(Cursor.Position);
            hit = _grid.HitTest(client.X, client.Y);
            if (hit.RowIndex != rowIndex)
                return;

            Rectangle row = _grid.GetRowDisplayRectangle(rowIndex, false);
            Point screenPoint = _grid.PointToScreen(row.Location);
            Rectangle anchor = new(screenPoint, row.Size);

            _artPreviewPopup ??= new AssetPackArtworkPreviewPopup();
            _artPreviewPopup.ShowPreview(_grid, owner, repository, displayVariant, images, anchor);
        }

        private void HideArtworkPreview()
        {
            _artPreviewTimer.Stop();
            _artPreviewRequest++;
            _artPreviewRow = -1;
            _artPreviewColumn = -1;
            if (_artPreviewPopup != null && !_artPreviewPopup.IsDisposed)
                _artPreviewPopup.Hide();
        }

        private void SetVisibleSelection(bool selected, bool clearAll)
        {
            if (clearAll)
            {
                foreach (AssetPackLibraryItem item in _items)
                    item.Selected = false;
            }

            _suppressSelectionChange = true;
            try
            {
                foreach (DataGridViewRow row in _grid.Rows)
                {
                    if (row.Tag is not AssetPackLibraryItem item) continue;
                    item.Selected = selected;
                    row.Cells[0].Value = selected;
                }
            }
            finally
            {
                _suppressSelectionChange = false;
            }
            UpdateSelectionState();
        }

        private void UpdateSelectionState()
        {
            int selected = _items.Count(item => item.Selected);
            int visible = _grid.Rows.Count;
            int missing = _items.Count(item => item.Selected && item.Status == AssetPackLibraryStatus.Missing);
            int updates = _items.Count(item => item.Selected && item.Status == AssetPackLibraryStatus.UpdateAvailable);
            int installed = _items.Count(item => item.Selected && item.Status != AssetPackLibraryStatus.Missing);

            _selectionLabel.Text = $"{selected} selected  •  {visible} shown  •  versions tracked from GitHub releases";
            _installSelected.Enabled = missing > 0;
            _updateSelected.Enabled = updates > 0;
            _uninstallSelected.Enabled = installed > 0;
        }

        private async Task InstallSelectedAsync()
        {
            List<AssetPackLibraryItem> items = _items.Where(item => item.Selected && item.Status == AssetPackLibraryStatus.Missing).ToList();
            if (items.Count == 0)
            {
                ModernDialog.ShowInfo(FindForm(), "Pocket Updater", "Select one or more missing image packs first.");
                return;
            }

            await ProcessInstallAsync(items, "Install Image Packs", "Install");
        }

        private async Task UpdateSelectedAsync()
        {
            List<AssetPackLibraryItem> items = _items.Where(item => item.Selected && item.Status == AssetPackLibraryStatus.UpdateAvailable).ToList();
            if (items.Count == 0)
            {
                ModernDialog.ShowInfo(FindForm(), "Pocket Updater", "Select one or more image packs with an available update first.");
                return;
            }

            await ProcessInstallAsync(items, "Update Image Packs", "Update");
        }

        private async Task ProcessInstallAsync(List<AssetPackLibraryItem> items, string title, string verb)
        {
            if (!ModernDialog.ShowConfirm(FindForm(), title, $"{verb} {items.Count} selected image pack(s)?"))
                return;

            var progressItems = items
                .Select(item => new AssetPackProgressItem(
                    item.Owner,
                    item.Repository,
                    string.Equals(item.Variant, "Default", StringComparison.OrdinalIgnoreCase) ? null : item.Pack.variant,
                    item.Variant))
                .ToList();

            SetActionsEnabled(false);
            try
            {
                using var progress = new ModernAssetPackProgressForm(
                    PocketTargetContext.SelectedPath,
                    progressItems,
                    verb);
                progress.ShowDialog(FindForm());
                await RefreshAsync();
                LibraryChangeNotifier.Notify();
            }
            finally
            {
                SetActionsEnabled(true);
                UpdateSelectionState();
            }
        }

        private async Task UninstallSelectedAsync()
        {
            List<AssetPackLibraryItem> items = _items.Where(item => item.Selected && item.Status != AssetPackLibraryStatus.Missing).ToList();
            if (items.Count == 0)
            {
                ModernDialog.ShowInfo(FindForm(), "Pocket Updater", "Select one or more installed image packs first.");
                return;
            }

            if (!ModernDialog.ShowConfirm(FindForm(), "Uninstall Image Packs",
                    $"Uninstall {items.Count} selected image pack(s)?\n\nTracked artwork files belonging only to those packs will be deleted from Platforms/_images."))
                return;

            SetActionsEnabled(false);
            try
            {
                string target = PocketTargetContext.SelectedPath;
                int removed = await Task.Run(() =>
                {
                    ServiceHelper.Initialize(target, Directory.GetCurrentDirectory(), forceReload: true);
                    int count = 0;
                    foreach (AssetPackLibraryItem item in items)
                    {
                        if (ServiceHelper.PlatformImagePacksService.UninstallTracked(item.Owner, item.Repository,
                                string.Equals(item.Variant, "Default", StringComparison.OrdinalIgnoreCase) ? null : item.Pack.variant))
                            count++;
                    }
                    return count;
                });

                ModernDialog.ShowInfo(FindForm(), "Pocket Updater", $"Uninstalled {removed} image pack(s) and removed their tracked artwork files.");
                await RefreshAsync();
                LibraryChangeNotifier.Notify();
            }
            catch (Exception ex)
            {
                ModernDialog.ShowError(FindForm(), "Unable to uninstall image packs", ex.Message);
                await RefreshAsync();
            }
            finally
            {
                SetActionsEnabled(true);
                UpdateSelectionState();
            }
        }

        private void SetActionsEnabled(bool enabled)
        {
            _refreshButton.Enabled = enabled;
            if (!enabled)
            {
                _installSelected.Enabled = false;
                _updateSelected.Enabled = false;
                _uninstallSelected.Enabled = false;
                _selectionLabel.Text = "Working… please keep Pocket Updater open.";
            }
        }

        private static bool IsUpdateAvailable(string installedVersion, string latestVersion)
        {
            if (string.IsNullOrWhiteSpace(installedVersion) || string.IsNullOrWhiteSpace(latestVersion))
                return false;
            if (string.Equals(installedVersion.Trim(), latestVersion.Trim(), StringComparison.OrdinalIgnoreCase))
                return false;

            try
            {
                string installedSemver = SemverUtil.FindSemver(installedVersion);
                string latestSemver = SemverUtil.FindSemver(latestVersion);
                if (!string.IsNullOrWhiteSpace(installedSemver) && !string.IsNullOrWhiteSpace(latestSemver))
                    return SemverUtil.SemverCompare(latestSemver, installedSemver);
            }
            catch { }

            return !string.Equals(installedVersion.Trim(), latestVersion.Trim(), StringComparison.OrdinalIgnoreCase);
        }

        private static string NormalizeVariant(string variant) => string.IsNullOrWhiteSpace(variant) ? string.Empty : variant.Trim();

        private static Guna2Panel SummaryCard(string text, UiIcon icon, out Label value, Color valueColor, Padding margin)
        {

            var card = Card();
            card.Margin = margin;

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                BackColor = Color.Transparent,
                Padding = new Padding(14, 12, 16, 12),
                Margin = new Padding(0)
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 52));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 29));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            var iconHost = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Margin = new Padding(0)
            };
            var tile = IconTile(icon, valueColor, new Point(0, 6), 38);
            iconHost.Controls.Add(tile);
            layout.Controls.Add(iconHost, 0, 0);
            layout.SetRowSpan(iconHost, 2);

            var caption = Label(text, 11F, FontStyle.Bold, ModernTheme.TextPrimary);
            caption.Dock = DockStyle.Fill;
            caption.AutoSize = false;
            caption.Margin = new Padding(0);
            caption.TextAlign = ContentAlignment.BottomLeft;

            value = Label("—", 24F, FontStyle.Bold, valueColor);
            value.Dock = DockStyle.Fill;
            value.AutoSize = false;
            value.Margin = new Padding(0);
            value.TextAlign = ContentAlignment.TopLeft;

            layout.Controls.Add(caption, 1, 0);
            layout.Controls.Add(value, 1, 1);
            card.Controls.Add(layout);
            return card;
        }

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

        private static void PrepareActionButton(Guna2Button button)
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

        private static Guna2Button Button(string text, int width, bool primary)
        {
            var button = new Guna2Button
            {
                Text = text,
                Size = new Size(width, 36),
                BorderRadius = 9,
                FillColor = primary ? ModernTheme.Accent : ModernTheme.SurfaceRaised,
                ForeColor = ModernTheme.TextPrimary,
                BorderThickness = primary ? 0 : 1,
                BorderColor = ModernTheme.BorderStrong,
                Font = ModernTheme.BodyBoldFont,
                Cursor = Cursors.Hand
            };
            button.HoverState.FillColor = primary ? ModernTheme.AccentHover : ModernTheme.SurfaceHover;
            button.DisabledState.FillColor = Color.FromArgb(53, 64, 82);
            button.DisabledState.ForeColor = Color.FromArgb(123, 137, 158);
            return button;
        }
    }
}
