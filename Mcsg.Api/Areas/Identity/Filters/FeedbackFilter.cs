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

namespace Mcsg.Api.Areas.Identity.Filters;

using Common.Core.Filters;

/// <summary>
/// Feedback filter
/// </summary>
public class FeedbackFilter : BaseFilter
{
    #region -- Classes --

    /// <summary>
    /// Search
    /// </summary>
    public new class Search : BaseFilter.Search
    {
    }

    #endregion
}
