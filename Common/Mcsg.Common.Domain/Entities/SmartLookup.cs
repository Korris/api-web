using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

using Common;
using Core.Enums;

[Table("SmartLookups")]
public class SmartLookup : BaseEntity
{
    public string? Keyword { get; set; }
    public LookupKeywordType KeywordType { get; set; } = LookupKeywordType.None;
    public int CountCriteria { get; set; }
}
