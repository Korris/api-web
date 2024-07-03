using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Social.Api.Controllers
{
    using Interfaces;
    using Models;
    using Requests;

    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
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
        public async Task<IActionResult> UpdateUserProfile(UserProfileUpdateR req)
        {
            var result = await _userService.UpdateUserProfile(req);
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
        public async Task<IActionResult> UpdateUserAvatar([FromForm] UserAvatarUpdateRequest userAvatarUpdateRequest)
        {
            var result = await _userService.UpdateUserAvatar(userAvatarUpdateRequest);
            return Ok(result);
        }

        [HttpPut("cover-photo")]
        public async Task<IActionResult> UpdateUserCoverPhoto([FromForm] UserCoverPhotoUpdateRequest userCoverPhotoUpdateRequest)
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
        /// Todo Delete AllowAnonymouse after test
        [AllowAnonymous]
        [HttpGet("similar-name-mention")]
        public async Task<IActionResult> GetSimilarProfileNamesMention(string? name)
        {
            var result = await _userService.GetSimilarProfilesMentionAsync(name);
            return Ok(result);
        }
    }
}
