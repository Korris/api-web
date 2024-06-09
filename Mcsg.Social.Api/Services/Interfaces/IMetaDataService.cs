using Mcsg.Social.Api.DTOs;
using Mcsg.Social.Api.Models;

namespace Mcsg.Social.Api.Services.Interfaces
{
    public interface IMetaDataService
    {
        Task<MetaDataResponse> AddMetaDataToObject<T>(MetaDataReq request, Guid objId);
    }
}
