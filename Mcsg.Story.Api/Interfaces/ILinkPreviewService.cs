namespace Mcsg.Story.Api.Interfaces;

using Dtos;

public interface ILinkPreviewService
{
    MetaDataDto GetMetaDataByUrl(string url);
}
