using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Mcsg.Media.Tool.Workers;

using Common.Core.Interfaces;
using Common.SeedWork.Enums;
using Common.SeedWork.Extensions;
using Interfaces;
using Models;
using Services;

internal abstract class BaseWorker
{
    public BaseWorker(ISetting setting, IStorageClient sc)
    {
        _setting = setting;
        _sc = sc;

        DbService = new DbService(setting.DefaultConnection);
        Pools = new List<WorkerPoolItem>();
        NotiService = new NotificationService();
    }

    public void AddToPools(Guid id, Task task)
    {
        Pools.Add(new WorkerPoolItem { Id = id, Task = task });
    }

    public async Task<string> DownloadBlobAsync(string path, Guid resourceId)
    {
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp");
        var newName = Path.GetFileNameWithoutExtension(path) + resourceId.ToString() + Path.GetExtension(path);
        filePath = Path.Combine(filePath, Path.GetDirectoryName(path), newName);
        if (!Directory.Exists(Path.GetDirectoryName(filePath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        }

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        var fs = await _sc.GetStrategy(MinioInstanceType.Default).GetObject(path, null);
        fs.ToFile(filePath);

        return filePath;
    }

    public async Task UploadBlobAsync(string localFile, string remoteUri)
    {
        var fs = File.OpenRead(localFile);
        await _sc.GetStrategy(MinioInstanceType.Default).PutObject(fs, remoteUri, null);
        fs.Close();
    }

    public string RunFfmpeg(string input, string output, string command)
    {
        var arguments = $"-i {input} {command} {output}";
        return RunProcess("ffmpeg", arguments).GetAwaiter().GetResult();
    }

    public string RunFfprobe(string input, string command)
    {
        var arguments = $"-v {command} {input}";
        return RunProcess("ffprobe", arguments).GetAwaiter().GetResult();
    }

    public bool HasAvailableSlot()
    {
        return Pools.Count() < _setting.PoolSize;
    }

    public void CleanupPool()
    {
        var pools = Pools.Where(x => x.IsRunning == false);
        foreach (var item in pools)
        {
            item.Task.Dispose();
        }
        Pools.RemoveAll(x => x.IsRunning == false);
        GC.SuppressFinalize(this);
        GC.Collect();
    }

    private async Task<string> RunProcess(string exepath, string arguments)
    {
        var psi = new ProcessStartInfo
        {
            FileName = exepath,
            Arguments = arguments,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardError = true
        };

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var enviromentPath = Environment.GetEnvironmentVariable("PATH");
            var paths = (enviromentPath + "").Split(';');

            var filePath = paths.Select(p => Path.Combine(p, $"{exepath}.exe")).Where(p => File.Exists(p)).FirstOrDefault();
            if (filePath != null)
            {
                exepath = filePath;
            }
        }

        // Start the process
        using Process process = new() { StartInfo = psi };
        var outputBuilder = new StringBuilder();
        var errorBuilder = new StringBuilder();

        // Capture the output
        process.OutputDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                outputBuilder.AppendLine(e.Data);
                Console.WriteLine(e.Data);
            }
        };

        process.ErrorDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                errorBuilder.AppendLine(e.Data);
                Console.WriteLine(e.Data);
            }
        };

        process.Start();

        // Start reading output and error asynchronously
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        // Wait for process to exit asynchronously
        await process.WaitForExitAsync();

        var output = outputBuilder.ToString();
        var error = errorBuilder.ToString();

        return string.IsNullOrWhiteSpace(error) ? output : $"Error: {error}";
    }

    #region -- Fields --

    /// <summary>
    /// Setting
    /// </summary>
    protected readonly ISetting _setting;

    /// <summary>
    /// Storage client
    /// </summary>
    protected readonly IStorageClient _sc;

    protected List<WorkerPoolItem> Pools { get; }

    protected DbService DbService { get; }

    protected NotificationService NotiService { get; }

    #endregion
}
