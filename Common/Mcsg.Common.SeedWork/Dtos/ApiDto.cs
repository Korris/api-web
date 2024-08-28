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
        Mobile = new MobileDto();
        Web = new WebDto();
    }

    #endregion

    #region -- Properties --

    /// <summary>
    /// Admin
    /// </summary>
    public AdminDto Admin { get; }

    /// <summary>
    /// Mobile
    /// </summary>
    public MobileDto Mobile { get; }

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
        /// <summary>
        /// Analytic
        /// </summary>
        public string? Analytic { get; set; }

        /// <summary>
        /// Sync
        /// </summary>
        public string? Sync { get; set; }
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
    /// Web
    /// </summary>
    public class WebDto : MobileDto
    {
        /// <summary>
        /// Media
        /// </summary>
        public string? Media { get; set; }

        /// <summary>
        /// Realtime
        /// </summary>
        public string? Realtime { get; set; }

        /// <summary>
        /// Wallet
        /// </summary>
        public string? Wallet { get; set; }
    }

    #endregion
}
