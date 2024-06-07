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

namespace Mcsg.Common.SeedWork.Responses;

/// <summary>
/// Search response
/// </summary>
/// <remarks>
/// Initialize
/// </remarks>
/// <param name="pageNum">Page number</param>
/// <param name="pageSize">Page size</param>
/// <param name="paging">Allow paging</param>
public class SearchResponse(ushort pageNum, ushort pageSize, bool paging) : SingleResponse()
{
    #region -- Properties --

    /// <summary>
    /// Total records
    /// </summary>
    public int TotalRecords { get; set; }

    /// <summary>
    /// Page number
    /// </summary>
    public ushort PageNum
    {
        get { return _paging ? _pageNum : (ushort)1; }
        private set { _pageNum = value; }
    }

    /// <summary>
    /// Page size
    /// </summary>
    public int PageSize
    {
        get { return _paging ? _pageSize : TotalRecords; }
        private set { _pageSize = value; }
    }

    /// <summary>
    /// Total pages
    /// </summary>
    public ushort TotalPages
    {
        get
        {
            var t = (double)TotalRecords / PageSize;
            var res = (ushort)Math.Ceiling(t);
            return res;
        }
    }

    /// <summary>
    /// Paging information
    /// </summary>
    public string PagingInfo
    {
        get
        {
            if (TotalRecords == 0)
            {
                return "No data";
            }

            var fr = (PageNum - 1) * PageSize + 1;
            var to = PageSize * PageNum;

            if (to > TotalRecords)
            {
                to = TotalRecords;
            }

            return $"Displaying {fr} - {to} of {TotalRecords}";
        }
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// Page number
    /// </summary>
    private ushort _pageNum = pageNum;

    /// <summary>
    /// Page size
    /// </summary>
    private int _pageSize = pageSize;

    /// <summary>
    /// Allow paging
    /// </summary>
    private bool _paging = paging;

    #endregion
}
