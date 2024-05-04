namespace Pannella.Models;

public class UpdateProcessCompleteEventArgs : EventArgs
{
    public string Message { get; set; }
    public List<Dictionary<string, string>> InstalledCores { get; set; }
    public List<string> InstalledAssets { get; set; }
    public List<string> SkippedAssets { get; set; }
    public string FirmwareUpdated { get; set; } = string.Empty;
    public List<string> MissingBetaKeys { get; set; }
    public bool SkipOutro;
}
