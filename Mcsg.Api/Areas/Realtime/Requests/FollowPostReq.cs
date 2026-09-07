using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Api.Areas.Realtime.Requests;

using Common.Core.Requests;

public class FollowPostReq : BaseR
{
    public string PostHashId { get; set; }
    public Guid PostId { get; set; }
    public Guid AuthorId { get; set; }
    public Guid ActorId { get; set; }

    [NotMapped]
    public string MicroService { get; set; }
}
