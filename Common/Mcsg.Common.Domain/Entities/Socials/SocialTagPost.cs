using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

using SeedWork;

[Table("SocialTagPosts")]
public class SocialTagPost : AuditableEntity
{
    public Guid TagId { get; set; }
    public Guid PostId { get; set; }
}