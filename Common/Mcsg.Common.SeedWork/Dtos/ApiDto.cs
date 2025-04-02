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

namespace Mcsg.Common.SeedWork.Dtos;

/// <summary>
/// API data transfer object
/// </summary>
public class ApiDto
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public ApiDto()
    {
        Admin = new AdminDto();
        Analytic = new AnalyticDto();
        Mobile = new MobileDto();
        Wallet = new WalletDto();
        Web = new WebDto();
        Chat = new ChatDto();
    }

    #endregion

    #region -- Properties --

    /// <summary>
    /// Admin
    /// </summary>
    public AdminDto Admin { get; }

    /// <summary>
    /// Analytic
    /// </summary>
    public AnalyticDto Analytic { get; }

    /// <summary>
    /// Chat
    /// </summary>
    public ChatDto Chat { get; }

    /// <summary>
    /// Mobile
    /// </summary>
    public MobileDto Mobile { get; }

    /// <summary>
    /// Wallet
    /// </summary>
    public WalletDto Wallet { get; }

    /// <summary>
    /// Web
    /// </summary>
    public WebDto Web { get; }

    #endregion

    #region -- Classes --

    /// <summary>
    /// Admin
    /// </summary>
    public class AdminDto : MobileDto
    {
    }

    /// <summary>
    /// Analytic
    /// </summary>
    public class AnalyticDto : MobileDto
    {
        /// <summary>
        /// Analytic
        /// </summary>
        public string? Analytic { get; set; }
    }

    /// <summary>
    /// Chat
    /// </summary>
    public class ChatDto : MobileDto
    {
        /// <summary>
        /// Chat
        /// </summary>
        public string? Chat { get; set; }
    }

    /// <summary>
    /// Mobile
    /// </summary>
    public class MobileDto
    {
        /// <summary>
        /// Comic
        /// </summary>
        public string? Comic { get; set; }

        /// <summary>
        /// Document
        /// </summary>
        public string? Document { get; set; }

        /// <summary>
        /// Identity
        /// </summary>
        public string? Identity { get; set; }

        /// <summary>
        /// Social
        /// </summary>
        public string? Social { get; set; }

        /// <summary>
        /// Story
        /// </summary>
        public string? Story { get; set; }
    }

    /// <summary>
    /// Wallet
    /// </summary>
    public class WalletDto
    {
        /// <summary>
        /// Wallet
        /// </summary>
        public string? Wallet { get; set; }

        /// <summary>
        /// WalletCoin
        /// </summary>
        public string? WalletCoin { get; set; }
    }

    /// <summary>
    /// Web
    /// </summary>
    public class WebDto : MobileDto
    {
        /// <summary>
        /// Media
        /// </summary>
        [Obsolete]
        public string? Media { get; set; }

        /// <summary>
        /// Realtime
        /// </summary>
        public string? Realtime { get; set; }
    }

    #endregion
}
