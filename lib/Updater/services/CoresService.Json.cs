using Newtonsoft.Json;
using Pannella.Models.Analogue.Data;
using Pannella.Models.Analogue.Video;
using Pannella.Models.OpenFPGA_Cores_Inventory;
using AnalogueCore = Pannella.Models.Analogue.Core.Core;

namespace Pannella.Services;

public partial class CoresService
{
    public Platform ReadPlatformJson(string identifier)
    {
        var info = this.ReadCoreJson(identifier);

        if (info == null || info.metadata?.platform_ids == null || info.metadata.platform_ids.Length == 0)
        {
            return null;
        }

        // cores with multiple platforms won't work...not sure any exist right now?
        string platformsFolder = Path.Combine(this.installPath, "Platforms");
        string dataFile = Path.Combine(platformsFolder, info.metadata.platform_ids[0] + ".json");

        if (!File.Exists(dataFile))
        {
            return null;
        }

        try
        {
            var platforms = JsonConvert.DeserializeObject<Dictionary<string, Platform>>(File.ReadAllText(dataFile));

            return platforms != null && platforms.TryGetValue("platform", out var platform) ? platform : null;
        }
        catch
        {
            return null;
        }
    }

    public bool HasMissingPlatformJson(Core core, AnalogueCore localCore = null)
    {
        if (localCore == null && core != null)
        {
            localCore = this.ReadCoreJson(core.identifier);
        }

        var platformIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        if (localCore?.metadata?.platform_ids != null)
        {
            foreach (var id in localCore.metadata.platform_ids)
            {
                if (!string.IsNullOrWhiteSpace(id))
                {
                    platformIds.Add(id);
                }
            }
        }

        if (core != null && !string.IsNullOrWhiteSpace(core.platform_id))
        {
            platformIds.Add(core.platform_id);
        }

        if (core != null && string.IsNullOrEmpty(core.platform_id) && platformIds.Count > 0)
        {
            core.platform_id = platformIds.First();
        }

        if (platformIds.Count == 0)
        {
            return false;
        }

        string platformsFolder = Path.Combine(this.installPath, "Platforms");

        foreach (var id in platformIds)
        {
            string dataFile = Path.Combine(platformsFolder, id + ".json");

            if (!File.Exists(dataFile))
            {
                return true;
            }
        }

        return false;
    }

    public AnalogueCore ReadCoreJson(string identifier)
    {
        string file = Path.Combine(this.installPath, "Cores", identifier, "core.json");

        if (!File.Exists(file))
        {
            return null;
        }

        string json = File.ReadAllText(file);
        AnalogueCore config = JsonConvert.DeserializeObject<Dictionary<string, AnalogueCore>>(json)["core"];

        return config;
    }

    public DataJSON ReadDataJson(string identifier)
    {
        string file = Path.Combine(this.installPath, "Cores", identifier, "data.json");

        if (!File.Exists(file))
        {
            // Log missing data.json for diagnostics and return null to preserve current contract
            this.WriteMessage($"data.json not found for core '{identifier}' at '{file}'");
            return null;
        }

        string json = File.ReadAllText(file);
        DataJSON data = JsonConvert.DeserializeObject<DataJSON>(json);

        return data;
    }

    public Video ReadVideoJson(string identifier)
    {
        string file = Path.Combine(this.installPath, "Cores", identifier, "video.json");

        if (!File.Exists(file))
        {
            return null;
        }

        string json = File.ReadAllText(file);
        Video config = JsonConvert.DeserializeObject<Dictionary<string, Video>>(json)["video"];

        return config;
    }
}
