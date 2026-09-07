using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Api.Areas.Social.Controllers;

using Mcsg.Api.Areas.Social.Attributes;
using Mcsg.Api.Areas.Social.Interfaces;
using Mcsg.Api.Areas.Social.Requests;

[ApiController]
[Route("api/social/[controller]"), Authorize]
public class FileController : ControllerBase
{
    #region -- Methods --

    public FileController(IFileService fileService)
    {
        _fileService = fileService;
    }

    [DisableRequestSizeLimit]
    [ServiceFilter(typeof(MediaOnlyAttribute))]
    [HttpPost("upload-media")]
    public async Task<IActionResult> UploadMedia([FromForm] FileCreateR request)
    {
        request.Analyze(HttpContext);
        var result = await _fileService.UploadFileAsync(request);
        return Ok(result);
    }

    [DisableRequestSizeLimit]
    [ServiceFilter(typeof(MediaOnlyAttribute))]
    [HttpPost("upload-images")]
    public async Task<IActionResult> UploadImage([FromForm] FileCreateR request)
    {
        request.Analyze(HttpContext);
        var result = await _fileService.UploadImageAsync(request);
        return Ok(result);
    }

    #endregion

    #region -- Fields --

    private readonly IFileService _fileService;

    #endregion
}