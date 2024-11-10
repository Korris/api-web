using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class SocialPostReactionConfiguration : BaseConfiguration<SocialPostReaction>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<SocialPostReaction> builder)
    {
        builder.ToTable("SocialPostReactions", DbSchema.Social);
        builder.HasIndex(x => new { x.TargetId, x.ParentId, x.AuthorId });
    }
}