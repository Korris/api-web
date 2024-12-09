using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Mcsg.Identity.Api.Controllers;

using Common.Core.Requests;
using Common.SeedWork.Responses;
using Interfaces;
using Requests;
using static Common.SeedWork.Constants.Setting;

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
    /// <param name="mediator"></param>
    /// <param name="setting"></param>
    /// <param name="userService"></param>
    public UserController(IMediator mediator, ISetting setting, IUserService userService)
    {
        _mediator = mediator;
        _setting = setting;
        _userService = userService;
    }

    [HttpGet("info/{userName}")]
    public async Task<IActionResult> GetUser(string userName)
    {
        var req = new BaseR(HttpContext);
        var result = await _userService.GetUserByUserNameAsync(req.UserId, userName);
        return Ok(result);
    }

    [HttpGet("following")]
    public async Task<IActionResult> GetFollowingProfiles([FromQuery] UserNamePagingR request)
    {
        request.Analyze(HttpContext);
        var result = await _userService.GetFollowingProfilesAsync(request);
        return Ok(result);
    }

    [HttpGet("followed")]
    public async Task<IActionResult> GetFollowedUser([FromQuery] UserNamePagingR request)
    {
        request.Analyze(HttpContext);
        var result = await _userService.GetFollowedProfileAsync(request);
        return Ok(result);
    }

    [HttpGet("current-user"), Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        var req = new BaseR(HttpContext);
        var result = await _userService.GetCurrentUserAsync(req.UserId);
        return Ok(result);
    }

    /// <summary>
    /// UpdateUserName
    /// </summary>
    /// <param name="request">Request</param>
    /// <returns>Returns the result</returns>
    [HttpPost("v1/UpdateUserName"), Authorize]
    [ProducesResponseType(typeof(SingleResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> UpdateUserName([FromBody] UserNameUpdateR request)
    {
        request.Analyze(HttpContext);
        var response = await _mediator.Send(request);
        return Ok(response);
    }

    [HttpPut("profile"), Authorize]
    public async Task<IActionResult> UpdateUserProfile(UserProfileUpdateR request)
    {
        request.Analyze(HttpContext);
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
        var req = new BaseR(HttpContext);
        var result = await _userService.GetSuggestedProfilesNotFollowedAsync(req.UserId, userName);
        return Ok(result);
    }

    #region -- UserHistory --
    /// <summary>
    /// Get lastest username
    /// </summary>
    /// <param name="request">Request</param>
    /// <returns>Returns the result</returns>
    [HttpPost("v1/UserHistoryGetLatest")]
    [ProducesResponseType(typeof(SingleResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> UserHistoryGetLatest([FromBody] UserHistoryGetLatestR request)
    {
        request.Analyze(HttpContext);
        var response = await _mediator.Send(request);
        return Ok(response);
    }
    #endregion

    /// <summary>
    /// SyncToAna
    /// </summary>
    /// <param name="request">Request</param>
    /// <returns>Returns the result</returns>
    [HttpPost("v1/SyncToAna"), Authorize(Policy = Policy.Admin)]
    [ProducesResponseType(typeof(SingleResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> SyncToAna([FromBody] UserSyncToAnaR request)
    {
        request.Analyze(HttpContext);
        var response = await _mediator.Send(request);
        return Ok(response);
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// Mediator
    /// </summary>
    private readonly IMediator _mediator;

    /// <summary>
    /// Setting
    /// </summary>
    private readonly ISetting _setting;

    /// <summary>
    /// User service
    /// </summary>
    private readonly IUserService _userService;

    #endregion
}
