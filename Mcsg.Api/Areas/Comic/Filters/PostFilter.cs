namespace Mcsg.Api.Areas.Comic.Filters;

using Common.Core.Enums;
using Common.Core.Filters;

/// <summary>
/// Post filter
/// </summary>
public class PostFilter : BaseFilter
{
    #region -- Classes --

    /// <summary>
    /// Search
    /// </summary>
    public new class Search : BaseFilter.Search
    {
        #region -- Properties --

        /// <summary>
        /// FrDate
        /// </summary>
        public DateTime? FrDate { get; set; }

        /// <summary>
        /// ToDate
        /// </summary>
        public DateTime? ToDate { get; set; }

        /// <summary>
        /// Permissions
        /// </summary>
        public List<PostPermission>? Permissions { get; set; }

        /// <summary>
        /// Hashtag (LoadFeed)
        /// </summary>
        public string? Hashtag { get; set; }

        #endregion
    }

    #endregion
}
