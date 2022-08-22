using System;
using System.Collections.Generic;
using pannella.analoguepocket;
using System.Text.Json;
namespace Pocket_Updater
{
    public class ArcadeRomDownloader
    {
        private string _misterDB = "some_url_or_path_to_mister_db";
        private string _mameSetArchiveUrlBase = "http://archive.org/path/to/mame-set/";
        public string CoresFile { get; set; }

        public ArcadeRomDownloader(string coresFile)
        {
            //constructor. do stuff here to initialize, if needed
            //maybe call DownloadDB()?
            CoresFile = coresFile;
        }

        public void DownloadDB()
        {
            //download the mister rom db from _misterDB
        }

        //this is probably the function that we would call from the UI to download the rom(s) from the mame set for every core
        public void DownloadAll()
        {
            //loop through each core and call GetRomsForCore(coreName) for each one   
            List<Core> cores = _readCoresFile();
            foreach (Core c in cores)
            {
                GetRomsForCore(c.platform);
            }
        }

        //download roms for just one specific core
        public void GetRomsForCore(string coreName)
        {
            //use the coreName to check the _misterDB for a list of available roms
            //loop through each rom and call _downloadRom()
        }

        private void _downloadRom(string romName)
        {
            //download file from mame set 
            //url = _mameSetArchiveUrlBase + romName
        }

        private List<Core> _readCoresFile()
        {
            string json = File.ReadAllText(CoresFile);
            List<Core>? coresList = JsonSerializer.Deserialize<List<Core>>(json);
            if (coresList != null)
            {
                return coresList;
            }

            return null;
        }

    }
}