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
using Enums;
using Interfaces;
using static Dtos.ConnectionDto;
using static Dtos.StorageDto;

/// <summary>
/// Setting base
/// </summary>
public class SettingBase : ISettingBase
{
    #region -- Implements --

    /// <summary>
    /// Retrieves a MinIO instance based on the specified instance type.
    /// </summary>
    /// <param name="instance">The type of the MinIO instance to retrieve.</param>
    /// <returns>The corresponding <see cref="MinioInstanceDto"/> object.</returns>
    public MinioInstanceDto GetMinio(MinioInstanceType instance)
    {
        return Minio.Storages[(int)instance];
    }

    /// <summary>
    /// Project prefix
    /// </summary>
    public string Prefix { get; set; }

    /// <summary>
    /// Environment (local, dev, uat, stg and pro)
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
    /// Storage
    /// </summary>
    public string? Storage { get; set; }

    /// <summary>
    /// API
    /// </summary>
    public ApiDto Api { get; }

    /// <summary>
    /// RPC
    /// </summary>
    public ApiDto Rpc { get; }

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
        Rpc = new ApiDto();

        EncryptKey = string.Empty;
    }

    /// <summary>
    /// Load API URL
    /// </summary>
    /// <param name="dic">Dictionary</param>
    /// <param name="isLocal">Is local</param>
    /// <param name="hasProtocol">Has protocol</param>
    public void LoadApiUrl(Dictionary<string, string>? dic, bool isLocal, bool hasProtocol)
    {
        if (isLocal)
        {
            dic = hasProtocol ? _hostDicHttp1 : _hostDicHttps;
        }

        if (dic == null)
        {
            return;
        }

        string? val;

        #region -- Api.Admin --
        if (dic.TryGetValue("HostAnalyticAdmin", out val)) Api.Admin.Analytic = val;
        if (dic.TryGetValue("HostComicAdmin", out val)) Api.Admin.Comic = val;
        if (dic.TryGetValue("HostIdentityAdmin", out val)) Api.Admin.Identity = val;
        if (dic.TryGetValue("HostSocialAdmin", out val)) Api.Admin.Social = val;
        if (dic.TryGetValue("HostStoryAdmin", out val)) Api.Admin.Story = val;
        if (dic.TryGetValue("HostSyncAdmin", out val)) Api.Admin.Sync = val;
        if (dic.TryGetValue("HostCloneSiteAdmin", out val)) Api.Admin.CloneSite = val;
        #endregion

        #region -- Api.Mobile --
        if (dic.TryGetValue("HostComicMobile", out val)) Api.Mobile.Comic = val;
        if (dic.TryGetValue("HostIdentityMobile", out val)) Api.Mobile.Identity = val;
        if (dic.TryGetValue("HostSocialMobile", out val)) Api.Mobile.Social = val;
        if (dic.TryGetValue("HostStoryMobile", out val)) Api.Mobile.Story = val;
        #endregion

        #region -- Api.Web --
        if (dic.TryGetValue("HostComic", out val)) Api.Web.Comic = val;
        if (dic.TryGetValue("HostIdentity", out val)) Api.Web.Identity = val;
        if (dic.TryGetValue("HostMedia", out val)) Api.Web.Media = val;
        if (dic.TryGetValue("HostRealtime", out val)) Api.Web.Realtime = val;
        if (dic.TryGetValue("HostSocial", out val)) Api.Web.Social = val;
        if (dic.TryGetValue("HostStory", out val)) Api.Web.Story = val;
        if (dic.TryGetValue("HostWallet", out val)) Api.Web.Wallet = val;
        #endregion
    }

    /// <summary>
    /// Load RPC URL
    /// </summary>
    /// <param name="dic">Dictionary</param>
    /// <param name="isLocal">Is local</param>
    public void LoadRpcUrl(Dictionary<string, string>? dic, bool isLocal)
    {
        if (isLocal)
        {
            dic = _hostDicHttp2;
        }

        if (dic == null)
        {
            return;
        }

        string? val;

        #region -- Rpc.Admin --
        if (dic.TryGetValue("RpcAnalyticAdmin", out val)) Rpc.Admin.Analytic = val;
        if (dic.TryGetValue("RpcComicAdmin", out val)) Rpc.Admin.Comic = val;
        if (dic.TryGetValue("RpcIdentityAdmin", out val)) Rpc.Admin.Identity = val;
        if (dic.TryGetValue("RpcSocialAdmin", out val)) Rpc.Admin.Social = val;
        if (dic.TryGetValue("RpcStoryAdmin", out val)) Rpc.Admin.Story = val;
        if (dic.TryGetValue("RpcSyncAdmin", out val)) Rpc.Admin.Sync = val;
        #endregion

        #region -- Rpc.Mobile --
        if (dic.TryGetValue("RpcComicMobile", out val)) Rpc.Mobile.Comic = val;
        if (dic.TryGetValue("RpcIdentityMobile", out val)) Rpc.Mobile.Identity = val;
        if (dic.TryGetValue("RpcSocialMobile", out val)) Rpc.Mobile.Social = val;
        if (dic.TryGetValue("RpcStoryMobile", out val)) Rpc.Mobile.Story = val;
        #endregion

        #region -- Rpc.Web --
        if (dic.TryGetValue("RpcComic", out val)) Rpc.Web.Comic = val;
        if (dic.TryGetValue("RpcIdentity", out val)) Rpc.Web.Identity = val;
        if (dic.TryGetValue("RpcMedia", out val)) Rpc.Web.Media = val;
        if (dic.TryGetValue("RpcRealtime", out val)) Rpc.Web.Realtime = val;
        if (dic.TryGetValue("RpcSocial", out val)) Rpc.Web.Social = val;
        if (dic.TryGetValue("RpcStory", out val)) Rpc.Web.Story = val;
        if (dic.TryGetValue("RpcWallet", out val)) Rpc.Web.Wallet = val;
        #endregion
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// Host dictionary
    /// </summary>
    private readonly Dictionary<string, string> _hostDicHttps = new()
    {
        { "HostComic", "https://localhost:44303" },
        { "HostIdentity", "https://localhost:44305" },
        { "HostMedia", "https://localhost:44306" },
        { "HostRealtime", "https://localhost:44307" },
        { "HostSocial", "https://localhost:44308" },
        { "HostStory", "https://localhost:44309" },
        { "HostWallet", "https://localhost:44310" },

        { "HostComicMobile", "https://localhost:44311" },
        { "HostIdentityMobile", "https://localhost:44312" },
        { "HostSocialMobile", "https://localhost:44313" },
        { "HostStoryMobile", "https://localhost:44314" },

        { "HostAnalyticAdmin", "https://localhost:44302" },
        { "HostComicAdmin", "https://localhost:44315" },
        { "HostIdentityAdmin", "https://localhost:44316" },
        { "HostSocialAdmin", "https://localhost:44317" },
        { "HostStoryAdmin", "https://localhost:44318" },
        { "HostSyncAdmin", "https://localhost:44319" },
        { "HostCloneSiteAdmin", "https://localhost:44320" }
    };

    /// <summary>
    /// Host dictionary (Http1)
    /// </summary>
    private readonly Dictionary<string, string> _hostDicHttp1 = new()
    {
        { "HostComic", "http://localhost:54103" },
        { "HostIdentity", "http://localhost:54105" },
        { "HostMedia", "http://localhost:54106" },
        { "HostRealtime", "http://localhost:54107" },
        { "HostSocial", "http://localhost:54108" },
        { "HostStory", "http://localhost:54109" },
        { "HostWallet", "http://localhost:54110" },

        { "HostComicMobile", "http://localhost:54111" },
        { "HostIdentityMobile", "http://localhost:54112" },
        { "HostSocialMobile", "http://localhost:54113" },
        { "HostStoryMobile", "http://localhost:54114" },

        { "HostAnalyticAdmin", "http://localhost:54102" },
        { "HostComicAdmin", "http://localhost:54115" },
        { "HostIdentityAdmin", "http://localhost:54116" },
        { "HostSocialAdmin", "http://localhost:54117" },
        { "HostStoryAdmin", "http://localhost:54118" },
        { "HostSyncAdmin", "http://localhost:54119" },
        { "HostCloneSiteAdmin", "http://localhost:54120" }
    };

    /// <summary>
    /// Rpc dictionary (Http2)
    /// </summary>
    private readonly Dictionary<string, string> _hostDicHttp2 = new()
    {
        { "RpcComic", "http://localhost:54203" },
        { "RpcIdentity", "http://localhost:54205" },
        { "RpcMedia", "http://localhost:54206" },
        { "RpcRealtime", "http://localhost:54207" },
        { "RpcSocial", "http://localhost:54208" },
        { "RpcStory", "http://localhost:54209" },
        { "RpcWallet", "http://localhost:54210" },

        { "RpcComicMobile", "http://localhost:54211" },
        { "RpcIdentityMobile", "http://localhost:54212" },
        { "RpcSocialMobile", "http://localhost:54213" },
        { "RpcStoryMobile", "http://localhost:54214" },

        { "RpcAnalyticAdmin", "http://localhost:54202" },
        { "RpcComicAdmin", "http://localhost:54215" },
        { "RpcIdentityAdmin", "http://localhost:54216" },
        { "RpcSocialAdmin", "http://localhost:54217" },
        { "RpcStoryAdmin", "http://localhost:54218" },
        { "RpcSyncAdmin", "http://localhost:54219" }
    };

    #endregion
}
