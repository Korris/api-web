using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Api.Areas.Social.Controllers;

using Mcsg.Api.Areas.Social.Dtos;
using Mcsg.Api.Areas.Social.Interfaces;

[ApiController]
[Route("api/social/[controller]")]
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
