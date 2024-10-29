namespace Mcsg.Document.Api.Models;

using Common.Core.Enums;

public class UserMentionModel
{
    public Guid Id { get; set; }
    public Guid LocationId { get; set; }
    public MentionLocationType LocationType { get; set; }
    public Guid EntityId { get; set; }
    public EntityType EntityType { get; set; }
    public string ProfileName { get; set; }
    public string? UserName { get; set; }
    public int Length { get; set; }
    public int Offset { get; set; }
    public string Text { get; set; }
}
