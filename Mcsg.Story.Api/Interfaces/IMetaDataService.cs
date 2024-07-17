namespace Mcsg.Story.Api.Interfaces;

using Dtos;

public interface IMetaDataService
{
    Task<MetaDataDto> AddMetaDataToObject<T>(MetaDataDto request, Guid objId);
}
