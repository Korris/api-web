namespace Mcsg.Comic.Api.Interfaces;

using Lib.Data.Domain.Entities;

public interface IJobService
{
    Task CreateConvertJob(ComicResource resource, string userName, string userAvatar, string blobName);
}
