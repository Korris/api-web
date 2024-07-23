namespace Mcsg.Social.Api.Interfaces;

using Common.Domain.Entities;

public interface IJobService
{
    Task CreateConvertJob(SocialResource resource, string userName, string userAvatar, string blobName);
}
