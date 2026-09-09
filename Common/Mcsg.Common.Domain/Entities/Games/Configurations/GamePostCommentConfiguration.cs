using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

/// <summary>
/// EF configuration for game."GamePostComments" (mirrors the Comic counterpart)
/// </summary>
public class GamePostCommentConfiguration : BaseConfiguration<GamePostComment>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<GamePostComment> builder)
    {
        builder.ToTable("GamePostComments", DbSchema.Game);
        builder.HasIndex(x => new { x.PostId, x.ParentId, x.AuthorId });
    }
}
