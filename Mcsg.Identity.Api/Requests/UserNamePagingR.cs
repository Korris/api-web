namespace Mcsg.Identity.Api.Requests;

using Common.Core.Requests;

public class UserNamePagingR : PaginatedR
{
    public string? UserName { get; set; }
}
