using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Api.Areas.Document.Controllers;

using Mcsg.Api.Areas.Document.Dtos;
using Mcsg.Api.Areas.Document.Interfaces;

[ApiController]
[Route("api/document/[controller]")]
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
