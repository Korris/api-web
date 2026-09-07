namespace Mcsg.Api.Areas.Document.Models;

public class FavoritePostResponse
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public bool IsFavorite { get; set; }
}
