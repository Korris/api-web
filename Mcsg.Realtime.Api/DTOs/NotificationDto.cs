namespace Mcsg.Realtime.Api.DTOs
{
    public class NotificationDto
    {
        public Guid Id { get; set; }
        public string Status { get; set; }
        public Guid NotificationObjectId { get; set; }
        public Guid ReceiverId { get; set; }
        public Guid ActorId { get; set; }
        public string Message { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
