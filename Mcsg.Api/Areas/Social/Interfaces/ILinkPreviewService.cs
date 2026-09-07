namespace Mcsg.Api.Areas.Social.Interfaces;

using Mcsg.Api.Areas.Social.Dtos;

public interface ILinkPreviewService
{
    Task<MetaDataDto> GetMetaDataByUrl(string url);
}
