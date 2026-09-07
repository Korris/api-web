using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Api.Areas.Comic.Controllers;

using Mcsg.Api.Areas.Comic.Attributes;
using Mcsg.Api.Areas.Comic.Interfaces;
using Mcsg.Api.Interfaces;
using Mcsg.Api.Areas.Comic.Requests;

[ApiController]
[Route("api/comic/[controller]"), Authorize]
public class FileController : ControllerBase
{
    #region -- Methods --

    public FileController(IFileService fileService, ILogger<FileController> logger)
    {
        _fileService = fileService;
        _logger = logger;
    }

    [DisableRequestSizeLimit]
    [ServiceFilter(typeof(MediaOnlyAttribute))]
    [HttpPost("upload-media")]
    public async Task<IActionResult> UploadMedia([FromForm] FileCreateR request)
    {
        // Gap between headers reaching the pod and the multipart body being fully read + bound
        _logger.LogInformation("[UPLOAD-MEDIA] body read+bound {Ms}ms after headers reached pod, contentLength={Bytes}",
            Middlewares.RequestTimingMiddleware.ElapsedSinceRequestStart(HttpContext).ToString("F0"), Request.ContentLength);

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

    private readonly ILogger<FileController> _logger;

    #endregion
}