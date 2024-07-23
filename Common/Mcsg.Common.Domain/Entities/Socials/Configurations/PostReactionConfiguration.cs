using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class PostReactionConfiguration : BaseConfiguration<PostReaction>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<PostReaction> builder)
    {
        builder.ToTable("PostReactions", DbSchema.Social);
        builder.HasIndex(x => new { x.TargetId, x.ParentId, x.AuthorId });
        builder.HasOne(typeof(Post)).WithMany().HasForeignKey("TargetId");
        builder.HasOne(typeof(User)).WithMany().HasForeignKey("AuthorId");
    }
}