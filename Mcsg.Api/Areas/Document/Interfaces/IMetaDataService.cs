namespace Mcsg.Api.Areas.Document.Interfaces;

using Mcsg.Api.Areas.Document.Dtos;

public interface IMetaDataService
{
    Task<MetaDataDto> AddMetaDataToObject<T>(MetaDataDto request, Guid objId);
}
