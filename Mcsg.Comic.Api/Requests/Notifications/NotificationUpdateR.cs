using System.ComponentModel.DataAnnotations;

namespace Mcsg.Comic.Api.Requests;

public class NotificationUpdateR
{
    [Required]
    public Guid NotificationId { get; set; }
}
