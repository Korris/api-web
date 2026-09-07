namespace Mcsg.Api.Areas.Document.Requests;

using Common.Core.Requests;

public class DocumentPostByProFileNameR : PaginatedR
{
    public string? Keyword { get; set; }
    public string? SearchBy { get; set; }
}
