namespace Mcsg.Social.Api.Interfaces
{
    using DTOs;
    using Models;

    public interface IMetaDataService
    {
        Task<MetaDataResponse> AddMetaDataToObject<T>(MetaDataReq request, Guid objId);
    }
}
