namespace Mcsg.Social.Api.Interfaces;

using Common.SeedWork.Responses;
using Dtos;
using Models;
using Requests;

public interface IFavoriteService
{
    Task<bool> AddTagToFavoriteAsync(Guid tagId);

    Task<bool> RemovePostToFavoriteAsync(Guid postId);
    Task<bool> RemoveTagToFavoriteAsync(Guid tagId);

    Task<PagedResponse<FavoriteTagResponse>> GetTagFavoriteAsync(FavoriteTagR req);
    Task<PagedResponse<FavoritePostResponse>> GetPostFavoriteAsync(FavoritePostR req);
    Task<PagedResponse<FeedDto>> GetPostFavoriteByUserAsync(FavoritePostR req);

}
