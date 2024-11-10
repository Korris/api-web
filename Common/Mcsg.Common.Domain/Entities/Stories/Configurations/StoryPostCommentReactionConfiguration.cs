using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class StoryPostCommentReactionConfiguration : BaseConfiguration<StoryPostCommentReaction>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<StoryPostCommentReaction> builder)
    {
        builder.ToTable("StoryPostCommentReactions", DbSchema.Story);
        builder.HasIndex(x => new { x.TargetId, x.ParentId, x.AuthorId });
    }
}