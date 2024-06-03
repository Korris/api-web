using Mcsg.Admin.Api.DTOs.Posts;
using Mcsg.Admin.Api.Services.Interface;
using Mcsg.Lib.Common.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Admin.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = $"{RoleNames.Admin},{RoleNames.SysAdmin}")]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        public PostController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetList([FromQuery] PostListReq request)
        {
            var result = await _postService.GetListAsync(request);
            return Ok(result);
        }

        [HttpPut("deactive/{id}")]
        public async Task<IActionResult> DeactivePost(Guid id, [FromBody] PostStatusReq request)
        {
            var result = await _postService.DeactivePostAsync(id, request);
            return Ok(result);
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = RoleNames.SysAdmin)]
        public async Task<IActionResult> DeleteUser(Guid id, [FromBody] PostStatusReq request)
        {
            var result = await _postService.DeletePostAsync(id, request);
            return Ok(result);
        }
    }
}
