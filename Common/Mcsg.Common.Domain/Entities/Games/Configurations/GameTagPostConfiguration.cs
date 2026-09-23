using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

/// <summary>
/// EF configuration for game."GameTagPosts" (mirrors ComicTagPostConfiguration)
/// </summary>
public class GameTagPostConfiguration : BaseConfiguration<GameTagPost>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<GameTagPost> builder)
    {
        builder.ToTable("GameTagPosts", DbSchema.Game);
        builder.HasIndex(x => new { x.TagId, x.PostId });
    }
}
