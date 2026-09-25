namespace Mcsg.Api.Areas.TapShow.Models;

using Common.SeedWork.Responses;

/// <summary>
/// Comment preview attached to posts in lists (same shape as Story CommentPagedResults)
/// </summary>
public class CommentPagedResults<T> : PagedResponse<T>
{
    public int TotalComments { get; set; }

    public CommentPagedResults(List<T> items, int totalItems, int pageNumber = 1, int pageSize = 10) : base(items, totalItems, pageNumber, pageSize)
    {
    }
}
