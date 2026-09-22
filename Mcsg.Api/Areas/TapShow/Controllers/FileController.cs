using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Api.Areas.TapShow.Controllers;

using Mcsg.Api.Areas.TapShow.Attributes;
using Mcsg.Api.Areas.TapShow.Constants;
using Mcsg.Api.Areas.TapShow.Interfaces;
using Mcsg.Api.Areas.TapShow.Requests;

/// <summary>
/// TapShow uploads (api/tapshow/file). One image endpoint used for post thumbnails and segment images.
/// </summary>
[ApiController]
[Route("api/tapshow/[controller]"), Authorize]
public class FileController : ControllerBase
{
    #region -- Methods --

    public FileController(ITapShowFileService fileService)
    {
        _fileService = fileService;
    }

    /// <summary>
    /// Upload an image → hashId to send as ThumbnailHashId (post), ImageHashId (segment) or AvatarHashId (character)
    /// </summary>
    [DisableRequestSizeLimit]
    [ServiceFilter(typeof(MediaOnlyAttribute))]
    [HttpPost("upload-media")]
    public async Task<IActionResult> UploadImage([FromForm] FileCreateR request)
    {
        request.Analyze(HttpContext);
        return Ok(await _fileService.UploadImageAsync(request));
    }

    /// <summary>
    /// Upload the voice-over audio of a segment (mp3, m4a, aac, wav, ogg) → hashId to send as AudioHashId.
    /// Request body capped at 31 MB before the SystemSettings "TapShowAudioSize" check.
    /// </summary>
    [RequestSizeLimit(TapShowConfig.AudioRequestLimitBytes)]
    [RequestFormLimits(MultipartBodyLengthLimit = TapShowConfig.AudioRequestLimitBytes)]
    [HttpPost("upload-audio")]
    public async Task<IActionResult> UploadAudio([FromForm] FileCreateR request)
    {
        request.Analyze(HttpContext);
        return Ok(await _fileService.UploadAudioAsync(request));
    }

    #endregion

    #region -- Fields --

    private readonly ITapShowFileService _fileService;

    #endregion
}
