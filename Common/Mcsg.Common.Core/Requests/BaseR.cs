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
using Common.SeedWork.Responses;
using static Common.SeedWork.Constants.Setting;

/// <summary>
/// Base request
/// </summary>
public class BaseR : IRequest<SingleResponse>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public BaseR()
    {
    }

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="hc">HTTP context</param>
    public BaseR(HttpContext hc)
    {
        _hc = hc;
    }

    /// <summary>
    /// Analyze
    /// </summary>
    /// <param name="hc">HTTP context</param>
    public void Analyze(HttpContext hc)
    {
        _hc = hc;
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
    /// UserName logged in
    /// </summary>
    [SwaggerSchema(ReadOnly = true)]
    public string? UserName => _hc?.User.Identity?.Name;

    /// <summary>
    /// UserId logged in
    /// </summary>
    [SwaggerSchema(ReadOnly = true)]
    public Guid? UserId => Payload?.RootElement.GetProperty("id").GetGuid();

    /// <summary>
    /// ProfileName logged in
    /// </summary>
    [SwaggerSchema(ReadOnly = true)]
    public string? ProfileName => Payload?.RootElement.GetProperty("profileName").GetString();

    /// <summary>
    /// ProfileId logged in
    /// </summary>
    [SwaggerSchema(ReadOnly = true)]
    public string? ProfileId => Payload?.RootElement.GetProperty("profileId").GetString();

    /// <summary>
    /// UserFolder logged in
    /// </summary>
    [SwaggerSchema(ReadOnly = true)]
    public string? UserFolder => Payload?.RootElement.GetProperty("userFolder").GetString();

    /// <summary>
    /// UserAvatar logged in
    /// </summary>
    [SwaggerSchema(ReadOnly = true)]
    public string? UserAvatar => Payload?.RootElement.GetProperty("userAvatar").GetString();

    /// <summary>
    /// User IsPremium
    /// </summary>
    [SwaggerSchema(ReadOnly = true)]
    public bool? IsPremium => Payload?.RootElement.GetProperty("isPremium").GetBoolean();

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
    public bool FromMobile => FromIos || FromAndroid;

    /// <summary>
    /// From iOS
    /// </summary>
    [SwaggerSchema(ReadOnly = true)]
    public bool FromIos => "ios".Equals(DeviceType, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// From Android
    /// </summary>
    [SwaggerSchema(ReadOnly = true)]
    public bool FromAndroid => "android".Equals(DeviceType, StringComparison.OrdinalIgnoreCase);

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
    /// Device type
    /// </summary>
    [SwaggerSchema(ReadOnly = true)]
    public string? DeviceType
    {
        get
        {
            var headers = _hc?.Request.Headers;

            var key = nameof(DeviceType);
            if (headers != null && headers.ContainsKey(key))
            {
                return headers[key];
            }

            return null;
        }
    }

    /// <summary>
    /// Origin address
    /// </summary>
    [SwaggerSchema(ReadOnly = true)]
    public string? OriginAddress
    {
        get
        {
            var headers = _hc?.Request.Headers;

            var key = "Origin";
            if (headers != null && headers.ContainsKey(key))
            {
                return headers[key].ToString();
            }

            return null;
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
    public bool IsRoleAdmin => _hc?.User?.IsInRole(McsgRole.SysAdmin) == true || _hc?.User?.IsInRole(McsgRole.Admin) == true || _hc?.User?.IsInRole(McsgRole.ContentAdmin) == true;

    /// <summary>
    /// Is role user
    /// </summary>
    [SwaggerSchema(ReadOnly = true)]
    public bool IsRoleUser => _hc?.User?.IsInRole(McsgRole.User) == true;

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
