namespace Mcsg.Media.Tool.Workers;

using Common.Core.Interfaces;
using Common.Domain;
using Common.SeedWork.Enums;
using Common.SeedWork.Extensions;
using Interfaces;
using Models;
using Services;

internal abstract class BaseWorker
{
    public BaseWorker(IMcsgContext context, ISetting setting, IStorageClient sc)
    {
        _context = context;
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

    public async Task<long> UploadBlobAsync(string localFile, string remoteUri)
    {
        var fs = File.OpenRead(localFile);
        var res = fs.Length;
        await _sc.GetStrategy(MinioInstanceType.Default).PutObject(fs, remoteUri, null);
        fs.Close();
        return res;
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

    #region -- Fields --

    /// <summary>
    /// DB context
    /// </summary>
    protected readonly IMcsgContext _context;

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
