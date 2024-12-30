using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Mcsg.Realtime.Api.Controllers;

using Common.Core.Controllers;
using Common.SeedWork.Responses;
using Interfaces;
using Requests;

[ApiController]
[Route("[controller]")]
public class AuthenticationController : BaseController
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="mediator"></param>
    /// <param name="setting"></param>
    public AuthenticationController(IMediator mediator, ISetting setting) : base(mediator)
    {
        _setting = setting;
        DomainName = _setting.Domain;
    }

    /// <summary>
    /// ForceLogout
    /// </summary>
    /// <returns>Return the result</returns>
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
    /// <returns>Return the result</returns>
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
    /// <returns>Return the result</returns>
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

    /// <summary>
    /// Setting
    /// </summary>
    private readonly ISetting _setting;

    #endregion
}
