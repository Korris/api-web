namespace Mcsg.Api.Areas.Social.Interfaces;

using Common.Domain.Entities;

public interface IJobService
{
    Task CreateConvertJob(SocialResource resource, string userName, string userAvatar, string blobName);
}
