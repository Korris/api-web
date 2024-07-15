using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Social.Api.Controllers;

using Common.Core.Requests;
using Interfaces;
using Requests;

[Route("[controller]")]
[ApiController]
[Authorize]
public class UserController : ControllerBase
{
    #region -- Methods --

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("current-user")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var result = await _userService.GetCurrentUserAsync();
        return Ok(result);
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateUserProfile(UserProfileUpdateR request)
    {
        var req = new BaseR(HttpContext);
        request.IsPremium = req.IsPremium == true;
        var result = await _userService.UpdateUserProfile(request);
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpGet("avatar/{userId}")]
    public async Task<IActionResult> GetUserAvatar(Guid userId)
    {
        var result = await _userService.GetUserAvatar(userId);
        return Ok(result);
    }

    [HttpPut("avatar")]
    public async Task<IActionResult> UpdateUserAvatar([FromForm] UserAvatarUpdateR userAvatarUpdateRequest)
    {
        var result = await _userService.UpdateUserAvatar(userAvatarUpdateRequest);
        return Ok(result);
    }

    [HttpPut("cover-photo")]
    public async Task<IActionResult> UpdateUserCoverPhoto([FromForm] UserCoverPhotoUpdateR userCoverPhotoUpdateRequest)
    {
        var result = await _userService.UpdateUserCoverPhoto(userCoverPhotoUpdateRequest);
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpGet("similar-name")]
    public async Task<IActionResult> GetSimilarName(string name)
    {
        var result = await _userService.GetSimilarNameAsync(name);
        return Ok(result);
    }

    /// <summary>
    /// //TODO Delete [AllowAnonymous] after test
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpGet("similar-name-mention")]
    public async Task<IActionResult> GetSimilarProfileNamesMention(string? name)
    {
        var result = await _userService.GetSimilarProfilesMentionAsync(name);
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpGet("suggested-profiles-not-followed")]
    public async Task<IActionResult> GetSuggestedProfilesNotFollowed([FromQuery] string userName)
    {
        var result = await _userService.GetSuggestedProfilesNotFollowedAsync(userName);
        return Ok(result);
    }

    [HttpPost("follow/{userId}")]
    public async Task<IActionResult> FollowUser(Guid userId)
    {
        var result = await _userService.FollowUserAsync(userId);
        return Ok(result);
    }

    [HttpPost("unfollow/{userId}")]
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
