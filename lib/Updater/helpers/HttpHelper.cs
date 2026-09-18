using System.Net;

namespace Pannella.Helpers;

public class HttpHelper
{
    private static HttpHelper instance;
    private static readonly object SYNC_LOCK = new();
    private HttpClient client;

    public event EventHandler<DownloadProgressEventArgs> DownloadProgressUpdate;

    private HttpHelper()
    {
        this.CreateClient();
    }

    public static HttpHelper Instance
    {
        get
        {
            lock (SYNC_LOCK)
            {
                return instance ??= new HttpHelper();
            }
        }
    }

    public void DownloadFile(string uri, string outputPath, int timeout = 100)
    {
        bool console = false;

        //MessageBox.Show("Download Location: " + uri);

        try
        {
            _ = Console.WindowWidth;
            console = true;
        }
        catch
        {
            // Ignore
        }

        UpdateCancellation.ThrowIfCancellationRequested();

        using var timeoutCts = new CancellationTokenSource();
        timeoutCts.CancelAfter(TimeSpan.FromSeconds(timeout));
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(timeoutCts.Token, UpdateCancellation.Token);

        if (!Uri.TryCreate(uri, UriKind.Absolute, out _))
        {
            throw new InvalidOperationException("URI is invalid.");
        }

        try
        {
            using HttpResponseMessage responseMessage = this.client
                .GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, cts.Token)
                .GetAwaiter().GetResult();

        // Just in case the HttpClient doesn't throw the error on 404 like it should.
        if (responseMessage.StatusCode == HttpStatusCode.NotFound)
        {
            throw new HttpRequestException("Not Found.", null, HttpStatusCode.NotFound);
        }

        var totalSize = responseMessage.Content.Headers.ContentLength ?? -1L;
        var readSoFar = 0L;
        var buffer = new byte[4096];
        var isMoreToRead = true;

            using var stream = responseMessage.Content.ReadAsStream(cts.Token);
            using var fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write);

            while (isMoreToRead)
            {
                cts.Token.ThrowIfCancellationRequested();
                var read = stream.ReadAsync(buffer, 0, buffer.Length, cts.Token).GetAwaiter().GetResult();

                if (read == 0)
                {
                    isMoreToRead = false;

                    if (console)
                    {
                        Console.Write("\r");
                    }
                }
                else
                {
                    readSoFar += read;

                    var progress = totalSize > 0 ? (double)readSoFar / totalSize : 0d;

                    if (console && totalSize > 0)
                    {
                        ConsoleHelper.ShowProgressBar(readSoFar, totalSize);
                    }

                    DownloadProgressEventArgs args = new()
                    {
                        Progress = progress,
                        BytesReceived = readSoFar,
                        TotalBytes = totalSize
                    };

                    OnDownloadProgressUpdate(args);

                    fileStream.Write(buffer, 0, read);
                }
            }
        }
        catch (OperationCanceledException) when (UpdateCancellation.IsCancellationRequested)
        {

            try
            {
                if (File.Exists(outputPath))
                    File.Delete(outputPath);
            }
            catch
            {

            }

            throw;
        }
    }

    public string GetHTML(string uri, bool allowRedirect = true)
    {
        if (!Uri.TryCreate(uri, UriKind.Absolute, out _))
        {
            throw new InvalidOperationException("URI is invalid.");
        }

        if (!allowRedirect)
        {
            this.CreateClient(false);
        }

        UpdateCancellation.ThrowIfCancellationRequested();
        var response = this.client.GetAsync(uri, UpdateCancellation.Token).GetAwaiter().GetResult();
        string html = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        UpdateCancellation.ThrowIfCancellationRequested();

        if (!allowRedirect)
        {
            this.CreateClient();
        }

        return html;
    }

    private void CreateClient(bool allowRedirect = true)
    {
        this.client = new HttpClient(new HttpClientHandler { AllowAutoRedirect = allowRedirect });
        this.client.Timeout = TimeSpan.FromMinutes(10); // 10min
    }

    private void OnDownloadProgressUpdate(DownloadProgressEventArgs e)
    {
        EventHandler<DownloadProgressEventArgs> handler = DownloadProgressUpdate;

        handler?.Invoke(this, e);
    }
}

public class DownloadProgressEventArgs : EventArgs
{
    public double Progress;
    public long BytesReceived;
    public long TotalBytes;
}
