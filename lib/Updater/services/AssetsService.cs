using System.IO.Compression;
using Newtonsoft.Json;
using Pannella.Helpers;

namespace Pannella.Services;

public class AssetsService
{
    private const string BLACKLIST = "https://raw.githubusercontent.com/mattpannella/pupdate/main/blacklist.json";

    private readonly bool useLocalBlacklist;
    private List<string> blacklist;

    public List<string> Blacklist
    {
        get
        {
            if (this.blacklist == null)
            {
                string json = useLocalBlacklist
                    ? File.ReadAllText("blacklist.json")
                    : HttpHelper.Instance.GetHTML(BLACKLIST);

                this.blacklist = JsonConvert.DeserializeObject<List<string>>(json);
            }

            return this.blacklist;
        }
    }

    public AssetsService(bool useLocalBlacklist)
    {
        this.useLocalBlacklist = useLocalBlacklist;
    }

    public static void BackupSaves(string directory, string backupLocation)
    {
        BackupDirectory(directory, "Saves", backupLocation);
    }

    public static void BackupMemories(string directory, string backupLocation)
    {
        BackupDirectory(directory, "Memories", backupLocation);
    }

    private static void BackupDirectory(string rootDirectory, string folderName, string backupLocation)
    {
        if (string.IsNullOrEmpty(rootDirectory))
        {
            throw new ArgumentNullException(nameof(rootDirectory));
        }

        if (string.IsNullOrEmpty(backupLocation))
        {
            throw new ArgumentNullException(nameof(backupLocation));
        }

        string resolvedBackupLocation = Path.IsPathRooted(backupLocation)
            ? backupLocation
            : Path.Combine(rootDirectory, backupLocation);

        Console.WriteLine($"Compressing and backing up {folderName} directory...");
        string savesPath = Path.Combine(rootDirectory, folderName);
        string fileName = $"{folderName}_Backup_{DateTime.Now:yyyy-MM-dd_HH.mm.ss}.zip";
        string archiveName = Path.Combine(resolvedBackupLocation, fileName);

        if (Directory.Exists(savesPath))
        {
            if (!Directory.Exists(resolvedBackupLocation))
            {
                Directory.CreateDirectory(resolvedBackupLocation);
            }

            ZipFile.CreateFromDirectory(savesPath, archiveName);
            Console.WriteLine($"Complete. Backup saved to '{archiveName}'.");
        }
        else
        {
            Console.WriteLine($"No {folderName} directory found, skipping backup...");
        }
    }
}
