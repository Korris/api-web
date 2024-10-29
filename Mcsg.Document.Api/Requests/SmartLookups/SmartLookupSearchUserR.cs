namespace Mcsg.Document.Api.Requests;

using Common.Core.Requests;

public class SmartLookupSearchUserR : PaginatedR
{
    public string? ProfileName { get; set; }
}
