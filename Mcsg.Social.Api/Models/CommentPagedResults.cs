namespace Mcsg.Social.Api.Models;

using Lib.Data.Entities.Common;

public class CommentPagedResults<T> : PagedResults<T>
{
    public int TotalComments { get; set; }

    public CommentPagedResults(int totalItems, int pageNumber = 1, int pageSize = 10) : base(totalItems, pageNumber, pageSize)
    {
    }
}
