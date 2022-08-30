using System;
using System.Text.Json;
using System.IO;
using pannella.analoguepocket;

namespace Pocket_Updater
{
    public class CoreManager
    {
        private string _coresFile { get; set; }
        private List<Core> _cores;

        public CoreManager(string coresFile)
        {
            if (File.Exists(coresFile))
            {
                _coresFile = coresFile;
                _readCoresFile();
            }
            else
            {
                throw new FileNotFoundException("Cores json file not found: " + coresFile);
            }
        }

        public void SaveCores(List<Core> cores)
        {
            _cores = cores;
            SaveCoresFile();
        }

        private void _readCoresFile(bool force = false)
        {
            if (_cores == null || force)
            {
                string json = File.ReadAllText(_coresFile);
                List<Core>? coresList = JsonSerializer.Deserialize<List<Core>>(json);
                if (coresList != null)
                {
                    _cores = coresList;
                    //return true;
                }

                //return false;
            }
        }

        public bool SaveCoresFile()
        {
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.WriteIndented = true;
            string json = JsonSerializer.Serialize(_cores,options);
            File.WriteAllText(_coresFile, json);

            return true;
        }

        public void DisableCore(string platform)
        {
            _readCoresFile();
            if (_cores != null)
            {
                for(int i = 0; i < _cores.Count; i++)
                {
                    if (_cores[i].platform == platform)
                    {
                        _cores[i].skip = true;
                    }
                }

                SaveCoresFile();
            }
        }

        public void EnableCore(string platform)
        {
            _readCoresFile();
            if (_cores != null)
            {
                for (int i = 0; i < _cores.Count; i++)
                {
                    if (_cores[i].platform == platform)
                    {
                        _cores[i].skip = false;
                    }
                }

                SaveCoresFile();
            }
        }

        public void AddCore(Core core)
        {
            _readCoresFile();
            _cores.Add(core);
            SaveCoresFile();
        }

        public void UpdateCore(Core core, string platform)
        {
            _readCoresFile();
            if (_cores != null)
            {
                for (int i = 0; i < _cores.Count; i++)
                {
                    if (_cores[i].platform == platform)
                    {
                        _cores[i] = core;
                    }
                }

                SaveCoresFile();
            }
        }

        public List<Core> GetCores()
        {
            return _cores;
        }

    }
}
