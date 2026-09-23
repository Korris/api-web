using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

/// <summary>
/// EF configuration for tapshow."TapShowTagPosts" (mirrors ComicTagPostConfiguration)
/// </summary>
public class TapShowTagPostConfiguration : BaseConfiguration<TapShowTagPost>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<TapShowTagPost> builder)
    {
        builder.ToTable("TapShowTagPosts", DbSchema.TapShow);
        builder.HasIndex(x => new { x.TagId, x.PostId });
    }
}
