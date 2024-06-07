#region Information
/*
 * Author       : Toan Nguyen Van
 * Email        : nvt87x@gmail.com
 * Phone        : +84 345 515 010
 * ------------------------------- *
 * Create       : 2024-Jan-21 08:37
 * Update       : 2024-Jan-21 08:37
 * Checklist    : 1.0
 * Status       : New
 */
#endregion

using MediatR;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Common.Core.Controllers;

/// <summary>
/// Base controller
/// </summary>
[ApiController]
[Route("v1/[controller]")]
[ApiExplorerSettings(GroupName = "v1")]
public abstract class BaseController : ControllerBase
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="mediator">Mediator</param>
    public BaseController(IMediator mediator)
    {
        _mediator = mediator;
    }

    #endregion

    #region -- Properties --

    /// <summary>
    /// Domain name
    /// </summary>
    protected string? DomainName { get; set; }

    /// <summary>
    /// Absolute URI
    /// </summary>
    protected string AbsoluteUri
    {
        get
        {
            if (string.IsNullOrWhiteSpace(DomainName))
            {
                return Request.GetDisplayUrl();
            }

            string? rewriteUrl;

            var key = "X-Original-URL";
            if (Request.Headers.ContainsKey(key))
            {
                rewriteUrl = Request.Headers[key].ToString();
            }
            else
            {
                var path = Request.Path.ToUriComponent();
                var query = Request.QueryString.ToUriComponent();
                rewriteUrl = string.Concat(path, query);
            }

            return DomainName + rewriteUrl;
        }
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// Mediator
    /// </summary>
    protected readonly IMediator _mediator;

    #endregion
}
