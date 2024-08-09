using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Social.Api.Controllers;
using Interfaces;
using Requests;

[Route("[controller]")]
[ApiController]
public class ProfileController : ControllerBase
{
    #region -- Methods --

    public ProfileController(IMediator mediator, IUserService userService, IPostService postService, IFeedService feedService)
    {
        _mediator = mediator;
        _userService = userService;
        _postService = postService;
        _feedService = feedService;
    }

    [HttpGet("info/{userName}")]
    public async Task<IActionResult> GetUser(string userName)
    {
        var result = await _userService.GetUserByUserNameAsync(userName);
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

    /// <summary>
    /// Mediator
    /// </summary>
    private readonly IMediator _mediator;

    private readonly IUserService _userService;

    private readonly IPostService _postService;

    private readonly IFeedService _feedService;

    #endregion
}
