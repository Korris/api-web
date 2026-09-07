using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Mcsg.Api.Interfaces;

namespace Mcsg.Api.Areas.Social.Controllers;

using Common.Core.Controllers;
using Common.SeedWork.Responses;
using Mcsg.Api.Areas.Social.Interfaces;
using Mcsg.Api.Areas.Social.Requests;
using static Common.SeedWork.Constants.Setting;

/// <summary>
/// SubPost controller
/// </summary>
// Full prefix: the old service stripped /api/social via UseApiPathRewrite; BaseController alone would map to /v1/SubPost and collide across areas
[Route("api/social/v1/[controller]")]
public class SubPostController : BaseController
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="mediator">Mediator</param>
    /// <param name="setting">Setting</param>
    public SubPostController(IMediator mediator, ISetting setting) : base(mediator)
    {
        _setting = setting;
        DomainName = _setting.Domain;
    }

    /// <summary>
    /// Delete
    /// </summary>
    /// <returns>Return the result</returns>
    [HttpDelete("Delete"), Authorize]
    [ProducesResponseType(typeof(SingleResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> Delete([FromBody] SubPostDeleteR request)
    {
        request.Analyze(HttpContext);

        var response = await _mediator.Send(request);
        response.ReturnUrl = AbsoluteUri;

        return Ok(response);
    }

    /// <summary>
    /// SyncToAna
    /// </summary>
    /// <param name="request">Request</param>
    /// <returns>Return the result</returns>
    [HttpPost("SyncToAna"), Authorize(Policy = Policy.Admin)]
    [ProducesResponseType(typeof(SingleResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> SyncToAna([FromBody] SubPostSyncToAnaR request)
    {
        request.Analyze(HttpContext);
        var response = await _mediator.Send(request);
        return Ok(response.Data);
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// Setting
    /// </summary>
    private readonly ISetting _setting;

    #endregion
}
