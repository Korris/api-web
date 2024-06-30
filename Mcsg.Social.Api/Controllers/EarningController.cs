using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Social.Api.Controllers
{
    using DTOs;
    using Interfaces;
    using Models;
    using Models.Earning;

    [ApiController]
    [Route("[controller]")]
    public class EarningController : ControllerBase
    {
        private readonly IEarningService _earningService;
        public EarningController(IEarningService earningService)
        {
            _earningService = earningService;
        }

        [Authorize]
        [HttpGet("dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            var data = await _earningService.GetDataDashboardAsync();
            return Ok(data);
        }

        [Authorize]
        [HttpGet("earning-status")]
        public async Task<IActionResult> CheckUserStatus()
        {
            var result = await _earningService.CheckUserEarningStatusAsync();
            return Ok(result);
        }

        [Authorize]
        [HttpPost("enable-earning")]
        public async Task<IActionResult> EnableEarning(EnableEarningModeRequest req)
        {
            await _earningService.EnableEarningAsync(req);
            return Ok();
        }

        [Authorize]
        [HttpGet("my-comic-story-list")]
        public async Task<IActionResult> GetMyComicStorySeries()
        {
            var result = await _earningService.GetMyComicStoryListAsync();
            return Ok(result);
        }

        [Authorize]
        [HttpGet("report-detail")]
        public async Task<IActionResult> GetReportOfSeries(string hashId, [FromQuery] ChapterListReq req)
        {
            var result = await _earningService.GetReportOfSeriesAsync(hashId, req);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("affiliate-code")]
        public IActionResult GetAffiliateCode([FromQuery] AffiliateCodeRequest req)
        {
            var result = _earningService.GetAffiliateCode(req);
            return Ok(result);
        }
    }
}
