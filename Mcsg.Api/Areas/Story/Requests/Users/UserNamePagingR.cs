namespace Mcsg.Api.Areas.Story.Requests;

using Common.Core.Requests;

public class UserNamePagingR : PaginatedR
{
    public string? UserName { get; set; }
}
