namespace Mcsg.Api.Areas.Comic.Requests;

using Common.Core.Requests;

public class ComicPostListSeriesR : PaginatedR
{
    public string? HashTag { get; set; }
}
