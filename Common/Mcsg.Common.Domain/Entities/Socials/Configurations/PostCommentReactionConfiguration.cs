using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class PostCommentReactionConfiguration : BaseConfiguration<PostCommentReaction>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<PostCommentReaction> builder)
    {
        builder.ToTable("PostCommentReactions", DbSchema.Social);
        builder.HasIndex(x => new { x.TargetId, x.ParentId, x.AuthorId });
        builder.HasOne(typeof(PostComment)).WithMany().HasForeignKey("TargetId");
        builder.HasOne(typeof(User)).WithMany().HasForeignKey("AuthorId");
    }
}