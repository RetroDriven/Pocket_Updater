namespace Pannella.Models;

public sealed class OverallProgressEventArgs : EventArgs
{
    public int Completed { get; init; }
    public int Total { get; init; }
    public string CurrentCore { get; init; } = string.Empty;

    public double Progress => Total <= 0 ? 1d : Math.Clamp((double)Completed / Total, 0d, 1d);
}
