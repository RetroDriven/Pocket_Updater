using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroDriven
{
    public class Settings_Control
    {
        public static string[,] Get_Settings()
        {
            string GT_1 = "SetGithubApiKey";
            string GT_2 = "github_token";
            string DF_1 = "DownloadFirmware";
            string DF_2 = "download_firmware";
            string PI_1 = "PreserveImages";
            string PI_2 = "preserve_images";
            string DA_1 = "DownloadAssets";
            string DA_2 = "download_assets";
            //string[] Setings = new string[] { GitHub_Token, Download_Firmware, Core_Images, Download_Assets };

            string[,] Settings = new string[4,2] { { GT_1, GT_2 }, { DF_1, DF_2 },{ PI_1, PI_2 }, {DA_1, DA_2 } };
            return Settings;
        }
    }
}
