using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

/// <summary>
/// EF configuration for game."GamePosts" (mirrors ComicPostConfiguration)
/// </summary>
public class GamePostConfiguration : BaseConfiguration<GamePost>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<GamePost> builder)
    {
        builder.ToTable("GamePosts", DbSchema.Game);
        builder.Property(x => x.HashId).IsRequired();
        builder.HasIndex(x => new { x.HashId, x.UserId, x.Type, x.Id }).IsUnique();
    }
}
