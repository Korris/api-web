using System.ComponentModel.DataAnnotations;

namespace Mcsg.Api.DTOs
{
    public class UpdateNotificationReq
    {
        [Required]
        public Guid NotificationId { get; set; }
    }
}
