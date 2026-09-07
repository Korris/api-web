using System.ComponentModel.DataAnnotations;

namespace Mcsg.Api.Areas.Social.Requests;

using Common.Core.Requests;

public class NotificationUpdateR : BaseR
{
    [Required]
    public Guid NotificationId { get; set; }
}
