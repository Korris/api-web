namespace Mcsg.Api.Areas.Game.Interfaces;

using Common.SeedWork.Responses;
using Mcsg.Api.Areas.Game.Models;
using Mcsg.Api.Areas.Game.Requests;

/// <summary>
/// Game post CRUD + query
/// </summary>
public interface IGamePostService
{
    Task<GamePostResponse> CreateAsync(GamePostCreateR request);
    Task<GamePostResponse> UpdateAsync(GamePostUpdateR request);
    Task<bool> DeleteAsync(GameHashIdR request);
    Task<GamePostResponse> GetAsync(GameHashIdR request);
    Task<PagedResponse<GamePostResponse>> ListAsync(GamePostListR request);
    Task<PagedResponse<GamePostResponse>> ListMineAsync(GamePostListR request);
}
