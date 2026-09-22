namespace Mcsg.Api.Areas.TapShow.Interfaces;

using Mcsg.Api.Areas.TapShow.Models;
using Mcsg.Api.Areas.TapShow.Requests;

/// <summary>
/// Chapters of a TapShow post: owner CRUD, public list + detail (segment graph)
/// </summary>
public interface ITapShowChapterService
{
    Task<ChapterResponse> CreateAsync(ChapterCreateR request);
    Task<ChapterResponse> UpdateAsync(ChapterUpdateR request);
    Task<bool> DeleteAsync(TapShowHashIdR request);
    Task<List<ChapterResponse>> ListByPostAsync(PostHashIdR request);
    Task<ChapterDetailResponse> GetAsync(TapShowHashIdR request);
}
