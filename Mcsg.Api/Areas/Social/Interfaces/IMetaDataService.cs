namespace Mcsg.Api.Areas.Social.Interfaces;

using Mcsg.Api.Areas.Social.Dtos;

public interface IMetaDataService
{
    Task<MetaDataDto> AddMetaDataToObject<T>(MetaDataDto request, Guid objId);
}
