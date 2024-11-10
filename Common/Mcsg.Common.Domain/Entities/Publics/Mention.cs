namespace Mcsg.Common.Domain.Entities;

using Core.Enums;
using SeedWork;

public partial class Mention : AuditableEntity
{
    public Guid EntityId { get; set; }
    public EntityType EntityType { get; set; }
    public Guid LocationId { get; set; }
    public MentionLocationType LocationType { get; set; }
    public int Length { get; set; }
    public int Offset { get; set; }
    public string? Text { get; set; }
}
