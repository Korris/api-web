using System.ComponentModel.DataAnnotations;

namespace Mcsg.Social.Api.Requests
{
    public class UpdateNotificationReq
    {
        [Required]
        public Guid NotificationId { get; set; }
    }
}
