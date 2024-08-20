using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

using Core.Enums;
using SeedWork;

public class SmartLookupUser : EntityId
{
    public Guid EntityId { get; set; }
    public EntityType EntityType { get; set; }
    public Guid UserId { get; set; }
    public string? Keyword { get; set; }
    public LookupKeywordType KeywordType { get; set; }

    [Column(TypeName = "timestamp")]
    public DateTime CreatedOn { get; set; }
}
