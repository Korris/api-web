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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Mcsg.Common.Core.Requests;

using Common.SeedWork.Constants;
using Common.SeedWork.Extensions;
using Common.SeedWork.Responses;
using static Common.Core.Constants.Setting;
using static Common.SeedWork.Constants.Setting.Role;

/// <summary>
/// Base request
/// </summary>
public abstract class BaseR : IRequest<SingleResponse>
{
    #region -- Methods --

    /// <summary>
    /// Analyze
    /// </summary>
    /// <param name="hc">HTTP context</param>
    public void Analyze(HttpContext hc)
    {
        _hc = hc;
    }

    /// <summary>
    /// Detect mobile call API
    /// </summary>
    /// <param name="agent">For detecting mobile to call API</param>
    public void DetectMobileCall(string agent)
    {
        var userAgent = UserAgent;

        if (string.IsNullOrWhiteSpace(agent) || string.IsNullOrWhiteSpace(userAgent))
        {
            return;
        }

        var arr = agent.Split(';', StringSplitOptions.RemoveEmptyEntries);

        foreach (var i in arr)
        {
            FromMobile = userAgent.Contains(i);

            if (FromMobile)
            {
                break;
            }
        }
    }

    /// <summary>
    /// Get absolute URI
    /// </summary>
    public string GetAbsoluteUri(string domain)
    {
        if (_hc == null)
        {
            return string.Empty;
        }

        var request = _hc.Request;
        if (string.IsNullOrWhiteSpace(domain))
        {
            return request.GetDisplayUrl();
        }

        string? rewriteUrl;

        var key = "X-Original-URL";
        if (request.Headers.ContainsKey(key))
        {
            rewriteUrl = request.Headers[key].ToString();
        }
        else
        {
            var path = request.Path.ToUriComponent();
            var query = request.QueryString.ToUriComponent();
            rewriteUrl = string.Concat(path, query);
        }

        return domain + rewriteUrl;
    }

    #endregion

    #region -- Properties --

    /// <summary>
    /// Current UserName logged in
    /// </summary>
    [SwaggerSchema(ReadOnly = true)]
    public string? CurrentUserName => _hc?.User.Identity?.Name;

    /// <summary>
    /// Current UserId logged in
    /// </summary>
    [SwaggerSchema(ReadOnly = true)]
    public ulong? CurrentUserId => Payload?.RootElement.GetProperty("id").GetUInt64();

    /// <summary>
    /// The folder name is stored in MinIO
    /// </summary>
    [SwaggerSchema(ReadOnly = true)]
    public string Folder
    {
        get
        {
            var id = CurrentUserId == null ? 0 : CurrentUserId.Value;
            var res = id.ToSerialNumber("U");
            return $"{FolderMinIO.User}/{res}";
        }
    }

    /// <summary>
    /// UserAgent
    /// </summary>
    [SwaggerSchema(ReadOnly = true)]
    public string? UserAgent
    {
        get
        {
            var headers = _hc?.Request.Headers;

            var key = "User-Agent";
            if (headers != null && headers.ContainsKey(key))
            {
                return headers[key].ToString();
            }

            return null;
        }
    }

    /// <summary>
    /// From mobile
    /// </summary>
    [SwaggerSchema(ReadOnly = true)]
    public bool FromMobile { get; private set; }

    /// <summary>
    /// Remote IP address
    /// </summary>
    [SwaggerSchema(ReadOnly = true)]
    public string? RemoteIp
    {
        get
        {
            var headers = _hc?.Request.Headers;

            var key = "X-Forwarded-For";
            if (headers != null && headers.ContainsKey(key))
            {
                return headers[key].ToString();
            }
            else
            {
                var t = _hc?.Connection.RemoteIpAddress;
                if (t != null)
                {
                    return t.MapToIPv4().ToString();
                }
            }

            return null;
        }
    }

    /// <summary>
    /// Timezone offset (minute)
    /// </summary>
    [SwaggerSchema(ReadOnly = true)]
    public int TimezoneOffset
    {
        get
        {
            var headers = _hc?.Request.Headers;

            var key = nameof(TimezoneOffset);
            if (headers != null && headers.ContainsKey(key))
            {
                return Convert.ToInt32(headers[key]);
            }

            return 0;
        }
    }

    /// <summary>
    /// Action time
    /// </summary>
    [SwaggerSchema(ReadOnly = true)]
    public DateTime ActionTime => DateTime.UtcNow.AddMinutes(-TimezoneOffset);

    /// <summary>
    /// Timezone
    /// </summary>
    [SwaggerSchema(ReadOnly = true)]
    public string Timezone
    {
        get
        {
            var t = TimeSpan.FromMinutes(TimezoneOffset);
            var sign = TimezoneOffset < 0 ? "+" : "-";
            return sign + t.ToString("hh':'mm");
        }
    }

    /// <summary>
    /// Is role admin
    /// </summary>
    [SwaggerSchema(ReadOnly = true)]
    public bool IsRoleAdmin => _hc?.User?.IsInRole(SuperAdmin) == true || _hc?.User?.IsInRole(Admin) == true;

    /// <summary>
    /// Is role tenant
    /// </summary>
    [SwaggerSchema(ReadOnly = true)]
    public bool IsRoleTenant => _hc?.User?.IsInRole(SuperTenant) == true || _hc?.User?.IsInRole(Tenant) == true;

    /// <summary>
    /// Is role customer
    /// </summary>
    [SwaggerSchema(ReadOnly = true)]
    public bool IsRoleCustomer => _hc?.User?.IsInRole(Customer) == true;

    /// <summary>
    /// UT mode (automatically rollback data when the test is done)
    /// </summary>
    [JsonIgnore]
    [SwaggerSchema(ReadOnly = true)]
    public bool UtMode { get; set; }

    /// <summary>
    /// Payload
    /// </summary>
    private JsonDocument? Payload
    {
        get
        {
            var user = _hc?.User;
            var payload = user?.Claims.Where(p => p.Type == Setting.Payload).Select(p => p.Value).FirstOrDefault();
            if (payload != null)
            {
                return JsonDocument.Parse(payload);
            }

            return null;
        }
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// HTTP context
    /// </summary>
    protected HttpContext? _hc;

    #endregion
}
