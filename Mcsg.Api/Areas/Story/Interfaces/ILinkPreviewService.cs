namespace Mcsg.Api.Areas.Story.Interfaces;

using Mcsg.Api.Areas.Story.Dtos;

public interface ILinkPreviewService
{
    MetaDataDto GetMetaDataByUrl(string url);
}
