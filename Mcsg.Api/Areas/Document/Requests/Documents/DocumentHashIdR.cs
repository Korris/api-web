namespace Mcsg.Api.Areas.Document.Requests;

using Common.Core.Requests;

public class DocumentHashIdR : PaginatedR
{
    public string? HashId { get; set; }

    public bool IsLoadChapters { get; set; }
}
