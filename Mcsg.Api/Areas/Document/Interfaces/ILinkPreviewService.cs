namespace Mcsg.Api.Areas.Document.Interfaces;

using Mcsg.Api.Areas.Document.Dtos;

public interface ILinkPreviewService
{
    MetaDataDto GetMetaDataByUrl(string url);
}
