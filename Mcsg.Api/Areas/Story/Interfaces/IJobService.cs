namespace Mcsg.Api.Areas.Story.Interfaces;

using Common.Domain.Entities;

public interface IJobService
{
    Task CreateConvertJob(StoryResource resource, string userName, string userAvatar, string blobName);
}
