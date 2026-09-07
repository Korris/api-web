using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Mcsg.Api.Areas.Realtime.Controllers;

using Common.Core.Controllers;
using Common.SeedWork.Responses;
using Mcsg.Api.Interfaces;
using Mcsg.Api.Areas.Realtime.Requests;

[ApiController]
[Route("api/realtime/[controller]")]
public class AuthenticationController : BaseController
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public AuthenticationController(IMediator mediator, ISetting setting) : base(mediator)
    {
        _setting = setting;
        DomainName = _setting.Domain;
    }

    /// <summary>
    /// ForceLogout
    /// </summary>
    [HttpPost("ForceLogout"), Authorize]
    [ProducesResponseType(typeof(SingleResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> ForceLogout([FromBody] AuthenticationForceLogoutR request)
    {
        request.Analyze(HttpContext);

        var response = await _mediator.Send(request);
        response.ReturnUrl = AbsoluteUri;

        return Ok(response);
    }

    /// <summary>
    /// ResetPassword
    /// </summary>
    [HttpPost("ResetPassword")]
    [ProducesResponseType(typeof(SingleResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> ResetPassword([FromBody] AuthenticationResetPasswordR request)
    {
        request.Analyze(HttpContext);

        var response = await _mediator.Send(request);
        response.ReturnUrl = AbsoluteUri;

        return Ok(response);
    }

    /// <summary>
    /// VerifyOtp
    /// </summary>
    [HttpPost("VerifyOtp"), Authorize]
    [ProducesResponseType(typeof(SingleResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> VerifyOtp([FromBody] AuthenticationVerifyOtpR request)
    {
        request.Analyze(HttpContext);

        var response = await _mediator.Send(request);
        response.ReturnUrl = AbsoluteUri;

        return Ok(response);
    }

    #endregion

    #region -- Fields --

    private readonly ISetting _setting;

    #endregion
}
