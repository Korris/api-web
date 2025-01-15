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
        SiteName = "FocFoc";
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
        if (dic.TryGetValue("ApiAdminComic", out val)) Api.Admin.Comic = val;
        if (dic.TryGetValue("ApiAdminDocument", out val)) Api.Admin.Document = val;
        if (dic.TryGetValue("ApiAdminIdentity", out val)) Api.Admin.Identity = val;
        if (dic.TryGetValue("ApiAdminSocial", out val)) Api.Admin.Social = val;
        if (dic.TryGetValue("ApiAdminStory", out val)) Api.Admin.Story = val;
        #endregion

        #region -- Api.Analytic --
        if (dic.TryGetValue("ApiAnalyticAnalytic", out val)) Api.Analytic.Analytic = val;
        #endregion

        #region -- Api.Mobile --
        if (dic.TryGetValue("ApiMobileComic", out val)) Api.Mobile.Comic = val;
        if (dic.TryGetValue("ApiMobileDocument", out val)) Api.Mobile.Document = val;
        if (dic.TryGetValue("ApiMobileIdentity", out val)) Api.Mobile.Identity = val;
        if (dic.TryGetValue("ApiMobileSocial", out val)) Api.Mobile.Social = val;
        if (dic.TryGetValue("ApiMobileStory", out val)) Api.Mobile.Story = val;
        if (dic.TryGetValue("ApiMobileNotification", out val)) Api.Mobile.Notification = val;
        #endregion

        #region -- Api.Wallet --
        if (dic.TryGetValue("ApiWalletWallet", out val)) Api.Wallet.Wallet = val;
        #endregion

        #region -- Api.Web --
        if (dic.TryGetValue("ApiWebComic", out val)) Api.Web.Comic = val;
        if (dic.TryGetValue("ApiWebDocument", out val)) Api.Web.Document = val;
        if (dic.TryGetValue("ApiWebIdentity", out val)) Api.Web.Identity = val;
        if (dic.TryGetValue("ApiWebSocial", out val)) Api.Web.Social = val;
        if (dic.TryGetValue("ApiWebStory", out val)) Api.Web.Story = val;
        if (dic.TryGetValue("ApiWebRealtime", out val)) Api.Web.Realtime = val;
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
        if (dic.TryGetValue("RpcAdminComic", out val)) Rpc.Admin.Comic = val;
        if (dic.TryGetValue("RpcAdminDocument", out val)) Rpc.Admin.Document = val;
        if (dic.TryGetValue("RpcAdminIdentity", out val)) Rpc.Admin.Identity = val;
        if (dic.TryGetValue("RpcAdminSocial", out val)) Rpc.Admin.Social = val;
        if (dic.TryGetValue("RpcAdminStory", out val)) Rpc.Admin.Story = val;
        #endregion

        #region -- Rpc.Analytic --
        if (dic.TryGetValue("RpcAnalyticAnalytic", out val)) Rpc.Analytic.Analytic = val;
        #endregion

        #region -- Rpc.Mobile --
        if (dic.TryGetValue("RpcMobileComic", out val)) Rpc.Mobile.Comic = val;
        if (dic.TryGetValue("RpcMobileDocument", out val)) Rpc.Mobile.Document = val;
        if (dic.TryGetValue("RpcMobileIdentity", out val)) Rpc.Mobile.Identity = val;
        if (dic.TryGetValue("RpcMobileSocial", out val)) Rpc.Mobile.Social = val;
        if (dic.TryGetValue("RpcMobileStory", out val)) Rpc.Mobile.Story = val;
        if (dic.TryGetValue("RpcMobileNotification", out val)) Rpc.Mobile.Notification = val;
        #endregion

        #region -- Rpc.Wallet --
        if (dic.TryGetValue("RpcWalletWallet", out val)) Rpc.Wallet.Wallet = val;
        #endregion

        #region -- Rpc.Web --
        if (dic.TryGetValue("RpcWebComic", out val)) Rpc.Web.Comic = val;
        if (dic.TryGetValue("RpcWebDocument", out val)) Rpc.Web.Document = val;
        if (dic.TryGetValue("RpcWebIdentity", out val)) Rpc.Web.Identity = val;
        if (dic.TryGetValue("RpcWebSocial", out val)) Rpc.Web.Social = val;
        if (dic.TryGetValue("RpcWebStory", out val)) Rpc.Web.Story = val;
        if (dic.TryGetValue("RpcWebRealtime", out val)) Rpc.Web.Realtime = val;
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
        { "ApiAdminComic", "https://localhost:44301" },
        { "ApiAdminDocument", "https://localhost:44302" },
        { "ApiAdminIdentity", "https://localhost:44303" },
        { "ApiAdminSocial", "https://localhost:44304" },
        { "ApiAdminStory", "https://localhost:44305" },

        { "ApiAnalyticAnalytic", "https://localhost:44311" },

        { "ApiMobileComic", "https://localhost:44321" },
        { "ApiMobileDocument", "https://localhost:44322" },
        { "ApiMobileIdentity", "https://localhost:44323" },
        { "ApiMobileSocial", "https://localhost:44324" },
        { "ApiMobileStory", "https://localhost:44325" },
        { "ApiMobileNotification", "http://bumcheo-dev-service-01:3030" },

        { "ApiWalletWallet", "https://localhost:44331" },

        { "ApiWebComic", "https://localhost:44341" },
        { "ApiWebDocument", "https://localhost:44342" },
        { "ApiWebIdentity", "https://localhost:44343" },
        { "ApiWebSocial", "https://localhost:44344" },
        { "ApiWebStory", "https://localhost:44345" },
        { "ApiWebRealtime", "https://localhost:44348" }
    };

    /// <summary>
    /// Host dictionary (Http1)
    /// </summary>
    private readonly Dictionary<string, string> _hostDicHttp1 = new()
    {
        { "ApiAdminComic", "http://localhost:54101" },
        { "ApiAdminDocument", "http://localhost:54102" },
        { "ApiAdminIdentity", "http://localhost:54103" },
        { "ApiAdminSocial", "http://localhost:54104" },
        { "ApiAdminStory", "http://localhost:54105" },

        { "ApiAnalyticAnalytic", "http://localhost:54111" },

        { "ApiMobileComic", "http://localhost:54121" },
        { "ApiMobileDocument", "http://localhost:54122" },
        { "ApiMobileIdentity", "http://localhost:54123" },
        { "ApiMobileSocial", "http://localhost:54124" },
        { "ApiMobileStory", "http://localhost:54125" },
        { "ApiMobileNotification", "http://bumcheo-dev-service-01:3030" },

        { "ApiWalletWallet", "http://localhost:54131" },

        { "ApiWebComic", "http://localhost:54141" },
        { "ApiWebDocument", "http://localhost:54142" },
        { "ApiWebIdentity", "http://localhost:54143" },
        { "ApiWebSocial", "http://localhost:54144" },
        { "ApiWebStory", "http://localhost:54145" },
        { "ApiWebRealtime", "http://localhost:54148" }
    };

    /// <summary>
    /// Rpc dictionary (Http2)
    /// </summary>
    private readonly Dictionary<string, string> _hostDicHttp2 = new()
    {
        { "RpcAdminComic", "http://localhost:54201" },
        { "RpcAdminDocument", "http://localhost:54202" },
        { "RpcAdminIdentity", "http://localhost:54203" },
        { "RpcAdminSocial", "http://localhost:54204" },
        { "RpcAdminStory", "http://localhost:54205" },

        { "RpcAnalyticAnalytic", "http://localhost:54211" },

        { "RpcMobileComic", "http://localhost:54221" },
        { "RpcMobileDocument", "http://localhost:54222" },
        { "RpcMobileIdentity", "http://localhost:54223" },
        { "RpcMobileSocial", "http://localhost:54224" },
        { "RpcMobileStory", "http://localhost:54225" },
        { "RpcMobileNotification", "http://bumcheo-dev-service-01:3130" },

        { "RpcWalletWallet", "http://localhost:54231" },

        { "RpcWebComic", "http://localhost:54241" },
        { "RpcWebDocument", "http://localhost:54242" },
        { "RpcWebIdentity", "http://localhost:54243" },
        { "RpcWebSocial", "http://localhost:54244" },
        { "RpcWebStory", "http://localhost:54245" },
        { "RpcWebRealtime", "http://localhost:54248" }
    };

    #endregion
}
