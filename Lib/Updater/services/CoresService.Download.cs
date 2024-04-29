using System.Collections;
using Newtonsoft.Json;
using Pannella.Exceptions;
using Pannella.Helpers;
using Pannella.Models;
using Pannella.Models.Analogue.Data;
using Pannella.Models.Analogue.Instance;
using Pannella.Models.Analogue.Instance.Simple;
using Pannella.Models.InstancePackager;
using Pannella.Models.OpenFPGA_Cores_Inventory;
using Pannella.Models.Settings;
using AnalogueCore = Pannella.Models.Analogue.Core.Core;
using ArchiveFile = Pannella.Models.Archive.File;
using DataSlot = Pannella.Models.Analogue.Shared.DataSlot;
using File = System.IO.File;
using InstancePackagerDataSlot =  Pannella.Models.InstancePackager.DataSlot;

namespace Pannella.Services;

public partial class CoresService
{
    public void DownloadCoreAssets(List<Core> cores)
    {
        List<string> installedAssets = new List<string>();
        List<string> skippedAssets = new List<string>();
        List<string> missingBetaKeys = new List<string>();

        if (cores == null)
        {
            WriteMessage("List of cores is required.");
            return;
        }

        foreach (var core in cores)
        {
            try
            {
                string name = core.identifier;

                if (name == null)
                {
                    WriteMessage("Core Name is required. Skipping.");
                    continue;
                }

                WriteMessage(core.identifier);

                var results = this.DownloadAssets(core);

                installedAssets.AddRange((List<string>)results["installed"]);
                skippedAssets.AddRange((List<string>)results["skipped"]);

                if ((bool)results["missingBetaKey"])
                {
                    missingBetaKeys.Add(core.identifier);
                }

                Divide();
            }
            catch (Exception e)
            {
                WriteMessage("Uh oh something went wrong.");
#if DEBUG
                WriteMessage(e.ToString());
#else
                WriteMessage(e.Message);
#endif
            }
        }

        UpdateProcessCompleteEventArgs args = new UpdateProcessCompleteEventArgs
        {
            Message = "All Done",
            InstalledAssets = installedAssets,
            SkippedAssets = skippedAssets,
            MissingBetaKeys = missingBetaKeys,
            SkipOutro = false,
        };

        OnUpdateProcessComplete(args);
    }

    public Dictionary<string, object> DownloadAssets(Core core)
    {
        List<string> installed = new List<string>();
        List<string> skipped = new List<string>();
        bool missingBetaKey = false;

        if (!this.settingsService.GetConfig().download_assets ||
            !this.settingsService.GetCoreSettings(core.identifier).download_assets)
        {
            return new Dictionary<string, object>
            {
                { "installed", installed },
                { "skipped", skipped },
                { "missingBetaKey", false }
            };
        }

        WriteMessage("Looking for Assets...");
        Archive archive = this.archiveService.GetArchive(core.identifier);
        AnalogueCore info = this.ReadCoreJson(core.identifier);
        // cores with multiple platforms won't work...not sure any exist right now?
        string platformPath = Path.Combine(this.installPath, "Assets", info.metadata.platform_ids[0]);

        DataJSON dataJson = this.ReadDataJson(core.identifier);

        if (dataJson.data.data_slots.Length > 0)
        {
            foreach (DataSlot slot in dataJson.data.data_slots)
            {
                if (slot.filename != null &&
                    !slot.filename.EndsWith(".sav") &&
                    !this.assetsService.Blacklist.Contains(slot.filename))
                {
                    string path;

                    if (slot.IsCoreSpecific())
                    {
                        path = Path.Combine(platformPath, core.identifier);
                    }
                    else
                    {
                        path = Path.Combine(platformPath, "common");

                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                    }

                    List<string> files = new() { slot.filename };

                    if (slot.alternate_filenames != null)
                    {
                        files.AddRange(slot.alternate_filenames.Where(f => !this.assetsService.Blacklist.Contains(f)));
                    }

                    foreach (string file in files)
                    {
                        string filePath = Path.Combine(path, file);
                        ArchiveFile archiveFile = this.archiveService.GetArchiveFile(file, core.identifier);

                        if (File.Exists(filePath) && CheckCrc(filePath, archiveFile))
                        {
                            if (!this.settingsService.GetConfig().suppress_already_installed)
                                WriteMessage($"Already installed: {file}");
                        }
                        else
                        {
                            bool result = this.archiveService.DownloadArchiveFile(archive, archiveFile, path);

                            if (result)
                            {
                                installed.Add(filePath.Replace(this.installPath, string.Empty));
                            }
                            else
                            {
                                skipped.Add(filePath.Replace(this.installPath, string.Empty));
                            }
                        }
                    }
                }
            }
        }

        if (archive.type == ArchiveType.core_specific_archive && archive.enabled && !archive.has_instance_jsons)
        {
            var files = this.archiveService.GetArchiveFiles(archive);

            string commonPath = Path.Combine(platformPath, "common");

            Directory.CreateDirectory(commonPath);

            foreach (var file in files)
            {
                string filePath = Path.Combine(commonPath, file.name);

                if (File.Exists(filePath) && CheckCrc(filePath, file))
                {
                    if (!this.settingsService.GetConfig().suppress_already_installed)
                        WriteMessage($"Already installed: {file.name}");
                }
                else
                {
                    bool result = this.archiveService.DownloadArchiveFile(archive, file, commonPath);

                    if (result)
                    {
                        installed.Add(filePath.Replace(this.installPath, string.Empty));
                    }
                    else
                    {
                        skipped.Add(filePath.Replace(this.installPath, string.Empty));
                    }
                }
            }
        }

        // These cores have instance json files and the roms are not in the default archive.
        // Check to see if they have a core specific archive defined, skip otherwise.
        // this is getting gross
        if (core.identifier is "Mazamars312.NeoGeo" or "Mazamars312.NeoGeo_Overdrive" or "Mazamars312.NeoGeo_Analogizer" or "obsidian.Vectrex"
            && archive.type != ArchiveType.core_specific_archive)
        {
            return new Dictionary<string, object>
            {
                { "installed", installed },
                { "skipped", skipped },
                { "missingBetaKey", false }
            };
        }

        if (CheckInstancePackager(core.identifier))
        {
            this.BuildInstanceJson(core.identifier);

            return new Dictionary<string, object>
            {
                { "installed", installed },
                { "skipped", skipped },
                { "missingBetaKey", false }
            };
        }

        string instancesDirectory = Path.Combine(this.installPath, "Assets", info.metadata.platform_ids[0], core.identifier);

        if (Directory.Exists(instancesDirectory))
        {
            string[] files = Directory.GetFiles(instancesDirectory, "*.json", SearchOption.AllDirectories);

            foreach (string file in files)
            {
                try
                {
                    // skip mac ._ files
                    if (File.GetAttributes(file).HasFlag(FileAttributes.Hidden))
                    {
                        continue;
                    }

                    if (this.settingsService.GetConfig().skip_alternative_assets &&
                        file.Contains(Path.Combine(instancesDirectory, "_alternatives")))
                    {
                        continue;
                    }

                    InstanceJSON instanceJson = JsonConvert.DeserializeObject<InstanceJSON>(File.ReadAllText(file));

                    if (instanceJson.instance.data_slots is { Length: > 0 })
                    {
                        string dataPath = instanceJson.instance.data_path;

                        foreach (DataSlot slot in instanceJson.instance.data_slots)
                        {
                            var platformId = info.metadata.platform_ids[core.beta_slot_platform_id_index];

                            if (!CheckBetaMd5(slot, core.beta_slot_id, platformId))
                            {
                                // Moved message to the CheckBetaMd5 method
                                missingBetaKey = true;
                            }

                            if (!this.assetsService.Blacklist.Contains(slot.filename) &&
                                !slot.filename.EndsWith(".sav"))
                            {
                                string commonPath = Path.Combine(platformPath, "common");

                                if (!Directory.Exists(commonPath))
                                    Directory.CreateDirectory(commonPath);

                                string slotDirectory = Path.Combine(commonPath, dataPath);
                                string slotPath = Path.Combine(slotDirectory, slot.filename);
                                ArchiveFile archiveFile = this.archiveService.GetArchiveFile(slot.filename, core.identifier);

                                if (File.Exists(slotPath) && CheckCrc(slotPath, archiveFile))
                                {
                                    if (!this.settingsService.GetConfig().suppress_already_installed)
                                        WriteMessage($"Already installed: {slot.filename}");
                                }
                                else
                                {
                                    bool result = this.archiveService.DownloadArchiveFile(archive, archiveFile, slotDirectory);

                                    if (result)
                                    {
                                        installed.Add(slotPath.Replace(this.installPath, string.Empty));
                                    }
                                    else
                                    {
                                        skipped.Add(slotPath.Replace(this.installPath, string.Empty));
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    WriteMessage($"Error while processing '{file}'");
#if DEBUG
                    WriteMessage(e.ToString());
#else
                    WriteMessage(e.Message);
#endif
                }
            }
        }

        Dictionary<string, object> results = new Dictionary<string, object>
        {
            { "installed", installed },
            { "skipped", skipped },
            { "missingBetaKey", missingBetaKey }
        };

        return results;
    }

    public void BuildInstanceJson(string identifier, bool overwrite = true)
    {
        if (!this.settingsService.GetConfig().build_instance_jsons)
        {
            return;
        }

        string instancePackagerFile = Path.Combine(this.installPath, "Cores", identifier, "instance-packager.json");

        if (!File.Exists(instancePackagerFile))
        {
            return;
        }

        WriteMessage("Building instance json files.");
        InstanceJsonPackager jsonPackager = JsonConvert.DeserializeObject<InstanceJsonPackager>(File.ReadAllText(instancePackagerFile));
        string commonPath = Path.Combine(this.installPath, "Assets", jsonPackager.platform_id, "common");
        bool warning = false;

        foreach (string dir in Directory.GetDirectories(commonPath, "*", SearchOption.AllDirectories))
        {
            SimpleInstanceJSON simpleInstanceJson = new SimpleInstanceJSON();
            SimpleInstance instance = new SimpleInstance();
            string dirName = Path.GetFileName(dir);

            try
            {
                instance.data_path = dir.Replace(commonPath + Path.DirectorySeparatorChar, string.Empty) + "/";

                List<SimpleDataSlot> slots = new();
                string jsonFileName = dirName + ".json";

                foreach (InstancePackagerDataSlot slot in jsonPackager.data_slots)
                {
                    string[] files = Directory.GetFiles(dir, slot.filename);
                    int index = slot.id;

                    switch (slot.sort)
                    {
                        case "single":
                        case "ascending":
                            Array.Sort(files);
                            break;

                        case "descending":
                            IComparer myComparer = new ReverseComparer();
                            Array.Sort(files, myComparer);
                            break;
                    }

                    if (slot.required && !files.Any())
                    {
                        throw new MissingRequiredInstanceFiles("Missing required files.");
                    }

                    foreach (string file in files)
                    {
                        if (File.GetAttributes(file).HasFlag(FileAttributes.Hidden))
                        {
                            continue;
                        }

                        SimpleDataSlot current = new();
                        string filename = Path.GetFileName(file);

                        if (slot.as_filename)
                        {
                            jsonFileName = Path.GetFileNameWithoutExtension(file) + ".json";
                        }

                        current.id = index.ToString();
                        current.filename = filename;
                        index++;
                        slots.Add(current);
                    }
                }

                var limit = (long)jsonPackager.slot_limit["count"];

                if (slots.Count == 0 || (jsonPackager.slot_limit != null && slots.Count > limit))
                {
                    WriteMessage($"Unable to build {jsonFileName}");
                    warning = true;
                    continue;
                }

                instance.data_slots = slots.ToArray();
                simpleInstanceJson.instance = instance;

                string[] parts = dir.Split(commonPath);
                parts = parts[1].Split(jsonFileName.Remove(jsonFileName.Length - 5));
                string subDirectory = string.Empty;

                if (parts[0].Length > 1)
                {
                    subDirectory = parts[0].Trim(Path.DirectorySeparatorChar);
                }

                string outputFile = Path.Combine(this.installPath, jsonPackager.output, subDirectory, jsonFileName);

                if (!overwrite && File.Exists(outputFile))
                {
                    WriteMessage($"{jsonFileName} already exists.");
                }
                else
                {
                    string json = JsonConvert.SerializeObject(simpleInstanceJson, Formatting.Indented);

                    WriteMessage($"Saving '{jsonFileName}'...");

                    FileInfo file = new FileInfo(outputFile);

                    file.Directory.Create(); // If the directory already exists, this method does nothing.

                    File.WriteAllText(outputFile, json);
                }
            }
            catch (MissingRequiredInstanceFiles)
            {
                // Do nothing.
            }
            catch (Exception)
            {
                WriteMessage($"Unable to build {dirName}");
            }
        }

        if (warning)
        {
            var message = (string)jsonPackager.slot_limit["message"];

            WriteMessage(message);
        }

        WriteMessage("Finished");
    }

    public bool CheckInstancePackager(string identifier)
    {
        string instancePackagerFile = Path.Combine(this.installPath, "Cores", identifier, "instance-packager.json");

        return File.Exists(instancePackagerFile);
    }
}
