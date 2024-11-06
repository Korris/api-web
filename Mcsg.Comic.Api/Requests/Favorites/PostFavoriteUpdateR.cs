namespace Mcsg.Comic.Api.Requests;

using Common.Core.Requests;

public class PostFavoriteUpdateR : BaseR
{
    public Guid PostId { get; set; }
}
