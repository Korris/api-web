using Mcsg.Lib.AzureBlobStorage;
using Mcsg.Media.Tool.Models;
using Mcsg.Media.Tool.Services;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace Mcsg.Media.Tool.Workers
{
    internal abstract class BaseWorker
    {
        protected List<WorkerPoolItem> Pools { get; }
        protected DbService DbService { get; }
        protected NotificationService NotiService { get; }
        protected IAzureBlobStorageService BlobStorageService { get; }
        protected IConfiguration Configuration { get; }
        protected string StorageAccountName { get; }
        protected const string MediaContainer = "media";

        public BaseWorker(IConfiguration configuration)
        {
            Configuration = configuration;
            DbService = new DbService(configuration["ConnectionStrings:DefaultConnection"]);
            BlobStorageService = new AzureBlobStorageService(configuration["ConnectionStrings:StorageConnection"]);
            StorageAccountName = configuration["ConnectionStrings:StorageAccountName"];
            Pools = new List<WorkerPoolItem>();
            NotiService = new NotificationService(configuration);
        }

        public void AddToPools(Guid id, Task task)
        {
            Pools.Add(new WorkerPoolItem { Id = id, Task = task });
        }

        public int PoolSize => int.Parse(Configuration["AppSettings:PoolSize"]);

        public string CloneFFmpeg()
        {
            var src = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ffmpeg.exe");
            var des = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"ffmpeg_{Guid.NewGuid()}.exe");
            File.Copy(src, des);
            return des;
        }

        public void DisposeClonedFFmpeg(string path)
        {
            File.Delete(path);
        }

        public async Task<string> DownloadBlobAsync(string path, Guid resourceId)
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, MediaContainer);
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

            await BlobStorageService.DownloadAsync(path, MediaContainer, filePath);
            return filePath;
        }

        public async Task UploadBlobAsync(string localFile, string remoteUri)
        {
            FileStream fileStream = File.OpenRead(localFile);

            if (await BlobStorageService.IsExistAsync(remoteUri, MediaContainer))
                await BlobStorageService.DeleteAsync(remoteUri, MediaContainer);

            await BlobStorageService.UploadAsync(remoteUri, fileStream, MediaContainer);
            fileStream.Close();
        }

        public string EncryptKey => Configuration["AppSettings:EncryptKey"];

        public void RunFFmeg(string exepath, string input, string output, string command)
        {
            ProcessStartInfo psi = new()
            {
                FileName = exepath,
                Arguments = $"-i {input} {command} {output}",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true,
            };

            // Start the FFmpeg process.
            Process process = new()
            {
                StartInfo = psi
            };

            process.OutputDataReceived += (sender, e) => Console.WriteLine(e.Data);
            process.ErrorDataReceived += (sender, e) => Console.WriteLine(e.Data);

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
            process.Dispose();
        }

        public bool HasAvailableSlot()
        {
            return Pools.Count() < PoolSize;
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
    }
}