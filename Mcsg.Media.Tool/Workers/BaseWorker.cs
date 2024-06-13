using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Mcsg.Media.Tool.Workers
{
    using Common.Core.Interfaces;
    using Common.Core.Storages;
    using Common.SeedWork.Extensions;
    using Models;
    using Services;

    internal abstract class BaseWorker
    {
        protected List<WorkerPoolItem> Pools { get; }
        protected DbService DbService { get; }
        protected NotificationService NotiService { get; }
        protected IConfiguration Configuration { get; }
        protected string StorageAccountName { get; }
        protected const string MediaContainer = "media";

        public BaseWorker(IConfiguration configuration, IStorageClient sc)
        {
            Configuration = configuration;
            DbService = new DbService(configuration["ConnectionStrings:DefaultConnection"]);
            StorageAccountName = configuration["ConnectionStrings:StorageAccountName"];
            Pools = new List<WorkerPoolItem>();
            NotiService = new NotificationService(configuration);

            sc.SetStrategy(new StorageMinio());
            _sc = sc;
        }

        public void AddToPools(Guid id, Task task)
        {
            Pools.Add(new WorkerPoolItem { Id = id, Task = task });
        }

        public int PoolSize => int.Parse(Configuration["AppSettings:PoolSize"]);

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

            var objectName = $"{MediaContainer}/{path}";
            var fs = await _sc.GetObject(objectName);
            fs.ToFile(filePath);

            return filePath;
        }

        public async Task UploadBlobAsync(string localFile, string remoteUri)
        {
            var fileStream = File.OpenRead(localFile);

            var objectName = $"{MediaContainer}/{remoteUri}";
            await _sc.PutObject(fileStream, objectName, null);

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

        #region -- Fields --

        /// <summary>
        /// Storage client
        /// </summary>
        protected readonly IStorageClient _sc;

        /// <summary>
        /// 10 years
        /// </summary>
        protected readonly int _expiryInSeconds = 10 * 365 * 24 * 60 * 60; // 10 years

        #endregion
    }
}