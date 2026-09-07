namespace Mcsg.Api.Areas.Document.Interfaces;

using Common.Domain.Entities;

public interface IJobService
{
    Task CreateConvertJob(DocumentResource resource, string userName, string userAvatar, string blobName);
}
