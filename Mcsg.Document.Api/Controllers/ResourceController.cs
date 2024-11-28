using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Document.Api.Controllers;

using Common.Core.Controllers;
using Interfaces;
using Requests;

/// <summary>
/// Resource controller
/// </summary>
public class ResourceController : BaseController
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="mediator">Mediator</param>
    /// <param name="setting">Setting</param>
    public ResourceController(IMediator mediator, ISetting setting) : base(mediator)
    {
        _setting = setting;
        DomainName = _setting.Domain;
    }

    /// <summary>
    /// View
    /// </summary>
    /// <param name="request"></param>
    /// <returns>Return the result</returns>
    [HttpPatch("View"), Authorize]
    public async Task<IActionResult> View([FromBody] ResourceViewR request)
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
