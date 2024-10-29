using System.ComponentModel.DataAnnotations;

namespace Mcsg.Document.Api.Requests;

public class NotificationUpdateR
{
    [Required]
    public Guid NotificationId { get; set; }
}
