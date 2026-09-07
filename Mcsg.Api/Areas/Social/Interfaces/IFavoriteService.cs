namespace Mcsg.Api.Areas.Social.Interfaces;

using Common.Core.Requests;
using Common.SeedWork.Responses;
using Mcsg.Api.Areas.Social.Dtos;
using Mcsg.Api.Areas.Social.Models;
using Mcsg.Api.Areas.Social.Requests;

public interface IFavoriteService
{
    Task<bool> AddTagToFavoriteAsync(IdBaseR request);

    Task<bool> RemovePostToFavoriteAsync(IdBaseR request);
    Task<bool> RemoveTagToFavoriteAsync(IdBaseR request);

    Task<PagedResponse<FavoriteTagResponse>> GetTagFavoriteAsync(FavoriteTagR req);
    Task<PagedResponse<FeedDto>> GetPostFavoriteByUserAsync(FavoritePostR req);
}
