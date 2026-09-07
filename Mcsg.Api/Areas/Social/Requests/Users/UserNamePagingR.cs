namespace Mcsg.Api.Areas.Social.Requests;

using Common.Core.Requests;

public class UserNamePagingR : PaginatedR
{
    public string? UserName { get; set; }
}
