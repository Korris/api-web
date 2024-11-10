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

using SeedWork.Constants;
using SeedWork.Dtos;

/// <summary>
/// Paging request for POST or PATCH method (support paging on server)
/// </summary>
public class PagingR : IdBaseR
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public PagingR()
    {
        PageNum = 1;
        PageSize = Setting.PageSize;
        Sort = [];
        Paging = true;
    }

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="filter">Filter</param>
    /// <param name="sort">Sort</param>
    public PagingR(object filter, List<SortDto> sort) : this()
    {
        Filter = filter;
        Sort = sort;
    }

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="filter">Filter</param>
    /// <param name="sort">Sort</param>
    /// <param name="paging">Allow paging</param>
    public PagingR(object filter, List<SortDto> sort, bool paging) : this(filter, sort)
    {
        Paging = paging;
    }

    #endregion

    #region -- Properties --

    /// <summary>
    /// Allow paging
    /// </summary>
    public bool Paging { get; set; }

    /// <summary>
    /// Filter
    /// </summary>
    public object? Filter { get; set; }

    /// <summary>
    /// Sort
    /// </summary>
    public List<SortDto> Sort { get; set; }

    /// <summary>
    /// Page number
    /// </summary>
    public ushort PageNum
    {
        get
        {
            return _pageNum;
        }
        set
        {
            if (value < 1)
            {
                value = 1;
            }

            _pageNum = value;
        }
    }

    /// <summary>
    /// Page size
    /// </summary>
    public ushort PageSize
    {
        get
        {
            return _pageSize;
        }
        set
        {
            if (value < 1)
            {
                value = 1;
            }

            _pageSize = value;
        }
    }

    /// <summary>
    /// Offset
    /// </summary>
    public ushort Offset
    {
        get
        {
            var res = (PageNum - 1) * PageSize;
            return (ushort)res;
        }
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// Page number
    /// </summary>
    private ushort _pageNum;

    /// <summary>
    /// Page size
    /// </summary>
    private ushort _pageSize;

    #endregion
}
