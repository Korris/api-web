using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class SocialSubPostReactionConfiguration : BaseConfiguration<SocialSubPostReaction>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<SocialSubPostReaction> builder)
    {
        builder.ToTable("SocialSubPostReactions", DbSchema.Social);
        builder.HasIndex(x => new { x.TargetId, x.ParentId, x.AuthorId });
        builder.HasOne(typeof(SocialSubPost)).WithMany().HasForeignKey("TargetId");
        builder.HasOne(typeof(User)).WithMany().HasForeignKey("AuthorId");
    }
}