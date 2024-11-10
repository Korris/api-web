using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class ComicPostReactionConfiguration : BaseConfiguration<ComicPostReaction>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<ComicPostReaction> builder)
    {
        builder.ToTable("ComicPostReactions", DbSchema.Comic);
        builder.HasIndex(x => new { x.TargetId, x.ParentId, x.AuthorId });
    }
}