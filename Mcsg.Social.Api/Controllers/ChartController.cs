using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Social.Api.Controllers;

using Common.Core.Enums;
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

    /// <summary>
    /// Chart information for User
    /// </summary>
    /// <returns></returns>
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
        var result = await _chartService.GetFollowersChartInfo(req.UserId, req.TimezoneOffset, isGetDataIn7Days);
        return Ok(result);
    }

    /// <summary>
    /// Chart information for Comic, Document and Story
    /// </summary>
    /// <param name="isGetDataIn7Days"></param>
    /// <param name="postType"></param>
    /// <returns></returns>
    [HttpGet("chart-info")]
    public async Task<IActionResult> GetChartInfo(bool isGetDataIn7Days, PostType postType)
    {
        var req = new BaseR(HttpContext);
        var result = await _chartService.GetChartInfo(req.UserId, req.TimezoneOffset, isGetDataIn7Days, postType);
        return Ok(result);
    }

    [HttpGet("interaction-chart-info")]
    public async Task<IActionResult> GetInteractionChartInfo(bool isGetDataIn7Days)
    {
        var req = new BaseR(HttpContext);
        var result = await _chartService.GetInteractionChartInfo(req.UserId, req.TimezoneOffset, isGetDataIn7Days);
        return Ok(result);
    }
    #endregion

    #region -- Fields --

    private readonly IChartService _chartService;

    #endregion
}
