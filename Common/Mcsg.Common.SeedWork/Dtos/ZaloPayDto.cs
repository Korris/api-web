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
/// ZaloPay data transfer object
/// </summary>
public class ZaloPayDto
{
    #region -- Properties --

    /// <summary>
    /// AppId
    /// </summary>
    public string AppId { get; set; } = default!;

    /// <summary>
    /// AppUser
    /// </summary>
    public string AppUser { get; set; } = default!;

    /// <summary>
    /// Key1
    /// </summary>
    public string Key1 { get; set; } = default!;

    /// <summary>
    /// Key2
    /// </summary>
    public string Key2 { get; set; } = default!;

    /// <summary>
    /// Url
    /// </summary>
    public string Url { get; set; } = default!;

    /// <summary>
    /// ConfigName
    /// </summary>
    public string ConfigName => "zalopayapp";

    /// <summary>
    /// RedirectUrl
    /// </summary>
    public string RedirectUrl { get; set; } = default!;

    /// <summary>
    /// CallBackUrl
    /// </summary>
    public string CallBackUrl { get; set; } = default!;

    /// <summary>
    /// QueryScheduleMinutes
    /// </summary>
    public int QueryScheduleMinutes { get; set; }

    #endregion
}
