using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Social.Api.Controllers;

using Common.Core.Enums;
using Enums;
using Interfaces;
using Requests;

[Route("[controller]")]
[ApiController]
public class ProfileController : ControllerBase
{
    #region -- Methods --

    public ProfileController(IUserService userService, IPostService postService, IFeedService feedService)
    {
        _userService = userService;
        _postService = postService;
        _feedService = feedService;
    }

    [HttpGet("current-user")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        var result = await _userService.GetCurrentUserAsync();
        return Ok(result);
    }
    [HttpGet("info/{userName}")]
    public async Task<IActionResult> GetUser(string userName)
    {
        var result = await _userService.GetUserByUserNameAsync(userName);
        return Ok(result);
    }

    [HttpGet("feeds/{userName}")]
    public async Task<IActionResult> GetUserFeed(string userName, [FromQuery] FeedLoadReq loadReq)
    {
        loadReq.UserName = userName;
        var result = await _feedService.GetFeedsAsync(loadReq, LoadFeedType.ALL);
        return Ok(result);
    }
    [HttpGet("comics/{userName}")]
    public async Task<IActionResult> GetUserComic(string userName, [FromQuery] ComicTopPostR loadReq)
    {
        var result = await _postService.GetSeriesByUserByPage(PostType.Comic, userName, loadReq);
        return Ok(result);
    }
    [HttpGet("stories/{userName}")]
    public async Task<IActionResult> GetUserStories(string userName, [FromQuery] ComicTopPostR loadReq)
    {
        var result = await _postService.GetSeriesByUserByPage(PostType.Story, userName, loadReq);
        return Ok(result);
    }
    [AllowAnonymous]
    [HttpGet("following")]
    public async Task<IActionResult> GetFollowingProfiles([FromQuery] UserNamePagingR request)
    {
        var result = await _userService.GetFollowingProfilesAsync(request);
        return Ok(result);
    }

    [HttpGet("followed")]
    public async Task<IActionResult> GetFollowedUser([FromQuery] UserNamePagingR request)
    {
        var result = await _userService.GetFollowedProfileAsync(request);
        return Ok(result);
    }

    #endregion

    #region -- Fields --

    private readonly IUserService _userService;

    private readonly IPostService _postService;

    private readonly IFeedService _feedService;

    #endregion
}
