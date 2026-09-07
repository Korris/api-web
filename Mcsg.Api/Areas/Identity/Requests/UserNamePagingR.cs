namespace Mcsg.Api.Areas.Identity.Requests;

using Common.Core.Requests;

public class UserNamePagingR : PaginatedR
{
    public string? UserName { get; set; }
}
