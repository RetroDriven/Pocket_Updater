using Newtonsoft.Json;
using Pannella.Helpers;
using Pannella.Models;
using Pannella.Models.Analogue.Instance;
using Pannella.Models.OpenFPGA_Cores_Inventory;
using Pannella.Services;
using AnalogueCore = Pannella.Models.Analogue.Core.Core;

namespace Pocket_Updater.UI
{
    internal enum CoreLibraryStatus
    {
        Installed,
        Missing,
        UpdateAvailable
    }

    internal sealed class CoreLibraryItem
    {
        public Core Core { get; init; } = null!;
        public string Identifier => Core.identifier ?? string.Empty;
        public string Name => Core.platform?.name ?? Core.identifier ?? "Unknown";
        public string Developer
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Core.identifier) || !Core.identifier.Contains('.'))
                    return Core.repository?.owner ?? "Unknown";
                return Core.identifier.Split('.')[0];
            }
        }
        public string Category { get; init; } = "Other";
        public string PlatformId { get; init; } = string.Empty;
        public string InstalledVersion { get; init; } = string.Empty;
        public string LatestVersion => Core.version ?? string.Empty;
        public string RepositoryUrl
        {
            get
            {
                string owner = Core.repository?.owner ?? string.Empty;
                string repository = Core.repository?.name ?? string.Empty;
                return string.IsNullOrWhiteSpace(owner) || string.IsNullOrWhiteSpace(repository)
                    ? string.Empty
                    : $"https://github.com/{owner}/{repository}";
            }
        }
        public CoreLibraryStatus Status { get; init; }
        public bool EnabledForUpdate { get; set; }
        public int RequiredAssets { get; set; }
        public int PresentAssets { get; set; }
        public int MissingAssets => Math.Max(0, RequiredAssets - PresentAssets);
    }

    internal sealed class AssetRequirementItem
    {
        public string CoreIdentifier { get; init; } = string.Empty;
        public string Platform { get; init; } = string.Empty;
        public string Type { get; init; } = string.Empty;
        public string FileName { get; init; } = string.Empty;
        public string FullPath { get; init; } = string.Empty;
        public bool Present { get; init; }
    }

    internal sealed class PlatformAssetSummary
    {
        public string Platform { get; init; } = string.Empty;
        public string CoreIdentifier { get; init; } = string.Empty;
        public int Installed { get; init; }
        public int Required { get; init; }
        public int Missing => Math.Max(0, Required - Installed);
        public string DominantType { get; init; } = "ROM / Data";
    }

    internal sealed class LibrarySnapshot
    {
        public string TargetPath { get; init; } = string.Empty;
        public DateTime ScannedAt { get; init; } = DateTime.Now;
        public List<CoreLibraryItem> Cores { get; init; } = new();
        public List<AssetRequirementItem> Assets { get; init; } = new();
        public List<PlatformAssetSummary> AssetSummaries { get; init; } = new();
        public int AvailableImagePacks { get; init; }
        public int InstalledImagePacks { get; init; }
        public int MissingImagePacks => Math.Max(0, AvailableImagePacks - InstalledImagePacks);
        public int LocalArtworkFiles { get; init; }
        public bool FirmwareStatusKnown { get; init; }
        public string LatestFirmwareVersion { get; init; } = string.Empty;
        public string LatestFirmwareFileName { get; init; } = string.Empty;
        public bool LatestFirmwareFilePresent { get; init; }
        public string FirmwareStatusError { get; init; } = string.Empty;
        public string LatestFirmwareReleaseNotesHtml { get; init; } = string.Empty;
        public bool FirmwareUpdateAvailable => FirmwareStatusKnown && !LatestFirmwareFilePresent;

        public int TotalCores => Cores.Count;
        public int InstalledCores => Cores.Count(x => x.Status != CoreLibraryStatus.Missing);
        public int MissingCores => Cores.Count(x => x.Status == CoreLibraryStatus.Missing);
        public int CoreUpdates => Cores.Count(x => x.Status == CoreLibraryStatus.UpdateAvailable);
        public int RequiredAssets => Assets.Count;
        public int InstalledAssets => Assets.Count(x => x.Present);
        public int MissingAssets => Assets.Count(x => !x.Present);
    }

    internal static class LibraryInventoryService
    {

        private static readonly SemaphoreSlim ScanGate = new(1, 1);

        public static async Task<LibrarySnapshot> LoadAsync(string targetPath, bool refreshFirmware = false)
        {
            await ScanGate.WaitAsync().ConfigureAwait(false);
            try
            {
                return await Task.Run(() => Load(targetPath, refreshFirmware)).ConfigureAwait(false);
            }
            finally
            {
                ScanGate.Release();
            }
        }

        public static LibrarySnapshot Load(string targetPath, bool refreshFirmware = false)
        {
            string configPath = Directory.GetCurrentDirectory();
            ServiceHelper.Initialize(targetPath, configPath, forceReload: true);

            var cores = ServiceHelper.CoresService.Cores ?? new List<Core>();
            var coreItems = new List<CoreLibraryItem>();
            var assets = new List<AssetRequirementItem>();

            foreach (Core core in cores.OrderBy(c => c.platform?.name ?? c.identifier))
            {
                bool installed = ServiceHelper.CoresService.IsInstalled(core.identifier);
                string localVersion = string.Empty;
                string platformId = core.platform_id ?? string.Empty;
                if (installed)
                {
                    try
                    {
                        AnalogueCore? localCore = ServiceHelper.CoresService.ReadCoreJson(core.identifier);
                        localVersion = localCore?.metadata?.version ?? string.Empty;
                        if (string.IsNullOrWhiteSpace(platformId)
                            && localCore?.metadata?.platform_ids is { Length: > 0 })
                        {
                            platformId = localCore.metadata.platform_ids[0] ?? string.Empty;
                        }
                    }
                    catch
                    {
                        localVersion = string.Empty;
                    }
                }

                bool updateAvailable = installed && !string.IsNullOrWhiteSpace(core.version)
                    && !string.Equals(localVersion, core.version, StringComparison.OrdinalIgnoreCase);

                var item = new CoreLibraryItem
                {
                    Core = core,
                    Category = NormalizeCategory(core.platform?.category),
                    PlatformId = platformId,
                    InstalledVersion = localVersion,
                    Status = updateAvailable
                        ? CoreLibraryStatus.UpdateAvailable
                        : installed ? CoreLibraryStatus.Installed : CoreLibraryStatus.Missing,
                    EnabledForUpdate = !ServiceHelper.SettingsService.GetCoreSettings(core.identifier).skip
                };

                if (installed)
                {
                    var coreAssets = ScanCoreAssets(targetPath, core);
                    assets.AddRange(coreAssets);
                    item.RequiredAssets = coreAssets.Count;
                    item.PresentAssets = coreAssets.Count(a => a.Present);
                }

                coreItems.Add(item);
            }

            var summaries = assets
                .GroupBy(a => new { a.Platform, a.CoreIdentifier })
                .Select(group => new PlatformAssetSummary
                {
                    Platform = group.Key.Platform,
                    CoreIdentifier = group.Key.CoreIdentifier,
                    Installed = group.Count(x => x.Present),
                    Required = group.Count(),
                    DominantType = group.GroupBy(x => x.Type).OrderByDescending(x => x.Count()).Select(x => x.Key).FirstOrDefault() ?? "ROM / Data"
                })
                .OrderBy(x => x.Platform)
                .ToList();

            int packCount = 0;
            int installedPackCount = 0;
            try
            {
                var packService = ServiceHelper.PlatformImagePacksService;
                var packs = packService.List?.ToList() ?? new List<PlatformImagePack>();
                var installedPacks = packService.GetInstalledPacks();
                packCount = packs.Count;

                static string NormalizeVariant(string value) => string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
                installedPackCount = packs.Count(pack => installedPacks.Any(record =>
                    string.Equals(record.owner ?? string.Empty, pack.owner ?? string.Empty, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(record.repository ?? string.Empty, pack.repository ?? string.Empty, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(NormalizeVariant(record.variant), NormalizeVariant(pack.variant), StringComparison.OrdinalIgnoreCase)));
            }
            catch
            {
                packCount = 0;
                installedPackCount = 0;
            }

            int artworkFiles = 0;
            try
            {
                string artworkPath = Path.Combine(targetPath, "Platforms", "_images");
                if (Directory.Exists(artworkPath))
                    artworkFiles = Directory.EnumerateFiles(artworkPath, "*", SearchOption.AllDirectories).Count();
            }
            catch
            {
                artworkFiles = 0;
            }

            FirmwareStatus firmwareStatus;
            try
            {
                firmwareStatus = ServiceHelper.FirmwareService.GetStatus(targetPath, refreshFirmware);
            }
            catch (Exception ex)
            {
                firmwareStatus = new FirmwareStatus
                {
                    CheckSucceeded = false,
                    ErrorMessage = ex.Message
                };
            }

            return new LibrarySnapshot
            {
                TargetPath = targetPath,
                Cores = coreItems,
                Assets = assets,
                AssetSummaries = summaries,
                AvailableImagePacks = packCount,
                InstalledImagePacks = installedPackCount,
                LocalArtworkFiles = artworkFiles,
                FirmwareStatusKnown = firmwareStatus.CheckSucceeded,
                LatestFirmwareVersion = firmwareStatus.LatestVersion,
                LatestFirmwareFileName = firmwareStatus.LatestFileName,
                LatestFirmwareFilePresent = firmwareStatus.LatestFilePresent,
                FirmwareStatusError = firmwareStatus.ErrorMessage,
                LatestFirmwareReleaseNotesHtml = firmwareStatus.ReleaseNotesHtml
            };
        }

        private static List<AssetRequirementItem> ScanCoreAssets(string targetPath, Core core)
        {
            var result = new List<AssetRequirementItem>();
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            AnalogueCore? localCore;
            try
            {
                localCore = ServiceHelper.CoresService.ReadCoreJson(core.identifier);
            }
            catch
            {
                return result;
            }

            if (localCore?.metadata?.platform_ids == null || localCore.metadata.platform_ids.Length == 0)
                return result;

            string platformId = localCore.metadata.platform_ids[0];
            string platformName = core.platform?.name ?? platformId;
            string platformRoot = Path.Combine(targetPath, "Assets", platformId);

            try
            {
                var dataJson = ServiceHelper.CoresService.ReadDataJson(core.identifier);
                if (dataJson?.data?.data_slots != null)
                {
                    foreach (var slot in dataJson.data.data_slots)
                    {
                        if (string.IsNullOrWhiteSpace(slot.filename) || slot.filename.EndsWith(".sav", StringComparison.OrdinalIgnoreCase))
                            continue;

                        string basePath = slot.IsCoreSpecific()
                            ? Path.Combine(platformRoot, core.identifier)
                            : Path.Combine(platformRoot, "common");

                        AddRequirement(result, seen, core.identifier, platformName, slot.name, slot.filename, Path.Combine(basePath, slot.filename));
                    }
                }
            }
            catch
            {

            }

            try
            {
                string instanceRoot = Path.Combine(platformRoot, core.identifier);
                if (Directory.Exists(instanceRoot))
                {
                    foreach (string file in Directory.EnumerateFiles(instanceRoot, "*.json", SearchOption.AllDirectories))
                    {
                        try
                        {
                            InstanceJSON? instance = JsonConvert.DeserializeObject<InstanceJSON>(File.ReadAllText(file));
                            if (instance?.instance?.data_slots == null)
                                continue;

                            string commonRoot = Path.Combine(platformRoot, "common", instance.instance.data_path ?? string.Empty);
                            foreach (var slot in instance.instance.data_slots)
                            {
                                if (string.IsNullOrWhiteSpace(slot.filename) || slot.filename.EndsWith(".sav", StringComparison.OrdinalIgnoreCase))
                                    continue;
                                AddRequirement(result, seen, core.identifier, platformName, slot.name, slot.filename, Path.Combine(commonRoot, slot.filename));
                            }
                        }
                        catch
                        {

                        }
                    }
                }
            }
            catch
            {

            }

            return result;
        }

        private static void AddRequirement(List<AssetRequirementItem> result, HashSet<string> seen,
            string coreIdentifier, string platform, string? slotName, string fileName, string fullPath)
        {
            string key = fullPath;
            if (!seen.Add(key))
                return;

            result.Add(new AssetRequirementItem
            {
                CoreIdentifier = coreIdentifier,
                Platform = platform,
                Type = ClassifyAsset(slotName, fileName),
                FileName = fileName,
                FullPath = fullPath,
                Present = File.Exists(fullPath)
            });
        }

        private static string ClassifyAsset(string? slotName, string fileName)
        {
            string probe = $"{slotName} {fileName}".ToLowerInvariant();
            if (probe.Contains("bios") || probe.Contains("bootrom") || probe.Contains("boot rom") || probe.Contains("firmware"))
                return "BIOS";
            if (probe.Contains("rom"))
                return "ROM";
            return "ROM / Data";
        }

        public static string NormalizeCategory(string? category)
        {
            string value = (category ?? string.Empty).Trim().ToLowerInvariant();
            if (value.Contains("arcade")) return "Arcade";
            if (value.Contains("console")) return "Console";
            if (value.Contains("computer")) return "Computer";
            if (value.Contains("handheld") || value.Contains("portable")) return "Handheld";
            return "Other";
        }
    }
}
