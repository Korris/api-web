using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Social.Api.Controllers
{
    using DTOs;
    using Interfaces;

    [ApiController]
    [Route("[controller]")]
    public class StoryController : ControllerBase
    {
        private readonly IStoryService _storyService;
        public StoryController(IStoryService storyService)
        {
            _storyService = storyService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostFeed(PostSeriesReq request)
        {
            var result = await _storyService.PostStory(request);
            return Ok(result);
        }
        [HttpGet("{hashId}")]
        public async Task<IActionResult> GetStory(string hashId, bool isLoadChapters = true)
        {
            var result = await _storyService.GetStory(hashId, isLoadChapters);
            return Ok(result);
        }
        [HttpPut("{hashId}")]
        [Authorize]
        public async Task<IActionResult> UpdateStory(string hashId, PostUpdateSeriesReq request)
        {
            var result = await _storyService.UpdateStory(hashId, request);
            return Ok(result);
        }
        [HttpPost("{hashId}/chapter")]
        [Authorize]
        public async Task<IActionResult> PostChapter(string hashId, ChapterStoryReq chapterPostReq)
        {
            var result = await _storyService.PostChapterToStory(hashId, chapterPostReq);
            return Ok(result);
        }
        [HttpDelete("{postId}")]
        [Authorize]
        public async Task<IActionResult> DeleteFeed(Guid postId)
        {
            var result = await _storyService.Delete(postId);
            return Ok(result);
        }

        [HttpGet("top")]
        public async Task<IActionResult> GetTopStory()
        {
            var result = await _storyService.GetTopStory();
            return Ok(result);
        }
        [HttpGet("list")]
        public async Task<IActionResult> GetAllTopStory([FromQuery] PostListSeriesReq request)
        {
            var result = await _storyService.GetTopStoryAsync(request);
            return Ok(result);
        }
        [HttpGet("relation")]
        public async Task<IActionResult> GetRelationStories([FromQuery] RelationPostSeriesReq request)
        {
            var result = await _storyService.GetRelationStoriesAsync(request);
            return Ok(result);
        }
        [HttpGet("top-hit")]
        public async Task<IActionResult> GetTopListHitStory([FromQuery] TopPostReq request)
        {
            var result = await _storyService.GetTopHitListStory(request);
            return Ok(result);
        }
        [HttpGet("top-latest")]
        public async Task<IActionResult> GetTopListLatestStory([FromQuery] TopPostReq request)
        {
            var result = await _storyService.GetTopLatestListStory(request);
            return Ok(result);
        }
        [HttpGet("top-completed")]
        public async Task<IActionResult> GetTopListCompletedStory([FromQuery] TopPostReq request)
        {
            var result = await _storyService.GetTopCompletedListStory(request);
            return Ok(result);
        }
        [HttpGet("{storyHashId}/chapters")]
        public async Task<IActionResult> GetChapters(string storyHashId, [FromQuery] ChapterListReq request)
        {
            var result = await _storyService.GetChapters(storyHashId, request);
            return Ok(result);
        }
        [HttpGet("{storyHashId}/chapters-list")]
        public async Task<IActionResult> GetChaptersList(string storyHashId)
        {
            var result = await _storyService.GetChaptersListSimple(storyHashId);
            return Ok(result);
        }
        [HttpGet("{storyHashId}/chapter/{chapterOrder}")]
        public async Task<IActionResult> GetChapter(string storyHashId, int chapterOrder)
        {
            var result = await _storyService.GetChapter(storyHashId, chapterOrder);
            return Ok(result);
        }
        [HttpPut("{storyHashId}/chapter/{chapterOrder}")]
        [Authorize]
        public async Task<IActionResult> PutChapter(string storyHashId, int chapterOrder, ChapterStoryReq chapterPostReq)
        {
            var result = await _storyService.UpdateChapterToStory(storyHashId, chapterOrder, chapterPostReq);
            return Ok(result);
        }
        [HttpPut("{storyHashId}/chapter-swap")]
        [Authorize]
        public async Task<IActionResult> PutSwapChapter(string storyHashId, ChapterOrderSwapReq orders)
        {
            var result = await _storyService.SwapChapterOrder(storyHashId, orders);
            return Ok(result);
        }
        [HttpDelete("{storyHashId}/chapter/{chapterOrder}")]
        [Authorize]
        public async Task<IActionResult> DeleteChapter(string storyHashId, int chapterOrder)
        {
            var result = await _storyService.DeleteChapter(storyHashId, chapterOrder);
            return Ok(result);
        }
        [HttpGet("my-stories")]
        [Authorize]
        public async Task<IActionResult> GetMyStories([FromQuery] PostListSeriesReq loadReq)
        {
            var result = await _storyService.GetMyStories(loadReq);
            return Ok(result);
        }

        [HttpGet("search-by-profileName")]
        public async Task<IActionResult> GetSearchComic([FromQuery] PostByProFileNameInput input)
        {
            var result = await _storyService.GetStoryByUserProfileName(input);
            return Ok(result);
        }

        [HttpGet("search-by-tagName")]
        public async Task<IActionResult> GetSearchComicByTagName([FromQuery] PostByTagNameInput input)
        {
            var result = await _storyService.GetStoryByTagName(input);
            return Ok(result);
        }
    }
}
