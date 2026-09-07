using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Api.Areas.Comic.Controllers;

using Mcsg.Api.Areas.Comic.Interfaces;
using Mcsg.Api.Interfaces;
using Mcsg.Api.Areas.Comic.Requests;

[ApiController]
[Route("api/comic/[controller]")]
public class TagController : ControllerBase
{
    #region -- Methods --

    public TagController(ITagService tagService)
    {
        _tagService = tagService;
    }

    [HttpGet("suggest")]
    [Authorize]
    public async Task<IActionResult> GetSuggestTags([FromQuery] TagSuggestR request)
    {
        var result = await _tagService.GetSuggestTags(request);
        return Ok(result);
    }

    [HttpGet("popular-tags")]
    public async Task<IActionResult> GetPopularTags([FromQuery] TagPopularR popularTagReq)
    {
        var result = await _tagService.GetPopularTags(popularTagReq);
        return Ok(result);
    }

    [HttpGet("favorite-tags")]
    public async Task<IActionResult> GetFavoriteTags([FromQuery] TagPopularR popularTagReq)
    {
        var result = await _tagService.GetPopularTags(popularTagReq);
        return Ok(result);
    }

    [HttpGet("today-trending-tags")]
    public async Task<IActionResult> GetTodayTrendingTags([FromQuery] TagTodayTrendingR todayTrendingTagReq)
    {
        var result = await _tagService.GetTodayTrendingTags(todayTrendingTagReq);
        return Ok(result);
    }

    [HttpGet("search-tag")]
    public async Task<IActionResult> SearchTag([FromQuery] TagSearchR input)
    {
        var result = await _tagService.SearchTagbyKeyword(input);
        return Ok(result);
    }

    [HttpGet("search-tags")]
    [Authorize]
    public async Task<IActionResult> SearchTags([FromQuery] TagSearchKeywordR searchTagsReq)
    {
        var result = await _tagService.SearchTagsByName(searchTagsReq);
        return Ok(result);
    }

    #endregion

    #region -- Fields --

    private readonly ITagService _tagService;

    #endregion
}
