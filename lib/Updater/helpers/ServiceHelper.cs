using Pannella.Models;
using Pannella.Services;

namespace Pannella.Helpers;

public static class ServiceHelper
{
    public static string UpdateDirectory { get; private set; } // move off this
    public static string TempDirectory { get; private set; }
    public static string ConfigDirectory { get; private set; }
    public static CoresService CoresService { get; private set; }
    public static SettingsService SettingsService { get; private set; }
    public static PlatformImagePacksService PlatformImagePacksService { get; private set; }
    public static FirmwareService FirmwareService { get; private set; }
    public static ArchiveService ArchiveService { get; private set; }
    public static AssetsService AssetsService { get; private set; }

    private static bool isInitialized;

    public static void Initialize(string path, string config_path, EventHandler<StatusUpdatedEventArgs> statusUpdated = null,
        EventHandler<UpdateProcessCompleteEventArgs> updateProcessComplete = null, bool forceReload = false)
    {
        if (!isInitialized || forceReload)
        {
            isInitialized = true;
            UpdateDirectory = path;
            ConfigDirectory = config_path;
            SettingsService = new SettingsService(ConfigDirectory);
            ArchiveService = new ArchiveService(SettingsService.GetConfig().archives,
                SettingsService.GetConfig().crc_check, SettingsService.GetConfig().use_custom_archive);
            TempDirectory = SettingsService.GetConfig().temp_directory ?? UpdateDirectory;
            AssetsService = new AssetsService(SettingsService.GetConfig().use_local_blacklist);
            CoresService = new CoresService(path, SettingsService, ArchiveService, AssetsService);
            SettingsService.InitializeCoreSettings(CoresService.Cores);

            SettingsService.Save();
            PlatformImagePacksService = new PlatformImagePacksService(path, SettingsService.GetConfig().github_token,
                SettingsService.GetConfig().use_local_image_packs);
            FirmwareService = new FirmwareService();

            if (statusUpdated != null)
            {
                PlatformImagePacksService.StatusUpdated += statusUpdated;
                FirmwareService.StatusUpdated += statusUpdated;
                CoresService.StatusUpdated += statusUpdated;
                ArchiveService.StatusUpdated += statusUpdated;
            }

            if (updateProcessComplete != null)
            {
                CoresService.UpdateProcessComplete += updateProcessComplete;
            }
        }
    }

    public static void ReloadSettings()
    {
        string configDirectory = string.IsNullOrWhiteSpace(ConfigDirectory) ? Directory.GetCurrentDirectory() : ConfigDirectory;
        SettingsService = new SettingsService(configDirectory, CoresService.Cores);
        //reload the archive service, in case that setting has changed
        ArchiveService = new ArchiveService(SettingsService.GetConfig().archives,
                SettingsService.GetConfig().crc_check, SettingsService.GetConfig().use_custom_archive);
        CoresService = new CoresService(UpdateDirectory, SettingsService, ArchiveService, AssetsService);
    }
}
