namespace Mcsg.Story.Api.Interfaces;

using Lib.Data.Domain.Entities;

public interface IJobService
{
    Task CreateConvertJob(StoryResource resource, string userName, string userAvatar, string blobName);
}
