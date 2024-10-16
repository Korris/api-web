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
    /// OTP
    /// </summary>
    public OtpDto Otp { get; }

    /// <summary>
    /// ZaloPay
    /// </summary>
    public ZaloPayDto ZaloPay { get; }

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
        Otp = new OtpDto();
        ZaloPay = new ZaloPayDto();

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
        if (dic.TryGetValue("ApiAnalyticAdmin", out val)) Api.Admin.Analytic = val;
        if (dic.TryGetValue("ApiComicAdmin", out val)) Api.Admin.Comic = val;
        if (dic.TryGetValue("ApiIdentityAdmin", out val)) Api.Admin.Identity = val;
        if (dic.TryGetValue("ApiSocialAdmin", out val)) Api.Admin.Social = val;
        if (dic.TryGetValue("ApiStoryAdmin", out val)) Api.Admin.Story = val;
        if (dic.TryGetValue("ApiSyncAdmin", out val)) Api.Admin.Sync = val;
        if (dic.TryGetValue("ApiCloneSiteAdmin", out val)) Api.Admin.CloneSite = val;
        #endregion

        #region -- Api.Mobile --
        if (dic.TryGetValue("ApiComicMobile", out val)) Api.Mobile.Comic = val;
        if (dic.TryGetValue("ApiIdentityMobile", out val)) Api.Mobile.Identity = val;
        if (dic.TryGetValue("ApiSocialMobile", out val)) Api.Mobile.Social = val;
        if (dic.TryGetValue("ApiStoryMobile", out val)) Api.Mobile.Story = val;
        #endregion

        #region -- Api.Web --
        if (dic.TryGetValue("ApiComicWeb", out val)) Api.Web.Comic = val;
        if (dic.TryGetValue("ApiIdentityWeb", out val)) Api.Web.Identity = val;
        if (dic.TryGetValue("ApiMediaWeb", out val)) Api.Web.Media = val;
        if (dic.TryGetValue("ApiRealtimeWeb", out val)) Api.Web.Realtime = val;
        if (dic.TryGetValue("ApiSocialWeb", out val)) Api.Web.Social = val;
        if (dic.TryGetValue("ApiStoryWeb", out val)) Api.Web.Story = val;
        if (dic.TryGetValue("ApiWalletWeb", out val)) Api.Web.Wallet = val;
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
        if (dic.TryGetValue("RpcComicWeb", out val)) Rpc.Web.Comic = val;
        if (dic.TryGetValue("RpcIdentityWeb", out val)) Rpc.Web.Identity = val;
        if (dic.TryGetValue("RpcMediaWeb", out val)) Rpc.Web.Media = val;
        if (dic.TryGetValue("RpcRealtimeWeb", out val)) Rpc.Web.Realtime = val;
        if (dic.TryGetValue("RpcSocialWeb", out val)) Rpc.Web.Social = val;
        if (dic.TryGetValue("RpcStoryWeb", out val)) Rpc.Web.Story = val;
        if (dic.TryGetValue("RpcWalletWeb", out val)) Rpc.Web.Wallet = val;
        #endregion
    }

    #endregion

    #region -- Properties --

    /// <summary>
    /// X API key
    /// </summary>
    public static string? XApiKey { get; set; }

    /// <summary>
    /// Development mode
    /// </summary>
    public static bool DevelopmentMode { get; set; }

    #endregion

    #region -- Fields --

    /// <summary>
    /// Host dictionary
    /// </summary>
    private readonly Dictionary<string, string> _hostDicHttps = new()
    {
        { "ApiComicWeb", "https://localhost:44303" },
        { "ApiIdentityWeb", "https://localhost:44305" },
        { "ApiMediaWeb", "https://localhost:44306" },
        { "ApiRealtimeWeb", "https://localhost:44307" },
        { "ApiSocialWeb", "https://localhost:44308" },
        { "ApiStoryWeb", "https://localhost:44309" },
        { "ApiWalletWeb", "https://localhost:44310" },

        { "ApiComicMobile", "https://localhost:44311" },
        { "ApiIdentityMobile", "https://localhost:44312" },
        { "ApiSocialMobile", "https://localhost:44313" },
        { "ApiStoryMobile", "https://localhost:44314" },

        { "ApiAnalyticAdmin", "https://localhost:44302" },
        { "ApiComicAdmin", "https://localhost:44315" },
        { "ApiIdentityAdmin", "https://localhost:44316" },
        { "ApiSocialAdmin", "https://localhost:44317" },
        { "ApiStoryAdmin", "https://localhost:44318" },
        { "ApiSyncAdmin", "https://localhost:44319" },
        { "ApiCloneSiteAdmin", "https://localhost:44320" }
    };

    /// <summary>
    /// Host dictionary (Http1)
    /// </summary>
    private readonly Dictionary<string, string> _hostDicHttp1 = new()
    {
        { "ApiComicWeb", "http://localhost:54103" },
        { "ApiIdentityWeb", "http://localhost:54105" },
        { "ApiMediaWeb", "http://localhost:54106" },
        { "ApiRealtimeWeb", "http://localhost:54107" },
        { "ApiSocialWeb", "http://localhost:54108" },
        { "ApiStoryWeb", "http://localhost:54109" },
        { "ApiWalletWeb", "http://localhost:54110" },

        { "ApiComicMobile", "http://localhost:54111" },
        { "ApiIdentityMobile", "http://localhost:54112" },
        { "ApiSocialMobile", "http://localhost:54113" },
        { "ApiStoryMobile", "http://localhost:54114" },

        { "ApiAnalyticAdmin", "http://localhost:54102" },
        { "ApiComicAdmin", "http://localhost:54115" },
        { "ApiIdentityAdmin", "http://localhost:54116" },
        { "ApiSocialAdmin", "http://localhost:54117" },
        { "ApiStoryAdmin", "http://localhost:54118" },
        { "ApiSyncAdmin", "http://localhost:54119" },
        { "ApiCloneSiteAdmin", "http://localhost:54120" }
    };

    /// <summary>
    /// Rpc dictionary (Http2)
    /// </summary>
    private readonly Dictionary<string, string> _hostDicHttp2 = new()
    {
        { "RpcComicWeb", "http://localhost:54203" },
        { "RpcIdentityWeb", "http://localhost:54205" },
        { "RpcMediaWeb", "http://localhost:54206" },
        { "RpcRealtimeWeb", "http://localhost:54207" },
        { "RpcSocialWeb", "http://localhost:54208" },
        { "RpcStoryWeb", "http://localhost:54209" },
        { "RpcWalletWeb", "http://localhost:54210" },

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
