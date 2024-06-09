using Mcsg.Lib.Data.Domain.Entities;

namespace Mcsg.Social.Api.Services.Interfaces
{
    public interface IJobService
    {
        Task CreateConvertJob(Resource resource, string userName, string userAvatar, string blobName);
    }
}
