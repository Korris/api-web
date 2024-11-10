namespace Mcsg.Common.Domain.Entities;

using Core.Enums;
using SeedWork;

public partial class SmartLookup : EntityId
{
    public Guid EntityId { get; set; }
    public EntityType EntityType { get; set; }
    public string? Keyword { get; set; }
    public LookupKeywordType KeywordType { get; set; } = LookupKeywordType.None;
    public int CountCriteria { get; set; }
}
