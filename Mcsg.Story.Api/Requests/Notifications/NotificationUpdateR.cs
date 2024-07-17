using System.ComponentModel.DataAnnotations;

namespace Mcsg.Story.Api.Requests;

public class NotificationUpdateR
{
    [Required]
    public Guid NotificationId { get; set; }
}
