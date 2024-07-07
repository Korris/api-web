namespace Mcsg.Social.Api.Interfaces;

using Models;
using Requests;

public interface IMetaDataService
{
    Task<MetaDataResponse> AddMetaDataToObject<T>(MetaDataReq request, Guid objId);
}
