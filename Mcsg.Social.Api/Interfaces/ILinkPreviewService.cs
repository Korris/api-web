namespace Mcsg.Social.Api.Interfaces
{
    using Models;

    public interface ILinkPreviewService
    {
        MetaDataResponse GetMetaDataByUrl(string url);
    }
}
