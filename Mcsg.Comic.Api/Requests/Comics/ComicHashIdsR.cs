namespace Mcsg.Comic.Api.Requests;

using Common.Core.Requests;

public class ComicHashIdsR : PaginatedR
{
    public string? HashIds { get; set; }
}
