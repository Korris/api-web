using MediatR;
using Microsoft.AspNetCore.Authorization;
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
    /// ResizeImage
    /// </summary>
    /// <param name="request">Request</param>
    /// <returns>Return the result</returns>
    [HttpPost("ResizeImage"), Authorize(Policy = Policy.Admin)]
    [ProducesResponseType(typeof(SingleResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> ResizeImage([FromBody] PatchResizeImageR request)
    {
        if (request.Otp != CommonPrefix)
        {
            return Unauthorized();
        }

        request.Analyze(HttpContext);

        var response = await _mediator.Send(request);
        response.ReturnUrl = request.GetAbsoluteUri(_setting.Domain);

        return Ok(response);
    }

    /// <summary>
    /// Encrypt the Email, Phone, and SocialId in the Users and UserSocials tables
    /// </summary>
    /// <param name="request">Request</param>
    /// <returns>Return the result</returns>
    [HttpPost("EncryptEmail"), Authorize(Policy = Policy.Admin)]
    [ProducesResponseType(typeof(SingleResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> EncryptEmail([FromBody] PatchEncryptEmailR request)
    {
        if (request.Otp != CommonPrefix)
        {
            return Unauthorized();
        }

        request.Analyze(HttpContext);

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