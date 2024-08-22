namespace Mcsg.Comic.Api.Requests;

using Common.Core.Requests;

public class ComicPostByProFileNameR : PaginatedR
{
    public string? Keyword { get; set; }
    public string? SearchBy { get; set; }
}
