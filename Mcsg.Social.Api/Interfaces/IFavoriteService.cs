namespace Mcsg.Social.Api.Interfaces
{
    using Lib.Data.Entities.Common;
    using Models;
    using Requests;

    public interface IFavoriteService
    {
        Task<bool> AddPostToFavoriteAsync(Guid postId);
        Task<bool> AddTagToFavoriteAsync(Guid tagId);

        Task<bool> RemovePostToFavoriteAsync(Guid postId);
        Task<bool> RemoveTagToFavoriteAsync(Guid tagId);

        Task<PagedResults<FavoriteTagResponse>> GetTagFavoriteAsync(FavoriteTagReq req);
        Task<PagedResults<FavoritePostResponse>> GetPostFavoriteAsync(FavoritePostReq req);
        Task<PagedResults<FeedResponse>> GetPostFavoriteByUserAsync(FavoritePostReq req);

    }
}
