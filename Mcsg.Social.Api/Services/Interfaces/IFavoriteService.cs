using Mcsg.Social.Api.DTOs;
using Mcsg.Social.Api.Models;
using Mcsg.Lib.Data.Entities.Common;

namespace Mcsg.Social.Api.Services.Interfaces
{
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
