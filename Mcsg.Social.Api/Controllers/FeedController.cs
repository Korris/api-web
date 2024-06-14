using Mcsg.Social.Api.DTOs;
using Mcsg.Social.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Social.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FeedController : ControllerBase
    {
        private readonly IFeedService _feedService;
        private readonly IPostReactService _postReactService;
        public FeedController(IFeedService feedService, IPostReactService postReactService)
        {
            _feedService = feedService;
            _postReactService = postReactService;
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostFeed(FeedPostReq request)
        {
            var result = await _feedService.PostFeedAsync(request);
            return Ok(result);
        }
        [HttpPut("{hashId}")]
        [Authorize]
        public async Task<IActionResult> UpdateFeed(string hashId, UpdateFeedPostReq request)
        {
            var result = await _feedService.UpdateFeedAsync(hashId, request);
            return Ok(result);
        }
        [HttpGet("list")]
        public async Task<IActionResult> GetFeeds([FromQuery] FeedLoadReq request)
        {
            var result = await _feedService.GetFeedsAsync(request, Mcsg.Social.Api.Enums.LoadFeedType.ALL);
            return Ok(result);
        }
        [HttpGet("display-setting")]
        public IActionResult GetFeedDisplayConfig()
        {
            var result = _feedService.GetFeedDisplayConfig();
            return Ok(result);
        }
        [HttpGet("trending")]
        public async Task<IActionResult> GetTredingFeeds([FromQuery] FeedLoadReq request)
        {
            var result = await _feedService.GetFeedsAsync(request, Mcsg.Social.Api.Enums.LoadFeedType.TRENDING);
            return Ok(result);
        }
        [HttpGet("hot")]
        public async Task<IActionResult> GetHotFeeds([FromQuery] FeedLoadReq request)
        {
            var result = await _feedService.GetFeedsAsync(request, Mcsg.Social.Api.Enums.LoadFeedType.HOT);
            return Ok(result);
        }
        [HttpGet("list/{tagName}")]
        public async Task<IActionResult> GetFeedByTag(string tagName, [FromQuery] FeedLoadReq request)
        {
            var result = await _feedService.GetFeedsByTagAsync(tagName, request);
            return Ok(result);
        }
        [HttpGet("search/{keyword}")]
        public async Task<IActionResult> GetFeedByKeyword(string keyword, [FromQuery] SearchKeywordReq request)
        {
            var result = await _feedService.GetFeedByKeywordAsync(keyword, request);
            return Ok(result);
        }
        [HttpGet("{hashId}")]
        public async Task<IActionResult> GetFeed(string hashId)
        {
            var result = await _feedService.GetFeedAsync(hashId);
            return Ok(result);
        }

        [HttpGet("subpost/{hashId}")]
        public async Task<IActionResult> GetFeedSubPost(string hashId)
        {
            var result = await _feedService.GetFeedSubPostAsync(hashId);
            return Ok(result);
        }

        [HttpDelete("{postId}")]
        [Authorize]
        public async Task<IActionResult> DeleteFeed(Guid postId)
        {
            var result = await _feedService.DeleteFeedAsync(postId);
            return Ok(result);
        }

        [HttpGet("{id}/reactions")]
        public async Task<IActionResult> GetPostReactsByType(Guid id, [FromQuery] ReactionByTargetRequest request)
        {
            var result = await _postReactService.GetReactionsByTargetAsync(id, request);
            return Ok(result);
        }

        [HttpPost("report")]
        [Authorize]
        public async Task<IActionResult> ReportFeed([FromBody] ReportPostReq request)
        {
            var result = await _feedService.ReportFeedAsync(request);
            return Ok(result);
        }
    }
}
