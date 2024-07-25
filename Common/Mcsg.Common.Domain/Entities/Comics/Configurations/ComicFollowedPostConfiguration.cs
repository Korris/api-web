using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class ComicFollowedPostConfiguration : BaseConfiguration<ComicFollowedPost>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<ComicFollowedPost> builder)
    {
        builder.ToTable("ComicFollowedPosts", DbSchema.Comic);
        builder.HasIndex(x => new { x.PostId, x.CreatedBy, x.Id }).IsUnique();
        builder.HasOne(typeof(ComicPost)).WithMany().HasForeignKey("PostId");

    }
}