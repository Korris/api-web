using System.Text.Json.Serialization;

namespace Mcsg.Social.Api.Models;

using Common.Core.Enums;
using Common.SeedWork.Converters;

public class NotificationQueryResult
{
    public Guid Id { get; set; }
    public Guid ReceiverId { get; set; }
    public NotificationStatus Status { get; set; }
    public Guid? LocationId { get; set; }
    public string LocationHashId { get; set; }
    public NotificationEntityType EntityType { get; set; }
    public Guid? EntityId { get; set; }
    public string EntityHashId { get; set; }
    public NotificationAction Action { get; set; }

    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime CreatedOn { get; set; }

    public Guid ActorId { get; set; }
    public string ActorName { get; set; }
    public string Avatar { get; set; }
    public int? ReactionType { get; set; }
}
