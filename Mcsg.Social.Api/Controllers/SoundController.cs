using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Social.Api.Controllers
{
    using Interfaces;
    using Requests;

    [ApiController]
    [Route("[controller]")]
    public class SoundController : ControllerBase
    {
        private readonly ISoundService _soundService;
        public SoundController(ISoundService soundService)
        {
            _soundService = soundService;
        }

        [HttpGet("list")]
        [Authorize]
        public async Task<IActionResult> GetAllSound([FromQuery] BackgroundMediaLoadReq request)
        {
            var result = await _soundService.GetAllSoundAsync(request);
            return Ok(result);
        }

        [HttpGet("recently-used")]
        [Authorize]
        public async Task<IActionResult> GetRecentlyUseSound([FromQuery] BackgroundMediaLoadReq request)
        {
            var result = await _soundService.GetRecentlyUseSoundAsync(request);
            return Ok(result);
        }
        [HttpGet("search")]
        [Authorize]
        public async Task<IActionResult> SearchSound([FromQuery] SearchSoundReq request)
        {
            var result = await _soundService.SearchSoundAsync(request);
            return Ok(result);
        }
    }
}
