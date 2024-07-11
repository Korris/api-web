namespace Mcsg.Social.Api.Interfaces;

using Dtos;

public interface ILinkPreviewService
{
    MetaDataDto GetMetaDataByUrl(string url);
}
