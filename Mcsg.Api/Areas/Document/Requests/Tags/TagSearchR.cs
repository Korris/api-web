namespace Mcsg.Api.Areas.Document.Requests;

using Common.Core.Requests;

public class TagSearchR : PaginatedR
{
    public string? Name { get; set; }
}
