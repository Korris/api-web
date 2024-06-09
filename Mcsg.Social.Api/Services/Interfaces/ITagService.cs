using Mcsg.Social.Api.DTOs;
using Mcsg.Social.Api.Models;
using Mcsg.Social.Api.Models.Tag;
using Mcsg.Lib.Data.Entities.Common;

namespace Mcsg.Social.Api.Services.Interfaces
{
    public interface ITagService
    {
        Task<List<string>> AddTagsToPost(Guid postId, List<string> tags);
        Task<List<string>> UpdateTagsToPost(Guid postId, List<string> tags);
        Task<IEnumerable<TagSuggestView>> GetSuggestTags(TagSuggestReq tagSuggestReq);
        Task<PagedResults<PopularTagResponse>> GetPopularTags(PopularTagReq popularTagReq);
        Task<PagedResults<TodayTrendingTagResponse>> GetTodayTrendingTags(TodayTrendingTagReq todayTrendingTagReq);
        Task<List<TagView>> GetTagsByPostIdAsync(Guid postId);
        Task<List<TagByPostResponse>> GetTagsByPostHashIdAsync(string postHashId);

    }
}
