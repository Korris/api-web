using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Api.Areas.Story.Controllers;

using Mcsg.Api.Areas.Story.Dtos;
using Mcsg.Api.Areas.Story.Interfaces;

[ApiController]
[Route("api/story/[controller]")]
public class LinkPreviewController : ControllerBase
{
    #region -- Methods --

    public LinkPreviewController(ILinkPreviewService linkPreviewService)
    {
        _linkPreviewService = linkPreviewService;
    }
    [HttpGet("get-metadata")]
    public ActionResult<MetaDataDto> GetMetaData(string url)
    {
        var result = _linkPreviewService.GetMetaDataByUrl(url);
        return Ok(result);
    }

    #endregion

    #region -- Fields --

    private readonly ILinkPreviewService _linkPreviewService;

    #endregion
}
