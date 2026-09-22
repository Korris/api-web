using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

/// <summary>
/// EF configuration for tapshow."TapShowPostComments" (mirrors the Game counterpart)
/// </summary>
public class TapShowPostCommentConfiguration : BaseConfiguration<TapShowPostComment>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<TapShowPostComment> builder)
    {
        builder.ToTable("TapShowPostComments", DbSchema.TapShow);
        builder.HasIndex(x => new { x.PostId, x.ParentId, x.AuthorId });
    }
}
