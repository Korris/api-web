using System.ComponentModel.DataAnnotations;

namespace Mcsg.Story.Api.Requests;

using Common.Core.Requests;

public class NotificationUpdateR : BaseR
{
    [Required]
    public Guid NotificationId { get; set; }
}
