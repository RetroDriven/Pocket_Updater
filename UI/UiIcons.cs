using System.Drawing.Drawing2D;

namespace Pocket_Updater.UI
{
    internal enum UiIcon
    {
        Home,
        Update,
        Core,
        RomBios,
        AssetPack,
        Logs,
        About,
        Search,
        Refresh,
        Download,
        Folder,
        Check,
        Missing,
        Package,
        Arcade,
        Console,
        Computer,
        Handheld,
        Grid,
        Settings,
        Shield,
        Clock,
        Bolt,
        Trash,
        List
    }

    internal static class UiIcons
    {
        public static Image Get(UiIcon icon, Color color, int size = 24)
        {
            var bitmap = new Bitmap(size, size);
            using var g = Graphics.FromImage(bitmap);
            g.Clear(Color.Transparent);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.CompositingQuality = CompositingQuality.HighQuality;

            float s = size / 24f;
            float X(float v) => v * s;
            using var pen = new Pen(color, Math.Max(1.6f, 1.9f * s))
            {
                StartCap = LineCap.Round,
                EndCap = LineCap.Round,
                LineJoin = LineJoin.Round
            };
            using var thin = new Pen(color, Math.Max(1.2f, 1.5f * s))
            {
                StartCap = LineCap.Round,
                EndCap = LineCap.Round,
                LineJoin = LineJoin.Round
            };
            using var brush = new SolidBrush(color);

            switch (icon)
            {
                case UiIcon.Home:
                    g.DrawLines(pen, new[] { new PointF(X(3), X(11)), new PointF(X(12), X(4)), new PointF(X(21), X(11)) });
                    g.DrawLines(pen, new[] { new PointF(X(5.5f), X(9.5f)), new PointF(X(5.5f), X(20)), new PointF(X(18.5f), X(20)), new PointF(X(18.5f), X(9.5f)) });
                    g.DrawRectangle(thin, X(10), X(14), X(4), X(6));
                    break;

                case UiIcon.Update:
                case UiIcon.Refresh:
                    g.DrawArc(pen, X(4), X(4), X(16), X(16), 205, 245);
                    g.DrawLines(pen, new[] { new PointF(X(17.5f), X(3.8f)), new PointF(X(20.3f), X(7.5f)), new PointF(X(15.7f), X(7.5f)) });
                    if (icon == UiIcon.Update)
                    {
                        g.DrawLine(pen, X(12), X(8), X(12), X(16));
                        g.DrawLines(pen, new[] { new PointF(X(8.8f), X(13)), new PointF(X(12), X(16.2f)), new PointF(X(15.2f), X(13)) });
                    }
                    break;

                case UiIcon.Core:
                    g.DrawRoundedRectangle(pen, new RectangleF(X(6), X(6), X(12), X(12)), X(2));
                    g.DrawRectangle(thin, X(9), X(9), X(6), X(6));
                    for (int i = 0; i < 4; i++)
                    {
                        float p = X(8 + i * 2.6f);
                        g.DrawLine(thin, p, X(3.5f), p, X(6));
                        g.DrawLine(thin, p, X(18), p, X(20.5f));
                        g.DrawLine(thin, X(3.5f), p, X(6), p);
                        g.DrawLine(thin, X(18), p, X(20.5f), p);
                    }
                    break;

                case UiIcon.RomBios:
                case UiIcon.Console:
                    using (var path = new GraphicsPath())
                    {
                        path.AddBezier(
                            new PointF(X(4), X(10)),
                            new PointF(X(5), X(6)),
                            new PointF(X(8), X(6)),
                            new PointF(X(10), X(8)));
                        path.AddLine(new PointF(X(10), X(8)), new PointF(X(14), X(8)));
                        path.AddBezier(
                            new PointF(X(14), X(8)),
                            new PointF(X(16), X(6)),
                            new PointF(X(19), X(6)),
                            new PointF(X(20), X(10)));
                        path.AddBezier(
                            new PointF(X(20), X(10)),
                            new PointF(X(22), X(16)),
                            new PointF(X(19), X(19)),
                            new PointF(X(16.5f), X(16)));
                        path.AddLine(new PointF(X(7.5f), X(16)), new PointF(X(16.5f), X(16)));
                        path.AddBezier(
                            new PointF(X(7.5f), X(16)),
                            new PointF(X(5), X(19)),
                            new PointF(X(3.5f), X(17)),
                            new PointF(X(4), X(10)));
                        g.DrawPath(pen, path);
                    }
                    g.DrawLine(thin, X(8), X(10), X(8), X(14));
                    g.DrawLine(thin, X(6), X(12), X(10), X(12));
                    g.FillEllipse(brush, X(15), X(10.5f), X(2), X(2));
                    g.FillEllipse(brush, X(18), X(12.5f), X(2), X(2));
                    break;

                case UiIcon.AssetPack:
                    g.DrawRoundedRectangle(pen, new RectangleF(X(4), X(5), X(16), X(14)), X(2));
                    g.FillEllipse(brush, X(7), X(8), X(2.5f), X(2.5f));
                    g.DrawLines(thin, new[] { new PointF(X(6), X(17)), new PointF(X(10.5f), X(12.5f)), new PointF(X(13), X(15)), new PointF(X(15.5f), X(12)), new PointF(X(19), X(16)) });
                    break;

                case UiIcon.Logs:
                    g.DrawRoundedRectangle(pen, new RectangleF(X(6), X(3.5f), X(12), X(17)), X(1.5f));
                    g.DrawLine(thin, X(9), X(8), X(15), X(8));
                    g.DrawLine(thin, X(9), X(12), X(15), X(12));
                    g.DrawLine(thin, X(9), X(16), X(14), X(16));
                    break;

                case UiIcon.About:
                    g.DrawEllipse(pen, X(4), X(4), X(16), X(16));
                    g.FillEllipse(brush, X(11), X(7), X(2), X(2));
                    g.DrawLine(pen, X(12), X(11), X(12), X(16));
                    break;

                case UiIcon.Search:
                    g.DrawEllipse(pen, X(4), X(4), X(11), X(11));
                    g.DrawLine(pen, X(14), X(14), X(20), X(20));
                    break;

                case UiIcon.Download:
                    g.DrawLine(pen, X(12), X(4), X(12), X(15));
                    g.DrawLines(pen, new[] { new PointF(X(8), X(11)), new PointF(X(12), X(15)), new PointF(X(16), X(11)) });
                    g.DrawLine(pen, X(5), X(19), X(19), X(19));
                    break;

                case UiIcon.Folder:
                    using (var path = new GraphicsPath())
                    {
                        path.AddLines(new[] { new PointF(X(3.5f), X(7)), new PointF(X(9), X(7)), new PointF(X(11), X(9)), new PointF(X(20.5f), X(9)), new PointF(X(20.5f), X(19)), new PointF(X(3.5f), X(19)) });
                        path.CloseFigure();
                        g.DrawPath(pen, path);
                    }
                    break;

                case UiIcon.Check:
                    g.DrawEllipse(pen, X(3.5f), X(3.5f), X(17), X(17));
                    g.DrawLines(pen, new[] { new PointF(X(7.5f), X(12)), new PointF(X(10.5f), X(15)), new PointF(X(16.5f), X(8.5f)) });
                    break;

                case UiIcon.Missing:
                    g.DrawEllipse(pen, X(3.5f), X(3.5f), X(17), X(17));
                    g.DrawLine(pen, X(8.5f), X(8.5f), X(15.5f), X(15.5f));
                    g.DrawLine(pen, X(15.5f), X(8.5f), X(8.5f), X(15.5f));
                    break;

                case UiIcon.Package:
                    g.DrawLines(pen, new[] { new PointF(X(5), X(8)), new PointF(X(12), X(4)), new PointF(X(19), X(8)), new PointF(X(12), X(12)), new PointF(X(5), X(8)) });
                    g.DrawLines(pen, new[] { new PointF(X(5), X(8)), new PointF(X(5), X(16)), new PointF(X(12), X(20)), new PointF(X(19), X(16)), new PointF(X(19), X(8)) });
                    g.DrawLine(thin, X(12), X(12), X(12), X(20));
                    break;

                case UiIcon.Arcade:
                    g.DrawRoundedRectangle(pen, new RectangleF(X(4), X(5), X(16), X(15)), X(2));
                    g.DrawLine(pen, X(9), X(14), X(9), X(9));
                    g.FillEllipse(brush, X(7.2f), X(7), X(3.6f), X(3.6f));
                    g.FillEllipse(brush, X(14), X(11), X(2.5f), X(2.5f));
                    g.FillEllipse(brush, X(17), X(14), X(2.5f), X(2.5f));
                    break;

                case UiIcon.Computer:
                    g.DrawRoundedRectangle(pen, new RectangleF(X(3.5f), X(4), X(17), X(12)), X(1.5f));
                    g.DrawLine(pen, X(12), X(16), X(12), X(19));
                    g.DrawLine(pen, X(8), X(20), X(16), X(20));
                    break;

                case UiIcon.Handheld:
                    g.DrawRoundedRectangle(pen, new RectangleF(X(6), X(3), X(12), X(18)), X(3));
                    g.DrawRectangle(thin, X(8), X(6), X(8), X(7));
                    g.DrawLine(thin, X(10), X(16), X(10), X(19));
                    g.DrawLine(thin, X(8.5f), X(17.5f), X(11.5f), X(17.5f));
                    g.FillEllipse(brush, X(14), X(16), X(2), X(2));
                    break;

                case UiIcon.Grid:
                    for (int row = 0; row < 2; row++)
                        for (int col = 0; col < 2; col++)
                            g.DrawRoundedRectangle(pen, new RectangleF(X(4 + col * 9), X(4 + row * 9), X(7), X(7)), X(1));
                    break;

                case UiIcon.Settings:
                    g.DrawEllipse(pen, X(8), X(8), X(8), X(8));
                    for (int i = 0; i < 8; i++)
                    {
                        double a = Math.PI * 2 * i / 8.0;
                        float x1 = X(12 + (float)Math.Cos(a) * 6.2f);
                        float y1 = X(12 + (float)Math.Sin(a) * 6.2f);
                        float x2 = X(12 + (float)Math.Cos(a) * 9f);
                        float y2 = X(12 + (float)Math.Sin(a) * 9f);
                        g.DrawLine(pen, x1, y1, x2, y2);
                    }
                    break;

                case UiIcon.Shield:
                    using (var path = new GraphicsPath())
                    {
                        path.AddLines(new[] { new PointF(X(12), X(3)), new PointF(X(19), X(6)), new PointF(X(18), X(14)), new PointF(X(12), X(20)), new PointF(X(6), X(14)), new PointF(X(5), X(6)), new PointF(X(12), X(3)) });
                        g.DrawPath(pen, path);
                    }
                    g.DrawLines(thin, new[] { new PointF(X(8.5f), X(11.5f)), new PointF(X(11), X(14)), new PointF(X(15.5f), X(9)) });
                    break;

                case UiIcon.Clock:
                    g.DrawEllipse(pen, X(4), X(4), X(16), X(16));
                    g.DrawLine(pen, X(12), X(7.5f), X(12), X(12));
                    g.DrawLine(pen, X(12), X(12), X(15.5f), X(14));
                    break;

                case UiIcon.Bolt:
                    g.DrawLines(pen, new[] { new PointF(X(13), X(2.5f)), new PointF(X(7), X(12)), new PointF(X(12), X(12)), new PointF(X(10.5f), X(21)), new PointF(X(17.5f), X(10)), new PointF(X(13), X(10)), new PointF(X(13), X(2.5f)) });
                    break;

                case UiIcon.Trash:
                    g.DrawRoundedRectangle(pen, new RectangleF(X(7), X(8), X(10), X(12)), X(1));
                    g.DrawLine(pen, X(5), X(7), X(19), X(7));
                    g.DrawLine(pen, X(9), X(4.5f), X(15), X(4.5f));
                    g.DrawLine(thin, X(10), X(11), X(10), X(17));
                    g.DrawLine(thin, X(14), X(11), X(14), X(17));
                    break;

                case UiIcon.List:
                    for (int i = 0; i < 3; i++)
                    {
                        float y = X(6 + i * 6);
                        g.FillEllipse(brush, X(4), y - X(1), X(2), X(2));
                        g.DrawLine(pen, X(9), y, X(20), y);
                    }
                    break;
            }

            return bitmap;
        }

        private static void DrawRoundedRectangle(this Graphics g, Pen pen, RectangleF rect, float radius)
        {
            using var path = RoundedRect(rect, radius);
            g.DrawPath(pen, path);
        }

        private static GraphicsPath RoundedRect(RectangleF bounds, float radius)
        {
            float d = radius * 2;
            var path = new GraphicsPath();
            path.AddArc(bounds.Left, bounds.Top, d, d, 180, 90);
            path.AddArc(bounds.Right - d, bounds.Top, d, d, 270, 90);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            path.AddArc(bounds.Left, bounds.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
