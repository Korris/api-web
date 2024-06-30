using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Social.Api.Controllers
{
    using Interfaces;
    using Requests;

    [ApiController]
    [Route("[controller]")]
    public class ReactionController : ControllerBase
    {
        private readonly IPostReactService _postReactService;
        private readonly ISubPostReactService _subPostReactService;
        private readonly ISubPostCommentReactService _subPostCommentReactService;
        private readonly IPostCommentReactService _postCommentReactService;
        public ReactionController(IPostReactService postReactService, ISubPostReactService subPostReactService, ISubPostCommentReactService subPostCommentReactService, IPostCommentReactService postCommentReactService)
        {
            _postReactService = postReactService;
            _subPostReactService = subPostReactService;
            _subPostCommentReactService = subPostCommentReactService;
            _postCommentReactService = postCommentReactService;
        }
        [HttpGet("post/{postId}")]
        public async Task<IActionResult> GetPostReacts(Guid postId)
        {
            var result = await _postReactService.GetReactions(postId);
            return Ok(result);
        }
        [HttpPost("post")]
        [Authorize]
        public async Task<IActionResult> AddPostReact(ReactReq request)
        {
            var result = await _postReactService.AddReactionToPost(request.TargetId, request.Type);
            return Ok(result);
        }
        [HttpDelete("post/{postId}")]
        [Authorize]
        public async Task<IActionResult> DeleteReact(Guid postId)
        {
            var result = await _postReactService.RemoveReactionToPost(postId);
            return Ok(result);
        }
        [HttpGet("sub-post/{subPostId}")]
        public async Task<IActionResult> GetSubPostReacts(Guid subPostId)
        {
            var result = await _subPostReactService.GetReactions(subPostId);
            return Ok(result);
        }
        [HttpPost("sub-post")]
        [Authorize]
        public async Task<IActionResult> AddSubPostReact(ReactReq request)
        {
            var result = await _subPostReactService.AddReactionToSubPost(request.TargetId, request.Type);
            return Ok(result);
        }

        [HttpDelete("sub-post/{subPostId}")]
        [Authorize]
        public async Task<IActionResult> DeleteSubPostReact(Guid subPostId)
        {
            var result = await _subPostReactService.RemoveReactionToSubPost(subPostId);
            return Ok(result);
        }
        [HttpGet("comment-post/{commentPostId}")]
        public async Task<IActionResult> GetCommentPostReacts(Guid commentPostId)
        {
            var result = await _postCommentReactService.GetReactions(commentPostId);
            return Ok(result);
        }
        [HttpPost("comment-post")]
        [Authorize]
        public async Task<IActionResult> AddCommentPostReact(ReactReq request)
        {
            var result = await _postCommentReactService.AddReaction(request.TargetId, request.Type);
            return Ok(result);
        }

        [HttpDelete("comment-post/{commentPostId}")]
        [Authorize]
        public async Task<IActionResult> DeleteCommentPostReact(Guid commentPostId)
        {
            var result = await _postCommentReactService.RemoveReaction(commentPostId);
            return Ok(result);
        }
        [HttpGet("comment-subpost/{commentSubPostId}")]
        public async Task<IActionResult> GetCommentSubPostReacts(Guid commentSubPostId)
        {
            var result = await _subPostCommentReactService.GetReactions(commentSubPostId);
            return Ok(result);
        }
        [HttpPost("comment-subpost")]
        [Authorize]
        public async Task<IActionResult> AddCommentSubPostReact(ReactReq request)
        {
            var result = await _subPostCommentReactService.AddReaction(request.TargetId, request.Type);
            return Ok(result);
        }

        [HttpDelete("comment-subpost/{commentSubPostId}")]
        [Authorize]
        public async Task<IActionResult> DeleteCommentSubPostReact(Guid commentSubPostId)
        {
            var result = await _subPostCommentReactService.RemoveReaction(commentSubPostId);
            return Ok(result);
        }
    }
}
