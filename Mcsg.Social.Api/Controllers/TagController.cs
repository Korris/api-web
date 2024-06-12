using Mcsg.Social.Api.DTOs;
using Mcsg.Social.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Identity.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TagController : ControllerBase
    {
        private readonly ITagService _tagService;
        public TagController(ITagService tagService)
        {
            _tagService = tagService;
        }
        [HttpGet("suggest")]
        [Authorize]
        public async Task<IActionResult> GetSuggestTags([FromQuery] TagSuggestReq request)
        {
            var result = await _tagService.GetSuggestTags(request);
            return Ok(result);
        }
        [HttpGet("popular-tags")]
        public async Task<IActionResult> GetPopularTags([FromQuery] PopularTagReq popularTagReq)
        {
            var result = await _tagService.GetPopularTags(popularTagReq);
            return Ok(result);
        }
        [HttpGet("favorite-tags")]
        public async Task<IActionResult> GetFavoriteTags([FromQuery] PopularTagReq popularTagReq)
        {
            var result = await _tagService.GetPopularTags(popularTagReq);
            return Ok(result);
        }

        [HttpGet("today-trending-tags")]
        public async Task<IActionResult> GetTodayTrendingTags([FromQuery] TodayTrendingTagReq todayTrendingTagReq)
        {
            var result = await _tagService.GetTodayTrendingTags(todayTrendingTagReq);
            return Ok(result);
        }

        [HttpGet("search-tag")]
        public async Task<IActionResult> SearchTag([FromQuery] SearchTagReq input)
        {
            var result = await _tagService.SearchTagbyKeyword(input);
            return Ok(result);
        }

        [HttpGet("search-tags")]
        [Authorize]
        public async Task<IActionResult> SearchTags([FromQuery] SearchTagsReq searchTagsReq)
        {
            var result = await _tagService.SearchTagsByName(searchTagsReq);
            return Ok(result);
        }
    }
}
