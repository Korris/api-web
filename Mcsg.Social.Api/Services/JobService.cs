using Mcsg.Social.Api.Constants;
using Mcsg.Social.Api.DTOs;
using Mcsg.Social.Api.Services.Interfaces;
using Mcsg.Lib.Common.Constants;
using Mcsg.Lib.Common.Extensions;
using Mcsg.Lib.Data.Domain.Entities;
using Mcsg.Lib.Data.Enums;
using Mcsg.Lib.Data.Repositories;
using Newtonsoft.Json;

namespace Mcsg.Social.Api.Services
{
    public class JobService : IJobService
    {
        private readonly IRepository<Job> _jobRepository;
        private readonly INotificationService _notificationService;
        public JobService(IRepository<Job> jobRepository, INotificationService notificationService)
        {
            _jobRepository = jobRepository;
            _notificationService = notificationService;
        }

        public async Task CreateConvertJob(Resource resource, string userName, string userAvatar, string blobName)
        {
            if (resource.Type == ResourceType.VIDEO)
                await ConvertVideo(resource, userName, userAvatar, blobName);
            if (resource.Type == ResourceType.AUDIO)
                await ConvertAudio(resource, userName, userAvatar, blobName);
        }

        private async Task ConvertVideo(Resource resource, string userName, string userAvatar, string blobName)
        {
            bool canConvert = true;
            string fileExtension = Path.GetExtension(blobName);
            if (!FileExt.AllowPlayAfterUpload.Contains(fileExtension, StringComparer.OrdinalIgnoreCase))
            {
                resource.Status = ResourceStatus.PROCESSING;
            }
            else
            {
                canConvert = resource.Size.ToMegabytes() > SystemConfig.ValidVideoSize;
            }

            if (canConvert)
            {
                var convertJob = new Job()
                {
                    JobCategory = JobCategory.Media,
                    Id = Guid.NewGuid(),
                    Status = JobStatus.Queued,
                    JobType = JobType.ConvertVideo,
                    Data = JsonConvert.SerializeObject(resource)
                };
                await _jobRepository.InsertAsync(convertJob);

                //Send notification when video process processing
                var notiReq = new VideoNotificationReq()
                {
                    Id = resource.Id,
                    AuthorId = resource.AuthorId.Value,
                    AuthorName = userName,
                    Action = NotificationAction.Processing,
                    HashId = resource.HashId,
                    TargetType = NotificationTargetType.None,
                    UserAvatar = userAvatar
                };

                await _notificationService.AddVideoNotificationAsync(notiReq);
            }
        }

        private async Task ConvertAudio(Resource resource, string userName, string userAvatar, string blobName)
        {
            bool canConvert = true;
            string fileExtension = Path.GetExtension(blobName);
            if (!FileExt.AllowPlayAfterUpload.Contains(fileExtension, StringComparer.OrdinalIgnoreCase))
            {
                resource.Status = ResourceStatus.PROCESSING;
            }
            else
            {
                canConvert = resource.Size.ToMegabytes() > SystemConfig.ValidVideoSize;
            }

            if (canConvert)
            {
                var convertJob = new Job()
                {
                    JobCategory = JobCategory.Media,
                    Id = Guid.NewGuid(),
                    Status = JobStatus.Queued,
                    JobType = JobType.ConvertAudio,
                    Data = JsonConvert.SerializeObject(resource),
                };
                await _jobRepository.InsertAsync(convertJob);
            }
        }
    }
}
