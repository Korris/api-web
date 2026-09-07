namespace Mcsg.Api.Areas.Story.Interfaces;

using Mcsg.Api.Areas.Story.Dtos;

public interface IMetaDataService
{
    Task<MetaDataDto> AddMetaDataToObject<T>(MetaDataDto request, Guid objId);
}
