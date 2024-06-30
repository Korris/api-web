namespace Mcsg.Social.Api.Interfaces
{
    using DTOs;
    using Lib.Data.Entities.Common;
    using Models;
    using Models.Tag;

    public interface ITagService
    {
        Task<List<string>> AddTagsToPost(Guid postId, List<string> tags);
        Task<List<string>> UpdateTagsToPost(Guid postId, List<string> tags);
        Task<IEnumerable<TagSuggestView>> GetSuggestTags(TagSuggestReq tagSuggestReq);
        Task<PagedResults<PopularTagResponse>> GetPopularTags(PopularTagReq popularTagReq);
        Task<PagedResults<TodayTrendingTagResponse>> GetTodayTrendingTags(TodayTrendingTagReq todayTrendingTagReq);
        Task<List<TagView>> GetTagsByPostIdAsync(Guid postId);
        Task<List<TagByPostResponse>> GetTagsByPostHashIdAsync(string postHashId);
        Task<PagedResults<TagSearchResponse>> SearchTagbyKeyword(SearchTagReq input);
        Task<IEnumerable<TagSearchResponse>> SearchTagsByName(SearchTagsReq request);
    }
}
