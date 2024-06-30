namespace Mcsg.Social.Api.Interfaces
{
    using Models;

    public interface ISmartLookupService
    {
        Task CalculateSmartLookupWhenDeletePostAsync(Guid postId);
        Task CalculateSmartLookupWhenCreatePostAsync();
        Task CalculateSmartLookupForTagAsync(List<string> tags);
        Task<IEnumerable<SmartLookupResponse>> SearchAsync(string keyword);
        Task<IEnumerable<RecentSearchResponse>> GetRecentListAsync();
        Task<bool> DeleteRecentSearchAsync(Guid id);
        Task<bool> AddRecentSearchAsync(AddRecentSearchRequest res);
    }
}
