using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

using SeedWork;
using Core.Enums;

[Table("SmartLookupUsers")]
public class SmartLookupUser : EntityId
{
    public Guid UserId { get; set; }
    public string? Keyword { get; set; }
    public LookupKeywordType KeywordType { get; set; }
    public DateTime CreatedOn { get; set; }
}
