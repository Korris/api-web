using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

public class SubPostReactionConfiguration : BaseConfiguration<SubPostReaction>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<SubPostReaction> builder)
    {
        builder.ToTable("SubPostReactions");
        builder.HasIndex(x => new { x.TargetId, x.ParentId, x.AuthorId });
        builder.HasOne(typeof(SubPost)).WithMany().HasForeignKey("TargetId");
        builder.HasOne(typeof(User)).WithMany().HasForeignKey("AuthorId");
    }
}