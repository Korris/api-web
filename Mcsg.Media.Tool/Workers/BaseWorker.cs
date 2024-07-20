using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Mcsg.Media.Tool.Workers
{
    using Common.Core.Interfaces;
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

            var fs = await _sc.Strategy.GetObject(path, null);
            fs.ToFile(filePath);

            return filePath;
        }

        public async Task UploadBlobAsync(string localFile, string remoteUri)
        {
            var fs = File.OpenRead(localFile);
            await _sc.Strategy.PutObject(fs, remoteUri, null);
            fs.Close();
        }

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
}
