namespace Mcsg.Comic.Api.Requests;

using Common.Core.Requests;

public class ComicPostByTagNameR : PaginatedR
{
    public string? TagName { get; set; }
}
