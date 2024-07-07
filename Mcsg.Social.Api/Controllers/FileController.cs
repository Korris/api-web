using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Social.Api.Controllers;

using Attributes;
using Interfaces;

[ApiController]
[Route("[controller]")]
public class FileController : ControllerBase
{
    #region -- Methods --

    public FileController(IFileService fileService)
    {
        _fileService = fileService;
    }

    [Authorize]
    [DisableRequestSizeLimit]
    [ServiceFilter(typeof(MediaOnlyAttribute))]
    [HttpPost("upload-media")]
    public async Task<IActionResult> UploadMedia(IFormFile file)
    {
        var result = await _fileService.UploadFileAsync(file);
        return Ok(result);
    }

    [Authorize]
    [DisableRequestSizeLimit]
    [ServiceFilter(typeof(MediaOnlyAttribute))]
    [HttpPost("upload-images")]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        var result = await _fileService.UploadImageAsync(file);
        return Ok(result);
    }

    #endregion

    #region -- Fields --

    private readonly IFileService _fileService;

    #endregion
}