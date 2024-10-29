namespace Mcsg.Document.Api.Requests;

using Common.Core.Requests;

public class DocumentPostByTagNameR : PaginatedR
{
    public string? TagName { get; set; }
}
