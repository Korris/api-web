using System.ComponentModel.DataAnnotations;

namespace Mcsg.Comic.Api.Requests;

using Common.Core.Requests;

public class NotificationUpdateR : BaseR
{
    [Required]
    public Guid NotificationId { get; set; }
}
