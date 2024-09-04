using Microsoft.AspNetCore.Http;
using System.ComponentModel;

namespace Mcsg.Common.Core.Requests;

/// <summary>
/// Paginated request
/// </summary>
public class PaginatedR : BaseR
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public PaginatedR() { }

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="hc">HTTP context</param>
    public PaginatedR(HttpContext hc)
    {
        _hc = hc;

        LogHeader();
    }

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="hc">HTTP context</param>
    /// <param name="hashIds">HashIds</param>
    public PaginatedR(HttpContext hc, string? hashIds) : this(hc)
    {
        HashIds = hashIds;
    }

    #endregion

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

    /// <summary>
    /// HashIds
    /// </summary>
    public string? HashIds { get; set; }

    #endregion
}
