using Mcsg.Function.Job.Services;
using Mcsg.Lib.Data.Enums;
using Mcsg.Lib.Data.Repositories;
using Mcsg.Lib.Data.Repositories.Interface;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Mcsg.Function.Job
{
    public class SmsFunction
    {
        private readonly ILogger<SmsFunction> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Lib.Data.Domain.Entities.Job> _jobRepository;
        private readonly ISmsService _smsService;

        public SmsFunction(ILogger<SmsFunction> logger,
            IUnitOfWork unitOfWork, ISmsService smsService)
        {
            _logger = logger;
            _smsService = smsService;
            _unitOfWork = unitOfWork;
            _jobRepository = unitOfWork.GetRepository<Lib.Data.Domain.Entities.Job>();
        }

        [Function(nameof(SmsFunction))]
        public async Task Run([QueueTrigger("smsqueue", Connection = "Function:AzureBlobStorageConnection")] Guid jobId)
        {
            _logger.LogInformation($"SMS Queue trigger function processed: {jobId}");
            var jobDb = await _jobRepository.GetByIdAsync(jobId);

            if (jobDb != null && jobDb.Status != JobStatus.Success)
            {
                jobDb.Status = JobStatus.Processing;
                await _jobRepository.UpdateAsync(jobDb);

                try
                {
                    await _smsService.SendSmsAsync(jobDb);

                    jobDb.Status = JobStatus.Success;
                    await _jobRepository.UpdateAsync(jobDb);

                    _logger.LogInformation($"SMS Queue trigger function Success: {jobId}");
                }
                catch (Exception ex)
                {
                    jobDb.Status = JobStatus.Failed;
                    jobDb.Error = $"{ex.Message} {ex.StackTrace}";
                    await _jobRepository.UpdateAsync(jobDb);
                    throw;
                }
            }
        }
    }
}
