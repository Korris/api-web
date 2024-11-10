using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class StorySubPostReactionConfiguration : BaseConfiguration<StorySubPostReaction>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<StorySubPostReaction> builder)
    {
        builder.ToTable("StorySubPostReactions", DbSchema.Story);
        builder.HasIndex(x => new { x.TargetId, x.ParentId, x.AuthorId });
    }
}