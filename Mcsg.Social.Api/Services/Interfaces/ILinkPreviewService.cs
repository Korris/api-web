using Mcsg.Social.Api.Models;

namespace Mcsg.Social.Api.Services.Interfaces
{
    public interface ILinkPreviewService
    {
        MetaDataResponse GetMetaDataByUrl(string url);
    }
}
