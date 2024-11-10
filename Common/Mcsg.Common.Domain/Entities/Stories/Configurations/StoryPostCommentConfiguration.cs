using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class StoryPostCommentConfiguration : BaseConfiguration<StoryPostComment>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<StoryPostComment> builder)
    {
        builder.ToTable("StoryPostComments", DbSchema.Story);
        builder.HasIndex(x => new { x.PostId, x.ParentId, x.AuthorId });
    }
}