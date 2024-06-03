using Mcsg.Admin.Api.DTOs.Posts;
using Mcsg.Lib.Data.Entities.Common;

namespace Mcsg.Admin.Api.Services.Interface
{
    public interface IPostService
    {
        Task<PagedResults<PostBasicResponse>> GetListAsync(PostListReq request);
        Task<bool> DeactivePostAsync(Guid id, PostStatusReq request);
        Task<bool> DeletePostAsync(Guid id, PostStatusReq request);
    }
}
