using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Api.Areas.Identity.Controllers;
using Mcsg.Api.Areas.Identity.Requests;

using Common.Core.Controllers;
using Mcsg.Api.Areas.Identity.Interfaces;
using Mcsg.Api.Interfaces;

/// <summary>
/// UserRecovery controller
/// </summary>
[Authorize]
[Route("api/identity/[controller]")]
public class UserRecoveryController : BaseController
{
    #region -- Methods --

    /// <summary>
    /// Initializes
    /// </summary>
    /// <param name="mediator">Mediator</param>
    /// <param name="setting">Setting</param>
    public UserRecoveryController(IMediator mediator, ISetting setting) : base(mediator)
    {
        _setting = setting;
        DomainName = _setting.Domain;
    }

    /// <summary>
    /// Update
    /// </summary>
    /// <returns>Return the result</returns>
    [HttpPost("Update")]
    public async Task<IActionResult> Update([FromBody] UserRecoveryUpdateR request)
    {
        request.Analyze(HttpContext);
        var response = await _mediator.Send(request);
        response.ReturnUrl = AbsoluteUri;
        return Ok(response);
    }

    /// <summary>
    /// Search
    /// </summary>
    /// <returns>Return the result</returns>
    [HttpPatch("Search")]
    public async Task<IActionResult> Search([FromBody] UserRecoverySearchR request)
    {
        request.Analyze(HttpContext);
        var response = await _mediator.Send(request);
        response.ReturnUrl = AbsoluteUri;
        return Ok(response);
    }

    /// <summary>
    /// View
    /// </summary>
    /// <returns>Return the result</returns>
    [HttpPatch("View")]
    public async Task<IActionResult> View([FromBody] UserRecoveryViewR request)
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
