namespace Mcsg.Api.Areas.Comic.Requests;

using Common.Core.Requests;

public class TagSearchR : PaginatedR
{
    public string? Name { get; set; }
}
