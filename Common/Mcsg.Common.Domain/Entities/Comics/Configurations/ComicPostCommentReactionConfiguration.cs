using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class ComicPostCommentReactionConfiguration : BaseConfiguration<ComicPostCommentReaction>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<ComicPostCommentReaction> builder)
    {
        builder.ToTable("ComicPostCommentReactions", DbSchema.Comic);
        builder.HasIndex(x => new { x.TargetId, x.ParentId, x.AuthorId });
        builder.HasOne(typeof(ComicPostComment)).WithMany().HasForeignKey("TargetId");
        builder.HasOne(typeof(User)).WithMany().HasForeignKey("AuthorId");
    }
}