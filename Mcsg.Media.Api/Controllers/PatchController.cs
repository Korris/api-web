using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Mcsg.Media.Api.Controllers;

using Common.SeedWork.Responses;
using Interfaces;
using Requests;
using static Common.SeedWork.Constants.Setting;

/// <summary>
/// Patch controller
/// </summary>
[ApiController]
[Route("[controller]")]
public class PatchController : ControllerBase
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="mediator">Mediator</param>
    /// <param name="setting">Setting</param>
    public PatchController(IMediator mediator, ISetting setting)
    {
        _mediator = mediator;
        _setting = setting;
    }

    /// <summary>
    /// Update the ShareUrl in the Resources table
    /// </summary>
    /// <returns>Return the result</returns>
    [HttpPost("UpdateShareUrl")]
    [ProducesResponseType(typeof(SingleResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> UpdateShareUrl([FromBody] PatchUpdateShareUrlR request)
    {
        if (request.Otp != CommonPrefix)
        {
            return Unauthorized();
        }

        request.Analyze(HttpContext);
        request.DetectMobileCall(_setting.MobileUserAgent);

        var response = await _mediator.Send(request);
        response.ReturnUrl = request.GetAbsoluteUri(_setting.Domain);

        return Ok(response);
    }

    /// <summary>
    /// Update the UserName in the Users table
    /// </summary>
    /// <returns>Return the result</returns>
    [HttpPost("UpdateUserName")]
    [ProducesResponseType(typeof(SingleResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> UpdateUserName([FromBody] PatchUpdateUserNameR request)
    {
        if (request.Otp != CommonPrefix)
        {
            return Unauthorized();
        }

        request.Analyze(HttpContext);
        request.DetectMobileCall(_setting.MobileUserAgent);

        var response = await _mediator.Send(request);
        response.ReturnUrl = request.GetAbsoluteUri(_setting.Domain);

        return Ok(response);
    }

    /// <summary>
    /// Move the folder in MinIO to match the user who owns it
    /// </summary>
    /// <returns>Return the result</returns>
    [HttpPost("MoveFolder")]
    [ProducesResponseType(typeof(SingleResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> MoveFolder([FromBody] PatchMoveFolderR request)
    {
        if (request.Otp != CommonPrefix)
        {
            return Unauthorized();
        }

        request.Analyze(HttpContext);
        request.DetectMobileCall(_setting.MobileUserAgent);

        var response = await _mediator.Send(request);
        response.ReturnUrl = request.GetAbsoluteUri(_setting.Domain);

        return Ok(response);
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// Mediator
    /// </summary>
    private readonly IMediator _mediator;

    /// <summary>
    /// Setting
    /// </summary>
    private readonly ISetting _setting;

    #endregion
}