using Guna.UI2.WinForms;
using System.Windows.Forms;

namespace Pocket_Updater.UI
{

    internal static class ModernScrollbars
    {
        public static TableLayoutPanel WrapVertical(Control scrollable, int width = 10)
        {
            var host = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.Transparent,
                Margin = new Padding(0),
                Padding = new Padding(0)
            };
            host.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            host.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, width + 6));
            host.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            scrollable.Dock = DockStyle.Fill;
            scrollable.Margin = new Padding(0);

            var bar = new Guna2VScrollBar
            {
                Dock = DockStyle.Fill,
                Width = width,
                Margin = new Padding(5, 2, 1, 2),
                FillColor = ModernTheme.Surface,
                ThumbColor = ModernTheme.BorderStrong,
                BorderColor = ModernTheme.Border,
                Minimum = 0,
                SmallChange = 1,
                LargeChange = 1
            };

            host.Controls.Add(scrollable, 0, 0);
            host.Controls.Add(bar, 1, 0);

            if (scrollable is DataGridView grid)
            {

                grid.ScrollBars = ScrollBars.None;

                bool syncing = false;

                void UpdateRange()
                {
                    if (grid.IsDisposed || bar.IsDisposed) return;

                    int displayed = 1;
                    try
                    {
                        displayed = Math.Max(1, grid.DisplayedRowCount(false));
                    }
                    catch
                    {
                        displayed = Math.Max(1, grid.ClientSize.Height / Math.Max(1, grid.RowTemplate.Height));
                    }

                    int maxFirst = Math.Max(0, grid.RowCount - displayed);
                    bar.Minimum = 0;
                    bar.Maximum = Math.Max(0, maxFirst);
                    bar.LargeChange = 1;
                    bar.SmallChange = 1;
                    bar.Enabled = maxFirst > 0;

                    int first = 0;
                    try
                    {
                        if (grid.RowCount > 0 && grid.FirstDisplayedScrollingRowIndex >= 0)
                            first = Math.Min(maxFirst, grid.FirstDisplayedScrollingRowIndex);
                    }
                    catch { }

                    if (bar.Value != first)
                        bar.Value = Math.Max(bar.Minimum, Math.Min(bar.Maximum, first));
                }

                bar.Scroll += (_, _) =>
                {
                    if (syncing || grid.RowCount == 0) return;
                    syncing = true;
                    try
                    {
                        int target = Math.Max(0, Math.Min(grid.RowCount - 1, bar.Value));
                        grid.FirstDisplayedScrollingRowIndex = target;
                    }
                    catch { }
                    finally { syncing = false; }
                };

                grid.Scroll += (_, _) =>
                {
                    if (syncing) return;
                    syncing = true;
                    try
                    {
                        int first = grid.FirstDisplayedScrollingRowIndex;
                        if (first >= 0)
                            bar.Value = Math.Max(bar.Minimum, Math.Min(bar.Maximum, first));
                    }
                    catch { }
                    finally { syncing = false; }
                };

                void ScrollByWheel(int delta)
                {
                    if (delta == 0 || grid.RowCount == 0) return;

                    int current = 0;
                    try
                    {
                        current = Math.Max(0, grid.FirstDisplayedScrollingRowIndex);
                    }
                    catch { }

                    int lines = SystemInformation.MouseWheelScrollLines;
                    if (lines <= 0) lines = 3;
                    int notches = Math.Max(1, Math.Abs(delta) / 120);
                    int amount = Math.Max(1, lines * notches);
                    int direction = delta > 0 ? -1 : 1;
                    int target = Math.Max(0, Math.Min(grid.RowCount - 1, current + (direction * amount)));

                    syncing = true;
                    try
                    {
                        grid.FirstDisplayedScrollingRowIndex = target;
                        bar.Value = Math.Max(bar.Minimum, Math.Min(bar.Maximum, target));
                    }
                    catch { }
                    finally { syncing = false; }
                }

                grid.MouseEnter += (_, _) =>
                {

                    if (!grid.Focused && grid.CanFocus)
                        grid.Focus();
                };
                grid.MouseWheel += (_, e) => ScrollByWheel(e.Delta);
                host.MouseWheel += (_, e) => ScrollByWheel(e.Delta);

                bool rangeUpdatePending = false;
                void QueueUpdate()
                {
                    if (rangeUpdatePending || host.IsDisposed || !host.IsHandleCreated) return;
                    rangeUpdatePending = true;
                    try
                    {
                        host.BeginInvoke((Action)(() =>
                        {
                            rangeUpdatePending = false;
                            UpdateRange();
                        }));
                    }
                    catch
                    {
                        rangeUpdatePending = false;
                    }
                }

                grid.RowsAdded += (_, _) => QueueUpdate();
                grid.RowsRemoved += (_, _) => QueueUpdate();
                grid.Resize += (_, _) => QueueUpdate();
                host.Resize += (_, _) => QueueUpdate();
                grid.HandleCreated += (_, _) => QueueUpdate();
            }
            else
            {

                host.ColumnStyles[1].Width = 0;
                bar.Visible = false;
            }

            return host;
        }
    }
}
