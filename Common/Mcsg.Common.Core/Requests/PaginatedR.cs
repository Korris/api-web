using System.ComponentModel;

namespace Mcsg.Common.Core.Requests;

/// <summary>
/// Paginated request
/// </summary>
public class PaginatedR : BaseR
{
    #region -- Properties --

    /// <summary>
    /// Page number
    /// </summary>
    [DefaultValue(1)]
    public int PageNumber { get; set; }

    /// <summary>
    /// Page size
    /// </summary>
    [DefaultValue(10)]
    public int PageSize { get; set; }

    /// <summary>
    /// Order by
    /// </summary>
    [DefaultValue("CreatedOn")]
    public string? OrderBy { get; set; }

    #endregion
}
