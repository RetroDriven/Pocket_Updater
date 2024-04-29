using System.IO.Compression;
using Pannella.Helpers;
using Pannella.Models.Analogue.Shared;
using Pannella.Models.Extras;
using Pannella.Models.OpenFPGA_Cores_Inventory;
using ArchiveFile = Pannella.Models.Archive.File;
using File = System.IO.File;

namespace Pannella.Services;

public partial class CoresService
{
    private IEnumerable<Core> GetLocalCores()
    {
        string coresDirectory = Path.Combine(this.installPath, "Cores");

        // Create if it doesn't exist. -- Should we do this?
        // Stops error from being thrown if we do.
        Directory.CreateDirectory(coresDirectory);

        string[] directories = Directory.GetDirectories(coresDirectory, "*", SearchOption.TopDirectoryOnly);
        List<Core> all = new List<Core>();

        foreach (string name in directories)
        {
            string n = Path.GetFileName(name);
            var matches = Cores.Where(i => i.identifier == n);

            if (!matches.Any())
            {
                Core c = new Core { identifier = n };
                c.platform = this.ReadPlatformJson(c.identifier);
                all.Add(c);
            }
        }

        return all;
    }

    public void RefreshLocalCores()
    {
        cores.AddRange(this.GetLocalCores());
    }

    private bool InstallGithubAsset(string identifier, string platformId, string downloadUrl)
    {
        if (downloadUrl == null)
        {
            WriteMessage("No release URL found...");

            return false;
        }

        WriteMessage($"Downloading file {downloadUrl}...");

        string zipPath = Path.Combine(ServiceHelper.TempDirectory, ZIP_FILE_NAME);

        HttpHelper.Instance.DownloadFile(downloadUrl, zipPath);

        WriteMessage("Extracting...");

        string tempDir = Path.Combine(ServiceHelper.TempDirectory, "temp", identifier);

        ZipHelper.ExtractToDirectory(zipPath, tempDir, true);

        // Clean problematic directories and files.
        Util.CleanDir(tempDir, this.installPath, this.settingsService.GetConfig().preserve_platforms_folder, platformId);

        // Move the files into place and delete our core's temp directory.
        WriteMessage("Installing...");
        Util.CopyDirectory(tempDir, this.installPath, true, true);
        Directory.Delete(tempDir, true);

        // See if the temp directory itself can be removed.
        // Probably not needed if we aren't going to multi-thread this, but this is an async function so let's future proof.
        if (!Directory.GetFiles(Path.Combine(ServiceHelper.TempDirectory, "temp")).Any())
        {
            Directory.Delete(Path.Combine(ServiceHelper.TempDirectory, "temp"));
        }

        File.Delete(zipPath);

        return true;
    }

    private void CheckForPocketExtras(string identifier)
    {
        var coreSettings = this.settingsService.GetCoreSettings(identifier);

        if (coreSettings.pocket_extras)
        {
            PocketExtra pocketExtra = this.GetPocketExtra(identifier);

            if (pocketExtra != null)
            {
                WriteMessage("Reapplying Pocket Extras...");
                this.GetPocketExtra(pocketExtra, this.installPath, false, true);
            }
        }
    }

    private bool CheckCrc(string filePath, ArchiveFile archiveFile)
    {
        if (!this.settingsService.GetConfig().crc_check)
        {
            return true;
        }

        if (archiveFile == null)
        {
            return true; // no checksum to compare to
        }

        if (Util.CompareChecksum(filePath, archiveFile.crc32))
        {
            return true;
        }

        WriteMessage($"{Path.GetFileName(filePath)}: Bad checksum!");
        return false;
    }

    private bool CheckBetaMd5(DataSlot slot, string betaSlotId, string platform)
    {
        if (slot.md5 != null && (betaSlotId != null && slot.id == betaSlotId))
        {
            string path = Path.Combine(this.installPath, "Assets", platform);
            string filePath = Path.Combine(path, "common", slot.filename);
            bool exists;
            bool checksum = false;

            if (!(exists = File.Exists(filePath)))
            {
                WriteMessage($"JT beta key not found at '{filePath}'");
            }
            else if (!(checksum = Util.CompareChecksum(filePath, slot.md5, Util.HashTypes.MD5)))
            {
                WriteMessage("JT beta key checksum validation failed.");
                WriteMessage($"Location: '{filePath}'");
            }

            return exists && checksum;
        }

        return true;
    }
}
