namespace Mcsg.Social.Api.Interfaces;

using Dtos;
using Models;

public interface IMetaDataService
{
    Task<MetaDataResponse> AddMetaDataToObject<T>(MetaDataDto request, Guid objId);
}
