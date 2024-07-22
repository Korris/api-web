using Newtonsoft.Json;

namespace Mcsg.Comic.Api.Services;

using Common.Core.Constants;
using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Core.Requests;
using Common.Domain;
using Common.Domain.Entities;
using Common.SeedWork.Extensions;
using Dtos;
using Interfaces;

public class JobService : IJobService
{
    public JobService(McsgDbContext context, ISetting setting)
    {
        _context = context;
        _setting = setting;
    }

    public async Task CreateConvertJob(ComicResource resource, string userName, string userAvatar, string blobName)
    {
        if (resource.Type == ResourceType.Video)
        {
            await ConvertVideo(resource, userName, userAvatar, blobName);
        }
        if (resource.Type == ResourceType.Audio)
        {
            await ConvertAudio(resource, userName, userAvatar, blobName);
        }
    }

    private async Task ConvertVideo(ComicResource resource, string userName, string userAvatar, string blobName)
    {
        bool canConvert = true;
        string fileExtension = Path.GetExtension(blobName);
        if (!Setting.FileExt.AllowPlayAfterUpload.Contains(fileExtension, StringComparer.OrdinalIgnoreCase))
        {
            resource.Status = ResourceStatus.Processing;
        }
        else
        {
            canConvert = resource.Size.ToMegabytes() > Setting.PostConfig.ValidVideoSize;
        }

        if (canConvert)
        {
            var convertJob = new Job
            {
                JobCategory = JobCategory.Media,
                Id = Guid.NewGuid(),
                Status = JobStatus.Queued,
                JobType = JobType.ConvertVideo,
                Data = JsonConvert.SerializeObject(resource)
            };
            await _context.Jobs.AddAsync(convertJob);
            await _context.SaveChangesAsync();

            // Send notification when video process processing
            var notiReq = new VideoNotificationR
            {
                Id = resource.Id,
                AuthorId = resource.AuthorId.Value,
                AuthorName = userName,
                Action = NotificationAction.Processing,
                HashId = resource.HashId,
                TargetType = Setting.NotificationTargetType.None,
                UserAvatar = userAvatar
            };

            await AddVideoNotificationAsync(notiReq);
        }
    }

    private async Task ConvertAudio(ComicResource resource, string userName, string userAvatar, string blobName)
    {
        bool canConvert = true;
        string fileExtension = Path.GetExtension(blobName);
        if (!Setting.FileExt.AllowPlayAfterUpload.Contains(fileExtension, StringComparer.OrdinalIgnoreCase))
        {
            resource.Status = ResourceStatus.Processing;
        }
        else
        {
            canConvert = resource.Size.ToMegabytes() > Setting.PostConfig.ValidVideoSize;
        }

        if (canConvert)
        {
            var convertJob = new Job
            {
                JobCategory = JobCategory.Media,
                Id = Guid.NewGuid(),
                Status = JobStatus.Queued,
                JobType = JobType.ConvertAudio,
                Data = JsonConvert.SerializeObject(resource),
            };
            await _context.Jobs.AddAsync(convertJob);
            await _context.SaveChangesAsync();
        }
    }

    private async Task<bool> AddVideoNotificationAsync(VideoNotificationR req)
    {
        var baseUrl = _setting.Api.Realtime;
        var urlBuilder = new System.Text.StringBuilder();
        urlBuilder.Append(baseUrl != null ? baseUrl.TrimEnd('/') : "").Append("/notification/video");

        var url = urlBuilder.ToString();

        var response = await url.MakePostRequest(req);

        if (response.IsSuccessStatusCode)
        {
            string responseContent = await response.Content.ReadAsStringAsync();
            var responseBody = JsonConvert.DeserializeObject<ApiNotificationDto>(responseContent);

            return true;
        }
        else
        {
            return false;
        }
    }

    #region -- Fields --

    /// <summary>
    /// DB Context
    /// </summary>
    private readonly McsgDbContext _context;

    /// <summary>
    /// Setting
    /// </summary>
    private readonly ISetting _setting;

    #endregion
}
