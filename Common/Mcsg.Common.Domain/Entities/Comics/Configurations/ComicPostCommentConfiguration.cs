using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class ComicPostCommentConfiguration : BaseConfiguration<ComicPostComment>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<ComicPostComment> builder)
    {
        builder.ToTable("ComicPostComments", DbSchema.Comic);
        builder.HasIndex(x => new { x.PostId, x.ParentId, x.AuthorId });
    }
}