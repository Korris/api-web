namespace Mcsg.Admin.Api.Services.Interface
{
    using Dtos;
    using Lib.Data.Entities.Common;
    using Requests;

    public interface IPostService
    {
        Task<PagedResults<PostBasicResponse>> GetListAsync(PostListReq request);
        Task<bool> DeactivePostAsync(Guid id, PostStatusReq request);
        Task<bool> DeletePostAsync(Guid id, PostStatusReq request);
    }
}
