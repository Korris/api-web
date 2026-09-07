namespace Mcsg.Api.Areas.Comic.Requests;

using Common.Core.Requests;

public class ComicHashIdR : PaginatedR
{
    public string? HashId { get; set; }

    public bool IsLoadChapters { get; set; }
}
