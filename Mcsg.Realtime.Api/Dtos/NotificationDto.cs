namespace Mcsg.Realtime.Api.Dtos;

using Common.Core.Enums;

public class NotificationDto
{
    public Guid Id { get; set; }
    public NotificationStatus Status { get; set; }
    public Guid NotificationObjectId { get; set; }
    public Guid ReceiverId { get; set; }
    public Guid ActorId { get; set; }
    public string Message { get; set; }
    public DateTime CreatedOn { get; set; }
}
