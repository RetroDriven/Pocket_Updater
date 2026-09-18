using System.IO.Compression;
using Newtonsoft.Json;
using Pannella.Helpers;
using Pannella.Models;
using Pannella.Models.Github;
using File = System.IO.File;

namespace Pannella.Services;

public class PlatformImagePacksService : Base
{
    private const string END_POINT = "https://raw.githubusercontent.com/mattpannella/pupdate/main/image_packs.json";
    private const string REGISTRY_DIRECTORY = ".pocket_updater";
    private const string REGISTRY_FILE = "image_packs.json";

    private readonly bool useLocalImagePacks;
    private readonly string installPath;
    private readonly string githubToken;

    private List<PlatformImagePack> list;

    public List<PlatformImagePack> List
    {
        get
        {
            if (this.list == null)
            {
                string json = this.useLocalImagePacks
                    ? File.ReadAllText("image_packs.json")
                    : HttpHelper.Instance.GetHTML(END_POINT);

                this.list = JsonConvert.DeserializeObject<List<PlatformImagePack>>(json) ?? new List<PlatformImagePack>();
            }

            return list;
        }
    }

    public PlatformImagePacksService(string path, string githubToken = null, bool useLocalImagePacks = false)
    {
        this.installPath = path;
        this.githubToken = githubToken;
        this.useLocalImagePacks = useLocalImagePacks;
    }

    public string GetLatestVersion(string owner, string repository)
    {
        Release release = GithubApiService.GetLatestRelease(owner, repository, this.githubToken);
        return GetReleaseVersion(release);
    }

    public List<InstalledImagePackRecord> GetInstalledPacks()
    {
        string path = RegistryPath;
        if (!File.Exists(path))
            return new List<InstalledImagePackRecord>();

        try
        {
            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<List<InstalledImagePackRecord>>(json)
                ?? new List<InstalledImagePackRecord>();
        }
        catch
        {

            return new List<InstalledImagePackRecord>();
        }
    }

    public InstalledImagePackRecord GetInstalledPack(string owner, string repository, string variant)
    {
        return GetInstalledPacks().FirstOrDefault(record => RecordMatches(record, owner, repository, variant));
    }

    public void Install(string owner, string repository, string variant)
    {
        InstallTracked(owner, repository, variant);
    }

    public InstalledImagePackRecord InstallTracked(string owner, string repository, string variant)
    {
        UpdateCancellation.ThrowIfCancellationRequested();
        Release release = GithubApiService.GetLatestRelease(owner, repository, this.githubToken);
        if (release?.assets == null || release.assets.Count == 0)
            throw new Exception("GitHub release contains no downloadable assets.");

        Asset asset = SelectReleaseAsset(release, variant);
        if (asset == null || string.IsNullOrWhiteSpace(asset.browser_download_url))
            throw new Exception($"Unable to find a release ZIP for '{DisplayVariant(variant)}'.");

        string operationRoot = Path.Combine(ServiceHelper.TempDirectory, $"image_pack_{Guid.NewGuid():N}");
        string localFile = Path.Combine(operationRoot, "image_pack.zip");
        string extractPath = Path.Combine(operationRoot, "extract");
        Directory.CreateDirectory(operationRoot);

        try
        {
            WriteMessage($"Downloading image pack '{owner}/{repository}' ({DisplayVariant(variant)})...");
            HttpHelper.Instance.DownloadFile(asset.browser_download_url, localFile);
            UpdateCancellation.ThrowIfCancellationRequested();
            WriteMessage("Download complete.");

            WriteMessage("Installing image pack...");
            UpdateCancellation.ThrowIfCancellationRequested();
            Directory.CreateDirectory(extractPath);
            ZipHelper.ExtractToDirectory(localFile, extractPath, true);
            UpdateCancellation.ThrowIfCancellationRequested();

            string imagePack = FindPlatformImagePack(extractPath);
            string target = Path.Combine(this.installPath, "Platforms", "_images");
            Directory.CreateDirectory(target);

            List<string> files = Directory
                .EnumerateFiles(imagePack, "*", SearchOption.AllDirectories)
                .Select(path => NormalizeRelativePath(Path.GetRelativePath(imagePack, path)))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
                .ToList();

            Util.CopyDirectory(imagePack, target, true, true);

            var records = GetInstalledPacks();
            records.RemoveAll(record => RecordMatches(record, owner, repository, variant));

            var installed = new InstalledImagePackRecord
            {
                owner = owner ?? string.Empty,
                repository = repository ?? string.Empty,
                variant = NormalizeVariant(variant),
                version = GetReleaseVersion(release),
                installed_utc = DateTime.UtcNow,
                files = files
            };
            records.Add(installed);
            SaveInstalledPacks(records);

            WriteMessage("Image pack installation complete.");
            return installed;
        }
        finally
        {
            TryDeleteDirectory(operationRoot);
        }
    }

    public bool UninstallTracked(string owner, string repository, string variant)
    {
        List<InstalledImagePackRecord> records = GetInstalledPacks();
        InstalledImagePackRecord installed = records.FirstOrDefault(record => RecordMatches(record, owner, repository, variant));
        if (installed == null)
            return false;

        records.Remove(installed);

        string imagesRoot = Path.Combine(this.installPath, "Platforms", "_images");
        string fullRoot = Path.GetFullPath(imagesRoot)
            .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
            + Path.DirectorySeparatorChar;

        var referencedByOtherPacks = new HashSet<string>(
            records.SelectMany(record => record.files ?? new List<string>()).Select(NormalizeRelativePath),
            StringComparer.OrdinalIgnoreCase);

        var failed = new List<string>();
        int removedFiles = 0;

        foreach (string relative in installed.files ?? new List<string>())
        {
            string normalized = NormalizeRelativePath(relative);
            if (string.IsNullOrWhiteSpace(normalized) || referencedByOtherPacks.Contains(normalized))
                continue;

            string candidate = Path.GetFullPath(Path.Combine(imagesRoot,
                normalized.Replace('/', Path.DirectorySeparatorChar)));

            if (!candidate.StartsWith(fullRoot, StringComparison.OrdinalIgnoreCase))
            {
                failed.Add(normalized);
                continue;
            }

            if (!File.Exists(candidate))
                continue;

            try
            {
                File.SetAttributes(candidate, FileAttributes.Normal);
                File.Delete(candidate);
                if (File.Exists(candidate))
                    failed.Add(normalized);
                else
                    removedFiles++;
            }
            catch
            {
                failed.Add(normalized);
            }
        }

        PruneEmptyDirectories(imagesRoot);

        if (failed.Count > 0)
        {

            installed.files = failed;
            records.Add(installed);
            SaveInstalledPacks(records);
            throw new IOException($"Removed {removedFiles} file(s), but {failed.Count} image-pack file(s) could not be deleted. Close any program using those files and try again.");
        }

        SaveInstalledPacks(records);
        WriteMessage($"Uninstalled image pack '{owner}/{repository}' ({DisplayVariant(variant)}); removed {removedFiles} file(s).");
        return true;
    }

    private string RegistryPath => Path.Combine(this.installPath, REGISTRY_DIRECTORY, REGISTRY_FILE);

    private void SaveInstalledPacks(List<InstalledImagePackRecord> records)
    {
        string directory = Path.GetDirectoryName(RegistryPath);
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        File.WriteAllText(RegistryPath, JsonConvert.SerializeObject(records
            .OrderBy(record => record.owner, StringComparer.OrdinalIgnoreCase)
            .ThenBy(record => record.repository, StringComparer.OrdinalIgnoreCase)
            .ThenBy(record => record.variant, StringComparer.OrdinalIgnoreCase), Formatting.Indented));
    }

    private static Asset SelectReleaseAsset(Release release, string variant)
    {
        if (release?.assets == null)
            return null;

        if (!string.IsNullOrWhiteSpace(variant))
        {
            Asset exact = release.assets.FirstOrDefault(asset =>
                !string.IsNullOrWhiteSpace(asset?.name)
                && asset.name.EndsWith($"{variant}.zip", StringComparison.OrdinalIgnoreCase));
            if (exact != null)
                return exact;
        }

        return release.assets.FirstOrDefault(asset =>
            !string.IsNullOrWhiteSpace(asset?.name)
            && asset.name.EndsWith(".zip", StringComparison.OrdinalIgnoreCase));
    }

    private static string GetReleaseVersion(Release release)
    {
        if (!string.IsNullOrWhiteSpace(release?.tag_name))
            return release.tag_name.Trim();
        if (!string.IsNullOrWhiteSpace(release?.name))
            return release.name.Trim();
        return "Unknown";
    }

    private static bool RecordMatches(InstalledImagePackRecord record, string owner, string repository, string variant)
    {
        if (record == null) return false;
        return string.Equals(record.owner ?? string.Empty, owner ?? string.Empty, StringComparison.OrdinalIgnoreCase)
            && string.Equals(record.repository ?? string.Empty, repository ?? string.Empty, StringComparison.OrdinalIgnoreCase)
            && string.Equals(NormalizeVariant(record.variant), NormalizeVariant(variant), StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizeVariant(string variant) => string.IsNullOrWhiteSpace(variant) ? string.Empty : variant.Trim();
    private static string DisplayVariant(string variant) => string.IsNullOrWhiteSpace(variant) ? "Default" : variant.Trim();
    private static string NormalizeRelativePath(string path) => (path ?? string.Empty).Replace('\\', '/').TrimStart('/');

    private static string FindPlatformImagePack(string temp)
    {
        string path = Path.Combine(temp, "Platforms", "_images");
        if (Directory.Exists(path))
            return path;

        foreach (string d in Directory.EnumerateDirectories(temp))
        {
            path = Path.Combine(d, "Platforms", "_images");
            if (Directory.Exists(path))
                return path;
        }

        throw new Exception("Can't find image pack in the downloaded release.");
    }

    private static void PruneEmptyDirectories(string root)
    {
        if (!Directory.Exists(root)) return;

        foreach (string directory in Directory.EnumerateDirectories(root, "*", SearchOption.AllDirectories)
                     .OrderByDescending(path => path.Length))
        {
            try
            {
                if (!Directory.EnumerateFileSystemEntries(directory).Any())
                    Directory.Delete(directory);
            }
            catch { }
        }
    }

    private static void TryDeleteDirectory(string path)
    {
        try
        {
            if (Directory.Exists(path))
                Directory.Delete(path, true);
        }
        catch { }
    }
}

public class InstalledImagePackRecord
{
    public string owner { get; set; } = string.Empty;
    public string repository { get; set; } = string.Empty;
    public string variant { get; set; } = string.Empty;
    public string version { get; set; } = string.Empty;
    public DateTime installed_utc { get; set; }
    public List<string> files { get; set; } = new();
}
