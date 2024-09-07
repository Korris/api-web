using System.Collections.Concurrent;

namespace Mcsg.Media.Tool;

using Common.Core.Enums;
using Common.Core.Interfaces;
using Common.Domain;
using Common.Domain.Entities;
using Interfaces;
using Workers;

internal class WorkDistributor
{
    #region -- Methods --

    public WorkDistributor(IMcsgContext context, ISetting setting, IStorageClient sc)
    {
        _dbService = new DbService(setting.DefaultConnection);
        _sc = sc;
        _workers = new Dictionary<JobType, IWorker>
        {
            { JobType.ConvertVideo, new ConvertVideoWorker(context,setting, _sc) },
            { JobType.ConvertAudio, new ConvertAudioWorker(context, setting, _sc) }
        };

        LoadActiveJobs();
        CleanupWorkerPool();
    }

    /// <summary>
    /// Run
    /// </summary>
    /// <param name="cancellationToken">This allows you to stop the loop gracefully when needed</param>
    /// <returns></returns>
    public async Task Run(CancellationToken cancellationToken)
    {
        while (true)
        {
            if (_jobQueue.Count == 0)
            {
                await Task.Delay(100, cancellationToken); // add a delay to reduce CPU usage
                continue;
            }

            _jobQueue.TryDequeue(out Job? job);
            if (job != null)
            {
                _workers.TryGetValue(job.JobType, out IWorker? worker);
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

    private void CleanupWorkerPool()
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

    #endregion

    #region -- Fields --

    /// <summary>
    /// Storage client
    /// </summary>
    private readonly IStorageClient _sc;

    /// <summary>
    /// Dictionary worker
    /// </summary>
    private Dictionary<JobType, IWorker> _workers;

    /// <summary>
    /// Db service
    /// </summary>
    private readonly DbService _dbService;

    /// <summary>
    /// Job queue
    /// </summary>
    private readonly ConcurrentQueue<Job> _jobQueue = new();

    #endregion
}
