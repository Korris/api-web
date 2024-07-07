namespace Mcsg.Social.Api.Requests;

public class ComicPostListSeriesR : BasePageResultReq
{
    public string? HashTag { get; set; }
    public bool IsFavorite { get; set; }
}
