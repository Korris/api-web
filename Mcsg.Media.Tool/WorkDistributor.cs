using System.Collections.Concurrent;

namespace Mcsg.Media.Tool
{
    using Common.Core.Enums;
    using Common.Core.Interfaces;
    using Interfaces;
    using Lib.Data.Domain.Entities;
    using Workers;

    internal class WorkDistributor
    {
        private IDictionary<JobType, IWorker> _workers;
        private readonly DbService _dbService;
        private readonly ConcurrentQueue<Job> _jobQueue = new();

        public WorkDistributor(ISetting setting, IStorageClient sc)
        {
            _dbService = new DbService(setting.DefaultConnection);
            _sc = sc;

            LoadWorker(setting);
            LoadActiveJobs();
            TrytoCleanupWorkerPool();
        }

        public async Task Run()
        {
            while (true)
            {
                if (_jobQueue.Count() == 0)
                    continue;

                _jobQueue.TryDequeue(out Job job);
                if (job != null)
                {
                    _workers.TryGetValue(job.JobType, out IWorker worker);
                    if (worker != null && worker.HasAvailableSlot())
                    {
                        Console.WriteLine("Processing Job Id: {0}", job.Id);
                        worker.Execute(job);
                    }
                    else
                    {
                        _jobQueue.Enqueue(job);
                    }
                }
                GC.SuppressFinalize(this);
                GC.Collect();
                await Task.Delay(3000);
            }
        }

        private void LoadWorker(ISetting setting)
        {
            _workers = new Dictionary<JobType, IWorker>
            {
                { JobType.ConvertVideo, new ConvertVideoWorker(setting, _sc) },
                { JobType.ConvertAudio, new ConvertAudioWorker(setting, _sc) }
            };
        }

        private void LoadActiveJobs()
        {
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    await _dbService.LoadActiveJobs(_jobQueue);
                    await Task.Delay(2000);
                }
            });
        }

        private void TrytoCleanupWorkerPool()
        {
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    foreach (var worker in _workers.Values)
                    {
                        worker.CleanupPool();
                    }
                    await Task.Delay(2000);
                }
            });
        }

        #region -- Fields --

        /// <summary>
        /// Storage client
        /// </summary>
        private readonly IStorageClient _sc;

        #endregion
    }
}
