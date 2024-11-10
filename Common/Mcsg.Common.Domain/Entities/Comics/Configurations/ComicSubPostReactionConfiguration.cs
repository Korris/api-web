using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class ComicSubPostReactionConfiguration : BaseConfiguration<ComicSubPostReaction>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<ComicSubPostReaction> builder)
    {
        builder.ToTable("ComicSubPostReactions", DbSchema.Comic);
        builder.HasIndex(x => new { x.TargetId, x.ParentId, x.AuthorId });
    }
}