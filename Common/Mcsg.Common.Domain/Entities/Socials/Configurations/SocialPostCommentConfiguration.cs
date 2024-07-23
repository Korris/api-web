using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class SocialPostCommentConfiguration : BaseConfiguration<SocialPostComment>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<SocialPostComment> builder)
    {
        builder.ToTable("SocialPostComments", DbSchema.Social);
        builder.HasIndex(x => new { x.PostId, x.ParentId, x.AuthorId });
        builder.HasOne(typeof(SocialPost)).WithMany().HasForeignKey("PostId");
        builder.HasOne(typeof(User)).WithMany().HasForeignKey("AuthorId");
        builder.HasOne(typeof(SocialResource)).WithMany().HasForeignKey("ResourceId");
    }
}