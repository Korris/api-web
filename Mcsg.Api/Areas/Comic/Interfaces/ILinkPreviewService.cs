namespace Mcsg.Api.Areas.Comic.Interfaces;

using Mcsg.Api.Areas.Comic.Dtos;

public interface ILinkPreviewService
{
    MetaDataDto GetMetaDataByUrl(string url);
}
