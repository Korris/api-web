using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

/// <summary>
/// EF configuration for tapshow."TapShowChapters" (mirrors ComicSubPostConfiguration)
/// </summary>
public class TapShowChapterConfiguration : BaseConfiguration<TapShowChapter>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<TapShowChapter> builder)
    {
        builder.ToTable("TapShowChapters", DbSchema.TapShow);
        builder.Property(x => x.HashId).IsRequired();
        builder.HasIndex(x => new { x.PostId, x.HashId, x.AuthorId }).IsUnique();
        // Chapters are looked up by HashId alone (get / update / delete / segment create)
        builder.HasIndex(x => x.HashId);
    }
}
