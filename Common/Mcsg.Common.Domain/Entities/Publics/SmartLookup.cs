using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

using Core.Enums;
using SeedWork;

[Table("SmartLookups")]
public class SmartLookup : EntityId
{
    public string? Keyword { get; set; }
    public LookupKeywordType KeywordType { get; set; } = LookupKeywordType.None;
    public int CountCriteria { get; set; }
}
