using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

using Mcsg.Api.Interfaces;
namespace Mcsg.Api.Areas.Document.Controllers;

using Common.Core.Controllers;
using Common.SeedWork.Responses;
using Mcsg.Api.Areas.Document.Interfaces;
using Mcsg.Api.Areas.Document.Requests;

/// <summary>
/// Report controller
/// </summary>
public class ReportController : BaseController
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="mediator">Mediator</param>
    /// <param name="setting">Setting</param>
    public ReportController(IMediator mediator, ISetting setting) : base(mediator)
    {
        _setting = setting;
        DomainName = _setting.Domain;
    }

    /// <summary>
    /// Create
    /// </summary>
    /// <returns>Return the result</returns>
    [HttpPost("Create"), Authorize]
    [ProducesResponseType(typeof(SingleResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> Create([FromBody] ReportCreateR request)
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
