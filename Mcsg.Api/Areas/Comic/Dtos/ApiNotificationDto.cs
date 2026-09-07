namespace Mcsg.Api.Areas.Comic.Dtos;

public class ApiNotificationDto
{
    public string TargetType { get; set; }
    public Guid? LocationId { get; set; }
    public string LocationHashId { get; set; }
    public Guid? EntityId { get; set; }
    public string EntityHashId { get; set; }
    public string Message { get; set; }
    public Guid ActorId { get; set; }
    public string ActorName { get; set; }
}
