namespace Mcsg.Api.Areas.Comic.Interfaces;

using Mcsg.Api.Areas.Comic.Dtos;

public interface IMetaDataService
{
    Task<MetaDataDto> AddMetaDataToObject<T>(MetaDataDto request, Guid objId);
}
