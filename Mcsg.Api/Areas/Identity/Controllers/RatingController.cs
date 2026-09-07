using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Mcsg.Api.Areas.Identity.Controllers;

using Common.Core.Controllers;
using Common.SeedWork.Responses;
using Mcsg.Api.Areas.Identity.Interfaces;
using Mcsg.Api.Interfaces;
using Requests;

/// <summary>
/// Rating controller
/// </summary>
[ApiController]
[Route("api/identity/[controller]")]
public class RatingController : BaseController
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="mediator">Mediator</param>
    /// <param name="setting">Setting</param>
    public RatingController(IMediator mediator, ISetting setting) : base(mediator)
    {
        _setting = setting;
        DomainName = _setting.Domain;
    }

    /// <summary>
    /// Create
    /// </summary>
    /// <returns>Return the result</returns>
    [HttpPost("Create")]
    [ProducesResponseType(typeof(SingleResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> Create([FromBody] RatingCreateR request)
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
