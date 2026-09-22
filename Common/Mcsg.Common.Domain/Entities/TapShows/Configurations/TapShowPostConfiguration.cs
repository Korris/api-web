using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

/// <summary>
/// EF configuration for tapshow."TapShowPosts" (mirrors GamePostConfiguration)
/// </summary>
public class TapShowPostConfiguration : BaseConfiguration<TapShowPost>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<TapShowPost> builder)
    {
        builder.ToTable("TapShowPosts", DbSchema.TapShow);
        builder.Property(x => x.HashId).IsRequired();
        builder.HasIndex(x => new { x.HashId, x.UserId, x.Type, x.Id }).IsUnique();
    }
}
