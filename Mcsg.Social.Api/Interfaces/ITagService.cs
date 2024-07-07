namespace Mcsg.Social.Api.Interfaces;

using Lib.Data.Entities.Common;
using Models;
using Models.Tag;
using Requests;

public interface ITagService
{
    Task<List<string>> AddTagsToPost(Guid postId, List<string> tags);
    Task<List<string>> UpdateTagsToPost(Guid postId, List<string> tags);
    Task<IEnumerable<TagSuggestView>> GetSuggestTags(TagSuggestR tagSuggestReq);
    Task<PagedResults<PopularTagResponse>> GetPopularTags(TagPopularR popularTagReq);
    Task<PagedResults<TodayTrendingTagResponse>> GetTodayTrendingTags(TagTodayTrendingR todayTrendingTagReq);
    Task<List<TagView>> GetTagsByPostIdAsync(Guid postId);
    Task<List<TagByPostResponse>> GetTagsByPostHashIdAsync(string postHashId);
    Task<PagedResults<TagSearchResponse>> SearchTagbyKeyword(TagSearchR input);
    Task<IEnumerable<TagSearchResponse>> SearchTagsByName(TagSearchKeywordR request);
}
