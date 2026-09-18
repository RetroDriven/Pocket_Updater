using Pannella;
using Pocket_Updater.Forms.Message_Box;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Text.Json;
using Guna.UI2.WinForms;
using Pocket_Updater.Controls.Modern;
using Pocket_Updater.UI;

namespace Pocket_Updater
{
    public partial class Form1 : Form
    {
        private const string VERSION = "2.0.0";
        private const string API_URL = "https://api.github.com/repos/RetroDriven/Pocket_Updater/releases";
        private const string RELEASE_URL = "https://github.com/RetroDriven/Pocket_Updater/releases/latest";

        private Guna2Panel? _modernHeader;
        private Guna2Panel? _modernSidebar;
        private Guna2Panel? _modernContent;
        private HomeDashboard? _homeDashboard;
        private CoreLibrary? _coreLibrary;
        private RomBiosLibrary? _romBiosLibrary;
        private AssetPackLibrary? _assetPackLibrary;
        private LogViewer? _logViewer;
        private AboutPage? _aboutPage;
        private Guna2ComboBox? _targetSelector;
        private Guna2ComboBox? _targetDriveSelector;
        private Guna2Button? _targetRefreshButton;
        private Label? _targetLabel;
        private Label? _targetDriveLabel;
        private bool _updatingTargetControls;
        private readonly List<Guna2Button> _modernNavButtons = new();
        private readonly System.Windows.Forms.Timer _connectivityTimer = new() { Interval = 5000 };
        private bool _connectivityCheckRunning;

        private void BuildOverhaulShell()
        {
            SuspendLayout();

            BackColor = ModernTheme.AppBackground;
            MinimumSize = new Size(1280, 820);
            Size = new Size(1500, 900);

            Panel_Top.Visible = false;
            Panel_Menu.Visible = false;
            Panel_Main.Visible = false;

            var shell = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                BackColor = ModernTheme.AppBackground
            };
            shell.RowStyles.Add(new RowStyle(SizeType.Absolute, 78F));
            shell.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            _modernHeader = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                FillColor = ModernTheme.TopBar,
                BackColor = ModernTheme.TopBar,
                Margin = Padding.Empty,
                Padding = new Padding(22, 9, 12, 8)
            };

            var body = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                BackColor = ModernTheme.AppBackground
            };
            body.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 220F));
            body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            _modernSidebar = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                FillColor = ModernTheme.Sidebar,
                BackColor = ModernTheme.Sidebar,
                Margin = Padding.Empty,
                Padding = new Padding(14, 18, 14, 14)
            };
            _modernContent = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                FillColor = ModernTheme.AppBackground,
                BackColor = ModernTheme.AppBackground,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };

            body.Controls.Add(_modernSidebar, 0, 0);
            body.Controls.Add(_modernContent, 1, 0);
            shell.Controls.Add(_modernHeader, 0, 0);
            shell.Controls.Add(body, 0, 1);
            Controls.Add(shell);
            shell.BringToFront();

            BuildModernHeader();
            BuildModernSidebar();
            BuildModernPages();

            ResumeLayout(true);

            Shown += async (_, _) =>
            {
                PopulateTargetSelector();
                if (_homeDashboard != null)
                    await _homeDashboard.RefreshAsync();
            };
        }

        private void BuildModernHeader()
        {
            if (_modernHeader == null) return;

            var icon = new Guna2PictureBox
            {
                Image = Properties.Resources.icons8_handheld_game_64,
                SizeMode = PictureBoxSizeMode.Zoom,
                Location = new Point(20, 13),
                Size = new Size(48, 48),
                BackColor = Color.Transparent
            };
            var title = new Label
            {
                Text = "Pocket Updater",
                AutoSize = true,
                Font = new Font("Segoe UI", 19F, FontStyle.Bold, GraphicsUnit.Point),
                ForeColor = ModernTheme.TextPrimary,
                BackColor = Color.Transparent,
                Location = new Point(80, 11)
            };
            var version = new Label
            {
                Text = "v2",
                AutoSize = true,
                Font = new Font("Segoe UI", 13.5F, FontStyle.Bold, GraphicsUnit.Point),
                ForeColor = ModernTheme.AccentHover,
                BackColor = Color.Transparent
            };

            version.Location = new Point(title.Left + title.PreferredWidth - 6, title.Top - 2);

            var tagline = new Label
            {
                Text = "Simple updates. More time to play.",
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point),
                ForeColor = ModernTheme.TextSecondary,
                BackColor = Color.Transparent,
                Location = new Point(82, 47)
            };

            _targetSelector = new Guna2ComboBox
            {
                Width = 168,
                Height = 38,
                BorderRadius = 9,
                FillColor = ModernTheme.SurfaceRaised,
                BorderColor = ModernTheme.BorderStrong,
                ForeColor = ModernTheme.TextPrimary,
                Font = ModernTheme.BodyFont,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            _targetSelector.Items.AddRange(new object[] { "Current Directory", "Removable Storage" });
            _targetSelector.SelectedIndexChanged += TargetSelector_SelectedIndexChanged;

            _targetDriveSelector = new Guna2ComboBox
            {
                Width = 172,
                Height = 38,
                BorderRadius = 9,
                FillColor = ModernTheme.SurfaceRaised,
                BorderColor = ModernTheme.BorderStrong,
                ForeColor = ModernTheme.TextPrimary,
                Font = ModernTheme.BodyFont,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Visible = false
            };
            _targetDriveSelector.SelectedIndexChanged += TargetDriveSelector_SelectedIndexChanged;

            _targetRefreshButton = new Guna2Button
            {
                Width = 38,
                Height = 38,
                BorderRadius = 9,
                FillColor = ModernTheme.Success,
                BorderColor = ModernTheme.Success,
                BorderThickness = 0,
                Image = UiIcons.Get(UiIcon.Refresh, Color.White, 18),
                ImageSize = new Size(18, 18),
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Visible = false
            };
            _targetRefreshButton.HoverState.FillColor = Color.FromArgb(42, 184, 116);
            _targetRefreshButton.PressedColor = Color.FromArgb(35, 158, 99);
            _targetRefreshButton.Click += (_, _) => PopulateTargetDrives(preserveSelection: true, autoSelectFirst: true);

            _targetLabel = new Label
            {
                Text = "Update Location",
                AutoSize = true,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point),
                ForeColor = ModernTheme.TextPrimary,
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            _targetDriveLabel = new Label
            {
                Text = "Drive",
                AutoSize = true,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point),
                ForeColor = ModernTheme.TextPrimary,
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Visible = false
            };

            Update_Available.Parent = _modernHeader;
            Update_Available.AutoRoundedCorners = false;
            Update_Available.BorderRadius = 9;
            Update_Available.Height = 34;
            Update_Available.FillColor = ModernTheme.AccentSubtle;
            Update_Available.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            Update_Available.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            No_Internet.Parent = _modernHeader;
            No_Internet.AutoRoundedCorners = false;
            No_Internet.BorderRadius = 9;
            No_Internet.Height = 34;
            No_Internet.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            var minimize = NewControlBox(Guna.UI2.WinForms.Enums.ControlBoxType.MinimizeBox);
            var maximize = NewControlBox(Guna.UI2.WinForms.Enums.ControlBoxType.MaximizeBox);
            var close = new Guna2ControlBox
            {
                FillColor = Color.Transparent,
                IconColor = ModernTheme.TextSecondary,
                Size = new Size(34, 34),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            close.HoverState.FillColor = ModernTheme.Danger;
            _modernHeader.Controls.Add(icon);
            _modernHeader.Controls.Add(title);
            _modernHeader.Controls.Add(version);
            _modernHeader.Controls.Add(tagline);
            _modernHeader.Controls.Add(_targetLabel);
            _modernHeader.Controls.Add(_targetSelector);
            _modernHeader.Controls.Add(_targetDriveLabel);
            _modernHeader.Controls.Add(_targetDriveSelector);
            _modernHeader.Controls.Add(_targetRefreshButton);
            _modernHeader.Controls.Add(Update_Available);
            _modernHeader.Controls.Add(No_Internet);
            _modernHeader.Controls.Add(minimize);
            _modernHeader.Controls.Add(maximize);
            _modernHeader.Controls.Add(close);

            _modernHeader.Resize += (_, _) =>
            {
                int right = _modernHeader.ClientSize.Width - 14;
                close.Location = new Point(right - 34, 12);
                maximize.Location = new Point(right - 70, 12);
                minimize.Location = new Point(right - 106, 12);
                LayoutTargetPicker();
            };

            var drag = new Guna2DragControl
            {
                TargetControl = _modernHeader,
                UseTransparentDrag = true
            };
        }

        private Guna2ControlBox NewControlBox(Guna.UI2.WinForms.Enums.ControlBoxType type)
        {
            var box = new Guna2ControlBox
            {
                ControlBoxType = type,
                FillColor = Color.Transparent,
                IconColor = ModernTheme.TextSecondary,
                Size = new Size(34, 34),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            box.HoverState.FillColor = ModernTheme.SurfaceHover;
            return box;
        }

        private void BuildModernSidebar()
        {
            if (_modernSidebar == null) return;

            var nav = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 510,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                BackColor = Color.Transparent,
                Padding = new Padding(0),
                AutoScroll = false
            };
            _modernSidebar.Controls.Add(nav);

            AddNavButton(nav, "Home", UiIcon.Home, async () =>
            {
                ShowModernPage(_homeDashboard);
                if (_homeDashboard != null) await _homeDashboard.RefreshAsync();
            });
            AddNavButton(nav, "Cores", UiIcon.Core, async () =>
            {
                ShowModernPage(_coreLibrary);
                if (_coreLibrary != null) await _coreLibrary.RefreshAsync();
            });
            AddNavButton(nav, "ROMs / BIOS", UiIcon.RomBios, async () =>
            {
                ShowModernPage(_romBiosLibrary);
                if (_romBiosLibrary != null) await _romBiosLibrary.RefreshAsync();
            });
            AddNavButton(nav, "Asset Packs", UiIcon.AssetPack, async () =>
            {
                ShowModernPage(_assetPackLibrary);
                if (_assetPackLibrary != null) await _assetPackLibrary.RefreshAsync();
            });
            AddNavButton(nav, "Logs", UiIcon.Logs, () =>
            {
                _logViewer?.RefreshLog();
                ShowModernPage(_logViewer);
            });

            var divider = new Guna2Panel
            {
                Dock = DockStyle.Top,
                Height = 1,
                FillColor = ModernTheme.Border,
                BackColor = ModernTheme.Border,
                Margin = new Padding(4, 14, 4, 14)
            };
            nav.Controls.Add(divider);
            AddNavButton(nav, "About", UiIcon.About, () => ShowModernPage(_aboutPage));

        }

        private void AddNavButton(FlowLayoutPanel nav, string text, UiIcon icon, Action action)
        {
            var button = new Guna2Button
            {
                Text = text,
                ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton,
                Size = new Size(192, 50),
                Margin = new Padding(0, 0, 0, 7),
                BorderRadius = 10,
                FillColor = Color.Transparent,
                ForeColor = ModernTheme.TextSecondary,
                Font = new Font("Segoe UI", 10.8F, FontStyle.Bold, GraphicsUnit.Point),
                TextAlign = HorizontalAlignment.Left,
                TextOffset = new Point(-7, 0),
                Image = UiIcons.Get(icon, Color.FromArgb(184, 202, 230), 22),
                ImageAlign = HorizontalAlignment.Left,
                ImageOffset = new Point(-6, 0),
                ImageSize = new Size(22, 22),
                Cursor = Cursors.Hand
            };
            button.HoverState.FillColor = ModernTheme.SurfaceHover;
            button.HoverState.ForeColor = ModernTheme.TextPrimary;
            button.CheckedState.FillColor = ModernTheme.AccentSubtle;
            button.CheckedState.ForeColor = Color.White;
            button.Click += (_, _) => action();
            nav.Controls.Add(button);
            _modernNavButtons.Add(button);
            if (_modernNavButtons.Count == 1)
                button.Checked = true;
        }

        private void BuildModernPages()
        {
            if (_modernContent == null) return;

            _homeDashboard = new HomeDashboard();
            _coreLibrary = new CoreLibrary();
            _romBiosLibrary = new RomBiosLibrary();
            _assetPackLibrary = new AssetPackLibrary();
            _logViewer = new LogViewer();
            _aboutPage = new AboutPage();

            _homeDashboard.CoresRequested += async (_, _) =>
            {
                SetNavChecked("Cores");
                ShowModernPage(_coreLibrary);
                if (_coreLibrary != null) await _coreLibrary.RefreshAsync();
            };
            _homeDashboard.AssetsRequested += async (_, _) =>
            {
                SetNavChecked("ROMs / BIOS");
                ShowModernPage(_romBiosLibrary);
                if (_romBiosLibrary != null) await _romBiosLibrary.RefreshAsync();
            };
            _homeDashboard.AssetPacksRequested += async (_, _) =>
            {
                SetNavChecked("Asset Packs");
                ShowModernPage(_assetPackLibrary);
                if (_assetPackLibrary != null) await _assetPackLibrary.RefreshAsync();
            };

            Control[] pages =
            {
                _homeDashboard, _coreLibrary, _romBiosLibrary,
                _assetPackLibrary, _logViewer, _aboutPage
            };

            foreach (Control page in pages)
            {
                page.Dock = DockStyle.Fill;
                page.Margin = Padding.Empty;
                page.Visible = false;
                _modernContent.Controls.Add(page);
            }

            ShowModernPage(_homeDashboard);
        }

        private void ShowModernPage(Control? page)
        {
            if (_modernContent == null || page == null) return;
            foreach (Control control in _modernContent.Controls)
                control.Visible = false;
            page.Visible = true;
            page.BringToFront();
        }

        private void SetNavChecked(string text)
        {
            foreach (var button in _modernNavButtons)
                button.Checked = string.Equals(button.Text, text, StringComparison.OrdinalIgnoreCase);
        }

        private void PopulateTargetSelector()
        {
            if (_targetSelector == null) return;

            _updatingTargetControls = true;
            try
            {
                _targetSelector.SelectedIndexChanged -= TargetSelector_SelectedIndexChanged;

                string locationType = PocketTargetContext.SelectedLocationType;
                int modeIndex = _targetSelector.FindStringExact(locationType);
                _targetSelector.SelectedIndex = modeIndex >= 0 ? modeIndex : 0;

                PopulateTargetDrives(
                    preserveSelection: true,
                    autoSelectFirst: string.Equals(locationType, "Removable Storage", StringComparison.OrdinalIgnoreCase));

                UpdateTargetPickerVisibility();
            }
            finally
            {
                _targetSelector.SelectedIndexChanged += TargetSelector_SelectedIndexChanged;
                _updatingTargetControls = false;
            }

            if (string.Equals(PocketTargetContext.SelectedLocationType, "Removable Storage", StringComparison.OrdinalIgnoreCase)
                && _targetDriveSelector?.SelectedItem is PocketTarget selectedDrive)
            {
                PocketTargetContext.SelectRemovablePath(selectedDrive.Path);
            }

            LayoutTargetPicker();
            SyncLegacyTarget();
        }

        private void PopulateTargetDrives(bool preserveSelection, bool autoSelectFirst)
        {
            if (_targetDriveSelector == null) return;

            string previousPath = string.Empty;
            if (preserveSelection && _targetDriveSelector.SelectedItem is PocketTarget current)
                previousPath = current.Path;
            if (string.IsNullOrWhiteSpace(previousPath))
                previousPath = PocketTargetContext.SelectedDrivePath;

            bool oldUpdating = _updatingTargetControls;
            _updatingTargetControls = true;
            try
            {
                _targetDriveSelector.SelectedIndexChanged -= TargetDriveSelector_SelectedIndexChanged;
                _targetDriveSelector.Items.Clear();

                var drives = PocketTargetContext.GetRemovableTargets();
                foreach (var drive in drives)
                    _targetDriveSelector.Items.Add(drive);

                if (drives.Count == 0)
                    _targetDriveSelector.Items.Add("No Drives Found");

                int selectedIndex = -1;
                if (!string.IsNullOrWhiteSpace(previousPath))
                {
                    for (int i = 0; i < drives.Count; i++)
                    {
                        if (PathsEqual(drives[i].Path, previousPath))
                        {
                            selectedIndex = i;
                            break;
                        }
                    }
                }

                if (selectedIndex < 0 && autoSelectFirst && drives.Count > 0)
                    selectedIndex = 0;
                else if (drives.Count == 0)
                    selectedIndex = 0;

                _targetDriveSelector.SelectedIndex = selectedIndex;

                _targetDriveSelector.Enabled = true;
            }
            finally
            {
                _targetDriveSelector.SelectedIndexChanged += TargetDriveSelector_SelectedIndexChanged;
                _updatingTargetControls = oldUpdating;
            }

            if (!_updatingTargetControls
                && string.Equals(_targetSelector?.SelectedItem?.ToString(), "Removable Storage", StringComparison.OrdinalIgnoreCase))
            {
                if (_targetDriveSelector.SelectedItem is PocketTarget selected)
                    PocketTargetContext.SelectRemovablePath(selected.Path);
                else
                    PocketTargetContext.SelectRemovableModeWithoutDrive();

                SyncLegacyTarget();
            }
        }

        private void TargetSelector_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (_updatingTargetControls || _targetSelector == null) return;

            string mode = _targetSelector.SelectedItem?.ToString() ?? "Current Directory";
            if (string.Equals(mode, "Removable Storage", StringComparison.OrdinalIgnoreCase))
            {
                PopulateTargetDrives(preserveSelection: true, autoSelectFirst: true);

                if (_targetDriveSelector?.SelectedItem is PocketTarget selected)
                    PocketTargetContext.SelectRemovablePath(selected.Path);
                else
                    PocketTargetContext.SelectRemovableModeWithoutDrive();
            }
            else
            {
                PocketTargetContext.SelectCurrentDirectory();
            }

            UpdateTargetPickerVisibility();
            LayoutTargetPicker();
            SyncLegacyTarget();
        }

        private void TargetDriveSelector_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (_updatingTargetControls) return;
            if (_targetDriveSelector?.SelectedItem is not PocketTarget target) return;

            PocketTargetContext.SelectRemovablePath(target.Path);
            SyncLegacyTarget();
        }

        private void UpdateTargetPickerVisibility()
        {
            bool removable = string.Equals(
                _targetSelector?.SelectedItem?.ToString(),
                "Removable Storage",
                StringComparison.OrdinalIgnoreCase);

            if (_targetDriveSelector != null) _targetDriveSelector.Visible = removable;
            if (_targetDriveLabel != null) _targetDriveLabel.Visible = removable;
            if (_targetRefreshButton != null) _targetRefreshButton.Visible = removable;
        }

        private void LayoutTargetPicker()
        {
            if (_modernHeader == null || _targetSelector == null) return;

            int right = _modernHeader.ClientSize.Width - 14;
            bool removable = _targetDriveSelector?.Visible == true;

            int controlsLeft = right - 106;
            int contentRight = controlsLeft - 18;
            int gap = 10;
            int compactGap = 8;
            int controlY = Math.Max(0, (_modernHeader.ClientSize.Height - _targetSelector.Height) / 2);

            int locationLabelWidth = _targetLabel?.PreferredWidth ?? 112;
            int driveLabelWidth = _targetDriveLabel?.PreferredWidth ?? 38;

            int groupWidth = locationLabelWidth + gap + _targetSelector.Width;
            if (removable)
            {
                groupWidth += compactGap + driveLabelWidth + compactGap
                    + (_targetDriveSelector?.Width ?? 172) + compactGap
                    + (_targetRefreshButton?.Width ?? 38);
            }

            int groupLeft = contentRight - groupWidth;
            int x = groupLeft;

            if (_targetLabel != null)
            {
                int labelY = (_modernHeader.ClientSize.Height - _targetLabel.PreferredHeight) / 2;
                _targetLabel.Location = new Point(x, labelY);
                x = _targetLabel.Right + gap;
            }

            _targetSelector.Location = new Point(x, controlY);
            x = _targetSelector.Right;

            if (removable && _targetDriveSelector != null)
            {
                x += compactGap;
                if (_targetDriveLabel != null)
                {
                    int driveLabelY = (_modernHeader.ClientSize.Height - _targetDriveLabel.PreferredHeight) / 2;
                    _targetDriveLabel.Location = new Point(x, driveLabelY);
                    x = _targetDriveLabel.Right + compactGap;
                }

                _targetDriveSelector.Location = new Point(x, controlY);
                x = _targetDriveSelector.Right + compactGap;

                if (_targetRefreshButton != null)
                    _targetRefreshButton.Location = new Point(x, controlY);
            }

            int statusCenter = _modernHeader.ClientSize.Width / 2;
            int statusMinLeft = 360;
            int statusGap = 24;

            int CenterStatusBadge(Control badge)
            {
                int centeredLeft = statusCenter - (badge.Width / 2);
                int maxLeft = groupLeft - badge.Width - statusGap;

                if (maxLeft < statusMinLeft)
                    return Math.Max(12, maxLeft);

                return Math.Max(statusMinLeft, Math.Min(centeredLeft, maxLeft));
            }

            Update_Available.Location = new Point(CenterStatusBadge(Update_Available), controlY + 2);
            No_Internet.Location = new Point(CenterStatusBadge(No_Internet), controlY + 2);
        }

        private static bool PathsEqual(string first, string second)
        {
            try
            {
                string a = Path.GetFullPath(first).TrimEnd('\\', '/');
                string b = Path.GetFullPath(second).TrimEnd('\\', '/');
                return string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return string.Equals(first?.Trim(), second?.Trim(), StringComparison.OrdinalIgnoreCase);
            }
        }

        private void SyncLegacyTarget()
        {
            try
            {
                update_Pocket1.SelectUpdateTarget(PocketTargetContext.SelectedPath);
                image_Packs1.SelectUpdateTarget(PocketTargetContext.SelectedPath);
            }
            catch
            {

            }
        }

        private void LoadLogsIntoLegacyPage()
        {
            string logFile = Path.Combine(Directory.GetCurrentDirectory(), "Pocket_Updater_Log.txt");
            if (File.Exists(logFile))
            {
                logs1.textBox1.Text = File.ReadAllText(logFile);
                logs1.textBox1.SelectionStart = logs1.textBox1.Text.Length;
                logs1.textBox1.ScrollToCaret();
                logs1.Clear.Enabled = true;
            }
            else
            {
                logs1.textBox1.Text = "No Log File Found!";
                logs1.Clear.Enabled = false;
            }
        }

        public Form1()
        {
            InitializeComponent();
            Text = "Pocket Updater";
            BuildOverhaulShell();

            if (Check_Internet())
            {
                //Check for App Updates
                try
                {
                    using (WebClient client2 = new WebClient())
                    {
                        _ = CheckVersion_Load();
                    }
                }
                catch
                {
                    Message_Box form = new Message_Box();
                    form.label1.Text = "Failed to check for App Updates!";
                    form.Show();
                }
            }
            else
            {
                No_Internet.Visible = true;
            }

            _connectivityTimer.Tick += async (_, _) => await RefreshConnectivityBadgeAsync();
            _connectivityTimer.Start();
            FormClosed += (_, _) => _connectivityTimer.Stop();

        }

        private void ApplyModernShell()
        {
            BackColor = UI.ModernTheme.AppBackground;
            MinimumSize = new Size(1180, 700);

            Panel_Top.AutoRoundedCorners = false;
            Panel_Top.BorderRadius = 0;
            Panel_Top.Height = 64;
            Panel_Top.FillColor = UI.ModernTheme.TopBar;
            Panel_Top.BackColor = UI.ModernTheme.TopBar;
            Panel_Top.BorderColor = UI.ModernTheme.Border;
            Panel_Top.BorderThickness = 0;

            label1.Text = "Pocket Updater";
            label1.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point);
            label1.ForeColor = UI.ModernTheme.TextPrimary;
            label1.Location = new Point(20, 14);

            var versionLabel = new Label
            {
                AutoSize = true,
                Text = "v2  •  Keep your retro gaming library up to date",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point),
                ForeColor = UI.ModernTheme.TextSecondary,
                BackColor = Color.Transparent,
                Location = new Point(205, 23)
            };
            Panel_Top.Controls.Add(versionLabel);
            versionLabel.BringToFront();

            foreach (var controlBox in new[] { guna2ControlBox1, guna2ControlBox2, guna2ControlBox3 })
            {
                controlBox.AutoRoundedCorners = false;
                controlBox.FillColor = Color.Transparent;
                controlBox.IconColor = UI.ModernTheme.TextSecondary;
                controlBox.HoverState.FillColor = UI.ModernTheme.SurfaceHover;
                controlBox.Size = new Size(34, 34);
            }
            guna2ControlBox3.HoverState.FillColor = UI.ModernTheme.Danger;

            Update_Available.AutoRoundedCorners = false;
            Update_Available.BorderRadius = 9;
            Update_Available.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold, GraphicsUnit.Point);
            Update_Available.FillColor = UI.ModernTheme.AccentSubtle;
            Update_Available.HoverState.FillColor = UI.ModernTheme.Accent;
            Update_Available.ForeColor = UI.ModernTheme.TextPrimary;
            Update_Available.Height = 32;

            No_Internet.AutoRoundedCorners = false;
            No_Internet.BorderRadius = 9;
            No_Internet.FillColor = Color.FromArgb(82, 31, 43);
            No_Internet.DisabledState.FillColor = Color.FromArgb(82, 31, 43);
            No_Internet.DisabledState.ForeColor = Color.FromArgb(255, 205, 211);
            No_Internet.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold, GraphicsUnit.Point);
            No_Internet.Height = 32;

            Panel_Menu.AutoSize = false;
            Panel_Menu.Width = 246;
            Panel_Menu.BackColor = UI.ModernTheme.Sidebar;
            flowLayoutPanel1.AutoSize = false;
            flowLayoutPanel1.BackColor = UI.ModernTheme.Sidebar;
            flowLayoutPanel1.Padding = new Padding(12, 12, 12, 0);
            flowLayoutPanel1.WrapContents = false;

            pictureBox1.BackColor = UI.ModernTheme.Sidebar;
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Size = new Size(222, 70);
            pictureBox1.Margin = new Padding(0, 8, 0, 14);
            pictureBox1.Padding = new Padding(82, 6, 82, 6);

            var navButtons = new[] { Update_Pocket, Manage_Cores, Organize_Cores, Image_Packs, Logs, About };
            foreach (var button in navButtons)
            {
                button.AutoRoundedCorners = false;
                button.BorderRadius = 11;
                button.Size = new Size(222, 50);
                button.Margin = new Padding(0, 4, 0, 4);
                button.FillColor = Color.Transparent;
                button.HoverState.FillColor = UI.ModernTheme.SurfaceHover;
                button.CheckedState.FillColor = UI.ModernTheme.Accent;
                button.CheckedState.ForeColor = Color.White;
                button.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold, GraphicsUnit.Point);
                button.TextOffset = new Point(48, 0);
                button.CustomImages.ImageOffset = new Point(12, 0);
                button.CustomImages.ImageSize = new Size(28, 28);
            }

            Panel_Main.BackColor = UI.ModernTheme.AppBackground;
            Panel_Main.Padding = new Padding(0);

            foreach (Control page in Panel_Main.Controls)
            {
                page.BackColor = UI.ModernTheme.AppBackground;
                page.Margin = Padding.Empty;
            }
            update_Pocket1.AutoSize = false;

        }

        private async Task RefreshConnectivityBadgeAsync()
        {
            if (_connectivityCheckRunning || IsDisposed || Disposing)
                return;

            _connectivityCheckRunning = true;
            try
            {
                bool online = await HasInternetConnectionAsync();
                if (IsDisposed || Disposing)
                    return;

                No_Internet.Visible = !online;
                LayoutTargetPicker();
            }
            finally
            {
                _connectivityCheckRunning = false;
            }
        }

        private static async Task<bool> HasInternetConnectionAsync()
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
                return false;

            try
            {
                using var client = new HttpClient
                {
                    Timeout = TimeSpan.FromSeconds(4)
                };
                using var request = new HttpRequestMessage(HttpMethod.Get, "https://www.google.com/generate_204");
                using HttpResponseMessage response = await client.SendAsync(
                    request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task CheckVersion_Load()
        {
            using (WebClient client = new WebClient())
            {
                if (await CheckVersion())
                {
                    try
                    {
                        Update_Available.Visible = true;
                    }
                    catch
                    {
                        No_Internet.Visible = true;
                    }
                }
            }

        }
        async static Task<bool> CheckVersion()
        {
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(API_URL)

                };
                var agent = new ProductInfoHeaderValue("Pocket-Updater", "1.0");
                request.Headers.UserAgent.Add(agent);
                var response = await client.SendAsync(request).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                List<Pannella.Models.Github.Release>? releases = JsonSerializer.Deserialize<List<Pannella.Models.Github.Release>>(responseBody);

                string tag_name = releases[0].tag_name;
                string? v = Pannella.Helpers.SemverUtil.FindSemver(tag_name);
                if (v != null)
                {
                    return Pannella.Helpers.SemverUtil.SemverCompare(v, VERSION);
                    //return SemverUtil.SemverCompare(v, "1.0");

                }
                return false;
            }
            catch (HttpRequestException e)
            {
                Message_Box form = new Message_Box();
                form.label1.Text = e.ToString();
                form.Show();
                return false;

            }
        }
        private void Update_Pocket_Click(object sender, EventArgs e)
        {
            Hide_Controls();
            update_Pocket1.Visible = true;

        }
        private void Manage_Cores_Click(object sender, EventArgs e)
        {
            Hide_Controls();
            manageCores1.Visible = true;
            manageCores1.Enabled = true;
        }

        private void Hide_Controls()
        {
            update_Pocket1.Visible = false;
            manageCores1.Visible = false;
            organize_Cores1.Visible = false;
            image_Packs1.Visible = false;
            logs1.Visible = false;
            about1.Visible = false;
        }

        private void Organize_Cores_Click(object sender, EventArgs e)
        {
            Hide_Controls();
            organize_Cores1.Visible = true;
        }

        private void Image_Packs_Click(object sender, EventArgs e)
        {
            Hide_Controls();
            image_Packs1.Visible = true;
        }
        private void Logs_Click(object sender, EventArgs e)
        {
            Hide_Controls();
            string Current_Dir = Directory.GetCurrentDirectory();
            string LogFile = Current_Dir + "\\Pocket_Updater_Log.txt";

            if (File.Exists(LogFile))
            {
                logs1.textBox1.Text = File.ReadAllText(Current_Dir + "\\Pocket_Updater_Log.txt");
                logs1.textBox1.SelectionStart = logs1.textBox1.Text.Length;
                logs1.textBox1.ScrollToCaret();
                logs1.textBox1.Refresh();
                logs1.textBox1.Select();
            }
            else
            {
                logs1.textBox1.Text = "No Log File Found!";
                logs1.Clear.Enabled = false;
            }
            logs1.Visible = true;
        }

        private void Update_Available_Click(object sender, EventArgs e)
        {
            Process.Start("explorer", RELEASE_URL);
        }

        private void About_Click(object sender, EventArgs e)
        {
            Hide_Controls();
            about1.Visible = true;
        }
        public static bool Check_Internet()
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (var stream = client.OpenRead("http://www.google.com"))
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
