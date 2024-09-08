using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Social.Api.Controllers;

using Interfaces;
using Models.Earning;
using Requests;

[ApiController]
[Route("[controller]")]
public class EarningController : ControllerBase
{
    #region -- Methods --

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
    public async Task<IActionResult> EnableEarning(EarningEnableR req)
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
    public async Task<IActionResult> GetReportOfSeries(string hashId, [FromQuery] PostChapterListR req)
    {
        var result = await _earningService.GetReportOfSeriesAsync(hashId, req);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("affiliate-code")]
    public IActionResult GetAffiliateCode([FromQuery] EarningAffiliateCodeR req)
    {
        var result = _earningService.GetAffiliateCode(req);
        return Ok(result);
    }

    #endregion

    #region -- Fields --

    private readonly IEarningService _earningService;

    #endregion
}
