using Mcsg.Api.DTOs;
using Mcsg.Api.Models;

namespace Mcsg.Api.Services.Interfaces
{
    public interface IMetaDataService
    {
        Task<MetaDataResponse> AddMetaDataToObject<T>(MetaDataReq request, Guid objId);
    }
}
