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
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;

namespace Mcsg.Common.Core.Requests;

using Enums;
using Extensions;
using SeedWork.Constants;
using SeedWork.Enums;
using SeedWork.Extensions;
using SeedWork.Responses;
using static SeedWork.Constants.Setting;

/// <summary>
/// Base request
/// </summary>
public class BaseR : IRequest<SingleResponse>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public BaseR() { }

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="hc">HTTP context</param>
    public BaseR(HttpContext? hc)
    {
        _hc = hc;

        LogHeader();
    }

    /// <summary>
    /// Analyze
    /// </summary>
    /// <param name="hc">HTTP context</param>
    public void Analyze(HttpContext? hc)
    {
        _hc = hc;

        LogHeader();
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

    /// <summary>
    /// Set the cookie
    /// </summary>
    /// <param name="key">Key (unique indentifier)</param>
    /// <param name="value">Value to store in cookie object</param>
    /// <param name="timeout">Timeout</param>
    /// <param name="type">Time type</param>
    /// <param name="httpOnly">true if a cookie must not be accessible by client-side script; otherwise, false</param>
    public void SetCookie(string key, string value, double timeout, TimeType type, bool httpOnly)
    {
        if (_hc == null)
        {
            return;
        }

        if (timeout < 0)
        {
            timeout = 1;
        }

        TimeSpan? age = null;
        if (type == TimeType.Minute)
        {
            age = TimeSpan.FromMinutes(timeout);
        }
        else if (type == TimeType.Hour)
        {
            age = TimeSpan.FromHours(timeout);
        }
        else if (type == TimeType.Day)
        {
            age = TimeSpan.FromDays(timeout);
        }

        var option = new CookieOptions { HttpOnly = httpOnly, MaxAge = age };
        _hc.Response.Cookies.Append(key, value, option);
    }

    /// <summary>
    /// Delete the cookie
    /// </summary>
    /// <param name="key">Key (unique indentifier)</param>
    public void DelCookie(string key)
    {
        if (_hc == null)
        {
            return;
        }

        _hc.Response.Cookies.Delete(key);
    }

    /// <summary>
    /// Log header
    /// </summary>
    protected void LogHeader()
    {
        if (_hc == null)
        {
            return;
        }

        var dic = _hc.Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
        var json = JsonConvert.SerializeObject(dic, Formatting.Indented);

        // Debug only: ~25 lines per request and it contains Authorization / x-api-key, which must not sit in pod logs
        $"HTTP headers: {json}".LogDebug();
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
    public bool IsPremium => Payload?.RootElement.GetProperty("isPremium").GetBoolean() == true || IsAdministrator;

    /// <summary>
    /// IsLogin2Fa
    /// </summary>
    [SwaggerSchema(ReadOnly = true)]
    public bool IsLogin2Fa => Payload?.RootElement.GetProperty("isLogin2Fa").GetBoolean() ?? false;

    /// <summary>
    /// IsTransaction2Fa
    /// </summary>
    [SwaggerSchema(ReadOnly = true)]
    public bool IsTransaction2Fa => Payload?.RootElement.GetProperty("isTransaction2Fa").GetBoolean() ?? false;

    /// <summary>
    /// User MinioInstance
    /// </summary>
    [SwaggerSchema(ReadOnly = true)]
    public MinioInstanceType MinioInstance => (MinioInstanceType?)Payload?.RootElement.GetProperty("minioInstance").GetInt32() ?? MinioInstanceType.Default;

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
    /// Platform
    /// </summary>
    [SwaggerSchema(ReadOnly = true)]
    public string Platform => FromAndroid ? "Android" : FromIos ? "iOS" : "Web";

    /// <summary>
    /// Hides
    /// </summary>
    public List<int> Hides
    {
        get
        {
            var res = new List<HideOption>();

            if (IsAdministrator)
            {
                return [];
            }

            if (FromAndroid)
            {
                res.Add(HideOption.Android);
                res.Add(HideOption.IosAndroid);
                res.Add(HideOption.AndroidWeb);
            }
            else if (FromIos)
            {
                res.Add(HideOption.Ios);
                res.Add(HideOption.IosAndroid);
                res.Add(HideOption.IosWeb);
            }
            else
            {
                res.Add(HideOption.Web);
                res.Add(HideOption.IosWeb);
                res.Add(HideOption.AndroidWeb);
            }

            res.Add(HideOption.All);

            return res.Select(p => (int)p).ToList();
        }
    }

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
                var t = headers[key].ToString().Split(',', StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).Distinct();
                return string.Join(",", t);
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
    /// Is localhost
    /// </summary>
    [SwaggerSchema(ReadOnly = true)]
    public bool IsLocalhost
    {
        get
        {
            var headers = _hc?.Request.Headers;

            var key = nameof(IsLocalhost);
            if (headers != null && headers.ContainsKey(key))
            {
                return Convert.ToBoolean(headers[key]);
            }

            return false;
        }
    }

    /// <summary>
    /// Is logged in
    /// </summary>
    [SwaggerSchema(ReadOnly = true)]
    public bool IsLoggedIn
    {
        get
        {
            var headers = _hc?.Request.Headers;

            var key = nameof(IsLoggedIn);
            if (headers != null && headers.ContainsKey(key))
            {
                return Convert.ToBoolean(headers[key]);
            }

            return false;
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
    /// Is Administrator (IsRoleSysAdmin || IsRoleAdmin || IsRoleContentAdmin)
    /// </summary>
    [SwaggerSchema(ReadOnly = true)]
    public bool IsAdministrator => IsRoleSysAdmin || IsRoleAdmin || IsRoleContentAdmin;

    /// <summary>
    /// Is role SysAdmin
    /// </summary>
    [SwaggerSchema(ReadOnly = true)]
    public bool IsRoleSysAdmin => _hc?.User?.IsInRole(RoleName.SysAdmin) == true;

    /// <summary>
    /// Is role Admin
    /// </summary>
    [SwaggerSchema(ReadOnly = true)]
    public bool IsRoleAdmin => _hc?.User?.IsInRole(RoleName.Admin) == true;

    /// <summary>
    /// Is role ContentAdmin
    /// </summary>
    [SwaggerSchema(ReadOnly = true)]
    public bool IsRoleContentAdmin => _hc?.User?.IsInRole(RoleName.ContentAdmin) == true;

    /// <summary>
    /// Is role User
    /// </summary>
    [SwaggerSchema(ReadOnly = true)]
    public bool IsRoleUser => _hc?.User?.IsInRole(RoleName.User) == true;

    /// <summary>
    /// UT mode (automatically rollback data when the test is done)
    /// </summary>
    [JsonIgnore]
    [SwaggerSchema(ReadOnly = true)]
    public bool UtMode { get; set; }

    /// <summary>
    /// MicroService
    /// </summary>
    [JsonIgnore]
    [SwaggerSchema(ReadOnly = true)]
    public string MicroService { get; set; } = Enums.MicroService.Social.ToString();

    /// <summary>
    /// Old Client ID in Cookies or Headers
    /// </summary>
    public Guid ClientId
    {
        get
        {
            var key = nameof(ClientId);
            var t = _hc?.Request.Cookies[key];
            t ??= _hc?.Request.Headers[key];
            return t.ToGuid();
        }
    }

    /// <summary>
    /// Old Session ID in Cookies or Headers
    /// </summary>
    public Guid SessionId
    {
        get
        {
            var key = nameof(SessionId);
            var t = _hc?.Request.Cookies[key];
            t ??= _hc?.Request.Headers[key];
            return t.ToGuid();
        }
    }

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
