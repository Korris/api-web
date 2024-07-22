using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Analytic.Api.Controllers
{
    using Lib.Common.Extensions;
    using Lib.Common.Web.Extensions;
    using Models.Request;
    using Services;

    [Route("[controller]")]
    [ApiController]
    public class TrackingController : ControllerBase
    {
        private readonly ITrackingService _trackingService;
        private readonly IHttpContextAccessor _accessor;
        public TrackingController(ITrackingService trackingService, IHttpContextAccessor accessor)
        {
            _trackingService = trackingService;
            _accessor = accessor;
        }
        [HttpPost("add-view")]
        public async Task<IActionResult> AddView(AddViewReqSimple request)
        {
            var trackingReq = new AddViewReq
            {
                SubPostId = request.SubPostId,
                BrowserAgent = _accessor?.HttpContext?.Request?.Headers["User-Agent"],
                IpAddress = _accessor.GetIpAddress(),
                SessionId = _accessor?.HttpContext?.GetSessionId(),
            };
            await _trackingService.AddView(trackingReq);
            return Ok();
        }
        [HttpPost("add-view-simulate")]
        public async Task<IActionResult> AddViewSim(AddViewSimReqSimple request)
        {
            var trackingReq = new AddViewReq
            {
                SubPostId = request.SubPostId,
                BrowserAgent = _accessor?.HttpContext?.Request?.Headers["User-Agent"],
                //SessionId = _accessor?.HttpContext?.GetSessionId(),
                UserType = request.UserType,
            };
            await _trackingService.AddViewSim(trackingReq, request.NumberView);
            return Ok();
        }
        [HttpGet("post-view/{postId}")]
        public async Task<IActionResult> GetPostView(Guid postId)
        {
            return Ok(await _trackingService.GetPostView(postId));
        }
        [HttpGet("subpost-view/{subPostId}")]
        public async Task<IActionResult> GetSubPostView(Guid subPostId)
        {
            return Ok(await _trackingService.GetSubPostView(subPostId));
        }
    }
}
