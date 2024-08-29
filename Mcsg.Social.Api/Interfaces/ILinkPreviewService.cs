namespace Mcsg.Social.Api.Interfaces;

using Dtos;

public interface ILinkPreviewService
{
    Task<MetaDataDto> GetMetaDataByUrl(string url);
}
