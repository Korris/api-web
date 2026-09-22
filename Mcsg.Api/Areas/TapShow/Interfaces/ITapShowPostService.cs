namespace Mcsg.Api.Areas.TapShow.Interfaces;

using Common.SeedWork.Responses;
using Mcsg.Api.Areas.TapShow.Models;
using Mcsg.Api.Areas.TapShow.Requests;

/// <summary>
/// TapShow post CRUD + query
/// </summary>
public interface ITapShowPostService
{
    Task<TapShowPostResponse> CreateAsync(TapShowPostCreateR request);
    Task<TapShowPostResponse> UpdateAsync(TapShowPostUpdateR request);
    Task<bool> DeleteAsync(TapShowHashIdR request);
    Task<TapShowPostResponse> GetAsync(TapShowHashIdR request);
    Task<PagedResponse<TapShowPostResponse>> ListAsync(TapShowPostListR request);
    Task<PagedResponse<TapShowPostResponse>> ListMineAsync(TapShowPostListR request);
}
