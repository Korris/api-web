using Mcsg.Api.Models;

namespace Mcsg.Api.Services.Interfaces
{
    public interface ILinkPreviewService
    {
        MetaDataResponse GetMetaDataByUrl(string url);
    }
}
