using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

public class PostFavoriteConfiguration : BaseConfiguration<PostFavorite>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<PostFavorite> builder)
    {
        builder.ToTable("PostFavorites");
        builder.HasOne(typeof(SocialPost)).WithMany().HasForeignKey("PostId");
        builder.HasOne(typeof(User)).WithMany().HasForeignKey("UserId");
    }
}
