using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Social.Api.Controllers
{
    using DTOs;
    using Interfaces;

    [ApiController]
    [Route("[controller]")]
    public class ComicController : ControllerBase
    {
        private readonly IComicService _comicService;
        public ComicController(IComicService comicService)
        {
            _comicService = comicService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostFeed(PostSeriesReq request)
        {
            var result = await _comicService.PostComic(request);
            return Ok(result);
        }
        [HttpGet("{hashId}")]
        public async Task<IActionResult> GetComic(string hashId, bool isLoadChapters = true)
        {
            var result = await _comicService.GetComic(hashId, isLoadChapters);
            return Ok(result);
        }
        [HttpGet("top")]
        public async Task<IActionResult> GetTopComic()
        {
            var result = await _comicService.GetTopComic();
            return Ok(result);
        }
        [HttpGet("list")]
        public async Task<IActionResult> GetAllTopComic([FromQuery] PostListSeriesReq request)
        {
            var result = await _comicService.GetTopComicAsync(request);
            return Ok(result);
        }
        [HttpGet("relation")]
        public async Task<IActionResult> GetRelationComics([FromQuery] RelationPostSeriesReq request)
        {
            var result = await _comicService.GetRelationComicsAsync(request);
            return Ok(result);
        }
        [HttpGet("top-hit")]
        public async Task<IActionResult> GetTopListHitComic([FromQuery] TopPostReq request)
        {
            var result = await _comicService.GetTopHitListComic(request);
            return Ok(result);
        }
        [HttpGet("top-latest")]
        public async Task<IActionResult> GetTopListLatestComic([FromQuery] TopPostReq request)
        {
            var result = await _comicService.GetTopLatestListComic(request);
            return Ok(result);
        }
        [HttpGet("top-completed")]
        public async Task<IActionResult> GetTopListCompletedComic([FromQuery] TopPostReq request)
        {
            var result = await _comicService.GetTopCompletedListComic(request);
            return Ok(result);
        }

        [HttpGet("recommended")]
        public async Task<IActionResult> GetRecommendedComic(int number)
        {
            var result = await _comicService.GetRecommendedComic(number);
            return Ok(result);
        }
        [HttpPut("{hashId}")]
        [Authorize]
        public async Task<IActionResult> UpdateComic(string hashId, PostUpdateSeriesReq request)
        {
            var result = await _comicService.UpdateComic(hashId, request);
            return Ok(result);
        }
        [HttpGet("{hashId}/chapter/{order}")]
        public async Task<IActionResult> GetChapter(string hashId, int order)
        {
            var result = await _comicService.GetChapter(hashId, order);
            return Ok(result);
        }
        [HttpGet("{comicHashId}/chapters")]
        public async Task<IActionResult> GetChapters(string comicHashId, [FromQuery] ChapterListReq request)
        {
            var result = await _comicService.GetChapters(comicHashId, request);
            return Ok(result);
        }
        [HttpGet("{comicHashId}/chapters-list")]
        public async Task<IActionResult> GetChaptersList(string comicHashId)
        {
            var result = await _comicService.GetChaptersListSimple(comicHashId);
            return Ok(result);
        }
        [HttpPost("{comicHashId}/chapter")]
        [Authorize]
        public async Task<IActionResult> PostChapter(string comicHashId, ChapterComicReq chapterPostReq)
        {
            var result = await _comicService.PostChapterToComic(comicHashId, chapterPostReq);
            return Ok(result);
        }
        [HttpPut("{comicHashId}/chapter/{chapterOrder}")]
        [Authorize]
        public async Task<IActionResult> PutChapter(string comicHashId, int chapterOrder, ChapterComicReq chapterPostReq)
        {
            var result = await _comicService.UpdateChapterToComic(comicHashId, chapterOrder, chapterPostReq);
            return Ok(result);
        }
        [HttpPut("{comicHashId}/chapter-swap")]
        [Authorize]
        public async Task<IActionResult> PutSwapChapter(string comicHashId, ChapterOrderSwapReq orders)
        {
            var result = await _comicService.SwapChapterOrder(comicHashId, orders);
            return Ok(result);
        }
        [HttpDelete("{comicHashId}/chapter/{chapterOrder}")]
        [Authorize]
        public async Task<IActionResult> DeleteChapter(string comicHashId, int chapterOrder)
        {
            var result = await _comicService.DeleteChapter(comicHashId, chapterOrder);
            return Ok(result);
        }
        [HttpDelete("{postId}")]
        [Authorize]
        public async Task<IActionResult> DeleteFeed(Guid postId)
        {
            var result = await _comicService.Delete(postId);
            return Ok(result);
        }
        [HttpGet("my-comic")]
        [Authorize]
        public async Task<IActionResult> GetMyComic([FromQuery] PostListSeriesReq loadReq)
        {
            var result = await _comicService.GetMyComics(loadReq);
            return Ok(result);
        }

        [HttpGet("search-by-profileName")]
        public async Task<IActionResult> GetSearchComic([FromQuery] PostByProFileNameInput input)
        {
            var result = await _comicService.GetComicByUserProfileName(input);
            return Ok(result);
        }

        [HttpGet("search-by-tagName")]
        public async Task<IActionResult> GetSearchComicByTagName([FromQuery] PostByTagNameInput input)
        {
            var result = await _comicService.GetComicByTagName(input);
            return Ok(result);
        }
    }
}
