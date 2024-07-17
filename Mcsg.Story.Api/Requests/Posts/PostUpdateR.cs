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

namespace Mcsg.Story.Api.Requests;

/// <summary>
/// Request
/// </summary>
public class PostUpdateR : PostFormBase
{
    #region -- Properties --

    /// <summary>
    /// HashId
    /// </summary>
    public string? HashId { get; set; }

    #endregion
}
