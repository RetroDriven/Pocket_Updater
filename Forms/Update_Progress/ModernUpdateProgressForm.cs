using Guna.UI2.WinForms;
using Pannella.Helpers;
using Pannella.Models;
using Pannella.Services;
using Pocket_Updater.UI;

namespace Pocket_Updater.Forms.Update_Progress
{
    internal sealed class ModernUpdateProgressForm : Form
    {
        private readonly string _targetPath;
        private readonly string[]? _ids;
        private CoreUpdaterService? _updater;
        private bool _running;
        private bool _completed;
        private bool _cancelRequested;
        private bool _closeWhenStopped;
        private Dictionary<string, bool>? _originalSkipStates;
        private readonly Guna2ProgressBar _overallProgress;
        private readonly Label _overallPercentLabel;
        private readonly Label _overallDetail;
        private readonly Guna2ProgressBar _downloadProgress;
        private readonly Label _percentLabel;
        private readonly Label _currentItem;
        private readonly Label _currentDetail;
        private readonly Label _coresValue;
        private readonly Label _assetsValue;
        private readonly Label _skippedValue;
        private readonly Label _betaValue;
        private readonly Guna2TextBox _log;
        private readonly Guna2Button _minimizeButton;
        private readonly Guna2Button _stopButton;
        private readonly Guna2Button _closeButton;
        private readonly Guna2Button _headerMinimizeButton;
        private readonly Guna2Button _headerCloseButton;

        private readonly HashSet<string> _liveUpdatedCores = new(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> _liveDownloadedAssets = new(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> _liveSkippedAssets = new(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> _liveMissingBetaCores = new(StringComparer.OrdinalIgnoreCase);
        private string _liveCurrentCore = string.Empty;
        private UpdateProcessCompleteEventArgs? _completedResult;

        public ModernUpdateProgressForm(string targetPath, string[]? ids = null)
        {
            _targetPath = targetPath;
            _ids = ids;

            Text = "Updating Pocket";
            BackColor = ModernTheme.AppBackground;
            ForeColor = ModernTheme.TextPrimary;
            Font = ModernTheme.BodyFont;
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(980, 640);
            Size = new Size(1120, 720);
            ShowInTaskbar = true;

            var borderless = new Guna2BorderlessForm
            {
                ContainerControl = this,
                BorderRadius = 14,
                TransparentWhileDrag = false,
                DockIndicatorTransparencyValue = 0.6D
            };

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 6,
                BackColor = ModernTheme.AppBackground,
                Padding = new Padding(22)
            };
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 76));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 98));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 112));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 100));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));
            Controls.Add(root);

            var header = new Guna2Panel { Dock = DockStyle.Fill, FillColor = Color.Transparent, BackColor = Color.Transparent };
            var title = Label("Updating Pocket", 24, FontStyle.Bold, ModernTheme.TextPrimary);
            title.Location = new Point(58, 0);
            title.AutoSize = true;
            var subtitle = Label(_ids == null ? "Installing updates and syncing your library." : $"Updating {_ids.Length} selected core(s) and their required assets.", 10, FontStyle.Regular, ModernTheme.TextSecondary);
            subtitle.Location = new Point(60, 42);
            subtitle.AutoSize = true;

            _headerMinimizeButton = WindowButton("—");
            _headerMinimizeButton.Click += (_, _) => WindowState = FormWindowState.Minimized;
            _headerCloseButton = WindowButton("×");
            _headerCloseButton.HoverState.FillColor = Color.FromArgb(180, 48, 62);
            _headerCloseButton.Click += (_, _) => RequestStop(true);

            header.Resize += (_, _) =>
            {
                _headerCloseButton.Location = new Point(header.ClientSize.Width - 34, 0);
                _headerMinimizeButton.Location = new Point(header.ClientSize.Width - 72, 0);
            };
            header.Controls.Add(IconTile(UiIcon.Update, ModernTheme.AccentHover, new Point(0, 1)));
            header.Controls.Add(title);
            header.Controls.Add(subtitle);
            header.Controls.Add(_headerMinimizeButton);
            header.Controls.Add(_headerCloseButton);
            root.Controls.Add(header, 0, 0);

            var drag = new Guna2DragControl { TargetControl = header, UseTransparentDrag = true };

            var overallCard = Card();
            overallCard.Padding = new Padding(18);
            var overallTitle = Label("Overall Progress", 10.5F, FontStyle.Bold, ModernTheme.TextPrimary);
            overallTitle.Location = new Point(18, 12);
            overallTitle.AutoSize = true;
            _overallProgress = new Guna2ProgressBar
            {
                Location = new Point(18, 46),
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top,
                Height = 18,
                Width = 860,
                BorderRadius = 9,
                FillColor = ModernTheme.SurfaceRaised,
                ProgressColor = ModernTheme.Success,
                ProgressColor2 = Color.FromArgb(62, 220, 159),
                Value = 0
            };
            _overallPercentLabel = Label("0%", 18, FontStyle.Bold, ModernTheme.TextPrimary);
            _overallPercentLabel.AutoSize = false;
            _overallPercentLabel.Size = new Size(100, 36);
            _overallPercentLabel.TextAlign = ContentAlignment.MiddleRight;
            _overallPercentLabel.Location = new Point(900, 34);
            _overallDetail = Label("Preparing core update list…", 9, FontStyle.Regular, ModernTheme.TextSecondary);
            _overallDetail.Location = new Point(18, 70);
            _overallDetail.AutoSize = true;
            overallCard.Resize += (_, _) =>
            {
                const int rightPadding = 24;
                const int progressGap = 20;
                _overallPercentLabel.Left = overallCard.ClientSize.Width - _overallPercentLabel.Width - rightPadding;
                _overallProgress.Width = Math.Max(200, _overallPercentLabel.Left - _overallProgress.Left - progressGap);
            };
            overallCard.Controls.Add(overallTitle);
            overallCard.Controls.Add(_overallProgress);
            overallCard.Controls.Add(_overallPercentLabel);
            overallCard.Controls.Add(_overallDetail);
            root.Controls.Add(overallCard, 0, 1);

            var progressCard = Card();
            progressCard.Padding = new Padding(18);
            var progressTitle = Label("Current Transfer", 10.5F, FontStyle.Bold, ModernTheme.TextPrimary);
            progressTitle.Location = new Point(18, 12);
            progressTitle.AutoSize = true;
            _downloadProgress = new Guna2ProgressBar
            {
                Location = new Point(18, 48),
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top,
                Height = 20,
                Width = 860,
                BorderRadius = 10,
                FillColor = ModernTheme.SurfaceRaised,
                ProgressColor = ModernTheme.Accent,
                ProgressColor2 = Color.FromArgb(61, 207, 255),
                Value = 0
            };
            _percentLabel = Label("0%", 20, FontStyle.Bold, ModernTheme.TextPrimary);

            _percentLabel.AutoSize = false;
            _percentLabel.Size = new Size(100, 40);
            _percentLabel.TextAlign = ContentAlignment.MiddleRight;
            _percentLabel.Location = new Point(900, 34);
            progressCard.Resize += (_, _) =>
            {
                const int rightPadding = 24;
                const int progressGap = 20;
                _percentLabel.Left = progressCard.ClientSize.Width - _percentLabel.Width - rightPadding;
                _downloadProgress.Width = Math.Max(200, _percentLabel.Left - _downloadProgress.Left - progressGap);
            };
            var safe = Label("Your settings, save files, and existing games are preserved by the updater.", 9, FontStyle.Regular, ModernTheme.Success);
            safe.Location = new Point(18, 80);
            safe.AutoSize = true;
            progressCard.Controls.Add(progressTitle);
            progressCard.Controls.Add(_downloadProgress);
            progressCard.Controls.Add(_percentLabel);
            progressCard.Controls.Add(safe);
            root.Controls.Add(progressCard, 0, 2);

            var metrics = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 4, RowCount = 1, BackColor = Color.Transparent, Padding = new Padding(0, 10, 0, 10) };
            for (int i = 0; i < 4; i++) metrics.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));

            var coresMetric = Metric("Cores Updated", UiIcon.Core, out _coresValue, ModernTheme.Success, new Padding(0, 0, 6, 0));
            var assetsMetric = Metric("ROMs / BIOS Downloaded", UiIcon.RomBios, out _assetsValue, ModernTheme.AccentHover, new Padding(6, 0, 6, 0));
            var skippedMetric = Metric("Skipped Items", UiIcon.List, out _skippedValue, ModernTheme.Warning, new Padding(6, 0, 6, 0));
            var betaMetric = Metric("Beta Keys Needed", UiIcon.Missing, out _betaValue, ModernTheme.Danger, new Padding(6, 0, 0, 0));

            MakeMetricInteractive(coresMetric, "Cores Updated", ModernTheme.Success, GetUpdatedCoreDetails);
            MakeMetricInteractive(assetsMetric, "ROMs / BIOS Downloaded", ModernTheme.AccentHover, GetDownloadedAssetDetails);
            MakeMetricInteractive(skippedMetric, "Skipped Items", ModernTheme.Warning, GetSkippedAssetDetails);
            MakeMetricInteractive(betaMetric, "Beta Keys Needed", ModernTheme.Danger, GetMissingBetaDetails);

            metrics.Controls.Add(coresMetric, 0, 0);
            metrics.Controls.Add(assetsMetric, 1, 0);
            metrics.Controls.Add(skippedMetric, 2, 0);
            metrics.Controls.Add(betaMetric, 3, 0);
            root.Controls.Add(metrics, 0, 3);

            var main = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, BackColor = Color.Transparent };
            main.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42));
            main.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58));

            var current = Card();
            current.Margin = new Padding(0, 0, 8, 0);
            current.Padding = new Padding(18);
            var currentTitle = Label("Current Item", 11, FontStyle.Bold, ModernTheme.TextPrimary);
            currentTitle.Location = new Point(18, 16);
            currentTitle.AutoSize = true;
            _currentItem = Label("Preparing update…", 15, FontStyle.Bold, ModernTheme.TextPrimary);
            _currentItem.Location = new Point(18, 56);
            _currentItem.Size = new Size(390, 36);
            _currentDetail = Label("The updater will report each operation here as it runs.", 9.5F, FontStyle.Regular, ModernTheme.TextSecondary);
            _currentDetail.Location = new Point(18, 96);
            _currentDetail.Size = new Size(390, 90);
            current.Controls.Add(currentTitle);
            current.Controls.Add(_currentItem);
            current.Controls.Add(_currentDetail);

            var logCard = Card();
            logCard.Margin = new Padding(8, 0, 0, 0);
            logCard.Padding = new Padding(14);
            var logTitle = Label("Live Activity Log", 11, FontStyle.Bold, ModernTheme.TextPrimary);
            logTitle.Location = new Point(14, 12);
            logTitle.AutoSize = true;
            _log = new Guna2TextBox
            {
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                FillColor = Color.FromArgb(7, 13, 23),
                ForeColor = Color.FromArgb(197, 216, 240),
                BorderColor = ModernTheme.BorderStrong,
                BorderRadius = 9,
                Font = ModernTheme.MonoFont,
                Location = new Point(14, 46),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                Size = new Size(560, 200)
            };
            logCard.Resize += (_, _) => _log.Size = new Size(logCard.ClientSize.Width - 28, logCard.ClientSize.Height - 60);
            logCard.Controls.Add(logTitle);
            logCard.Controls.Add(_log);
            main.Controls.Add(current, 0, 0);
            main.Controls.Add(logCard, 1, 0);
            root.Controls.Add(main, 0, 4);

            var footer = new Guna2Panel { Dock = DockStyle.Fill, FillColor = Color.Transparent, BackColor = Color.Transparent };
            var state = Label("Preparing…", 9.5F, FontStyle.Bold, ModernTheme.TextSecondary);
            state.Name = "StateLabel";
            state.Location = new Point(2, 18);
            state.AutoSize = true;

            _minimizeButton = Button("Minimize", 112, false);
            _minimizeButton.Click += (_, _) => WindowState = FormWindowState.Minimized;

            _stopButton = Button("Stop Update", 132, false);
            _stopButton.FillColor = Color.FromArgb(111, 32, 43);
            _stopButton.BorderColor = Color.FromArgb(189, 64, 79);
            _stopButton.HoverState.FillColor = Color.FromArgb(142, 40, 55);
            _stopButton.Click += (_, _) => RequestStop(false);

            _closeButton = Button("Close", 104, false);
            SetButtonIcon(_closeButton, UiIcon.Missing, false);
            _closeButton.Enabled = false;
            _closeButton.Click += (_, _) => Close();

            footer.Resize += (_, _) =>
            {
                _closeButton.Location = new Point(footer.ClientSize.Width - _closeButton.Width, 8);
                _stopButton.Location = new Point(_closeButton.Left - _stopButton.Width - 10, 8);
                _minimizeButton.Location = new Point(_stopButton.Left - _minimizeButton.Width - 10, 8);
            };
            footer.Controls.Add(state);
            footer.Controls.Add(_minimizeButton);
            footer.Controls.Add(_stopButton);
            footer.Controls.Add(_closeButton);
            root.Controls.Add(footer, 0, 5);

            Shown += async (_, _) => await StartAsync();
            FormClosing += ModernUpdateProgressForm_FormClosing;
        }

        private async Task StartAsync()
        {
            if (_running || _completed) return;
            UpdateCancellation.Reset();
            _cancelRequested = false;
            _closeWhenStopped = false;
            _running = true;
            ResetLiveStats();
            SetState("Scanning and preparing update…");

            try
            {
                ServiceHelper.Initialize(_targetPath, Directory.GetCurrentDirectory(), OnStatus, null, true);
                if (_ids != null)
                {
                    _originalSkipStates = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
                    foreach (string id in _ids.Distinct(StringComparer.OrdinalIgnoreCase))
                    {
                        _originalSkipStates[id] = ServiceHelper.SettingsService.GetCoreSettings(id).skip;
                        ServiceHelper.SettingsService.EnableCore(id);
                    }
                    ServiceHelper.SettingsService.Save();
                }

                _updater = new CoreUpdaterService(
                    ServiceHelper.UpdateDirectory,
                    ServiceHelper.CoresService.Cores,
                    ServiceHelper.FirmwareService,
                    ServiceHelper.SettingsService,
                    ServiceHelper.CoresService);
                _updater.StatusUpdated += OnStatus;
                _updater.UpdateProcessComplete += OnComplete;
                _updater.OverallProgressUpdate += OnOverallProgress;
                HttpHelper.Instance.DownloadProgressUpdate += OnDownloadProgress;

                await Task.Run(() => _updater.RunUpdates(_ids));
            }
            catch (OperationCanceledException) when (_cancelRequested || UpdateCancellation.IsCancellationRequested)
            {
                AppendLog("Update canceled by user.");
                SetState("Update stopped safely.");
                _currentItem.Text = "Update stopped";
                _currentDetail.Text = "Completed items were kept. Any partially downloaded active file was removed and can be downloaded again on the next update.";
                _running = false;
                _stopButton.Enabled = false;
                _stopButton.Text = "Stopped";
                _headerCloseButton.Enabled = true;
                _closeButton.Enabled = true;
            }
            catch (Exception ex)
            {
                AppendLog("ERROR: " + ex.Message);
                SetState("Update stopped because of an error.");
                _currentItem.Text = "Update failed";
                _currentDetail.Text = ex.Message;
                _closeButton.Enabled = true;
                _stopButton.Enabled = false;
                _headerCloseButton.Enabled = true;
                _running = false;
            }
            finally
            {
                HttpHelper.Instance.DownloadProgressUpdate -= OnDownloadProgress;
                if (_updater != null)
                {
                    _updater.StatusUpdated -= OnStatus;
                    _updater.UpdateProcessComplete -= OnComplete;
                    _updater.OverallProgressUpdate -= OnOverallProgress;
                }

                try
                {
                    ServiceHelper.CoresService?.DeleteBetaKey();
                }
                catch
                {

                }

                RestoreSelectedCoreSkipStates();
                UpdateCancellation.Reset();

                if (_closeWhenStopped && !_running && !IsDisposed)
                    BeginInvoke((Action)Close);
            }
        }

        private void RestoreSelectedCoreSkipStates()
        {
            if (_originalSkipStates == null) return;
            try
            {
                foreach (var pair in _originalSkipStates)
                {
                    if (pair.Value)
                        ServiceHelper.SettingsService.DisableCore(pair.Key);
                    else
                        ServiceHelper.SettingsService.EnableCore(pair.Key);
                }
                ServiceHelper.SettingsService.Save();
            }
            catch
            {

            }
            finally
            {
                _originalSkipStates = null;
            }
        }

        private void OnStatus(object? sender, StatusUpdatedEventArgs e)
        {
            if (IsDisposed) return;
            BeginInvoke((Action)(() =>
            {
                string message = e.Message ?? string.Empty;
                AppendLog(message);
                _currentItem.Text = FriendlyCurrentItem(message);
                _currentDetail.Text = message;
                UpdateTransferProgressFromStatus(message);
                UpdateLiveStatsFromMessage(message);
            }));
        }

        private void UpdateTransferProgressFromStatus(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;

            string trimmed = message.Trim();
            string lower = trimmed.ToLowerInvariant();

            if (lower == "downloading core..."
                || lower.StartsWith("downloading file ")
                || lower.StartsWith("downloading '")
                || lower.StartsWith("downloading firmware"))
            {
                _downloadProgress.Value = 0;
                _percentLabel.Text = "0%";
                return;
            }

            if (lower.StartsWith("finished downloading '")
                || lower.StartsWith("download complete"))
            {
                _downloadProgress.Value = 100;
                _percentLabel.Text = "100%";
            }
        }

        private void OnDownloadProgress(object? sender, DownloadProgressEventArgs e)
        {
            if (IsDisposed) return;
            int value = (int)Math.Round(e.Progress * 100d);
            value = Math.Max(0, Math.Min(100, value));
            BeginInvoke((Action)(() =>
            {
                _downloadProgress.Value = value;
                _percentLabel.Text = value + "%";
            }));
        }

        private void OnOverallProgress(object? sender, OverallProgressEventArgs e)
        {
            if (IsDisposed) return;
            int value = e.Total <= 0 ? 100 : (int)Math.Round(e.Progress * 100d);
            value = Math.Max(0, Math.Min(100, value));
            BeginInvoke((Action)(() =>
            {
                _overallProgress.Value = value;
                _overallPercentLabel.Text = value + "%";
                _overallDetail.Text = e.Total <= 0
                    ? "No cores selected."
                    : $"{e.Completed} of {e.Total} cores processed";
            }));
        }

        private void OnComplete(object? sender, UpdateProcessCompleteEventArgs e)
        {
            if (IsDisposed) return;
            BeginInvoke((Action)(() =>
            {
                _completedResult = e;
                _coresValue.Text = (e.InstalledCores?.Count ?? 0).ToString();
                _assetsValue.Text = (e.InstalledAssets?.Count ?? 0).ToString();
                _skippedValue.Text = (e.SkippedAssets?.Count ?? 0).ToString();
                _betaValue.Text = (e.MissingBetaKeys?.Count ?? 0).ToString();
                _downloadProgress.Value = 100;
                _percentLabel.Text = "100%";
                _overallProgress.Value = 100;
                _overallPercentLabel.Text = "100%";
                _overallDetail.Text = "All selected cores processed.";
                SetState("Update complete.");
                _currentItem.Text = "All done";
                _currentDetail.Text = BuildCompletionText(e);
                WriteActivityLog();
                _completed = true;
                _running = false;
                _stopButton.Enabled = false;
                _stopButton.Text = "Complete";
                _headerCloseButton.Enabled = true;
                _closeButton.Enabled = true;

                int installedCores = e.InstalledCores?.Count ?? 0;
                int installedAssets = e.InstalledAssets?.Count ?? 0;
                int skippedAssets = e.SkippedAssets?.Count ?? 0;
                int missingBetaKeys = e.MissingBetaKeys?.Count ?? 0;
                string completionMessage =
                    $"Pocket update completed.\n\nCores updated: {installedCores}\nROM / BIOS files downloaded: {installedAssets}\nSkipped items: {skippedAssets}\nBeta keys needed: {missingBetaKeys}";

                ModernDialog.ShowSuccess(this, "Update Complete", completionMessage);
            }));
        }

        private void ResetLiveStats()
        {
            _completedResult = null;
            _liveUpdatedCores.Clear();
            _liveDownloadedAssets.Clear();
            _liveSkippedAssets.Clear();
            _liveMissingBetaCores.Clear();
            _liveCurrentCore = string.Empty;
            _coresValue.Text = "0";
            _assetsValue.Text = "0";
            _skippedValue.Text = "0";
            _betaValue.Text = "0";
            _overallProgress.Value = 0;
            _overallPercentLabel.Text = "0%";
            _overallDetail.Text = "Preparing core update list…";
        }

        private void UpdateLiveStatsFromMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;
            string trimmed = message.Trim();

            const string checkingPrefix = "Checking Core:";
            if (trimmed.StartsWith(checkingPrefix, StringComparison.OrdinalIgnoreCase))
            {
                _liveCurrentCore = trimmed[checkingPrefix.Length..].Trim();
                return;
            }

            if (trimmed.Equals("Installation complete.", StringComparison.OrdinalIgnoreCase))
            {
                string key = string.IsNullOrWhiteSpace(_liveCurrentCore)
                    ? $"core-{_liveUpdatedCores.Count + 1}"
                    : _liveCurrentCore;
                if (_liveUpdatedCores.Add(key))
                    _coresValue.Text = _liveUpdatedCores.Count.ToString();
                return;
            }

            if (trimmed.StartsWith("Finished downloading '", StringComparison.OrdinalIgnoreCase))
            {
                string file = ExtractQuotedValue(trimmed);
                string key = $"{_liveCurrentCore}|{file}";
                if (_liveDownloadedAssets.Add(key))
                    _assetsValue.Text = _liveDownloadedAssets.Count.ToString();
                return;
            }

            if (trimmed.StartsWith("Unable to find '", StringComparison.OrdinalIgnoreCase)
                || trimmed.StartsWith("There was a problem downloading '", StringComparison.OrdinalIgnoreCase)
                || trimmed.StartsWith("Something went wrong with '", StringComparison.OrdinalIgnoreCase))
            {
                string file = ExtractQuotedValue(trimmed);
                string key = $"{_liveCurrentCore}|{file}";
                if (_liveSkippedAssets.Add(key))
                    _skippedValue.Text = _liveSkippedAssets.Count.ToString();
                return;
            }

            string lower = trimmed.ToLowerInvariant();
            if (lower.Contains("beta key not found") || lower.Contains("beta key checksum validation failed"))
            {
                string key = string.IsNullOrWhiteSpace(_liveCurrentCore) ? trimmed : _liveCurrentCore;
                if (_liveMissingBetaCores.Add(key))
                    _betaValue.Text = _liveMissingBetaCores.Count.ToString();
            }
        }

        private IEnumerable<string> GetUpdatedCoreDetails()
        {
            if (_completedResult?.InstalledCores != null)
            {
                return _completedResult.InstalledCores.Select(item =>
                {
                    item.TryGetValue("platform", out string? platform);
                    item.TryGetValue("core", out string? core);
                    item.TryGetValue("version", out string? version);
                    string name = !string.IsNullOrWhiteSpace(platform) ? platform! : (core ?? "Unknown core");
                    string suffix = !string.IsNullOrWhiteSpace(version) ? $" — {version}" : string.Empty;
                    string id = !string.IsNullOrWhiteSpace(core) && !string.Equals(core, name, StringComparison.OrdinalIgnoreCase)
                        ? $" ({core})"
                        : string.Empty;
                    return name + id + suffix;
                }).ToList();
            }

            return _liveUpdatedCores.OrderBy(x => x).ToList();
        }

        private IEnumerable<string> GetDownloadedAssetDetails()
        {
            if (_completedResult?.InstalledAssets != null)
                return _completedResult.InstalledAssets.OrderBy(x => x).ToList();

            return _liveDownloadedAssets
                .Select(DetailValueFromLiveKey)
                .OrderBy(x => x)
                .ToList();
        }

        private IEnumerable<string> GetSkippedAssetDetails()
        {
            if (_completedResult?.SkippedAssets != null)
                return _completedResult.SkippedAssets.OrderBy(x => x).ToList();

            return _liveSkippedAssets
                .Select(DetailValueFromLiveKey)
                .OrderBy(x => x)
                .ToList();
        }

        private IEnumerable<string> GetMissingBetaDetails()
        {
            if (_completedResult?.MissingBetaKeys != null)
                return _completedResult.MissingBetaKeys.OrderBy(x => x).ToList();

            return _liveMissingBetaCores.OrderBy(x => x).ToList();
        }

        private static string DetailValueFromLiveKey(string value)
        {
            int separator = value.IndexOf('|');
            return separator >= 0 && separator < value.Length - 1 ? value[(separator + 1)..] : value;
        }

        private void MakeMetricInteractive(Guna2Panel card, string title, Color accent, Func<IEnumerable<string>> getItems)
        {
            void ShowDetails(object? sender, EventArgs e)
            {
                ModernDialog.ShowDetails(this, title, getItems(), accent);
            }

            void Wire(Control control)
            {
                control.Cursor = Cursors.Hand;
                control.Click += ShowDetails;
                foreach (Control child in control.Controls)
                    Wire(child);
            }

            Wire(card);
            card.MouseEnter += (_, _) =>
            {
                card.FillColor = ModernTheme.SurfaceHover;
                card.BorderColor = Color.FromArgb(120, accent);
            };
            card.MouseLeave += (_, _) =>
            {
                card.FillColor = ModernTheme.Surface;
                card.BorderColor = ModernTheme.Border;
            };
        }

        private static string ExtractQuotedValue(string message)
        {
            int first = message.IndexOf('\'');
            if (first < 0) return message;
            int second = message.IndexOf('\'', first + 1);
            if (second <= first) return message[first..];
            return message[(first + 1)..second];
        }

        private void SetState(string text)
        {
            var label = Controls.Find("StateLabel", true).FirstOrDefault() as Label;
            if (label != null) label.Text = text;
        }

        private void AppendLog(string message)
        {
            string line = $"[{DateTime.Now:HH:mm:ss}] {message}";
            _log.AppendText(line + Environment.NewLine);
            _log.SelectionStart = _log.TextLength;
            _log.ScrollToCaret();
        }

        private static string FriendlyCurrentItem(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return "Working…";
            string value = message.Trim();
            if (value.Length <= 52) return value;
            return value[..49] + "…";
        }

        private static string BuildCompletionText(UpdateProcessCompleteEventArgs e)
        {
            var parts = new List<string>();
            int cores = e.InstalledCores?.Count ?? 0;
            int assets = e.InstalledAssets?.Count ?? 0;
            int skipped = e.SkippedAssets?.Count ?? 0;
            if (cores > 0) parts.Add($"{cores} core update(s) installed");
            if (assets > 0) parts.Add($"{assets} ROM / BIOS file(s) downloaded");
            if (skipped > 0) parts.Add($"{skipped} item(s) skipped");
            if (!string.IsNullOrWhiteSpace(e.FirmwareUpdated)) parts.Add("Pocket firmware updated");
            if ((e.MissingBetaKeys?.Count ?? 0) > 0) parts.Add($"{e.MissingBetaKeys.Count} beta key(s) still required");
            return parts.Count == 0 ? "Everything checked was already up to date." : string.Join(" • ", parts);
        }

        private void WriteActivityLog()
        {
            try
            {
                string file = Path.Combine(Directory.GetCurrentDirectory(), "Pocket_Updater_Log.txt");
                string stamp = $"**Update Run On {DateTime.Now:MM-dd-yy HH:mm:ss}**";
                File.AppendAllText(file, Environment.NewLine + stamp + Environment.NewLine + _log.Text + Environment.NewLine);
            }
            catch
            {

            }
        }

        private void ModernUpdateProgressForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (!_running || _completed)
                return;

            e.Cancel = true;

            if (_cancelRequested)
            {
                _closeWhenStopped = true;
                return;
            }

            RequestStop(true);
        }

        private void RequestStop(bool closeAfter, bool confirm = true)
        {
            if (!_running)
            {
                if (closeAfter) Close();
                return;
            }

            if (_cancelRequested)
            {
                _closeWhenStopped |= closeAfter;
                return;
            }

            if (confirm)
            {
                bool stopUpdate = ModernDialog.ShowConfirm(
                    this,
                    "Stop update",
                    "Stop the current update?\n\nThe active download will be canceled safely. Completed files will be kept and any partial active download will be removed.");

                if (!stopUpdate)
                    return;
            }

            _cancelRequested = true;
            _closeWhenStopped |= closeAfter;
            _stopButton.Enabled = false;
            _stopButton.Text = "Stopping…";
            _headerCloseButton.Enabled = false;
            SetState("Stopping safely after the current operation…");
            AppendLog("Stop requested by user...");
            UpdateCancellation.RequestCancel();
        }

        private static Guna2Panel Metric(string captionText, UiIcon icon, out Label value, Color color, Padding margin)
        {
            var card = Card();
            card.Margin = margin;
            card.Controls.Add(IconTile(icon, color, new Point(12, 18), 36));
            var caption = Label(captionText, 8.8F, FontStyle.Bold, ModernTheme.TextSecondary);
            caption.Location = new Point(58, 11);
            caption.AutoSize = true;
            value = Label("0", 20, FontStyle.Bold, color);
            value.Location = new Point(58, 34);
            value.AutoSize = true;
            card.Controls.Add(caption);
            card.Controls.Add(value);
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

        private static Guna2Button WindowButton(string text)
        {
            var button = new Guna2Button
            {
                Text = text,
                Size = new Size(34, 30),
                BorderRadius = 7,
                FillColor = Color.Transparent,
                ForeColor = ModernTheme.TextSecondary,
                Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point),
                Cursor = Cursors.Hand,
                Animated = true
            };
            button.HoverState.FillColor = ModernTheme.SurfaceHover;
            button.HoverState.ForeColor = Color.White;
            return button;
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
            return button;
        }

        private static string ShortPath(string path)
        {
            if (path.Length <= 48) return path;
            return "…" + path[^47..];
        }
    }
}
