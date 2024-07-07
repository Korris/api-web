namespace Mcsg.Social.Api.Models;

public class MetaDataResponse
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Image { get; set; }
    public string? Url { get; set; }
    public string? Domain { get; set; }
}
public class MetaDataQueryDbResponse : MetaDataResponse
{
    public Guid Id { get; set; }
}
