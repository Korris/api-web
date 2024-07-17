namespace Mcsg.Story.Api.Models;

public class SimilarProfilesMention
{
    public Guid Id { get; set; }
    public string ProfileName { get; set; } = default!;
    public string? Avatar { get; set; }
    public string UserName { get; set; } = default!;
}
