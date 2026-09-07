namespace Mcsg.Api.Areas.Comic.Interfaces;

using Mcsg.Api.Areas.Comic.Models;
using Mcsg.Api.Areas.Comic.Requests;

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
