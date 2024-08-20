#region Information
/*
 * Author       : Toan Nguyen Van
 * Email        : nvt87x@gmail.com
 * Phone        : +84 345 515 010
 * ------------------------------- *
 * Create       : 2024-Jan-21 07:03
 * Update       : 2024-Jan-21 07:03
 * Checklist    : 1.0
 * Status       : New
 */
#endregion

namespace Mcsg.Common.SeedWork;

using Dtos;
using Interfaces;
using static Dtos.ConnectionDto;
using static SeedWork.Dtos.StorageDto;

/// <summary>
/// Setting base
/// </summary>
public class SettingBase : ISettingBase
{
    #region -- Implements --

    /// <summary>
    /// Project prefix
    /// </summary>
    public string Prefix { get; set; }

    /// <summary>
    /// Environment (local, dev, stg and pro)
    /// </summary>
    public string Environment { get; set; }

    /// <summary>
    /// Site name
    /// </summary>
    public string SiteName { get; set; }

    /// <summary>
    /// For detecting mobile to call API
    /// </summary>
    public string MobileUserAgent { get; set; }

    /// <summary>
    /// Domain
    /// </summary>
    public string Domain { get; set; }

    /// <summary>
    /// Swagger enabled
    /// </summary>
    public bool SwaggerEnabled { get; set; }

    /// <summary>
    /// Development mode
    /// </summary>
    public bool DevMode { get; set; }

    /// <summary>
    /// Protocols (An example such as 'Http1_80;Http2_81' means that Http1 runs on port 80 and Http2 runs on port 81)
    /// </summary>
    public string? Protocols { get; set; }

    /// <summary>
    /// Information
    /// </summary>
    public string? Information
    {
        get
        {
            return !IsProduction ? $"{Environment.ToUpper()}" : null;
        }
    }

    /// <summary>
    /// Is production mode
    /// </summary>
    public bool IsProduction
    {
        get
        {
            var t = Environment.ToLower();
            return t == "pro" || t == "production";
        }
    }

    /// <summary>
    /// Is local mode
    /// </summary>
    public bool IsLocal => Environment.Equals("local", StringComparison.CurrentCultureIgnoreCase);

    /// <summary>
    /// JSON Web Token
    /// </summary>
    public JwtDto Jwt { get; }

    /// <summary>
    /// Database
    /// </summary>
    public DatabaseDto Db { get; }

    /// <summary>
    /// Email
    /// </summary>
    public NotificationDto Email { get; }

    /// <summary>
    /// Queue
    /// </summary>
    public QueueDto Queue { get; }

    /// <summary>
    /// MinIO
    /// </summary>
    public MinioDto Minio { get; }

    /// <summary>
    /// API
    /// </summary>
    public ApiDto Api { get; }

    /// <summary>
    /// The origins that are allowed (CORS)
    /// </summary>
    public string? Origins { get; set; }

    /// <summary>
    /// Encrypt key
    /// </summary>
    public string EncryptKey { get; set; }

    #endregion

    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public SettingBase()
    {
        Prefix = string.Empty;
        Environment = string.Empty;
        SiteName = "Bumcheo";
        MobileUserAgent = string.Empty;
        Domain = string.Empty;

        Jwt = new JwtDto();
        Db = new DatabaseDto();
        Queue = new QueueDto();
        Email = new NotificationDto();
        Minio = new MinioDto();
        Api = new ApiDto();

        EncryptKey = string.Empty;
    }

    /// <summary>
    /// Load API URL
    /// </summary>
    /// <param name="dic">Dictionary</param>
    public void LoadApiUrl(Dictionary<string, string>? dic)
    {
        if (dic == null)
        {
            return;
        }

        #region -- Api.Admin --
        Api.Admin.Analytic = dic["HostAnalyticAdmin"];
        Api.Admin.Comic = dic["HostComicAdmin"];
        Api.Admin.Identity = dic["HostIdentityAdmin"];
        Api.Admin.Social = dic["HostSocialAdmin"];
        Api.Admin.Story = dic["HostStoryAdmin"];
        #endregion

        #region -- Api.Mobile --
        Api.Mobile.Comic = dic["HostComicMobile"];
        Api.Mobile.Identity = dic["HostIdentityMobile"];
        Api.Mobile.Social = dic["HostSocialMobile"];
        Api.Mobile.Story = dic["HostStoryMobile"];
        #endregion

        #region -- Api.Web --
        Api.Web.Comic = dic["HostComic"];
        Api.Web.Identity = dic["HostIdentity"];
        Api.Web.Media = dic["HostMedia"];
        Api.Web.Realtime = dic["HostRealtime"];
        Api.Web.Social = dic["HostSocial"];
        Api.Web.Story = dic["HostStory"];
        Api.Web.Wallet = dic["HostWallet"];
        #endregion
    }

    #endregion
}
