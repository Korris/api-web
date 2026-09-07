namespace Mcsg.Api.Areas.Comic.Models;

public class ReactionUpdateResponse
{
    public Guid TargetId { get; set; }
    public Guid ReactionId { get; set; }
    public string MicroService { get; set; }
    public Guid? SubPostId { get; set; }
    public bool IsDeleted { get; set; }
}