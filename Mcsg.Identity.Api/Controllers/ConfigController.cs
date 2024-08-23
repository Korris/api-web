using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text;

namespace Mcsg.Identity.Api.Controllers;

using Common.Core.Interfaces;
using Common.Domain;
using Common.SeedWork.Extensions;
using Common.SeedWork.Responses;
using Interfaces;
using static Common.SeedWork.Constants.Validator;

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
    /// <param name="context">DB context</param>
    /// <param name="setting">Setting</param>
    /// <param name="sc">Storage client</param>
    public ConfigController(IMcsgContext context, ISetting setting, IStorageClient sc)
    {
        _context = context;
        _setting = setting;
        _sc = sc;
    }

    /// <summary>
    /// Get
    /// </summary>
    /// <returns>Return the result</returns>
    [HttpGet]
    [ProducesResponseType(typeof(SingleResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> Get()
    {
        var res = new MultipleResponse();

        var s = _setting;
        res.SetSuccess(nameof(s.DevMode).ToCamelCase(), s.DevMode);
        res.SetSuccess(nameof(s.IsLocal).ToCamelCase(), s.IsLocal);
        res.SetSuccess(nameof(s.Environment).ToCamelCase(), s.Environment);
        res.SetSuccess(nameof(s.IsProduction).ToCamelCase(), s.IsProduction);
        res.SetSuccess(nameof(DateTime.UtcNow).ToCamelCase(), DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss"));

        res.SetSuccess(nameof(s.Api).ToCamelCase(), s.Api);

        res.SetSuccess(nameof(s.AccountDeletedAfter).ToCamelCase(), s.AccountDeletedAfter);
        res.SetSuccess(nameof(s.AccountCreatedAfter).ToCamelCase(), s.AccountCreatedAfter);

        var validators = new
        {
            ProfileName = new
            {
                ProfileName.Min,
                ProfileName.Max,
                ProfileName.Regex
            },
            UserNameFree = new
            {
                UserNameFree.Min,
                UserNameFree.Max,
                UserNameFree.Regex
            },
            UserNamePremium = new
            {
                UserNamePremium.Min,
                UserNamePremium.Max,
                UserNamePremium.Regex
            },
            Location = new
            {
                Location.Max,
                Location.Regex
            },
            Summary = new
            {
                Summary.Max
            },
            Title = new
            {
                Title.Max
            }
        };
        res.SetSuccess(nameof(validators), validators);

        try
        {
            var file = "config/validators.json";
            var ms = await _sc.Strategy.GetObject(file, null);
            var jsonFile = new StringBuilder(StreamExtension.ToString(ms)).ToString();
            res.SetSuccess(nameof(jsonFile), jsonFile);
        }
        catch { }

        #region -- SystemSettings --
        var dic = _context.SystemSettings.Where(p => !string.IsNullOrWhiteSpace(p.Key)).ToDictionary(p => p.Key + "", p => p.Value + "");
        string[] keys = {
            "MaintenanceFrDate",
            "MaintenanceToDate"
        };

        foreach (var key in keys)
        {
            if (!dic.TryGetValue(key, out var val))
            {
                continue;
            }

            res.SetSuccess(key.ToCamelCase(), val);
        }
        #endregion

        return Ok(res.Data);
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// DB context
    /// </summary>
    private readonly IMcsgContext _context;

    /// <summary>
    /// Setting
    /// </summary>
    private readonly ISetting _setting;

    /// <summary>
    /// Storage client
    /// </summary>
    private readonly IStorageClient _sc;

    #endregion
}