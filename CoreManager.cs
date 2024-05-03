using System;
using System.Text.Json;
using System.IO;
using Pannella.Services;
using Pannella.Models.OpenFPGA_Cores_Inventory;
using Pannella.Models.Settings;
using Pannella.Helpers;

namespace Pocket_Updater
{
    public class CoreManager
    {
        private SettingsService _settings;
        private string _settingsFile;
        private string _coresFile;
        private List<Core> _cores;

        public CoreManager(string settingsFile, string coresFile)
        {
            _settings = new SettingsService();
            if (File.Exists(settingsFile))
            {
                string json = File.ReadAllText(settingsFile);
                _settings = JsonSerializer.Deserialize<pannella.analoguepocket.Settings>(json);
            }
            _settingsFile = settingsFile;

            if(File.Exists(coresFile))
            {
                _coresFile = coresFile;
                string json = File.ReadAllText(coresFile);
                _cores = JsonSerializer.Deserialize<List<Core>>(json);
            }
            else
            {
                throw new Exception("Unable to find cores json file.");
            }
        }

        public bool SaveSettings()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(_settingsFile, JsonSerializer.Serialize(_settings, options));

            return true;
        }

        public void DisableCore(string platform)
        {
            if(_settings.coreSettings.ContainsKey(platform))
            {
                _settings.coreSettings[platform].skip = true;
            }
            else
            {
                CoreSettings core = new CoreSettings();
                core.skip = true;
                _settings.coreSettings.Add(platform, core);
            }
        }

        public void EnableCore(string platform)
        {
            if (_settings.coreSettings.ContainsKey(platform))
            {
                _settings.coreSettings[platform].skip = false;
            }
            else
            {
                CoreSettings core = new CoreSettings();
                core.skip = false;
                _settings.coreSettings.Add(platform, core);
            }
        }

        public void UpdateCore(CoreSettings core, string platform)
        {
            if (_settings.coreSettings.ContainsKey(platform))
            {
                _settings.coreSettings[platform] = core;
            }
            else
            {
                _settings.coreSettings.Add(platform, core);
            }
        }

        public Dictionary<string, CoreSettings> GetCoreSettings()
        {
            return _settings.coreSettings;
        }

        public List<Core> GetCores()
        {
            return _cores;
        }

    }
}
