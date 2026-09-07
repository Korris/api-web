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
/// Paged response
/// </summary>
/// <typeparam name="T">Type</typeparam>
public class PagedResponse<T>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="totalItems"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    public PagedResponse(int totalItems, int pageNumber = 1, int pageSize = 10)
    {
        Init(totalItems, pageNumber, pageSize);
        Items = [];
    }

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="items"></param>
    /// <param name="totalItems"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    public PagedResponse(List<T> items, int totalItems, int pageNumber = 1, int pageSize = 10)
    {
        Init(totalItems, pageNumber, pageSize);
        Items = items;
    }

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="totalItems"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    private void Init(int totalItems, int pageNumber, int pageSize)
    {
        if (pageSize <= 0) pageSize = 10;

        // Calculate total pages
        var totalPages = (int)Math.Ceiling(totalItems / (decimal)pageSize);

        // Ensure actual page isn't out of range
        if (pageNumber < 1)
        {
            pageNumber = 1;
        }
        else if (pageNumber > totalPages)
        {
            pageNumber = totalPages;
        }

        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalItems = totalItems;
        TotalPages = totalPages;
    }

    #endregion

    #region -- Properties --

    /// <summary>
    /// Items
    /// </summary>
    public IEnumerable<T> Items { get; set; }

    /// <summary>
    /// Total number of items to be paged
    /// </summary>
    public int TotalItems { get; set; }

    /// <summary>
    /// Maximum number of page navigation links to display, default is 5
    /// </summary>
    public int MaxNavigationPages { get; private set; } = 5;

    /// <summary>
    /// Current active page
    /// </summary>
    public int PageNumber { get; private set; } = 1;

    /// <summary>
    /// Number of items per page, default is 10
    /// </summary>
    public int PageSize { get; private set; } = 10;

    /// <summary>
    /// Total pages
    /// </summary>
    public int TotalPages { get; private set; }

    #endregion
}
