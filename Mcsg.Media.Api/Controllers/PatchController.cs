using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Mcsg.Media.Api.Controllers;

using Common.Core.Controllers;
using Common.SeedWork.Responses;
using Interfaces;
using Requests;
using static Common.SeedWork.Constants.Setting;

/// <summary>
/// Patch controller
/// </summary>
public class PatchController : BaseController
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="mediator">Mediator</param>
    /// <param name="setting">Setting</param>
    public PatchController(IMediator mediator, ISetting setting) : base(mediator)
    {
        _setting = setting;
        DomainName = _setting.Domain;
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