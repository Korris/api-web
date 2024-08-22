namespace Mcsg.Comic.Api.Requests;

using Common.Core.Requests;

public class ComicPostListSeriesR : PaginatedR
{
    public string? HashTag { get; set; }
    public bool IsFavorite { get; set; }
}
