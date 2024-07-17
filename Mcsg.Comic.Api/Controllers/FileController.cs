using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Comic.Api.Controllers;

using Attributes;
using Common.Core.Requests;
using Interfaces;

[ApiController]
[Route("[controller]"), Authorize]
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
    public async Task<IActionResult> UploadMedia(IFormFile file)
    {
        var req = new BaseR(HttpContext);
        var result = await _fileService.UploadFileAsync(file, req.UserId);
        return Ok(result);
    }

    [DisableRequestSizeLimit]
    [ServiceFilter(typeof(MediaOnlyAttribute))]
    [HttpPost("upload-images")]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        var req = new BaseR(HttpContext);
        var result = await _fileService.UploadImageAsync(file, req.UserId);
        return Ok(result);
    }

    #endregion

    #region -- Fields --

    private readonly IFileService _fileService;

    #endregion
}