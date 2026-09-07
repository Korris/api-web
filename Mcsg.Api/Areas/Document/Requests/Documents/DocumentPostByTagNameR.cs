namespace Mcsg.Api.Areas.Document.Requests;

using Common.Core.Requests;

public class DocumentPostByTagNameR : PaginatedR
{
    public string? TagName { get; set; }
}
