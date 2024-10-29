namespace Mcsg.Document.Api.Requests;

using Common.Core.Requests;

public class UserNamePagingR : PaginatedR
{
    public string? UserName { get; set; }
}
