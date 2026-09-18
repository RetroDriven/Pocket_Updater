using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO.Compression;
using System.Runtime.InteropServices;
using Guna.UI2.WinForms;
using Pannella.Helpers;
using Pannella.Models;
using Pannella.Models.Github;
using Pannella.Services;
using IOFile = System.IO.File;

namespace Pocket_Updater.UI
{
    internal sealed class PlatformArtworkPreview
    {
        public Bitmap Image { get; init; } = null!;
        public string PlatformId { get; init; } = string.Empty;
        public string SourceText { get; init; } = string.Empty;
    }

    internal static class PlatformArtworkPreviewService
    {
        private const int PocketWidth = 521;
        private const int PocketHeight = 165;
        private const int StoredWidth = PocketHeight;
        private const int StoredHeight = PocketWidth;
        private const int BytesPerPixel = 2;
        private const string PreferredPreviewOwner = "dyreschlock";
        private const string PreferredPreviewRepository = "pocket-platform-images";

        private sealed class CacheEntry
        {
            public DateTime LastWriteUtc { get; init; }
            public Bitmap Bitmap { get; init; } = null!;
        }

        private static readonly object CacheSync = new();
        private static readonly Dictionary<string, CacheEntry> Cache = new(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, Bitmap?> RemotePreviewCache = new(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, string> RemoteZipCache = new(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, IReadOnlyList<Bitmap>> PackSampleCache = new(StringComparer.OrdinalIgnoreCase);
        private static readonly HashSet<string> MissingRemotePlatforms = new(StringComparer.OrdinalIgnoreCase);

        public static PlatformArtworkPreview? TryLoad(CoreLibraryItem item, string targetPath)
        {
            if (item == null || string.IsNullOrWhiteSpace(targetPath) || string.IsNullOrWhiteSpace(item.PlatformId))
                return null;

            string platformId = item.PlatformId.Trim();
            string imagePath = Path.Combine(targetPath, "Platforms", "_images", platformId + ".bin");

            bool preferredInstalled = IsPreferredPackInstalledForPlatform(platformId);
            if (preferredInstalled)
            {
                PlatformArtworkPreview? preferredInstalledPreview = TryLoadFromPreferredPack(platformId, item.Category);
                if (preferredInstalledPreview != null)
                    return new PlatformArtworkPreview
                    {
                        Image = preferredInstalledPreview.Image,
                        PlatformId = preferredInstalledPreview.PlatformId,
                        SourceText = $"Artwork pack: {PreferredPreviewOwner} • installed"
                    };
            }

            if (IOFile.Exists(imagePath))
            {
                try
                {
                    Bitmap bitmap = GetOrDecode(imagePath);
                    return new PlatformArtworkPreview
                    {
                        Image = bitmap,
                        PlatformId = platformId,
                        SourceText = FindTrackedPackSource(platformId)
                    };
                }
                catch
                {

                }
            }

            return TryLoadFromPreferredPack(platformId, item.Category);
        }

        private static bool IsPreferredPackInstalledForPlatform(string platformId)
        {
            try
            {
                string expectedName = platformId + ".bin";
                return ServiceHelper.PlatformImagePacksService?
                    .GetInstalledPacks()
                    .Any(record =>
                        string.Equals(record.owner, PreferredPreviewOwner, StringComparison.OrdinalIgnoreCase)
                        && string.Equals(record.repository, PreferredPreviewRepository, StringComparison.OrdinalIgnoreCase)
                        && record.files?.Any(file =>
                            string.Equals(Path.GetFileName(file.Replace('/', Path.DirectorySeparatorChar)), expectedName, StringComparison.OrdinalIgnoreCase)) == true)
                    == true;
            }
            catch
            {
                return false;
            }
        }

        private static PlatformArtworkPreview? TryLoadFromPreferredPack(string platformId, string category)
        {
            if (string.IsNullOrWhiteSpace(platformId))
                return null;

            lock (CacheSync)
            {
                if (MissingRemotePlatforms.Contains(platformId))
                    return null;
            }

            try
            {
                PlatformImagePacksService? service = ServiceHelper.PlatformImagePacksService;
                List<PlatformImagePack> preferredPacks = service?.List?
                    .Where(pack => string.Equals(pack.owner, PreferredPreviewOwner, StringComparison.OrdinalIgnoreCase)
                        && string.Equals(pack.repository, PreferredPreviewRepository, StringComparison.OrdinalIgnoreCase))
                    .OrderBy(pack => PreferredVariantRank(pack.variant, category))
                    .ThenBy(pack => pack.variant ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                    .ToList()
                    ?? new List<PlatformImagePack>();

                foreach (PlatformImagePack pack in preferredPacks)
                {
                    string variant = string.IsNullOrWhiteSpace(pack.variant) ? string.Empty : pack.variant.Trim();
                    Bitmap? bitmap = TryLoadRemotePreview(pack.owner ?? string.Empty, pack.repository ?? string.Empty, variant, platformId);
                    if (bitmap == null)
                        continue;

                    return new PlatformArtworkPreview
                    {
                        Image = bitmap,
                        PlatformId = platformId,
                        SourceText = $"Preview from {PreferredPreviewOwner} • {DisplayVariant(variant)}"
                    };
                }
            }
            catch
            {

            }

            lock (CacheSync)
            {
                MissingRemotePlatforms.Add(platformId);
            }

            return null;
        }

        private static Bitmap? TryLoadRemotePreview(string owner, string repository, string variant, string platformId)
        {
            string cacheKey = $"{owner}/{repository}/{variant}|{platformId}";
            lock (CacheSync)
            {
                if (RemotePreviewCache.TryGetValue(cacheKey, out Bitmap? cached))
                    return cached;
            }

            try
            {
                string? zipPath = GetOrDownloadPreviewZip(owner, repository, variant);
                if (string.IsNullOrWhiteSpace(zipPath) || !IOFile.Exists(zipPath))
                {
                    lock (CacheSync)
                    {
                        RemotePreviewCache[cacheKey] = null;
                    }

                    return null;
                }

                using FileStream stream = IOFile.OpenRead(zipPath);
                using var archive = new ZipArchive(stream, ZipArchiveMode.Read, false);
                string expected = $"Platforms/_images/{platformId}.bin";
                ZipArchiveEntry? entry = archive.Entries.FirstOrDefault(candidate =>
                    string.Equals(NormalizeZipPath(candidate.FullName), expected, StringComparison.OrdinalIgnoreCase)
                    || NormalizeZipPath(candidate.FullName).EndsWith("/" + expected, StringComparison.OrdinalIgnoreCase));

                if (entry == null)
                {
                    lock (CacheSync)
                    {
                        RemotePreviewCache[cacheKey] = null;
                    }

                    return null;
                }

                using Stream entryStream = entry.Open();
                using var memory = new MemoryStream();
                entryStream.CopyTo(memory);
                Bitmap bitmap = DecodePlatformBin(memory.ToArray());

                lock (CacheSync)
                {
                    RemotePreviewCache[cacheKey] = bitmap;
                }

                return bitmap;
            }
            catch
            {
                lock (CacheSync)
                {
                    RemotePreviewCache[cacheKey] = null;
                }

                return null;
            }
        }

        internal static string? GetOrDownloadPreviewZip(string owner, string repository, string variant)
        {
            string token = ServiceHelper.SettingsService?.GetConfig()?.github_token;
            Release release = GithubApiService.GetLatestRelease(owner, repository, token);
            Asset? asset = SelectReleaseAsset(release, variant);
            if (asset == null || string.IsNullOrWhiteSpace(asset.browser_download_url))
                return null;

            string version = !string.IsNullOrWhiteSpace(release.tag_name)
                ? release.tag_name.Trim()
                : (!string.IsNullOrWhiteSpace(release.name) ? release.name.Trim() : "latest");
            string fileKey = SanitizeCacheKey($"{owner}_{repository}_{variant}_{version}");

            lock (CacheSync)
            {
                if (RemoteZipCache.TryGetValue(fileKey, out string? existing) && IOFile.Exists(existing))
                    return existing;
            }

            string cacheDirectory = Path.Combine(ServiceHelper.TempDirectory ?? Path.GetTempPath(), "pocket_updater_artwork_cache");
            Directory.CreateDirectory(cacheDirectory);
            string destination = Path.Combine(cacheDirectory, fileKey + ".zip");

            if (!IOFile.Exists(destination))
                HttpHelper.Instance.DownloadFile(asset.browser_download_url, destination, 120);

            lock (CacheSync)
            {
                RemoteZipCache[fileKey] = destination;
            }

            return destination;
        }

        private static Asset? SelectReleaseAsset(Release? release, string variant)
        {
            if (release?.assets == null || release.assets.Count == 0)
                return null;

            if (!string.IsNullOrWhiteSpace(variant))
            {
                Asset? exact = release.assets.FirstOrDefault(asset =>
                    !string.IsNullOrWhiteSpace(asset?.name)
                    && asset.name.EndsWith($"{variant}.zip", StringComparison.OrdinalIgnoreCase));
                if (exact != null)
                    return exact;
            }

            return release.assets.FirstOrDefault(asset =>
                !string.IsNullOrWhiteSpace(asset?.name)
                && asset.name.EndsWith(".zip", StringComparison.OrdinalIgnoreCase));
        }

        private static string NormalizeZipPath(string path) => (path ?? string.Empty).Replace('\\', '/').TrimStart('/');

        private static int PreferredVariantRank(string? variant, string? category)
        {
            string normalized = string.IsNullOrWhiteSpace(variant) ? string.Empty : variant.Trim().ToLowerInvariant();
            bool arcade = string.Equals(category, "Arcade", StringComparison.OrdinalIgnoreCase);

            if (arcade)
            {
                return normalized switch
                {
                    "arcade" => 0,
                    "home" => 5,
                    "home_jp_alts" => 6,
                    "home_pal_alts" => 7,
                    _ => 10
                };
            }

            return normalized switch
            {
                "home" => 0,
                "home_jp_alts" => 1,
                "home_pal_alts" => 2,
                "arcade" => 8,
                _ => 10
            };
        }

        internal static IReadOnlyList<Bitmap> TryLoadRandomPackImages(string owner, string repository, string variant, int count = 3)
        {
            if (string.IsNullOrWhiteSpace(owner) || string.IsNullOrWhiteSpace(repository) || count <= 0)
                return Array.Empty<Bitmap>();

            string key = $"{owner}/{repository}/{variant}";
            lock (CacheSync)
            {
                if (PackSampleCache.TryGetValue(key, out IReadOnlyList<Bitmap>? cached))
                    return cached;
            }

            try
            {
                string? zipPath = GetOrDownloadPreviewZip(owner, repository, variant);
                if (string.IsNullOrWhiteSpace(zipPath) || !IOFile.Exists(zipPath))
                    return Array.Empty<Bitmap>();

                using FileStream stream = IOFile.OpenRead(zipPath);
                using var archive = new ZipArchive(stream, ZipArchiveMode.Read, false);
                List<ZipArchiveEntry> entries = archive.Entries
                    .Where(entry =>
                    {
                        string name = NormalizeZipPath(entry.FullName);
                        return name.EndsWith(".bin", StringComparison.OrdinalIgnoreCase)
                            && name.Contains("Platforms/_images/", StringComparison.OrdinalIgnoreCase);
                    })
                    .ToList();

                if (entries.Count == 0)
                    return Array.Empty<Bitmap>();

                var random = new Random(HashCode.Combine(key, Environment.TickCount));
                for (int i = entries.Count - 1; i > 0; i--)
                {
                    int j = random.Next(i + 1);
                    (entries[i], entries[j]) = (entries[j], entries[i]);
                }

                var images = new List<Bitmap>();
                foreach (ZipArchiveEntry entry in entries.Take(Math.Min(count, entries.Count)))
                {
                    try
                    {
                        using Stream entryStream = entry.Open();
                        using var memory = new MemoryStream();
                        entryStream.CopyTo(memory);
                        images.Add(DecodePlatformBin(memory.ToArray()));
                    }
                    catch
                    {

                    }
                }

                IReadOnlyList<Bitmap> result = images;
                lock (CacheSync)
                {
                    PackSampleCache[key] = result;
                }
                return result;
            }
            catch
            {
                return Array.Empty<Bitmap>();
            }
        }

        private static string DisplayVariant(string? variant) => string.IsNullOrWhiteSpace(variant) ? "default" : variant.Trim();

        private static string SanitizeCacheKey(string value)
        {
            char[] chars = value
                .Select(ch => char.IsLetterOrDigit(ch) || ch == '-' || ch == '_' ? ch : '_')
                .ToArray();
            return new string(chars);
        }

        private static Bitmap GetOrDecode(string path)
        {
            DateTime stamp = IOFile.GetLastWriteTimeUtc(path);
            lock (CacheSync)
            {
                if (Cache.TryGetValue(path, out CacheEntry? cached) && cached.LastWriteUtc == stamp)
                    return cached.Bitmap;

                Bitmap decoded = DecodePlatformBin(path);
                if (cached != null)
                    cached.Bitmap.Dispose();

                Cache[path] = new CacheEntry { LastWriteUtc = stamp, Bitmap = decoded };
                return decoded;
            }
        }

        private static Bitmap DecodePlatformBin(string path) => DecodePlatformBin(IOFile.ReadAllBytes(path));

        internal static Bitmap DecodePlatformBin(byte[] data)
        {
            int expected = StoredWidth * StoredHeight * BytesPerPixel;
            if (data.Length < expected)
                throw new InvalidDataException("Platform artwork file is smaller than the documented 521x165 image payload.");

            var bitmap = new Bitmap(PocketWidth, PocketHeight, PixelFormat.Format24bppRgb);
            Rectangle bounds = new(0, 0, bitmap.Width, bitmap.Height);
            BitmapData locked = bitmap.LockBits(bounds, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            try
            {
                int stride = locked.Stride;
                byte[] pixels = new byte[stride * PocketHeight];

                int intensityByteOffset = DetectIntensityByteOffset(data);
                for (int storedY = 0; storedY < StoredHeight; storedY++)
                {
                    for (int storedX = 0; storedX < StoredWidth; storedX++)
                    {
                        int sourceOffset = ((storedY * StoredWidth) + storedX) * BytesPerPixel;
                        int intensity = 255 - data[sourceOffset + intensityByteOffset];

                        int x = PocketWidth - 1 - storedY;
                        int y = storedX;
                        int destinationOffset = (y * stride) + (x * 3);
                        byte shade = (byte)Math.Clamp(intensity, 0, 255);
                        pixels[destinationOffset] = shade;
                        pixels[destinationOffset + 1] = shade;
                        pixels[destinationOffset + 2] = shade;
                    }
                }

                Marshal.Copy(pixels, 0, locked.Scan0, pixels.Length);
            }
            finally
            {
                bitmap.UnlockBits(locked);
            }

            return bitmap;
        }

        private static int DetectIntensityByteOffset(byte[] data)
        {
            int min0 = 255;
            int max0 = 0;
            int min1 = 255;
            int max1 = 0;
            long activity0 = 0;
            long activity1 = 0;

            int length = Math.Min(data.Length, StoredWidth * StoredHeight * BytesPerPixel);
            for (int i = 0; i + 1 < length; i += BytesPerPixel)
            {
                byte first = data[i];
                byte second = data[i + 1];

                min0 = Math.Min(min0, first);
                max0 = Math.Max(max0, first);
                min1 = Math.Min(min1, second);
                max1 = Math.Max(max1, second);
                activity0 += first;
                activity1 += second;
            }

            int range0 = max0 - min0;
            int range1 = max1 - min1;
            if (range0 != range1)
                return range0 > range1 ? 0 : 1;

            return activity0 >= activity1 ? 0 : 1;
        }

        private static string FindTrackedPackSource(string platformId)
        {
            try
            {
                string expectedName = platformId + ".bin";
                InstalledImagePackRecord? match = ServiceHelper.PlatformImagePacksService?
                    .GetInstalledPacks()
                    .Where(record => record.files?.Any(file =>
                        string.Equals(Path.GetFileName(file.Replace('/', Path.DirectorySeparatorChar)), expectedName, StringComparison.OrdinalIgnoreCase)) == true)
                    .OrderByDescending(record => record.installed_utc)
                    .FirstOrDefault();

                if (match != null)
                {
                    string variant = string.IsNullOrWhiteSpace(match.variant) ? "default" : match.variant;
                    return $"Artwork pack: {match.owner} • {variant}";
                }
            }
            catch
            {

            }

            return "Installed platform artwork";
        }
    }

    internal sealed class CoreArtworkPreviewPopup : Form
    {
        private const int CornerRadius = 12;
        private static readonly Color PopupBackground = Color.FromArgb(8, 13, 23);
        private static readonly Color PopupBorder = Color.FromArgb(54, 74, 102);
        private readonly Guna2PictureBox _image;
        private readonly Label _title;
        private readonly Label _source;

        public CoreArtworkPreviewPopup()
        {
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            ShowInTaskbar = false;
            TopMost = true;
            BackColor = PopupBackground;
            Size = new Size(660, 255);

            Resize += (_, _) => ApplyRoundedRegion();
            Shown += (_, _) => ApplyRoundedRegion();

            var borderless = new Guna2BorderlessForm
            {
                ContainerControl = this,
                BorderRadius = CornerRadius,
                TransparentWhileDrag = false
            };

            var card = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                FillColor = PopupBackground,
                BorderColor = PopupBorder,
                BorderThickness = 1,
                BorderRadius = 12,
                Padding = new Padding(12),
                BackColor = PopupBackground
            };
            Controls.Add(card);

            _image = new Guna2PictureBox
            {
                Location = new Point(12, 12),
                Size = new Size(636, 201),
                BackColor = Color.Black,
                BorderRadius = 8,
                SizeMode = PictureBoxSizeMode.Zoom
            };
            card.Controls.Add(_image);

            _title = new Label
            {
                Location = new Point(14, 218),
                Size = new Size(628, 20),
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold, GraphicsUnit.Point),
                ForeColor = ModernTheme.TextPrimary,
                BackColor = Color.Transparent,
                AutoEllipsis = true
            };
            card.Controls.Add(_title);

            _source = new Label
            {
                Location = new Point(14, 238),
                Size = new Size(628, 17),
                Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point),
                ForeColor = ModernTheme.TextSecondary,
                BackColor = Color.Transparent,
                AutoEllipsis = true
            };
            card.Controls.Add(_source);
        }

        private void ApplyRoundedRegion()
        {
            if (ClientSize.Width <= 1 || ClientSize.Height <= 1)
                return;

            int diameter = CornerRadius * 2;
            using var path = new GraphicsPath();
            path.StartFigure();
            path.AddArc(0, 0, diameter, diameter, 180, 90);
            path.AddArc(ClientSize.Width - diameter - 1, 0, diameter, diameter, 270, 90);
            path.AddArc(ClientSize.Width - diameter - 1, ClientSize.Height - diameter - 1, diameter, diameter, 0, 90);
            path.AddArc(0, ClientSize.Height - diameter - 1, diameter, diameter, 90, 90);
            path.CloseFigure();

            Region? oldRegion = Region;
            Region = new Region(path);
            oldRegion?.Dispose();
        }

        protected override bool ShowWithoutActivation => true;

        protected override CreateParams CreateParams
        {
            get
            {
                const int WsExNoActivate = 0x08000000;
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= WsExNoActivate;
                return cp;
            }
        }

        public void ShowPreview(Control owner, CoreLibraryItem item, PlatformArtworkPreview preview, Rectangle anchorScreen)
        {
            _image.Image = preview.Image;
            _title.Text = string.IsNullOrWhiteSpace(preview.PlatformId)
                ? item.Name
                : $"{item.Name}  •  {preview.PlatformId}";
            _source.Text = preview.SourceText;

            Rectangle work = Screen.FromRectangle(anchorScreen).WorkingArea;
            int x = anchorScreen.Left;
            int y = anchorScreen.Top - Height - 8;
            if (y < work.Top + 8)
                y = anchorScreen.Bottom + 8;
            if (x + Width > work.Right - 8)
                x = work.Right - Width - 8;
            if (x < work.Left + 8)
                x = work.Left + 8;
            if (y + Height > work.Bottom - 8)
                y = Math.Max(work.Top + 8, work.Bottom - Height - 8);

            Location = new Point(x, y);
            if (!Visible)
            {
                Form? ownerForm = owner.FindForm();
                if (ownerForm != null)
                    Show(ownerForm);
                else
                    Show();
            }
            else
            {
                Invalidate();
            }
        }
    }
}
