namespace Mcsg.Api.Areas.Social.Requests;

using Common.Core.Requests;

public class TagSearchR : PaginatedR
{
    public string? Name { get; set; }
}
