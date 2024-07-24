namespace Mcsg.Common.Domain.Entities;

using Core.Enums;
using SeedWork;

public class SmartLookupUser : EntityId
{
    public Guid UserId { get; set; }
    public string? Keyword { get; set; }
    public LookupKeywordType KeywordType { get; set; }
    public DateTime CreatedOn { get; set; }
}
