namespace Mcsg.Api.Areas.Document.Requests;

using Common.Core.Requests;

public class DocumentPostListSeriesR : PaginatedR
{
    public string? HashTag { get; set; }
}
