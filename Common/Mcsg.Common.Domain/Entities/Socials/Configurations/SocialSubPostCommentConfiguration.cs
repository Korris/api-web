using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class SocialSubPostCommentConfiguration : BaseConfiguration<SocialSubPostComment>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<SocialSubPostComment> builder)
    {
        builder.ToTable("SocialSubPostComments", DbSchema.Social);
        builder.HasIndex(x => new { x.PostId, x.ParentId, x.AuthorId });
        builder.HasOne(typeof(SocialSubPost)).WithMany().HasForeignKey("PostId");
        builder.HasOne(typeof(User)).WithMany().HasForeignKey("AuthorId");
        builder.HasOne(typeof(SocialResource)).WithMany().HasForeignKey("ResourceId");
    }
}