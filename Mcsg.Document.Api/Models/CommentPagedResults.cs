namespace Mcsg.Document.Api.Models;

using Common.SeedWork.Responses;

public class CommentPagedResults<T> : PagedResponse<T>
{
    public int TotalComments { get; set; }

    public CommentPagedResults(int totalItems, int pageNumber = 1, int pageSize = 10) : base(totalItems, pageNumber, pageSize)
    {
    }
}
