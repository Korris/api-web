using Mcsg.Social.Api.Models;
using Mcsg.Social.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Social.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LinkPreviewController : ControllerBase
    {
        private readonly ILinkPreviewService _linkPreviewService;
        public LinkPreviewController(ILinkPreviewService linkPreviewService)
        {
            _linkPreviewService = linkPreviewService;
        }
        [HttpGet("get-metadata")]
        public ActionResult<MetaDataResponse> GetMetaData(string url)
        {
            var result = _linkPreviewService.GetMetaDataByUrl(url);
            return Ok(result);
        }
    }
}
