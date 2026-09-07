using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Api.Areas.Comic.Controllers;

using Mcsg.Api.Areas.Comic.Interfaces;
using Mcsg.Api.Interfaces;
using Mcsg.Api.Areas.Comic.Requests;

[ApiController]
[Route("api/comic/[controller]")]
public class SoundController : ControllerBase
{
    #region -- Methods --

    public SoundController(ISoundService soundService)
    {
        _soundService = soundService;
    }

    [HttpGet("list")]
    [Authorize]
    public async Task<IActionResult> GetAllSound([FromQuery] SoundBackgroundMediaLoadR request)
    {
        var result = await _soundService.GetAllSoundAsync(request);
        return Ok(result);
    }

    [HttpGet("recently-used")]
    [Authorize]
    public async Task<IActionResult> GetRecentlyUseSound([FromQuery] SoundBackgroundMediaLoadR request)
    {
        var result = await _soundService.GetRecentlyUseSoundAsync(request);
        return Ok(result);
    }
    [HttpGet("search")]
    [Authorize]
    public async Task<IActionResult> SearchSound([FromQuery] SoundSearchSoundR request)
    {
        var result = await _soundService.SearchSoundAsync(request);
        return Ok(result);
    }

    #endregion

    #region -- Fields --

    private readonly ISoundService _soundService;

    #endregion
}
