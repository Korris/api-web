namespace Mcsg.Api.Areas.Social.Interfaces;

using Common.SeedWork.Responses;
using Mcsg.Api.Areas.Social.Dtos;
using Mcsg.Api.Areas.Social.Models;
using Mcsg.Api.Areas.Social.Requests;

public interface ITagService
{
    Task<List<string>> AddTagsToPost(Guid postId, List<string> tags, Guid userId);
    Task<List<string>> UpdateTagsToPost(Guid postId, List<string>? tags, Guid userId);
    Task<IEnumerable<TagSuggestView>> GetSuggestTags(TagSuggestR tagSuggestReq);
    Task<PagedResponse<PopularTagResponse>> GetPopularTags(TagPopularR popularTagReq);
    Task<PagedResponse<TodayTrendingTagResponse>> GetTodayTrendingTags(TagTodayTrendingR todayTrendingTagReq);
    Task<List<TagViewDto>> GetTagsByPostIdAsync(Guid postId);
    Task<PagedResponse<TagSearchResponse>> SearchTagbyKeyword(TagSearchR input);
    Task<IEnumerable<TagSearchResponse>> SearchTagsByName(TagSearchKeywordR request);
}
