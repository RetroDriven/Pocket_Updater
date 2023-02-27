## Analogue Pocket Updater - Windows Application ##
[![](https://img.shields.io/static/v1?label=Sponsor&message=%E2%9D%A4&logo=GitHub&color=%23fe8e86)](https://github.com/sponsors/RetroDriven) [![Current Release](https://img.shields.io/github/v/release/RetroDriven/Pocket_Updater?label=Current%20Release)](https://github.com/RetroDriven/Pocket_Updater/releases/latest) ![Downloads](https://img.shields.io/github/downloads/RetroDriven/Pocket_Updater/latest/total?label=Downloads) ![GitHub all releases](https://img.shields.io/github/downloads/RetroDriven/Pocket_Updater/total?label=Total%20Downloads) ![ViewCount](https://views.whatilearened.today/views/github/RetroDriven/Pocket_Updater.svg) ![Twitter](https://img.shields.io/twitter/url/https/twitter.com/RetroDriven.svg?style=social&label=Follow%20%40RetroDriven)

This is a free Windows Application for updating the openFPGA Cores, Pocket Firmware, Required BIOS, and Arcade ROMS for your Analogue Pocket. You can also Organize your Cores and Download/Install Asset Image Packs.

This Application can be run from the Pocket's SD Card or from any location on your Windows Machine based on what fits your needs best.

## Bulding Exe from Source ##
Please note that the following dependencies below are required if you plan to edit/build the EXE from Source via Visual Studio.

[Crc32.NET](https://www.nuget.org/packages/Crc32.NET)

[Guna.UI2.WinForms](https://www.nuget.org/packages/Guna.UI2.WinForms)

## Updating ##

Select the "**Update Pocket**" Option

![image](https://github.com/RetroDriven/Pocket_Updater/blob/master/Sceenshots/Main.png)

#### Current Directory Location
This option allows you to Update everything locally first and manually copy over all of the Files/Folders to your Pocket's SD Card after the Updater has finished. 

![image](https://github.com/RetroDriven/Pocket_Updater/blob/master/Sceenshots/Update.png)

#### Removable Storage Location
This option allows you to Update directly to your Pocket's SD Card via plugging in your SD Card to your machine or by connecting your Pocket to your machine via USB Cable. (*Please use the Refresh button if you do not see your Pocket's SD Card Drive Letter within the Drop Down*)

![image](https://github.com/RetroDriven/Pocket_Updater/blob/master/Sceenshots/Update_Pocket.png)

Select the "**Update Cores**" Option and you will see a Status window as well as a message popup box when the Updates are done.

![image](https://github.com/RetroDriven/Pocket_Updater/blob/master/Sceenshots/Updates_Complete.png)

## Settings ##
Select "**Settings**" Option

![image](https://github.com/RetroDriven/Pocket_Updater/blob/master/Sceenshots/Settings.png)

#### Download Pocket Firmware
This gives you the option to Check/Download Pocket Firmware.

#### Download Roms/Bios
This gives you the option to Check/Download the required Arcade Roms and Core Bios files.

#### Preserve Platforms
This gives you the option to keep any custom Core Images that you are using vs. the Stock Core Images. It will also preserve the entire Platforms folder which includes Core Names and Categories.

#### Download Pre-Release Cores
This gives you the option to Download any pre-release Cores. These Cores are typically beta/testing and likely to have some bugs.

#### Delete Skipped Cores
This will Delete Cores from your Pocket's SD Card that you have unchecked for Updating/Downloading
*This option will be Enabled by Default

#### GitHub Token
This is an Optional Setting. If you're running up against the rate limit with the GitHub API, you can provide your Personal Access Token to resolve this issue.

## Manage Cores ##
Select the "**Manage Cores**" Option. From here you can pick and choose the Cores that you'd like to download.

![image](https://github.com/RetroDriven/Pocket_Updater/blob/master/Sceenshots/Manage_Cores.png)

## Organize Cores ##
Select the "**Organize Cores**" Option. From here you can customize the Core Names and Categories to your liking.

![image](https://github.com/RetroDriven/Pocket_Updater/blob/master/Sceenshots/Organize_Cores.png)

## Asset Image Packs ##
Select the "**Asset Image Packs**" Option. This will allow you to download from the most popular Image Packs out there to spice things up on your Pocket. More Image Packs will be added as they become available from Creators

![image](https://github.com/RetroDriven/Pocket_Updater/blob/master/Sceenshots/Image_Packs.png)

#### Current Directory Location
This option allows you to Update everything locally first and manually copy over all of the Files/Folders to your Pocket's SD Card after the Updater has finished. 

#### Removable Storage Location
This option allows you to Update directly to your Pocket's SD Card via plugging in your SD Card to your machine or by connecting your Pocket to your machine via USB Cable. (*Please use the Refresh button if you do not see your Pocket's SD Card Drive Letter within the Drop Down*)

## Update Log ##
Select the "**Logs**" Option

![image](https://github.com/RetroDriven/Pocket_Updater/blob/master/Sceenshots/Update_Log.png)

You can clear the Update Log File via the "**Clear Logs**" Button

## App Updates ##
This Application will self check for Updates when you run it. When an Update is found you will see an App Update Notification banner within the Titlebar like below. 

![image](https://github.com/RetroDriven/Pocket_Updater/blob/master/Sceenshots/Updates_Found.png)

## About
Select the "**About**" Option to see some Useful Information and Links.

![image](https://github.com/RetroDriven/Pocket_Updater/blob/master/Sceenshots/About.png)

## Credits ##
Special thanks to [mattpannella](https://github.com/mattpannella) for Collaborating with me and providing me with his Updater Library Files. His updater can be found [Here](https://github.com/mattpannella/pocket_core_autoupdate_net)
