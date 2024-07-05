using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Social.Api.Controllers
{
    using Common.Core.Enums;
    using Interfaces;
    using Requests;

    [ApiController]
    [Route("[controller]")]
    public class CommentController : ControllerBase
    {
        #region -- Methods --

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        #region Should remove after FE integrate code
        [Obsolete("This method is obsolete, please use /feed/latest")]
        [HttpGet("post/latest")]
        public async Task<IActionResult> GetPostLatestComment([FromQuery] Guid postId)
        {
            var result = await _commentService.GetLatestPostCommentInAsync(postId);
            return Ok(result);
        }

        [Obsolete("This method is obsolete, please use /sub-feed/latest")]
        [HttpGet("sub-post/latest")]
        public async Task<IActionResult> GetSubPostLatestComment([FromQuery] Guid postId)
        {
            var result = await _commentService.GetLatestSubPostCommentInAsync(postId);
            return Ok(result);
        }

        [Obsolete("This method is obsolete, please use /feed")]
        [HttpGet("post")]
        public async Task<IActionResult> GetPostComments([FromQuery] CommentLoadReq request)
        {
            var result = await _commentService.GetCommentsOfPostAsync(request);
            return Ok(result);
        }

        [Obsolete("This method is obsolete, please use /sub-feed")]
        [HttpGet("sub-post")]
        public async Task<IActionResult> GetSubPostComments([FromQuery] CommentLoadReq request)
        {
            var result = await _commentService.GetCommentsOfSubPostAsync(request, PostType.Feed);
            return Ok(result);
        }
        #endregion

        [HttpGet("feed/latest")]
        public async Task<IActionResult> GetFeedLatestComment([FromQuery] Guid postId)
        {
            var result = await _commentService.GetLatestPostCommentInAsync(postId);
            return Ok(result);
        }

        [HttpGet("sub-feed/latest")]
        public async Task<IActionResult> GetSubFeedLatestComment([FromQuery] Guid postId)
        {
            var result = await _commentService.GetLatestSubPostCommentInAsync(postId);
            return Ok(result);
        }

        [HttpGet("feed")]
        public async Task<IActionResult> GetFeedComments([FromQuery] CommentLoadReq request)
        {
            var result = await _commentService.GetCommentsOfPostAsync(request);
            return Ok(result);
        }

        [HttpGet("sub-feed")]
        public async Task<IActionResult> GetSubFeedComments([FromQuery] CommentLoadReq request)
        {
            var result = await _commentService.GetCommentsOfSubPostAsync(request, PostType.Feed);
            return Ok(result);
        }

        [HttpGet("comic")]
        public async Task<IActionResult> GetComicComments([FromQuery] CommentLoadReq request)
        {
            var result = await _commentService.GetCommentsOfPostAsync(request);
            return Ok(result);
        }

        [HttpGet("comic-chapter")]
        public async Task<IActionResult> GetComicChapterComments([FromQuery] CommentLoadReq request)
        {
            var result = await _commentService.GetCommentsOfSubPostAsync(request, PostType.Comic);
            return Ok(result);
        }

        [HttpGet("story")]
        public async Task<IActionResult> GetStoryComments([FromQuery] CommentLoadReq request)
        {
            var result = await _commentService.GetCommentsOfPostAsync(request);
            return Ok(result);
        }

        [HttpGet("story-chapter")]
        public async Task<IActionResult> GetStoryChapterComments([FromQuery] CommentLoadReq request)
        {
            var result = await _commentService.GetCommentsOfSubPostAsync(request, PostType.Story);
            return Ok(result);
        }

        [HttpGet("comment-most-reaction")]
        public async Task<IActionResult> GetCommentWithMostReaction([FromQuery] CommentMostReactionR input)
        {
            var result = await _commentService.GetCommentWithMostReaction(input);
            return Ok(result);
        }

        [HttpGet("reply-by-comment")]
        public async Task<IActionResult> GetReplyByCommentId([FromQuery] CommentReplyByCommentR input)
        {
            var result = await _commentService.GetReplyByCommentId(input);
            return Ok(result);
        }

        #endregion

        #region -- Fields --

        private readonly ICommentService _commentService;

        #endregion
    }
}
