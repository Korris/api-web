using Mcsg.Lib.Data.Domain.Entities;
using Mcsg.Lib.Data.Enums;
using Mcsg.Media.Tool.Actions;
using Mcsg.Media.Tool.Features;
using Mcsg.Media.Tool.Workers;
using Microsoft.Extensions.Configuration;
using System.Collections.Concurrent;

namespace Mcsg.Media.Tool
{
    internal class WorkDistributor
    {
        private IDictionary<JobType, IWorker> _workers;
        private readonly DbService _dbService;
        private readonly ConcurrentQueue<Job> _jobQueue = new();

        public WorkDistributor(IConfiguration configuration)
        {
            _dbService = new DbService(configuration["ConnectionStrings:DefaultConnection"]);
            LoadWorker(configuration);
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

        private void LoadWorker(IConfiguration configuration)
        {
            _workers = new Dictionary<JobType, IWorker>
            {
                { JobType.ConvertVideo, new ConvertVideoWorker(configuration) },
                { JobType.ConvertAudio, new ConvertAudioWorker(configuration) }
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
    }
}