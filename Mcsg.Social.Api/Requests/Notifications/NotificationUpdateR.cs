using System.ComponentModel.DataAnnotations;

namespace Mcsg.Social.Api.Requests
{
    public class NotificationUpdateR
    {
        [Required]
        public Guid NotificationId { get; set; }
    }
}
