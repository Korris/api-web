using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Lib.Data.Domain.Entities.Configurations
{
    public class PostFavoriteEntityConfiguration : BaseEntityConfiguration<PostFavorite>
    {
        public override void CreateEntityConfiguration(EntityTypeBuilder<PostFavorite> builder)
        {
            builder.ToTable("PostFavorites");
            builder.HasOne(typeof(Post)).WithMany().HasForeignKey("PostId");
            builder.HasOne(typeof(User)).WithMany().HasForeignKey("UserId");
        }
    }
}
