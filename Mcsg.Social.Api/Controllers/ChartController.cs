using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Social.Api.Controllers;

using Common.Core.Requests;
using Interfaces;

[ApiController]
[Route("[controller]")]
[Authorize]
public class ChartController : ControllerBase
{
    #region -- Methods --

    public ChartController(IChartService chartService)
    {
        _chartService = chartService;
    }

    [HttpGet("general-info")]
    public async Task<IActionResult> GetGeneralInfo()
    {
        var req = new BaseR(HttpContext);
        var result = await _chartService.GetGeneralInfo(req.UserId);
        return Ok(result);
    }

    [HttpGet("followers-chart-info")]
    public async Task<IActionResult> GetFollowersChartInfo(bool isGetDataIn7Days)
    {
        var req = new BaseR(HttpContext);
        var result = await _chartService.GetFollowersChartInfo(req.UserId, isGetDataIn7Days);
        return Ok(result);
    }

    [HttpGet("comic-story-chart-info")]
    public async Task<IActionResult> GetComicOrStoryChartInfo(bool isGetDataIn7Days, bool isComic)
    {
        var req = new BaseR(HttpContext);
        var result = await _chartService.GetComicOrStoryChartInfo(req.UserId, isGetDataIn7Days, isComic);
        return Ok(result);
    }

    [HttpGet("interaction-chart-info")]
    public async Task<IActionResult> GetInteractionChartInfo(bool isGetDataIn7Days)
    {
        var req = new BaseR(HttpContext);
        var result = await _chartService.GetInteractionChartInfo(req.UserId, isGetDataIn7Days);
        return Ok(result);
    }
    #endregion

    #region -- Fields --

    private readonly IChartService _chartService;

    #endregion
}
