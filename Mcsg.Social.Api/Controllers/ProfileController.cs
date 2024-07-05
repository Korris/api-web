using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Social.Api.Controllers
{
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
        [HttpGet("info/{profileName}")]
        public async Task<IActionResult> GetUser(string profileName)
        {
            var result = await _userService.GetUserByUserNameAsync(profileName);
            return Ok(result);
        }

        [HttpGet("feeds/{profileName}")]
        public async Task<IActionResult> GetUserFeed(string profileName, [FromQuery] FeedLoadReq loadReq)
        {
            loadReq.ProfileName = profileName;
            var result = await _feedService.GetFeedsAsync(loadReq, LoadFeedType.ALL);
            return Ok(result);
        }
        [HttpGet("comics/{profileName}")]
        public async Task<IActionResult> GetUserComic(string profileName, [FromQuery] ComicTopPostR loadReq)
        {
            var result = await _postService.GetSeriesByUserByPage(PostType.Comic, profileName, loadReq);
            return Ok(result);
        }
        [HttpGet("stories/{profileName}")]
        public async Task<IActionResult> GetUserStories(string profileName, [FromQuery] ComicTopPostR loadReq)
        {
            var result = await _postService.GetSeriesByUserByPage(PostType.Story, profileName, loadReq);
            return Ok(result);
        }

        #endregion

        #region -- Fields --

        private readonly IUserService _userService;

        private readonly IPostService _postService;

        private readonly IFeedService _feedService;

        #endregion
    }
}
