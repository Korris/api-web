using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Mcsg.Comic.Api.Controllers;

using Common.Core.Controllers;
using Common.SeedWork.Responses;
using Interfaces;
using Requests;
using static Common.SeedWork.Constants.Setting;

/// <summary>
/// SubPost controller
/// </summary>
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
    /// Search
    /// </summary>
    /// <param name="request">Request</param>
    /// <returns>Return the result</returns>
    [HttpPatch("Search"), Authorize]
    [ProducesResponseType(typeof(SingleResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> Search([FromBody] SubPostSearchR request)
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
        response.ReturnUrl = AbsoluteUri;
        return Ok(response);
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// Setting
    /// </summary>
    private readonly ISetting _setting;

    #endregion
}
