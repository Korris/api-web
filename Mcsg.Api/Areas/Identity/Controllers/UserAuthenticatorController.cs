using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Api.Areas.Identity.Controllers;
using Mcsg.Api.Areas.Identity.Requests;

using Common.Core.Controllers;
using Mcsg.Api.Areas.Identity.Interfaces;
using Mcsg.Api.Interfaces;

/// <summary>
/// UserAuthenticator controller
/// </summary>
[Authorize]
[Route("api/identity/[controller]")]
public class UserAuthenticatorController : BaseController
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="mediator">Mediator</param>
    /// <param name="setting">Setting</param>
    public UserAuthenticatorController(IMediator mediator, ISetting setting) : base(mediator)
    {
        _setting = setting;
        DomainName = _setting.Domain;
    }

    /// <summary>
    /// Create
    /// </summary>
    /// <returns>Return the result</returns>
    [HttpPost("Create")]
    public async Task<IActionResult> Create()
    {
        var request = new UserAuthenticatorCreateR();
        request.Analyze(HttpContext);

        var response = await _mediator.Send(request);
        response.ReturnUrl = AbsoluteUri;

        return Ok(response);
    }

    /// <summary>
    /// Update
    /// </summary>
    /// <returns>Return the result</returns>
    [HttpPost("Update")]
    public async Task<IActionResult> Update([FromBody] UserAuthenticatorUpdateR request)
    {
        request.Analyze(HttpContext);

        var response = await _mediator.Send(request);
        response.ReturnUrl = AbsoluteUri;

        return Ok(response);
    }

    /// <summary>
    /// Delete
    /// </summary>
    /// <param name="request"></param>
    /// <returns>Return the result</returns>
    [HttpDelete("Delete")]
    public async Task<IActionResult> Delete([FromBody] UserAuthenticatorDeleteR request)
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
