using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class SubPostCommentReactionConfiguration : BaseConfiguration<SubPostCommentReaction>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<SubPostCommentReaction> builder)
    {
        builder.ToTable("SubPostCommentReactions", DbSchema.Social);
        builder.HasIndex(x => new { x.TargetId, x.ParentId, x.AuthorId });
        builder.HasOne(typeof(SubPostComment)).WithMany().HasForeignKey("TargetId");
        builder.HasOne(typeof(User)).WithMany().HasForeignKey("AuthorId");
    }
}