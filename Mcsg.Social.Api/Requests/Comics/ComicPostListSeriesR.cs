namespace Mcsg.Social.Api.Requests;

public class ComicPostListSeriesR : BasePageResultR
{
    public string? HashTag { get; set; }
    public bool IsFavorite { get; set; }
}
