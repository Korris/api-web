namespace Mcsg.Api.Areas.Story.Dtos;

public class SoundRecentlyDto
{
    public int Total { get; set; }
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? Url { get; set; }
    public string? Thumbnail { get; set; }
    public string? ArtistName { get; set; }
    public int DurationSeconds { get; set; }
    public string? Duration { get; set; }
    public int Order { get; set; }
}
