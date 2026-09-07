namespace Mcsg.Api.Areas.Comic.Interfaces;

using Common.Domain.Entities;

public interface IJobService
{
    Task CreateConvertJob(ComicResource resource, string userName, string userAvatar, string blobName);
}
