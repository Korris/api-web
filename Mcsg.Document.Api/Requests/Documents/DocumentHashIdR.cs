namespace Mcsg.Document.Api.Requests;

using Common.Core.Requests;

public class DocumentHashIdR : PaginatedR
{
    public string? HashId { get; set; }

    public bool IsLoadChapters { get; set; }
}
