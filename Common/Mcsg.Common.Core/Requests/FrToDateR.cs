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

namespace Mcsg.Common.Core.Requests;

/// <summary>
/// FrToDate request
/// </summary>
public class FrToDateR : BaseR
{
    #region -- Properties --

    /// <summary>
    /// FrDate
    /// </summary>
    public DateTime FrDate { get; set; }

    /// <summary>
    /// ToDate
    /// </summary>
    public DateTime ToDate { get; set; }

    #endregion
}
