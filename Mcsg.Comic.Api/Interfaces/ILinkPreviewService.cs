namespace Mcsg.Comic.Api.Interfaces;

using Dtos;

public interface ILinkPreviewService
{
    MetaDataDto GetMetaDataByUrl(string url);
}
