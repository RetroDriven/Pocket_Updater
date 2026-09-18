namespace Pannella.Helpers;

public static class UpdateCancellation
{
    private static readonly object SyncRoot = new();
    private static CancellationTokenSource source = new();

    public static CancellationToken Token
    {
        get
        {
            lock (SyncRoot)
                return source.Token;
        }
    }

    public static bool IsCancellationRequested => Token.IsCancellationRequested;

    public static void Reset()
    {
        lock (SyncRoot)
        {
            source.Dispose();
            source = new CancellationTokenSource();
        }
    }

    public static void RequestCancel()
    {
        lock (SyncRoot)
        {
            if (!source.IsCancellationRequested)
                source.Cancel();
        }
    }

    public static void ThrowIfCancellationRequested()
    {
        Token.ThrowIfCancellationRequested();
    }
}
