namespace Mcsg.Api.Areas.Document.Interfaces;

using Common.Core.Requests;
using Common.SeedWork.Responses;
using Mcsg.Api.Areas.Document.Dtos;
using Mcsg.Api.Areas.Document.Models;
using Mcsg.Api.Areas.Document.Requests;

public interface IFavoriteService
{
    Task<bool> AddPostToFavoriteAsync(IdBaseR request);
    Task<bool> AddTagToFavoriteAsync(IdBaseR request);

    Task<bool> RemovePostToFavoriteAsync(IdBaseR request);
    Task<bool> RemoveTagToFavoriteAsync(IdBaseR request);

    Task<PagedResponse<FavoriteTagResponse>> GetTagFavoriteAsync(FavoriteTagR req);
    Task<PagedResponse<FeedDto>> GetPostFavoriteByUserAsync(FavoritePostR req);
}
