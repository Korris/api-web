namespace Mcsg.Story.Api.Interfaces;

using Common.SeedWork.Responses;
using Dtos;
using Models;
using Requests;

public interface IFavoriteService
{
    Task<bool> AddPostToFavoriteAsync(Guid postId);
    Task<bool> AddTagToFavoriteAsync(Guid tagId);

    Task<bool> RemovePostToFavoriteAsync(Guid postId);
    Task<bool> RemoveTagToFavoriteAsync(Guid tagId);

    Task<PagedResponse<FavoriteTagResponse>> GetTagFavoriteAsync(FavoriteTagR req);
    Task<PagedResponse<FeedDto>> GetPostFavoriteByUserAsync(FavoritePostR req);
}
