using Mcsg.Api.DTOs;
using Mcsg.Api.Services.Interfaces;
using Mcsg.Lib.Model.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Identity.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        public PostController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet("{postType}/tag/{tagName}")]
        public async Task<IActionResult> GetTopListHitComic(PostType postType, string tagName, [FromQuery] TopPostReq loadReq)
        {
            if (postType != PostType.STORY && postType != PostType.COMIC)
            {
                return NotFound();
            }
            var result = await _postService.GetSeriesByTagByPage(postType, tagName, loadReq);
            return Ok(result);
        }
    }
}
