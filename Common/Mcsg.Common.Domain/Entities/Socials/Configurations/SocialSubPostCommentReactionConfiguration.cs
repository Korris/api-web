using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class SocialSubPostCommentReactionConfiguration : BaseConfiguration<SocialSubPostCommentReaction>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<SocialSubPostCommentReaction> builder)
    {
        builder.ToTable("SocialSubPostCommentReactions", DbSchema.Social);
        builder.HasIndex(x => new { x.TargetId, x.ParentId, x.AuthorId });
    }
}