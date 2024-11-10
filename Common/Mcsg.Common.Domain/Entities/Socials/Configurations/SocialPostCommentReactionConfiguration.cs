using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class SocialPostCommentReactionConfiguration : BaseConfiguration<SocialPostCommentReaction>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<SocialPostCommentReaction> builder)
    {
        builder.ToTable("SocialPostCommentReactions", DbSchema.Social);
        builder.HasIndex(x => new { x.TargetId, x.ParentId, x.AuthorId });
    }
}