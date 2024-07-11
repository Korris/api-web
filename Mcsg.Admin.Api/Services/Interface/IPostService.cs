namespace Mcsg.Admin.Api.Services.Interface
{
    using Common.SeedWork.Responses;
    using Dtos;
    using Requests;

    public interface IPostService
    {
        Task<PagedResponse<PostBasicResponse>> GetListAsync(PostListReq request);
        Task<bool> DeactivePostAsync(Guid id, PostStatusReq request);
        Task<bool> DeletePostAsync(Guid id, PostStatusReq request);
    }
}
