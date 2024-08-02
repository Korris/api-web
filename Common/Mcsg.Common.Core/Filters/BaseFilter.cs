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

namespace Mcsg.Common.Core.Filters;

/// <summary>
/// Base filter
/// </summary>
public class BaseFilter
{
    #region -- Classes --

    /// <summary>
    /// Search
    /// </summary>
    public class Search
    {
        #region -- Properties --

        /// <summary>
        /// List Name
        /// </summary>
        public List<string>? Names { get; set; }

        /// <summary>
        /// Keyword
        /// </summary>
        public string? Keyword { get; set; }

        #endregion
    }

    #endregion
}
