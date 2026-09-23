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

    /// <summary>
    /// Public posts by hash ids, for the home feed hydration (latest-posts-by-type/detail). Missing / non-public ids are skipped.
    /// </summary>
    Task<List<TapShowPostResponse>> GetByHashIdsAsync(IEnumerable<string> hashIds, Guid? currentUserId);
}
