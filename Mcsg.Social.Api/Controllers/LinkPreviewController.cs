using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Social.Api.Controllers;

using Dtos;
using Interfaces;

[ApiController]
[Route("[controller]")]
public class LinkPreviewController : ControllerBase
{
    #region -- Methods --

    public LinkPreviewController(ILinkPreviewService linkPreviewService)
    {
        _linkPreviewService = linkPreviewService;
    }

    [HttpGet("get-metadata")]
    public async Task<ActionResult<MetaDataDto>> GetMetaData(string url)
    {
        var result = await _linkPreviewService.GetMetaDataByUrl(url);
        return Ok(result);
    }

    #endregion

    #region -- Fields --

    private readonly ILinkPreviewService _linkPreviewService;

    #endregion
}
