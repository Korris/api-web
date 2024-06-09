using System.ComponentModel.DataAnnotations;

namespace Mcsg.Social.Api.DTOs
{
    public class UpdateNotificationReq
    {
        [Required]
        public Guid NotificationId { get; set; }
    }
}
