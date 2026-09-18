using Newtonsoft.Json;
using Pannella.Helpers;
using Pannella.Models;
using Pannella.Models.Analogue;

namespace Pannella.Services;

public sealed class FirmwareStatus
{
    public bool CheckSucceeded { get; init; }
    public string LatestVersion { get; init; } = string.Empty;
    public string LatestFileName { get; init; } = string.Empty;
    public bool LatestFilePresent { get; init; }
    public string ErrorMessage { get; init; } = string.Empty;
    public string ReleaseNotesHtml { get; init; } = string.Empty;

    public bool UpdateFileAvailable => CheckSucceeded && !LatestFilePresent;
}

public class FirmwareService : Base
{
    private const string BASE_URL = "https://www.analogue.co/";
    private const string DETAILS = "support/pocket/firmware/{0}/details";
    private const string FILENAME_PATTERN = "pocket_firmware_*.bin";

    private static readonly object LatestLock = new();
    private static ReleaseDetails latest;

    private static ReleaseDetails GetDetails(string version = "latest", bool forceRefresh = false)
    {
        lock (LatestLock)
        {
            if (version == "latest" && forceRefresh)
            {
                latest = null;
            }

            if (version == "latest" && latest != null)
            {
                return latest;
            }

            string url = string.Format(BASE_URL + DETAILS, version);
            string response = HttpHelper.Instance.GetHTML(url);
            ReleaseDetails details = JsonConvert.DeserializeObject<ReleaseDetails>(response);

            if (version == "latest")
            {
                latest = details;
            }

            return details;
        }
    }

    public FirmwareStatus GetStatus(string path, bool forceRefresh = false)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
            {
                return new FirmwareStatus
                {
                    CheckSucceeded = false,
                    ErrorMessage = "The selected update location is not available."
                };
            }

            ReleaseDetails details = GetDetails("latest", forceRefresh);
            if (details == null || string.IsNullOrWhiteSpace(details.download_url))
            {
                return new FirmwareStatus
                {
                    CheckSucceeded = false,
                    ErrorMessage = "The latest Pocket firmware details could not be retrieved."
                };
            }

            string filename;
            try
            {
                filename = Path.GetFileName(new Uri(details.download_url).LocalPath);
            }
            catch
            {
                string[] parts = details.download_url.Split('/');
                filename = parts.Length == 0 ? string.Empty : parts[^1];
            }

            string filepath = Path.Combine(path, filename);
            bool present = File.Exists(filepath);

            if (present && !string.IsNullOrWhiteSpace(details.md5))
            {
                present = Util.CompareChecksum(filepath, details.md5, Util.HashTypes.MD5);
            }

            return new FirmwareStatus
            {
                CheckSucceeded = true,
                LatestVersion = details.version ?? string.Empty,
                LatestFileName = filename,
                LatestFilePresent = present,
                ReleaseNotesHtml = details.release_notes_html ?? string.Empty
            };
        }
        catch (Exception ex)
        {
            return new FirmwareStatus
            {
                CheckSucceeded = false,
                ErrorMessage = ex.Message
            };
        }
    }

    public string UpdateFirmware(string path)
    {
        string version = string.Empty;

        WriteMessage("Checking for firmware updates...");

        var details = GetDetails();
        string[] parts = details.download_url.Split("/");
        string filename = parts[parts.Length - 1];
        string filepath = Path.Combine(path, filename);

        if (!File.Exists(filepath) || !Util.CompareChecksum(filepath, details.md5, Util.HashTypes.MD5))
        {
            version = filename;

            var oldFiles = Directory.GetFiles(path, FILENAME_PATTERN);

            WriteMessage("Firmware update found. Downloading...");

            HttpHelper.Instance.DownloadFile(details.download_url, Path.Combine(path, filename));

            WriteMessage("Download Complete.");
            WriteMessage(Path.Combine(path, filename));

            foreach (string oldFile in oldFiles)
            {
                if (File.Exists(oldFile) && Path.GetFileName(oldFile) != filename)
                {
                    WriteMessage("Deleting old firmware file...");
                    File.Delete(oldFile);
                }
            }

            WriteMessage("To install firmware, restart your Pocket.");
        }
        else
        {
            WriteMessage("Firmware up to date.");
        }

        return version;
    }
}
