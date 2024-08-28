using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Realtime.Api.Requests;

public class FollowPostReq
{
    public string PostHashId { get; set; }
    public Guid PostId { get; set; }
    public Guid AuthorId { get; set; }
    public Guid ActorId { get; set; }

    [NotMapped]
    public string MicroService { get; set; }
}
