namespace Mcsg.Document.Api.Requests;

using Common.Core.Requests;

public class DocumentPostListSeriesR : PaginatedR
{
    public string? HashTag { get; set; }
}
