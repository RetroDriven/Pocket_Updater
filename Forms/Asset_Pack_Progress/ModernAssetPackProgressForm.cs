using Guna.UI2.WinForms;
using Pannella.Helpers;
using Pannella.Models;
using Pannella.Services;
using Pocket_Updater.UI;

namespace Pocket_Updater.Forms.Asset_Pack_Progress
{
    internal sealed record AssetPackProgressItem(string Owner, string Repository, string? Variant, string DisplayVariant);

    internal sealed class ModernAssetPackProgressForm : Form
    {
        private readonly string _targetPath;
        private readonly IReadOnlyList<AssetPackProgressItem> _items;
        private readonly string _verb;
        private PlatformImagePacksService? _service;
        private bool _running;
        private bool _completed;
        private bool _closeWhenStopped;
        private int _completedCount;
        private int _failedCount;
        private int _currentIndex;

        private readonly Guna2ProgressBar _overallProgress;
        private readonly Label _overallPercentLabel;
        private readonly Label _overallDetail;
        private readonly Guna2ProgressBar _downloadProgress;
        private readonly Label _percentLabel;
        private readonly Label _transferDetail;
        private readonly Label _currentItem;
        private readonly Label _currentDetail;
        private readonly Label _completedValue;
        private readonly Label _remainingValue;
        private readonly Label _failedValue;
        private readonly Label _positionValue;
        private readonly Guna2TextBox _log;
        private readonly Guna2Button _minimizeButton;
        private readonly Guna2Button _stopButton;
        private readonly Guna2Button _closeButton;
        private readonly Guna2Button _headerMinimizeButton;
        private readonly Guna2Button _headerCloseButton;

        public ModernAssetPackProgressForm(string targetPath, IReadOnlyList<AssetPackProgressItem> items, string verb)
        {
            _targetPath = targetPath;
            _items = items;
            _verb = string.IsNullOrWhiteSpace(verb) ? "Install" : verb;

            Text = $"{_verb} Image Packs";
            BackColor = ModernTheme.AppBackground;
            ForeColor = ModernTheme.TextPrimary;
            Font = ModernTheme.BodyFont;
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(940, 620);
            Size = new Size(1080, 700);
            ShowInTaskbar = true;

            _ = new Guna2BorderlessForm
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
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 116));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 96));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));
            Controls.Add(root);

            var header = new Guna2Panel { Dock = DockStyle.Fill, FillColor = Color.Transparent, BackColor = Color.Transparent };
            var title = MakeLabel($"{_verb} Image Packs", 24, FontStyle.Bold, ModernTheme.TextPrimary);
            title.Location = new Point(58, 0);
            title.AutoSize = true;
            var subtitle = MakeLabel($"Processing {_items.Count} selected image pack(s).", 10, FontStyle.Regular, ModernTheme.TextSecondary);
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
            header.Controls.Add(IconTile(UiIcon.AssetPack, Color.FromArgb(81, 194, 255), new Point(0, 1)));
            header.Controls.Add(title);
            header.Controls.Add(subtitle);
            header.Controls.Add(_headerMinimizeButton);
            header.Controls.Add(_headerCloseButton);
            root.Controls.Add(header, 0, 0);

            _ = new Guna2DragControl { TargetControl = header, UseTransparentDrag = true };

            var overallCard = Card();
            overallCard.Padding = new Padding(18);
            var overallTitle = MakeLabel("Overall Progress", 10.5F, FontStyle.Bold, ModernTheme.TextPrimary);
            overallTitle.Location = new Point(18, 12);
            overallTitle.AutoSize = true;
            _overallProgress = new Guna2ProgressBar
            {
                Location = new Point(18, 46),
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top,
                Height = 18,
                Width = 820,
                BorderRadius = 9,
                FillColor = ModernTheme.SurfaceRaised,
                ProgressColor = ModernTheme.Success,
                ProgressColor2 = Color.FromArgb(62, 220, 159),
                Value = 0
            };
            _overallPercentLabel = MakeLabel("0%", 18, FontStyle.Bold, ModernTheme.TextPrimary);
            _overallPercentLabel.AutoSize = false;
            _overallPercentLabel.Size = new Size(100, 36);
            _overallPercentLabel.TextAlign = ContentAlignment.MiddleRight;
            _overallPercentLabel.Location = new Point(870, 34);
            _overallDetail = MakeLabel($"0 of {_items.Count} packs processed", 9, FontStyle.Regular, ModernTheme.TextSecondary);
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
            var progressTitle = MakeLabel("Current Transfer", 10.5F, FontStyle.Bold, ModernTheme.TextPrimary);
            progressTitle.Location = new Point(18, 12);
            progressTitle.AutoSize = true;
            _downloadProgress = new Guna2ProgressBar
            {
                Location = new Point(18, 48),
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top,
                Height = 20,
                Width = 820,
                BorderRadius = 10,
                FillColor = ModernTheme.SurfaceRaised,
                ProgressColor = ModernTheme.Accent,
                ProgressColor2 = Color.FromArgb(61, 207, 255),
                Value = 0
            };
            _percentLabel = MakeLabel("0%", 20, FontStyle.Bold, ModernTheme.TextPrimary);

            _percentLabel.AutoSize = false;
            _percentLabel.Size = new Size(100, 40);
            _percentLabel.TextAlign = ContentAlignment.MiddleRight;
            _percentLabel.Location = new Point(870, 34);
            _transferDetail = MakeLabel("Waiting for the first download…", 9, FontStyle.Regular, ModernTheme.TextSecondary);
            _transferDetail.Location = new Point(18, 80);
            _transferDetail.AutoSize = true;
            progressCard.Resize += (_, _) =>
            {
                const int rightPadding = 24;
                const int progressGap = 20;
                _percentLabel.Left = progressCard.ClientSize.Width - _percentLabel.Width - rightPadding;
                _downloadProgress.Width = Math.Max(200, _percentLabel.Left - _downloadProgress.Left - progressGap);
            };
            progressCard.Controls.Add(progressTitle);
            progressCard.Controls.Add(_downloadProgress);
            progressCard.Controls.Add(_percentLabel);
            progressCard.Controls.Add(_transferDetail);
            root.Controls.Add(progressCard, 0, 2);

            var metrics = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 4, RowCount = 1, BackColor = Color.Transparent, Padding = new Padding(0, 8, 0, 8) };
            for (int i = 0; i < 4; i++) metrics.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            metrics.Controls.Add(Metric("Packs Completed", UiIcon.Check, out _completedValue, ModernTheme.Success, new Padding(0, 0, 6, 0)), 0, 0);
            metrics.Controls.Add(Metric("Remaining", UiIcon.List, out _remainingValue, ModernTheme.AccentHover, new Padding(6, 0, 6, 0)), 1, 0);
            metrics.Controls.Add(Metric("Failed", UiIcon.Missing, out _failedValue, ModernTheme.Danger, new Padding(6, 0, 6, 0)), 2, 0);
            metrics.Controls.Add(Metric("Current Pack", UiIcon.AssetPack, out _positionValue, Color.FromArgb(178, 132, 255), new Padding(6, 0, 0, 0)), 3, 0);
            root.Controls.Add(metrics, 0, 3);

            var main = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, BackColor = Color.Transparent };
            main.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42));
            main.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58));

            var current = Card();
            current.Margin = new Padding(0, 0, 8, 0);
            current.Padding = new Padding(18);
            var currentTitle = MakeLabel("Current Pack", 11, FontStyle.Bold, ModernTheme.TextPrimary);
            currentTitle.Location = new Point(18, 16);
            currentTitle.AutoSize = true;
            _currentItem = MakeLabel("Preparing image packs…", 15, FontStyle.Bold, ModernTheme.TextPrimary);
            _currentItem.Location = new Point(18, 56);
            _currentItem.Size = new Size(390, 38);
            _currentItem.AutoEllipsis = true;
            _currentDetail = MakeLabel("The selected repository and variant will appear here.", 9.5F, FontStyle.Regular, ModernTheme.TextSecondary);
            _currentDetail.Location = new Point(18, 100);
            _currentDetail.Size = new Size(390, 100);
            current.Resize += (_, _) =>
            {
                _currentItem.Width = Math.Max(120, current.ClientSize.Width - 36);
                _currentDetail.Width = Math.Max(120, current.ClientSize.Width - 36);
            };
            current.Controls.Add(currentTitle);
            current.Controls.Add(_currentItem);
            current.Controls.Add(_currentDetail);

            var logCard = Card();
            logCard.Margin = new Padding(8, 0, 0, 0);
            logCard.Padding = new Padding(14);
            var logTitle = MakeLabel("Live Activity Log", 11, FontStyle.Bold, ModernTheme.TextPrimary);
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
                Size = new Size(540, 200)
            };
            logCard.Resize += (_, _) => _log.Size = new Size(logCard.ClientSize.Width - 28, logCard.ClientSize.Height - 60);
            logCard.Controls.Add(logTitle);
            logCard.Controls.Add(_log);
            main.Controls.Add(current, 0, 0);
            main.Controls.Add(logCard, 1, 0);
            root.Controls.Add(main, 0, 4);

            var footer = new Guna2Panel { Dock = DockStyle.Fill, FillColor = Color.Transparent, BackColor = Color.Transparent };
            var state = MakeLabel("Preparing…", 9.5F, FontStyle.Bold, ModernTheme.TextSecondary);
            state.Name = "StateLabel";
            state.Location = new Point(2, 18);
            state.AutoSize = true;

            _minimizeButton = Button("Minimize", 112, false);
            _minimizeButton.Click += (_, _) => WindowState = FormWindowState.Minimized;

            _stopButton = Button("Stop", 112, false);
            _stopButton.FillColor = Color.FromArgb(111, 32, 43);
            _stopButton.BorderColor = Color.FromArgb(189, 64, 79);
            _stopButton.HoverState.FillColor = Color.FromArgb(142, 40, 55);
            _stopButton.Click += (_, _) => RequestStop(false);

            _closeButton = Button("Close", 104, false);
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

            UpdateMetrics();
            UpdateOverallProgress();
            Shown += async (_, _) => await StartAsync();
            FormClosing += ModernAssetPackProgressForm_FormClosing;
        }

        private async Task StartAsync()
        {
            if (_running || _completed) return;

            UpdateCancellation.Reset();
            _running = true;
            UpdateOverallProgress();
            SetState("Preparing image-pack operation…");
            AppendLog($"Starting {_verb.ToLowerInvariant()} operation for {_items.Count} image pack(s).");

            try
            {
                ServiceHelper.Initialize(_targetPath, Directory.GetCurrentDirectory(), forceReload: true);
                var config = ServiceHelper.SettingsService.GetConfig();
                config.preserve_platforms_folder = true;
                ServiceHelper.SettingsService.UpdateConfig(config);
                ServiceHelper.SettingsService.Save();

                _service = ServiceHelper.PlatformImagePacksService;
                _service.StatusUpdated += OnStatus;
                HttpHelper.Instance.DownloadProgressUpdate += OnDownloadProgress;

                for (int i = 0; i < _items.Count; i++)
                {
                    UpdateCancellation.ThrowIfCancellationRequested();
                    AssetPackProgressItem item = _items[i];
                    _currentIndex = i + 1;
                    _downloadProgress.Value = 0;
                    _percentLabel.Text = "0%";
                    _transferDetail.Text = $"Pack {_currentIndex} of {_items.Count} • waiting for download…";
                    _currentItem.Text = $"{item.Owner} • {item.DisplayVariant}";
                    _currentDetail.Text = $"GitHub: {item.Owner}/{item.Repository}\r\nVariant: {item.DisplayVariant}";
                    _positionValue.Text = $"{_currentIndex}/{_items.Count}";
                    UpdateMetrics();
                    SetState($"Downloading {_currentIndex} of {_items.Count}…");
                    AppendLog($"[{_currentIndex}/{_items.Count}] {item.Owner}/{item.Repository} ({item.DisplayVariant})");

                    try
                    {
                        await Task.Run(() => _service.InstallTracked(item.Owner, item.Repository, item.Variant));
                        _completedCount++;
                        _downloadProgress.Value = 100;
                        _percentLabel.Text = "100%";
                        AppendLog($"Completed: {item.Owner}/{item.Repository} ({item.DisplayVariant})");
                    }
                    catch (OperationCanceledException) when (UpdateCancellation.IsCancellationRequested)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        _failedCount++;
                        AppendLog($"FAILED: {item.Owner}/{item.Repository} ({item.DisplayVariant}) — {ex.Message}");
                    }

                    UpdateMetrics();
                    UpdateOverallProgress();
                }

                _overallProgress.Value = 100;
                _overallPercentLabel.Text = "100%";
                _overallDetail.Text = $"All {_items.Count} selected packs processed.";
                SetState(_failedCount == 0 ? "Image-pack operation complete." : "Image-pack operation completed with errors.");
                _currentItem.Text = _failedCount == 0 ? "All selected packs completed" : "Finished with some failures";
                _currentDetail.Text = $"Completed: {_completedCount}\r\nFailed: {_failedCount}\r\nTotal selected: {_items.Count}";
                _completed = true;
                _running = false;
                _stopButton.Enabled = false;
                _stopButton.Text = "Complete";
                _closeButton.Enabled = true;
                _headerCloseButton.Enabled = true;

                string completionMessage = _failedCount == 0
                    ? $"Image-pack operation completed successfully.\n\nCompleted: {_completedCount}\nTotal selected: {_items.Count}"
                    : $"Image-pack operation finished.\n\nCompleted: {_completedCount}\nFailed: {_failedCount}\nTotal selected: {_items.Count}";

                if (_failedCount == 0)
                    ModernDialog.ShowSuccess(this, "Image Packs Complete", completionMessage);
                else
                    ModernDialog.ShowWarning(this, "Image Packs Finished", completionMessage);
            }
            catch (OperationCanceledException) when (UpdateCancellation.IsCancellationRequested)
            {
                AppendLog("Operation stopped by user.");
                UpdateOverallProgress();
                SetState("Image-pack operation stopped.");
                _currentItem.Text = "Stopped";
                _currentDetail.Text = "Any pack that finished before cancellation remains installed.";
                _running = false;
                _stopButton.Enabled = false;
                _stopButton.Text = "Stopped";
                _closeButton.Enabled = true;
                _headerCloseButton.Enabled = true;
            }
            catch (Exception ex)
            {
                AppendLog("ERROR: " + ex.Message);
                UpdateOverallProgress();
                SetState("Image-pack operation stopped because of an error.");
                _currentItem.Text = "Operation failed";
                _currentDetail.Text = ex.Message;
                _running = false;
                _stopButton.Enabled = false;
                _closeButton.Enabled = true;
                _headerCloseButton.Enabled = true;
            }
            finally
            {
                HttpHelper.Instance.DownloadProgressUpdate -= OnDownloadProgress;
                if (_service != null)
                    _service.StatusUpdated -= OnStatus;
                UpdateCancellation.Reset();

                if (_closeWhenStopped && !_running && !IsDisposed)
                    BeginInvoke((Action)Close);
            }
        }

        private void OnStatus(object? sender, StatusUpdatedEventArgs e)
        {
            if (IsDisposed) return;
            BeginInvoke((Action)(() =>
            {
                string message = e.Message ?? string.Empty;
                AppendLog(message);
                string lower = message.ToLowerInvariant();
                if (lower.Contains("downloading image pack"))
                {
                    _downloadProgress.Value = 0;
                    _percentLabel.Text = "0%";
                    SetState($"Downloading pack {_currentIndex} of {_items.Count}…");
                }
                else if (lower.Contains("download complete"))
                {
                    SetState($"Installing pack {_currentIndex} of {_items.Count}…");
                }
                else if (lower.Contains("installing image pack"))
                {
                    SetState($"Installing pack {_currentIndex} of {_items.Count}…");
                }
            }));
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
                _transferDetail.Text = e.TotalBytes > 0
                    ? $"Pack {_currentIndex} of {_items.Count} • {FormatBytes(e.BytesReceived)} / {FormatBytes(e.TotalBytes)}"
                    : $"Pack {_currentIndex} of {_items.Count} • {FormatBytes(e.BytesReceived)} downloaded";
            }));
        }

        private void RequestStop(bool closeAfterStop)
        {
            if (!_running)
            {
                Close();
                return;
            }

            bool stop = ModernDialog.ShowConfirm(this, "Stop Image-Pack Operation",
                "Stop the current image-pack operation?\n\nA download in progress will be cancelled and its partial file removed. Packs that already completed will remain installed.");
            if (!stop) return;

            _closeWhenStopped = closeAfterStop;
            _stopButton.Enabled = false;
            _stopButton.Text = "Stopping…";
            _headerCloseButton.Enabled = false;
            SetState("Stopping after the current operation can safely exit…");
            UpdateCancellation.RequestCancel();
        }

        private void ModernAssetPackProgressForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (!_running) return;
            e.Cancel = true;
            RequestStop(true);
        }

        private void UpdateOverallProgress()
        {
            int processed = Math.Max(0, Math.Min(_items.Count, _completedCount + _failedCount));
            int value = _items.Count <= 0
                ? 100
                : (int)Math.Round(processed * 100d / _items.Count);

            value = Math.Max(0, Math.Min(100, value));
            _overallProgress.Value = value;
            _overallPercentLabel.Text = value + "%";

            if (_items.Count <= 0)
            {
                _overallDetail.Text = "No image packs selected.";
                return;
            }

            if (processed >= _items.Count)
            {
                _overallDetail.Text = $"All {_items.Count} selected packs processed.";
                return;
            }

            _overallDetail.Text = $"{processed} of {_items.Count} packs processed";
        }

        private void UpdateMetrics()
        {
            _completedValue.Text = _completedCount.ToString();
            _failedValue.Text = _failedCount.ToString();
            _remainingValue.Text = Math.Max(0, _items.Count - _completedCount - _failedCount).ToString();
            if (_currentIndex == 0)
                _positionValue.Text = $"0/{_items.Count}";
        }

        private void SetState(string text)
        {
            Control? state = Controls.Find("StateLabel", true).FirstOrDefault();
            if (state is Label label) label.Text = text;
        }

        private void AppendLog(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;
            string line = $"[{DateTime.Now:HH:mm:ss}] {message}";
            _log.AppendText((_log.TextLength > 0 ? Environment.NewLine : string.Empty) + line);
            _log.SelectionStart = _log.TextLength;
            _log.ScrollToCaret();
        }

        private static string FormatBytes(long bytes)
        {
            if (bytes < 1024) return $"{bytes} B";
            double kb = bytes / 1024d;
            if (kb < 1024) return $"{kb:0.0} KB";
            double mb = kb / 1024d;
            if (mb < 1024) return $"{mb:0.0} MB";
            return $"{mb / 1024d:0.00} GB";
        }

        private static string ShortPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return string.Empty;
            if (path.Length <= 52) return path;
            return "…" + path[^49..];
        }

        private static Guna2Panel Card() => new()
        {
            Dock = DockStyle.Fill,
            FillColor = ModernTheme.Surface,
            BorderColor = ModernTheme.Border,
            BorderThickness = 1,
            BorderRadius = 12,
            BackColor = Color.Transparent
        };

        private static Guna2Panel Metric(string text, UiIcon icon, out Label value, Color valueColor, Padding margin)
        {
            var card = Card();
            card.Margin = margin;
            card.Padding = new Padding(12, 10, 12, 8);
            var iconBox = IconTile(icon, valueColor, new Point(0, 0), 36);
            iconBox.Location = new Point(12, 13);
            var caption = MakeLabel(text, 9, FontStyle.Bold, ModernTheme.TextSecondary);
            caption.Location = new Point(58, 11);
            caption.AutoSize = true;
            value = MakeLabel("0", 19, FontStyle.Bold, valueColor);
            value.Location = new Point(58, 34);
            value.AutoSize = true;
            card.Controls.Add(iconBox);
            card.Controls.Add(caption);
            card.Controls.Add(value);
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

        private static Guna2Button Button(string text, int width, bool primary)
        {
            var button = new Guna2Button
            {
                Text = text,
                Width = width,
                Height = 38,
                BorderRadius = 9,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold, GraphicsUnit.Point),
                FillColor = primary ? ModernTheme.Accent : ModernTheme.SurfaceRaised,
                ForeColor = Color.White,
                BorderColor = primary ? ModernTheme.Accent : ModernTheme.BorderStrong,
                BorderThickness = primary ? 0 : 1,
                Cursor = Cursors.Hand
            };
            button.HoverState.FillColor = primary ? ModernTheme.AccentHover : ModernTheme.SurfaceHover;
            return button;
        }

        private static Guna2Button WindowButton(string text) => new()
        {
            Text = text,
            Size = new Size(32, 30),
            BorderRadius = 7,
            FillColor = Color.Transparent,
            ForeColor = ModernTheme.TextSecondary,
            Font = new Font("Segoe UI", 13F, FontStyle.Regular, GraphicsUnit.Point),
            Cursor = Cursors.Hand
        };

        private static Label MakeLabel(string text, float size, FontStyle style, Color color) => new()
        {
            Text = text,
            Font = new Font("Segoe UI", size, style, GraphicsUnit.Point),
            ForeColor = color,
            BackColor = Color.Transparent
        };
    }
}
