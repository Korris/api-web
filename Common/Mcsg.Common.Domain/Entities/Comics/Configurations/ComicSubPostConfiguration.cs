using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class ComicSubPostConfiguration : BaseConfiguration<ComicSubPost>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<ComicSubPost> builder)
    {
        builder.ToTable("ComicSubPosts", DbSchema.Comic);
        builder.HasIndex(x => new { x.PostId, x.HashId, x.AuthorId }).IsUnique();
    }
}