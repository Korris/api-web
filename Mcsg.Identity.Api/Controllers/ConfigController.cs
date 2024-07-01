using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Mcsg.Identity.Api.Controllers;

using Common.SeedWork.Extensions;
using Common.SeedWork.Responses;
using Interfaces;

/// <summary>
/// Config controller
/// </summary>
[ApiController]
[Route("[controller]")]
public class ConfigController : ControllerBase
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="setting">Setting</param>
    public ConfigController(ISetting setting)
    {
        _setting = setting;
    }

    /// <summary>
    /// Get
    /// </summary>
    /// <returns>Return the result</returns>
    [HttpGet]
    [ProducesResponseType(typeof(SingleResponse), (int)HttpStatusCode.OK)]
    public IActionResult Get()
    {
        var res = new MultipleResponse();

        var s = _setting;
        res.SetSuccess(nameof(s.DevMode).ToCamelCase(), s.DevMode);
        res.SetSuccess(nameof(s.IsLocal).ToCamelCase(), s.IsLocal);
        res.SetSuccess(nameof(s.Environment).ToCamelCase(), s.Environment);
        res.SetSuccess(nameof(s.IsProduction).ToCamelCase(), s.IsProduction);
        res.SetSuccess(nameof(DateTime.UtcNow).ToCamelCase(), DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss"));

        return Ok(res.Data);
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// Setting
    /// </summary>
    private readonly ISetting _setting;

    #endregion
}