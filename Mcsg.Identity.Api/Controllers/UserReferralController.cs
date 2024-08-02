using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Identity.Api.Controllers;

using Common.Core.Controllers;
using Requests;

/// <summary>
/// UserReferral controller
/// </summary>
public class UserReferralController : BaseController
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="mediator">Mediator</param>
    public UserReferralController(IMediator mediator) : base(mediator) { }

    /// <summary>
    /// Create
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("Create"), Authorize]
    public async Task<IActionResult> Create([FromBody] UserReferralCreateR request)
    {
        request.Analyze(HttpContext);
        var response = await _mediator.Send(request);
        return Ok(response);
    }

    /// <summary>
    /// Search
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPatch("Search")]
    public async Task<IActionResult> Search([FromBody] UserReferralSearchR request)
    {
        request.Analyze(HttpContext);
        var response = await _mediator.Send(request);
        return Ok(response);
    }

    #endregion
}
