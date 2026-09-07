namespace Mcsg.Api.Areas.Social.Models;

public class SmartLookupResponse
{
    public string Keyword { get; set; }
    public string KeywordType { get; set; }
    public string Avatar { get; set; }

    public string EntityType { get; set; }
    public Guid EntityId { get; set; }
}
