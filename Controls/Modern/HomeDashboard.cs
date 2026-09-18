using System.Net;
using System.Text.RegularExpressions;
using Guna.UI2.WinForms;
using Pannella.Models.Settings;
using Pannella.Services;
using Pocket_Updater.Forms.Update_Progress;
using Pocket_Updater.UI;

namespace Pocket_Updater.Controls.Modern
{
    internal sealed class HomeDashboard : UserControl
    {
        private readonly Label _scanLabel;
        private readonly Label _firmwareStatus;
        private readonly FirmwareReleaseNotesPopup _firmwarePopup;
        private readonly System.Windows.Forms.Timer _firmwarePopupHideTimer;
        private string _firmwareReleaseTitle = "Pocket Firmware Release Notes";
        private string _firmwareReleaseNotes = "Scan your library to load the latest Pocket firmware release notes.";
        private string _firmwareReleaseUrl = "https://www.analogue.co/support/pocket/firmware";
        private readonly Label _coverageValue;
        private readonly Guna2ProgressBar _coverageBar;
        private readonly Label _coresInstalled;
        private readonly Label _coresTotal;
        private readonly Label _coresMissing;
        private readonly Label _assetsInstalled;
        private readonly Label _assetsRequired;
        private readonly Label _assetsMissing;
        private readonly Label _packsInstalled;
        private readonly Label _packsTotal;
        private readonly Label _packsMissing;
        private readonly Label _attentionText;
        private readonly Guna2Button _scanButton;
        private readonly Dictionary<string, Guna2ToggleSwitch> _toggles = new(StringComparer.OrdinalIgnoreCase);
        private readonly SettingsService _settingsService;
        private readonly System.Windows.Forms.Timer _autoSaveTimer;
        private readonly System.Windows.Forms.Timer _savedStatusTimer;
        private readonly Guna2TextBox _alternateUrl;
        private readonly Guna2Panel _alternateUrlRow;
        private readonly TableLayoutPanel _settingsBody;
        private readonly Label _settingsStatus;
        private bool _loadingSettings = true;

        public event EventHandler? CoresRequested;
        public event EventHandler? AssetsRequested;
        public event EventHandler? AssetPacksRequested;

        public HomeDashboard()
        {
            BackColor = ModernTheme.AppBackground;
            Dock = DockStyle.Fill;
            Padding = new Padding(24, 22, 24, 18);
            AutoScroll = true;

            _settingsService = new SettingsService(Directory.GetCurrentDirectory());
            _firmwarePopup = new FirmwareReleaseNotesPopup();
            _firmwarePopupHideTimer = new System.Windows.Forms.Timer { Interval = 260 };
            _firmwarePopupHideTimer.Tick += (_, _) =>
            {
                _firmwarePopupHideTimer.Stop();
                _firmwarePopup.Hide();
            };
            _firmwarePopup.PreviewMouseEntered += (_, _) => _firmwarePopupHideTimer.Stop();
            _firmwarePopup.PreviewMouseLeft += (_, _) => StartFirmwarePopupHideTimer();
            _autoSaveTimer = new System.Windows.Forms.Timer { Interval = 650 };
            _autoSaveTimer.Tick += (_, _) =>
            {
                _autoSaveTimer.Stop();
                bool saved = SaveSettings(false);
                if (saved)
                {
                    ShowSavedStatus();
                }
                else
                {
                    _savedStatusTimer.Stop();
                    _settingsStatus.Text = "Save failed";
                    _settingsStatus.ForeColor = ModernTheme.Danger;
                }
            };

            _savedStatusTimer = new System.Windows.Forms.Timer { Interval = 3000 };
            _savedStatusTimer.Tick += (_, _) =>
            {
                _savedStatusTimer.Stop();
                ShowAutoSaveIdleStatus();
            };

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                ColumnCount = 1,
                RowCount = 4,
                Padding = Padding.Empty,
                Margin = Padding.Empty
            };
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 76));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 154));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 220));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            Controls.Add(root);

            var heading = new Guna2Panel { Dock = DockStyle.Fill, FillColor = Color.Transparent, BackColor = Color.Transparent, Margin = Padding.Empty };
            var headingIcon = IconTile(UiIcon.Home, ModernTheme.AccentHover, new Point(0, 1), 48);
            var title = NewLabel("Library Overview", 24, FontStyle.Bold, ModernTheme.TextPrimary);
            title.Location = new Point(62, 0);
            title.AutoSize = true;
            var subtitle = NewLabel("A quick look at what is installed, what is available, and what still needs attention.", 10, FontStyle.Regular, ModernTheme.TextSecondary);
            subtitle.Location = new Point(64, 41);
            subtitle.AutoSize = true;
            heading.Controls.Add(headingIcon);
            heading.Controls.Add(title);
            heading.Controls.Add(subtitle);
            root.Controls.Add(heading, 0, 0);

            var cards = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1,
                Padding = new Padding(0, 0, 0, 14),
                BackColor = Color.Transparent,
                Margin = Padding.Empty
            };
            cards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.333F));
            cards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.333F));
            cards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.334F));

            var coreCard = CreateMetricCard("Cores", UiIcon.Core, Color.FromArgb(72, 166, 255),
                out _coresInstalled, out _coresTotal, out _coresMissing, "Installed", "Total", "Missing");
            var assetCard = CreateMetricCard("ROMs / BIOS", UiIcon.RomBios, Color.FromArgb(165, 112, 255),
                out _assetsInstalled, out _assetsRequired, out _assetsMissing, "Present", "Required", "Missing");
            var packCard = CreateMetricCard("Asset Image Packs", UiIcon.AssetPack, Color.FromArgb(81, 194, 255),
                out _packsInstalled, out _packsTotal, out _packsMissing, "Installed", "Total", "Missing");

            coreCard.Margin = new Padding(0, 0, 8, 0);
            assetCard.Margin = new Padding(8, 0, 8, 0);
            packCard.Margin = new Padding(8, 0, 0, 0);

            MakeCardClickable(coreCard, () => CoresRequested?.Invoke(this, EventArgs.Empty));
            MakeCardClickable(assetCard, () => AssetsRequested?.Invoke(this, EventArgs.Empty));
            MakeCardClickable(packCard, () => AssetPacksRequested?.Invoke(this, EventArgs.Empty));

            cards.Controls.Add(coreCard, 0, 0);
            cards.Controls.Add(assetCard, 1, 0);
            cards.Controls.Add(packCard, 2, 0);
            root.Controls.Add(cards, 0, 1);

            var middle = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 0, 0, 14),
                Margin = Padding.Empty
            };
            middle.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 31));
            middle.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34));
            middle.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));

            var quick = CreateCard();
            quick.Margin = new Padding(0, 0, 8, 0);
            AddCardHeading(quick, "Quick Actions", UiIcon.Bolt, ModernTheme.AccentHover);
            _scanButton = NewButton("Scan Library", UiIcon.Search, 18, 64, 230, true);
            _scanButton.Click += async (_, _) => await RefreshAsync();
            var update = NewButton("Start Update", UiIcon.Update, 18, 112, 230, false);
            update.Click += async (_, _) => await StartUpdateAsync();
            var missing = NewButton("Review Missing", UiIcon.List, 18, 160, 230, false);
            missing.Click += (_, _) => AssetsRequested?.Invoke(this, EventArgs.Empty);
            quick.Controls.Add(_scanButton);
            quick.Controls.Add(update);
            quick.Controls.Add(missing);

            var system = CreateCard();
            system.Margin = new Padding(8, 0, 8, 0);
            AddCardHeading(system, "Library Status", UiIcon.Shield, ModernTheme.Success);
            _scanLabel = NewLabel("Not scanned yet", 9.3F, FontStyle.Regular, ModernTheme.TextSecondary);
            _scanLabel.Location = new Point(18, 68);
            _scanLabel.Size = new Size(360, 36);
            _scanLabel.AutoEllipsis = true;

            _firmwareStatus = NewLabel("Pocket firmware: not checked", 9.2F, FontStyle.Bold, ModernTheme.TextMuted);
            _firmwareStatus.Location = new Point(18, 108);
            _firmwareStatus.Size = new Size(360, 22);
            _firmwareStatus.AutoEllipsis = true;
            _firmwareStatus.Cursor = Cursors.Help;
            _firmwareStatus.MouseEnter += (_, _) => ShowFirmwarePopup();
            _firmwareStatus.MouseLeave += (_, _) => StartFirmwarePopupHideTimer();

            _coverageValue = NewLabel("0% complete", 10.5F, FontStyle.Bold, ModernTheme.TextPrimary);
            _coverageValue.Location = new Point(18, 136);
            _coverageValue.AutoSize = true;
            _coverageBar = new Guna2ProgressBar
            {
                Location = new Point(18, 164),
                Height = 12,
                Width = 300,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BorderRadius = 6,
                FillColor = ModernTheme.SurfaceRaised,
                ProgressColor = ModernTheme.Success,
                ProgressColor2 = ModernTheme.Accent,
                Value = 0
            };
            system.Controls.Add(_scanLabel);
            system.Controls.Add(_firmwareStatus);
            system.Controls.Add(_coverageValue);
            system.Controls.Add(_coverageBar);
            system.Resize += (_, _) => _coverageBar.Width = Math.Max(180, system.ClientSize.Width - 36);

            var attention = CreateCard();
            attention.Margin = new Padding(8, 0, 0, 0);
            AddCardHeading(attention, "Needs Attention", UiIcon.Missing, ModernTheme.Danger);
            _attentionText = NewLabel("Scan your library to see missing items and available updates.", 9.3F, FontStyle.Regular, ModernTheme.TextSecondary);
            _attentionText.Location = new Point(18, 68);
            _attentionText.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            _attentionText.Size = new Size(350, 150);
            _attentionText.AutoEllipsis = true;
            attention.Controls.Add(_attentionText);
            attention.Resize += (_, _) => _attentionText.Size = new Size(Math.Max(120, attention.ClientSize.Width - 36), Math.Max(90, attention.ClientSize.Height - 82));

            middle.Controls.Add(quick, 0, 0);
            middle.Controls.Add(system, 1, 0);
            middle.Controls.Add(attention, 2, 0);
            root.Controls.Add(middle, 0, 2);

            var settingsCard = CreateCard();
            settingsCard.Margin = Padding.Empty;

            var settingsHeader = new Guna2Panel
            {
                Dock = DockStyle.Top,
                Height = 58,
                FillColor = Color.Transparent,
                BackColor = Color.Transparent,
                Padding = Padding.Empty
            };
            var settingsIcon = IconTile(UiIcon.Settings, Color.FromArgb(112, 173, 255), new Point(16, 8), 42);
            var settingsTitle = NewLabel("Update Preferences", 14.5F, FontStyle.Bold, ModernTheme.TextPrimary);
            settingsTitle.Location = new Point(70, 6);
            settingsTitle.AutoSize = true;
            var settingsSubtitle = NewLabel("Choose what happens when Pocket Updater scans and updates your library.", 8.8F, FontStyle.Regular, ModernTheme.TextSecondary);
            settingsSubtitle.Location = new Point(71, 31);
            settingsSubtitle.AutoSize = true;

            _settingsStatus = NewLabel("Changes save automatically", 8.5F, FontStyle.Regular, ModernTheme.TextMuted);
            _settingsStatus.AutoSize = true;
            _settingsStatus.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            settingsHeader.Controls.Add(settingsIcon);
            settingsHeader.Controls.Add(settingsTitle);
            settingsHeader.Controls.Add(settingsSubtitle);
            settingsHeader.Controls.Add(_settingsStatus);
            settingsCard.Controls.Add(settingsHeader);

            _settingsBody = new TableLayoutPanel
            {
                Location = new Point(16, 60),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                ColumnCount = 2,
                RowCount = 2,
                BackColor = Color.Transparent,
                Padding = Padding.Empty,
                Margin = Padding.Empty
            };
            _settingsBody.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            _settingsBody.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            _settingsBody.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            _settingsBody.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));

            var downloadsSection = CreateSettingsSection();
            downloadsSection.Margin = new Padding(0, 0, 7, 8);
            var maintenanceSection = CreateSettingsSection();
            maintenanceSection.Margin = new Padding(7, 0, 0, 8);

            var downloadsOptions = CreateSettingsOptionsTable();
            var maintenanceOptions = CreateSettingsOptionsTable();
            downloadsSection.Controls.Add(downloadsOptions);
            maintenanceSection.Controls.Add(maintenanceOptions);

            string[,] leftDefs =
            {
                { "download_firmware", "Download Pocket Firmware", "Get the latest official Pocket firmware when needed." },
                { "download_assets", "Download ROMs / BIOS", "Fill required ROM and BIOS files for enabled cores." },
                { "preserve_platforms_folder", "Preserve Platforms", "Keep existing platform metadata while updating." },
                { "delete_skipped_cores", "Delete Skipped Cores", "Remove installed cores that you have disabled." },
                { "build_instance_jsons", "Generate Instance JSONs", "Create supported game instance files automatically." }
            };
            string[,] rightDefs =
            {
                { "fix_jt_names", "Fix Jotego Core Names", "Normalize Jotego core names after updates." },
                { "skip_alternative_assets", "Skip Alternative Arcade Files", "Skip alternate arcade ROM variants." },
                { "crc_check", "CRC Checks", "Verify ROM / BIOS file integrity after download." },
                { "backup_saves", "Backup Saves", "Back up Saves and Memories before updating." },
                { "use_custom_archive", "Use Alternate Download Location", "Use your configured custom ROM / BIOS source." }
            };

            for (int i = 0; i < 5; i++)
            {
                downloadsOptions.Controls.Add(CreateSettingRow(leftDefs[i, 0], leftDefs[i, 1], leftDefs[i, 2]), 0, i);
                maintenanceOptions.Controls.Add(CreateSettingRow(rightDefs[i, 0], rightDefs[i, 1], rightDefs[i, 2]), 0, i);
            }

            _alternateUrlRow = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                FillColor = Color.Transparent,
                BackColor = Color.Transparent,
                BorderThickness = 0,
                Margin = Padding.Empty
            };
            _settingsBody.SetColumnSpan(_alternateUrlRow, 2);

            var sourceTitle = NewLabel("Alternate Download URL", 9.0F, FontStyle.Bold, ModernTheme.TextPrimary);
            sourceTitle.AutoSize = true;

            _alternateUrl = new Guna2TextBox
            {
                Height = 38,
                BorderRadius = 9,
                FillColor = ModernTheme.SurfaceRaised,
                BorderColor = ModernTheme.BorderStrong,
                ForeColor = ModernTheme.TextPrimary,
                PlaceholderText = "https://updater.retrodriven.com",
                Font = ModernTheme.BodyFont,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            _alternateUrl.TextChanged += (_, _) => QueueAutoSave();

            _alternateUrlRow.Controls.Add(sourceTitle);
            _alternateUrlRow.Controls.Add(_alternateUrl);

            _settingsBody.Controls.Add(downloadsSection, 0, 0);
            _settingsBody.Controls.Add(maintenanceSection, 1, 0);
            _settingsBody.Controls.Add(_alternateUrlRow, 0, 1);
            settingsCard.Controls.Add(_settingsBody);

            settingsCard.Resize += (_, _) =>
            {
                _settingsStatus.Location = new Point(
                    Math.Max(300, settingsHeader.ClientSize.Width - _settingsStatus.PreferredWidth - 18),
                    19);

                _settingsBody.Size = new Size(
                    Math.Max(620, settingsCard.ClientSize.Width - 32),
                    Math.Max(210, settingsCard.ClientSize.Height - 72));

                int right = Math.Max(420, _alternateUrlRow.ClientSize.Width - 2);
                _alternateUrl.Width = Math.Min(420, Math.Max(300, _alternateUrlRow.ClientSize.Width / 3));
                _alternateUrl.Location = new Point(
                    Math.Max(320, right - _alternateUrl.Width),
                    7);
                sourceTitle.Location = new Point(
                    Math.Max(4, _alternateUrl.Left - sourceTitle.PreferredWidth - 14),
                    _alternateUrl.Top + Math.Max(0, (_alternateUrl.Height - sourceTitle.PreferredHeight) / 2));
            };
            root.Controls.Add(settingsCard, 0, 3);

            _toggles["use_custom_archive"].CheckedChanged += (_, _) => UpdateAlternateUrlState();
            LoadSettings();
            _loadingSettings = false;

            PocketTargetContext.TargetChanged += async (_, _) =>
            {
                if (Visible)
                    await RefreshAsync();
            };
            LibraryChangeNotifier.Changed += LibraryChangeNotifier_Changed;
            Disposed += (_, _) =>
            {
                LibraryChangeNotifier.Changed -= LibraryChangeNotifier_Changed;
                _autoSaveTimer.Dispose();
                _savedStatusTimer.Dispose();
            };
        }

        private async void LibraryChangeNotifier_Changed(object? sender, EventArgs e)
        {
            if (IsDisposed || !IsHandleCreated) return;
            try
            {
                await RefreshAsync();
            }
            catch
            {

            }
        }

        public async Task RefreshAsync()
        {
            if (_scanButton.Enabled == false)
                return;

            _scanButton.Enabled = false;
            _scanButton.Text = "Scanning…";
            _scanLabel.Text = "Scanning selected target…";
            _firmwareStatus.Text = "Pocket firmware: checking latest release…";
            _firmwareStatus.ForeColor = ModernTheme.TextMuted;
            _firmwareReleaseTitle = "Pocket Firmware Release Notes";
            _firmwareReleaseNotes = "Checking the latest official Pocket firmware release notes…";
            _firmwareReleaseUrl = "https://www.analogue.co/support/pocket/firmware";
            _attentionText.Text = "Checking core inventory, firmware, and local ROM / BIOS requirements…";

            try
            {
                LibrarySnapshot snapshot = await LibraryInventoryService.LoadAsync(PocketTargetContext.SelectedPath, refreshFirmware: true);
                ApplySnapshot(snapshot);
            }
            catch (Exception ex)
            {
                _scanLabel.Text = "Scan failed";
                _firmwareStatus.Text = "Pocket firmware: status unavailable";
                _firmwareStatus.ForeColor = ModernTheme.TextMuted;
                _firmwareReleaseTitle = "Pocket Firmware Release Notes";
                _firmwareReleaseNotes = "Firmware release notes are unavailable because the firmware status check failed.";
                _firmwareReleaseUrl = "https://www.analogue.co/support/pocket/firmware";
                _attentionText.Text = "Unable to scan the selected target. " + ex.Message;
            }
            finally
            {
                _scanButton.Text = "Scan Library";
                _scanButton.Enabled = true;
            }
        }

        private void ApplySnapshot(LibrarySnapshot snapshot)
        {
            _coresInstalled.Text = snapshot.InstalledCores.ToString();
            _coresTotal.Text = snapshot.TotalCores.ToString();
            _coresMissing.Text = snapshot.MissingCores.ToString();
            _assetsInstalled.Text = snapshot.InstalledAssets.ToString();
            _assetsRequired.Text = snapshot.RequiredAssets.ToString();
            _assetsMissing.Text = snapshot.MissingAssets.ToString();
            _packsInstalled.Text = snapshot.InstalledImagePacks.ToString();
            _packsTotal.Text = snapshot.AvailableImagePacks.ToString();
            _packsMissing.Text = snapshot.MissingImagePacks.ToString();

            int denominator = snapshot.TotalCores + snapshot.RequiredAssets;
            int numerator = snapshot.InstalledCores + snapshot.InstalledAssets;
            int percent = denominator == 0 ? 0 : (int)Math.Round(numerator * 100d / denominator);
            _coverageBar.Value = Math.Max(0, Math.Min(100, percent));
            _coverageValue.Text = $"{percent}% complete";
            _scanLabel.Text = $"Last scan: {snapshot.ScannedAt:g}\n{snapshot.CoreUpdates} core update(s) available";

            if (snapshot.FirmwareStatusKnown)
            {
                string version = FormatFirmwareVersion(snapshot.LatestFirmwareVersion);
                if (snapshot.FirmwareUpdateAvailable)
                {
                    _firmwareStatus.Text = $"Pocket Firmware {version} Available";
                    _firmwareStatus.ForeColor = ModernTheme.Warning;
                }
                else
                {
                    _firmwareStatus.Text = $"Pocket Firmware {version} Ready";
                    _firmwareStatus.ForeColor = ModernTheme.Success;
                }

                _firmwareReleaseTitle = $"Pocket Firmware {version} Release Notes";
                _firmwareReleaseNotes = FormatFirmwareReleaseNotes(snapshot.LatestFirmwareReleaseNotesHtml);
                if (string.IsNullOrWhiteSpace(_firmwareReleaseNotes))
                    _firmwareReleaseNotes = "No release notes were provided for this firmware release.";
                _firmwareReleaseUrl = BuildFirmwareReleaseUrl(snapshot.LatestFirmwareVersion);
            }
            else
            {
                _firmwareStatus.Text = "Pocket firmware: status unavailable";
                _firmwareStatus.ForeColor = ModernTheme.TextMuted;
                _firmwareReleaseTitle = "Pocket Firmware Release Notes";
                _firmwareReleaseNotes = string.IsNullOrWhiteSpace(snapshot.FirmwareStatusError)
                    ? "Firmware release notes are currently unavailable."
                    : "Firmware release notes are unavailable. " + snapshot.FirmwareStatusError;
                _firmwareReleaseUrl = "https://www.analogue.co/support/pocket/firmware";
            }

            var attention = new List<string>();
            if (snapshot.FirmwareUpdateAvailable)
            {
                string version = FormatFirmwareVersion(snapshot.LatestFirmwareVersion);
                attention.Add($"● Pocket Firmware {version} Available");
            }
            foreach (var item in snapshot.Cores.Where(x => x.Status == CoreLibraryStatus.UpdateAvailable).Take(3))
                attention.Add($"● {item.Name} — {item.InstalledVersion} → {item.LatestVersion}");
            foreach (var item in snapshot.AssetSummaries.Where(x => x.Missing > 0).Take(Math.Max(0, 5 - attention.Count)))
                attention.Add($"● {item.Platform} — {item.Missing} ROM / BIOS file(s) missing");
            if (attention.Count == 0)
                attention.Add("✓ No missing core files or version updates were found in this scan.");
            _attentionText.Text = string.Join(Environment.NewLine + Environment.NewLine, attention);

        }

        private static string FormatFirmwareReleaseNotes(string? html)
        {
            if (string.IsNullOrWhiteSpace(html))
                return string.Empty;

            string text = html;
            text = Regex.Replace(text, @"<\s*br\s*/?\s*>", "\n", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"<\s*li(?:\s+[^>]*)?>", "• ", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"<\s*/\s*(li|p|div|h[1-6]|ul|ol)\s*>", "\n", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"<[^>]+>", string.Empty, RegexOptions.Singleline);
            text = WebUtility.HtmlDecode(text);
            text = Regex.Replace(text, @"[ \t]+\r?\n", "\n");
            text = Regex.Replace(text, @"\r?\n[ \t]+", "\n");
            text = Regex.Replace(text, @"\r?\n{3,}", "\n\n");
            text = text.Trim();

            const int maxLength = 1800;
            if (text.Length > maxLength)
                text = text[..maxLength].TrimEnd() + "…";

            return text;
        }

        private void ShowFirmwarePopup()
        {
            _firmwarePopupHideTimer.Stop();
            if (IsDisposed || !_firmwareStatus.Visible)
                return;

            _firmwarePopup.ShowNotes(
                _firmwareStatus,
                _firmwareReleaseTitle,
                _firmwareReleaseNotes,
                _firmwareReleaseUrl);
        }

        private void StartFirmwarePopupHideTimer()
        {
            _firmwarePopupHideTimer.Stop();
            _firmwarePopupHideTimer.Start();
        }

        private static string BuildFirmwareReleaseUrl(string? version)
        {
            if (string.IsNullOrWhiteSpace(version))
                return "https://www.analogue.co/support/pocket/firmware";

            string value = version.Trim();
            if (value.StartsWith("v", StringComparison.OrdinalIgnoreCase))
                value = value[1..];

            return "https://www.analogue.co/support/pocket/firmware/" + Uri.EscapeDataString(value);
        }

        private static string FormatFirmwareVersion(string? version)
        {
            if (string.IsNullOrWhiteSpace(version))
                return "Latest";

            string value = version.Trim();
            return value.StartsWith("v", StringComparison.OrdinalIgnoreCase) ? value : $"v{value}";
        }

        private static void MakeCardClickable(Guna2Panel card, Action action)
        {
            void Wire(Control control)
            {
                control.Cursor = Cursors.Hand;
                control.Click += (_, _) => action();
                foreach (Control child in control.Controls)
                    Wire(child);
            }

            Color normal = card.FillColor;
            Wire(card);
            card.MouseEnter += (_, _) => card.FillColor = ModernTheme.SurfaceHover;
            card.MouseLeave += (_, _) => card.FillColor = normal;
        }

        private static Guna2Panel CreateSettingsSection()
        {
            return new Guna2Panel
            {
                Dock = DockStyle.Fill,
                FillColor = ModernTheme.SurfaceRaised,
                BorderColor = ModernTheme.BorderStrong,
                BorderThickness = 1,
                BorderRadius = 10,
                Padding = Padding.Empty
            };
        }

        private static TableLayoutPanel CreateSettingsOptionsTable()
        {
            var table = new TableLayoutPanel
            {
                Location = new Point(14, 10),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                ColumnCount = 1,
                RowCount = 5,
                BackColor = Color.Transparent,
                Padding = Padding.Empty,
                Margin = Padding.Empty
            };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            for (int i = 0; i < 5; i++)
                table.RowStyles.Add(new RowStyle(SizeType.Percent, 20));

            table.ParentChanged += (_, _) =>
            {
                if (table.Parent == null) return;
                table.Size = new Size(
                    Math.Max(220, table.Parent.ClientSize.Width - 28),
                    Math.Max(150, table.Parent.ClientSize.Height - 20));
                table.Parent.Resize += (_, _) =>
                {
                    table.Size = new Size(
                        Math.Max(220, table.Parent.ClientSize.Width - 28),
                        Math.Max(150, table.Parent.ClientSize.Height - 20));
                };
            };

            return table;
        }

        private Control CreateSettingRow(string key, string title, string description)
        {
            var row = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                FillColor = Color.Transparent,
                BackColor = Color.Transparent,
                Margin = new Padding(0, 0, 20, 0)
            };

            var label = NewLabel(title, 9.7F, FontStyle.Bold, ModernTheme.TextPrimary);
            label.AutoSize = true;
            label.Location = new Point(0, 0);

            var detail = NewLabel(description, 8.2F, FontStyle.Regular, ModernTheme.TextSecondary);
            detail.AutoSize = false;
            detail.Location = new Point(0, 22);
            detail.Height = 18;
            detail.AutoEllipsis = true;
            detail.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            var toggle = new Guna2ToggleSwitch
            {
                Size = new Size(40, 22),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                CheckedState = { FillColor = ModernTheme.Accent, BorderColor = ModernTheme.Accent, InnerColor = Color.White },
                UncheckedState = { FillColor = Color.FromArgb(66, 79, 99), BorderColor = ModernTheme.BorderStrong, InnerColor = Color.White }
            };
            row.Resize += (_, _) =>
            {
                int naturalRight = row.ClientSize.Width - toggle.Width - 4;
                int closeToText = Math.Min(naturalRight, 400);
                toggle.Location = new Point(Math.Max(250, closeToText), 10);
                detail.Width = Math.Max(150, toggle.Left - 14);
            };
            toggle.CheckedChanged += (_, _) => QueueAutoSave();

            row.Controls.Add(label);
            row.Controls.Add(detail);
            row.Controls.Add(toggle);
            _toggles[key] = toggle;
            return row;
        }

        private void LoadSettings()
        {
            try
            {
                Config config = _settingsService.GetConfig();
                SetToggle("download_firmware", config.download_firmware);
                SetToggle("download_assets", config.download_assets);
                SetToggle("preserve_platforms_folder", config.preserve_platforms_folder);
                SetToggle("delete_skipped_cores", config.delete_skipped_cores);
                SetToggle("build_instance_jsons", config.build_instance_jsons);
                SetToggle("fix_jt_names", config.fix_jt_names);
                SetToggle("skip_alternative_assets", config.skip_alternative_assets);
                SetToggle("crc_check", config.crc_check);
                SetToggle("backup_saves", config.backup_saves);
                SetToggle("use_custom_archive", config.use_custom_archive);
                _alternateUrl.Text = config.archives.FirstOrDefault(x => x.name == "custom")?.url ?? "https://updater.retrodriven.com";
                UpdateAlternateUrlState();
            }
            catch
            {
                _settingsStatus.Text = "Settings could not be loaded";
                _settingsStatus.ForeColor = ModernTheme.Danger;
            }
        }

        private bool SaveSettings(bool showMessage)
        {
            try
            {
                Config config = _settingsService.GetConfig();
                config.download_firmware = GetToggle("download_firmware");
                config.download_assets = GetToggle("download_assets");
                config.preserve_platforms_folder = GetToggle("preserve_platforms_folder");
                config.delete_skipped_cores = GetToggle("delete_skipped_cores");
                config.build_instance_jsons = GetToggle("build_instance_jsons");
                config.fix_jt_names = GetToggle("fix_jt_names");
                config.skip_alternative_assets = GetToggle("skip_alternative_assets");
                config.crc_check = GetToggle("crc_check");
                config.backup_saves = GetToggle("backup_saves");
                config.use_custom_archive = GetToggle("use_custom_archive");
                var custom = config.archives.FirstOrDefault(x => x.name == "custom");
                if (custom != null)
                    custom.url = _alternateUrl.Text.Trim();

                _settingsService.UpdateConfig(config);
                _settingsService.Save();
                if (showMessage)
                    ModernDialog.ShowInfo(FindForm(), "Pocket Updater", "Update settings saved.");
                return true;
            }
            catch (Exception ex)
            {
                if (showMessage)
                    ModernDialog.ShowError(FindForm(), "Unable to save settings", ex.Message);
                return false;
            }
        }

        private void QueueAutoSave()
        {
            if (_loadingSettings) return;
            _savedStatusTimer.Stop();
            _settingsStatus.Text = "Saving…";
            _settingsStatus.ForeColor = ModernTheme.TextMuted;
            _autoSaveTimer.Stop();
            _autoSaveTimer.Start();
        }

        private void ShowSavedStatus()
        {
            _settingsStatus.Text = "Changes Saved";
            _settingsStatus.ForeColor = ModernTheme.Success;
            _savedStatusTimer.Stop();
            _savedStatusTimer.Start();
        }

        private void ShowAutoSaveIdleStatus()
        {
            _settingsStatus.Text = "Changes save automatically";
            _settingsStatus.ForeColor = ModernTheme.TextMuted;
        }

        private async Task StartUpdateAsync()
        {
            if (!SaveSettings(false))
            {
                ModernDialog.ShowError(FindForm(), "Pocket Updater", "The update settings could not be saved.");
                return;
            }

            string target = PocketTargetContext.SelectedPath;
            if (!Directory.Exists(target))
            {
                ModernDialog.ShowWarning(FindForm(), "Pocket Updater", "The selected Pocket target is no longer available.");
                return;
            }

            using var progress = new ModernUpdateProgressForm(target);
            progress.ShowDialog(FindForm());
            await RefreshAsync();
            LibraryChangeNotifier.Notify();
        }

        private void UpdateAlternateUrlState()
        {
            bool enabled = GetToggle("use_custom_archive");
            _alternateUrl.Enabled = enabled;
            _alternateUrlRow.Visible = enabled;
            if (_settingsBody.RowStyles.Count > 1)
                _settingsBody.RowStyles[1].Height = enabled ? 52F : 0F;
        }

        private void SetToggle(string key, bool value)
        {
            if (_toggles.TryGetValue(key, out var toggle))
                toggle.Checked = value;
        }

        private bool GetToggle(string key) => _toggles.TryGetValue(key, out var toggle) && toggle.Checked;

        private static Guna2Panel CreateMetricCard(string title, UiIcon icon, Color iconColor,
            out Label value1, out Label value2, out Label value3,
            string caption1, string caption2, string caption3)
        {
            var card = CreateCard();
            AddCardHeading(card, title, icon, iconColor);

            var metrics = new TableLayoutPanel
            {
                Location = new Point(18, 80),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Size = new Size(350, 70),
                ColumnCount = 3,
                RowCount = 2,
                BackColor = Color.Transparent
            };
            metrics.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.333F));
            metrics.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.333F));
            metrics.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.334F));
            metrics.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
            metrics.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));

            value1 = NewLabel("—", 18, FontStyle.Bold, ModernTheme.Success);
            value2 = NewLabel("—", 18, FontStyle.Bold, ModernTheme.AccentHover);
            value3 = NewLabel("—", 18, FontStyle.Bold, ModernTheme.Danger);
            value1.Dock = value2.Dock = value3.Dock = DockStyle.Fill;
            value1.TextAlign = value2.TextAlign = value3.TextAlign = ContentAlignment.MiddleLeft;
            metrics.Controls.Add(value1, 0, 0);
            metrics.Controls.Add(value2, 1, 0);
            metrics.Controls.Add(value3, 2, 0);

            AddMetricCaptions(metrics, caption1, caption2, caption3);
            card.Controls.Add(metrics);
            card.Resize += (_, _) => metrics.Width = Math.Max(210, card.ClientSize.Width - 36);
            return card;
        }

        private static Guna2Panel CreateTwoMetricCard(string title, UiIcon icon, Color iconColor,
            out Label value1, out Label value2, string caption1, string caption2)
        {
            var card = CreateCard();
            AddCardHeading(card, title, icon, iconColor);
            var metrics = new TableLayoutPanel
            {
                Location = new Point(18, 80),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Size = new Size(350, 70),
                ColumnCount = 2,
                RowCount = 2,
                BackColor = Color.Transparent
            };
            metrics.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            metrics.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            metrics.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
            metrics.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
            value1 = NewLabel("—", 18, FontStyle.Bold, ModernTheme.TextPrimary);
            value2 = NewLabel("—", 18, FontStyle.Bold, ModernTheme.AccentHover);
            value1.Dock = value2.Dock = DockStyle.Fill;
            metrics.Controls.Add(value1, 0, 0);
            metrics.Controls.Add(value2, 1, 0);
            var c1 = NewLabel(caption1, 8.5F, FontStyle.Regular, ModernTheme.TextSecondary);
            var c2 = NewLabel(caption2, 8.5F, FontStyle.Regular, ModernTheme.TextSecondary);
            c1.Dock = c2.Dock = DockStyle.Fill;
            metrics.Controls.Add(c1, 0, 1);
            metrics.Controls.Add(c2, 1, 1);
            card.Controls.Add(metrics);
            card.Resize += (_, _) => metrics.Width = Math.Max(210, card.ClientSize.Width - 36);
            return card;
        }

        private static void AddMetricCaptions(TableLayoutPanel metrics, string caption1, string caption2, string caption3)
        {
            var c1 = NewLabel(caption1, 8.5F, FontStyle.Regular, ModernTheme.TextSecondary);
            var c2 = NewLabel(caption2, 8.5F, FontStyle.Regular, ModernTheme.TextSecondary);
            var c3 = NewLabel(caption3, 8.5F, FontStyle.Regular, ModernTheme.TextSecondary);
            c1.Dock = c2.Dock = c3.Dock = DockStyle.Fill;
            metrics.Controls.Add(c1, 0, 1);
            metrics.Controls.Add(c2, 1, 1);
            metrics.Controls.Add(c3, 2, 1);
        }

        private static void AddCardHeading(Control card, string text, UiIcon icon, Color color)
        {
            var tile = IconTile(icon, color, new Point(18, 16), 40);
            var label = NewLabel(text, 12.2F, FontStyle.Bold, ModernTheme.TextPrimary);
            label.Location = new Point(70, 25);
            label.AutoSize = true;
            card.Controls.Add(tile);
            card.Controls.Add(label);
        }

        private static Guna2Panel IconTile(UiIcon icon, Color color, Point location, int size)
        {
            var tile = new Guna2Panel
            {
                Location = location,
                Size = new Size(size, size),
                BorderRadius = 11,
                FillColor = Color.FromArgb(38, color),
                BackColor = Color.Transparent
            };
            var picture = new Guna2PictureBox
            {
                Image = UiIcons.Get(icon, color, Math.Max(20, size - 16)),
                SizeMode = PictureBoxSizeMode.CenterImage,
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };
            tile.Controls.Add(picture);
            return tile;
        }

        private static Guna2Panel CreateCard()
        {
            return new Guna2Panel
            {
                Dock = DockStyle.Fill,
                FillColor = ModernTheme.Surface,
                BackColor = Color.Transparent,
                BorderColor = ModernTheme.Border,
                BorderThickness = 1,
                BorderRadius = 14
            };
        }

        private static Label NewLabel(string text, float size, FontStyle style, Color color)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Segoe UI", size, style, GraphicsUnit.Point),
                ForeColor = color,
                BackColor = Color.Transparent
            };
        }

        private static Guna2Button NewButton(string text, UiIcon icon, int x, int y, int width, bool primary)
        {
            var iconColor = primary ? Color.White : Color.FromArgb(190, 208, 234);
            var button = new Guna2Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(width, 40),
                BorderRadius = 9,
                FillColor = primary ? ModernTheme.Accent : ModernTheme.SurfaceRaised,
                ForeColor = ModernTheme.TextPrimary,
                Font = ModernTheme.BodyBoldFont,
                Cursor = Cursors.Hand,
                BorderThickness = primary ? 0 : 1,
                BorderColor = ModernTheme.BorderStrong,
                TextAlign = HorizontalAlignment.Left,
                TextOffset = new Point(16, 0),
                Image = UiIcons.Get(icon, iconColor, 19),
                ImageAlign = HorizontalAlignment.Left,
                ImageOffset = new Point(10, 0),
                ImageSize = new Size(19, 19)
            };
            button.HoverState.FillColor = primary ? ModernTheme.AccentHover : ModernTheme.SurfaceHover;
            return button;
        }
    }
}
