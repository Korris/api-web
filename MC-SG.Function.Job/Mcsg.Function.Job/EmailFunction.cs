using Mcsg.Function.Job.Services;
using Mcsg.Lib.Data.Enums;
using Mcsg.Lib.Data.Repositories;
using Mcsg.Lib.Data.Repositories.Interface;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Entities = Mcsg.Lib.Data.Domain.Entities;

namespace Mcsg.Function.Job
{
    public class EmailFunction
    {
        private readonly ILogger<EmailFunction> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Entities.Job> _jobRepository;
        private readonly IEmailService _emailService;
        public EmailFunction(ILogger<EmailFunction> logger,
            IUnitOfWork unitOfWork, IEmailService emailService)
        {
            _emailService = emailService;
            _unitOfWork = unitOfWork;
            _jobRepository = unitOfWork.GetRepository<Entities.Job>();
            _logger = logger;
        }

        [Function(nameof(EmailFunction))]
        public async Task Run([QueueTrigger("emailqueue", Connection = "Function:AzureBlobStorageConnection")] Guid jobId)
        {
            string logMessage = $"C# Queue trigger function processed: {jobId}";
            _logger.LogInformation(logMessage);

            var jobDb = await _jobRepository.GetByIdAsync(jobId);
            if (jobDb != null && jobDb.Status != JobStatus.Success)
            {
                jobDb.Status = JobStatus.Processing;
                await _jobRepository.UpdateAsync(jobDb);

                try
                {
                    await _emailService.SendEmailAsync(jobDb);

                    jobDb.Status = JobStatus.Success;
                    await _jobRepository.UpdateAsync(jobDb);
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
