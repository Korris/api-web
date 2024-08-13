using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Identity.Api.Controllers;

using Common.Core.Requests;
using Interfaces;
using Requests;

/// <summary>
/// User controller
/// </summary>
[Route("[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="userService"></param>
    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("info/{userName}")]
    public async Task<IActionResult> GetUser(string userName)
    {
        var result = await _userService.GetUserByUserNameAsync(userName);
        return Ok(result);
    }

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

    [HttpGet("current-user"), Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        var result = await _userService.GetCurrentUserAsync();
        return Ok(result);
    }

    [HttpPut("profile"), Authorize]
    public async Task<IActionResult> UpdateUserProfile(UserProfileUpdateR request)
    {
        var req = new BaseR(HttpContext);
        request.IsPremium = req.IsPremium == true;
        var result = await _userService.UpdateUserProfile(request);
        return Ok(result);
    }

    [HttpGet("avatar/{userId}")]
    public async Task<IActionResult> GetUserAvatar(Guid userId)
    {
        var result = await _userService.GetUserAvatar(userId);
        return Ok(result);
    }

    [HttpPut("avatar"), Authorize]
    public async Task<IActionResult> UpdateUserAvatar([FromForm] UserAvatarUpdateR request)
    {
        request.Analyze(HttpContext);
        var result = await _userService.UpdateUserAvatar(request);
        return Ok(result);
    }

    [HttpPut("cover-photo"), Authorize]
    public async Task<IActionResult> UpdateUserCoverPhoto([FromForm] UserCoverPhotoUpdateR request)
    {
        request.Analyze(HttpContext);
        var result = await _userService.UpdateUserCoverPhoto(request);
        return Ok(result);
    }

    [HttpGet("similar-name")]
    public async Task<IActionResult> GetSimilarName(string name)
    {
        var result = await _userService.GetSimilarNameAsync(name);
        return Ok(result);
    }

    [HttpGet("similar-name-mention")]
    public async Task<IActionResult> GetSimilarProfileNamesMention(string? name)
    {
        var result = await _userService.GetSimilarProfilesMentionAsync(name);
        return Ok(result);
    }

    [HttpGet("suggested-profiles-not-followed")]
    public async Task<IActionResult> GetSuggestedProfilesNotFollowed([FromQuery] string userName)
    {
        var result = await _userService.GetSuggestedProfilesNotFollowedAsync(userName);
        return Ok(result);
    }

    [HttpPost("follow/{userId}"), Authorize]
    public async Task<IActionResult> FollowUser(Guid userId)
    {
        var result = await _userService.FollowUserAsync(userId);
        return Ok(result);
    }

    [HttpPost("unfollow/{userId}"), Authorize]
    public async Task<IActionResult> UnfollowUserAsync(Guid userId)
    {
        var result = await _userService.UnFollowUserAsync(userId);
        return Ok(result);
    }

    #endregion

    #region -- Fields --

    private readonly IUserService _userService;

    #endregion
}
