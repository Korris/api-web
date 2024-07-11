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
