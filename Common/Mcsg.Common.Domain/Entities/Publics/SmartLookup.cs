namespace Mcsg.Common.Domain.Entities;

using Core.Enums;
using SeedWork;

public class SmartLookup : EntityId
{
    public string? Keyword { get; set; }
    public LookupKeywordType KeywordType { get; set; } = LookupKeywordType.None;
    public int CountCriteria { get; set; }
}
