using Newtonsoft.Json;
using Pannella.Helpers;
using Pannella.Models.OpenFPGA_Cores_Inventory;
using Pannella.Models.Settings;

namespace Pannella.Services;

public class SettingsService
{
    private const string OLD_SETTINGS_FILENAME = "pocket_updater_settings.json";
    private const string SETTINGS_FILENAME = "pupdate_settings.json";
    private static readonly object FileSync = new();
    private const int FileRetryCount = 5;

    private readonly Settings settings;
    private readonly string settingsFile;
    private readonly List<Core> missingCores;
    private string? legacySettingsFile;

    public SettingsService(string settingsPath, List<Core>? cores = null)
        : this(settingsPath, cores, true)
    {
    }

    private SettingsService(string settingsPath, List<Core>? cores, bool saveOnLoad)
    {
        this.settings = new Settings();
        this.missingCores = new List<Core>();

        string file = Path.Combine(settingsPath, SETTINGS_FILENAME);
        string oldFile = Path.Combine(settingsPath, OLD_SETTINGS_FILENAME);
        this.settingsFile = file;
        string? json = null;

        lock (FileSync)
        {
            if (File.Exists(file))
            {
                json = ReadAllTextWithRetry(file);
            }
            else if (File.Exists(oldFile))
            {
                json = ReadAllTextWithRetry(oldFile);
                this.legacySettingsFile = oldFile;
            }

            if (!string.IsNullOrEmpty(json))
            {
                settings = JsonConvert.DeserializeObject<Settings>(json) ?? new Settings();
                settings.config?.Migrate();
            }

            // bandaid to fix old settings files
            settings.config ??= new Config();

            if (cores != null)
            {
                this.InitializeCoreSettings(cores);
            }

            if (saveOnLoad)
            {
                this.Save();
            }
        }
    }

    public static void SaveConfig(string settingsPath, Action<Config> update)
    {
        lock (FileSync)
        {
            var current = new SettingsService(settingsPath, null, false);
            update(current.GetConfig());
            current.Save();
        }
    }

    public void Save()
    {
        var options = new JsonSerializerSettings { ContractResolver = ArchiveContractResolver.Instance };
        var json = JsonConvert.SerializeObject(settings, Formatting.Indented, options);

        lock (FileSync)
        {
            for (int attempt = 0; ; attempt++)
            {
                try
                {
                    File.WriteAllText(this.settingsFile, json);
                    break;
                }
                catch (IOException) when (attempt < FileRetryCount - 1)
                {
                    Thread.Sleep(40 * (attempt + 1));
                }
            }

            if (this.legacySettingsFile != null)
            {
                File.Delete(this.legacySettingsFile);
                this.legacySettingsFile = null;
            }
        }
    }

    private static string ReadAllTextWithRetry(string path)
    {
        for (int attempt = 0; ; attempt++)
        {
            try
            {
                return File.ReadAllText(path);
            }
            catch (IOException) when (attempt < FileRetryCount - 1)
            {
                Thread.Sleep(40 * (attempt + 1));
            }
        }
    }

    /// <summary>
    /// loop through every core, and add any missing ones to the settings file
    /// </summary>
    public void InitializeCoreSettings(List<Core> cores)
    {
        settings.core_settings ??= new Dictionary<string, CoreSettings>();

        foreach (Core core in cores)
        {
            if (!settings.core_settings.ContainsKey(core.identifier))
            {
                this.missingCores.Add(core);
                DisableCore(core.identifier);
            }
        }
    }

    public void EnableCore(string name, bool? pocketExtras = null, string pocketExtrasVersion = null)
    {
        if (!settings.core_settings.TryGetValue(name, out CoreSettings coreSettings))
        {
            coreSettings = new CoreSettings();

            settings.core_settings.Add(name, coreSettings);
        }

        coreSettings.skip = false;

        if (pocketExtras.HasValue)
            coreSettings.pocket_extras = pocketExtras.Value;

        if (!string.IsNullOrEmpty(pocketExtrasVersion))
            coreSettings.pocket_extras_version = pocketExtrasVersion;
    }

    public void DisableCore(string name)
    {
        if (settings.core_settings.TryGetValue(name, out CoreSettings value))
        {
            value.skip = true;
        }
        else
        {
            CoreSettings core = new CoreSettings { skip = true };

            settings.core_settings.Add(name, core);
        }
    }

    public void DisablePocketExtras(string name)
    {
        if (settings.core_settings.TryGetValue(name, out CoreSettings value))
        {
            value.pocket_extras = false;
            value.pocket_extras_version = null;
        }
    }

    public List<Core> GetMissingCores() => this.missingCores;

    public void EnableMissingCores()
    {
        foreach (var core in this.missingCores)
        {
            EnableCore(core.identifier);
        }
    }

    public void DisableMissingCores()
    {
        foreach (var core in this.missingCores)
        {
            DisableCore(core.identifier);
        }
    }

    public Config GetConfig()
    {
        return settings.config;
    }

    // This is used by the RetroDriven Pocket Updater Windows Application
    // ReSharper disable once UnusedMember.Global
    public void UpdateConfig(Config config)
    {
        settings.config = config;
    }

    public CoreSettings GetCoreSettings(string name)
    {
        return settings.core_settings.TryGetValue(name, out CoreSettings value)
            ? value
            : new CoreSettings();
    }
}
