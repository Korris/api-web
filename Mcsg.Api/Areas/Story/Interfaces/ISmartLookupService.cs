namespace Mcsg.Api.Areas.Story.Interfaces;

using Mcsg.Api.Areas.Story.Models;
using Mcsg.Api.Areas.Story.Requests;

public interface ISmartLookupService
{
    Task CalculateSmartLookupWhenDeletePostAsync(Guid postId, string profileName);
    Task CalculateSmartLookupWhenCreatePostAsync(string profileName);
    Task CalculateSmartLookupForTagAsync(List<string> tags);
    Task<IEnumerable<SmartLookupResponse>> SearchAsync(string keyword);
    Task<IEnumerable<RecentSearchResponse>> GetRecentListAsync(Guid userId);
    Task<bool> DeleteRecentSearchAsync(Guid id);
    Task<bool> AddRecentSearchAsync(SmartLookupAddRecentSearchR res, Guid userId);
}
