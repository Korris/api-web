using Mcsg.Api.DTOs;
using Mcsg.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class FavoriteController : ControllerBase
    {
        private readonly IFavoriteService _favoriteService;
        public FavoriteController(IFavoriteService favoriteService)
        {
            _favoriteService = favoriteService;
        }

        [HttpPost("add-post-favorite")]
        public async Task<IActionResult> AddPostToFavorite([FromBody] Guid postId)
        {
            var result = await _favoriteService.AddPostToFavoriteAsync(postId);
            return Ok(result);
        }

        [HttpPost("add-tag-favorite")]
        public async Task<IActionResult> AddTagToFavorite([FromBody] Guid tagId)
        {
            var result = await _favoriteService.AddTagToFavoriteAsync(tagId);
            return Ok(result);
        }

        [HttpGet("tag-favorite")]
        public async Task<IActionResult> GetTagFavorite([FromQuery] FavoriteTagReq req)
        {
            var result = await _favoriteService.GetTagFavoriteAsync(req);
            return Ok(result);
        }

        [HttpGet("post-favorite")]
        public async Task<IActionResult> GetPostFavorite([FromQuery] FavoritePostReq req)
        {
            var result = await _favoriteService.GetPostFavoriteByUserAsync(req);
            return Ok(result);
        }

        [HttpDelete("remove-tag/{tagid}")]
        public async Task<IActionResult> RemoveTagFavorite(Guid tagid)
        {
            var result = await _favoriteService.RemoveTagToFavoriteAsync(tagid);
            return Ok(result);
        }

        [HttpDelete("remove-post/{postId}")]
        public async Task<IActionResult> RemovePostFavorite(Guid postId)
        {
            var result = await _favoriteService.RemovePostToFavoriteAsync(postId);
            return Ok(result);
        }
    }
}
