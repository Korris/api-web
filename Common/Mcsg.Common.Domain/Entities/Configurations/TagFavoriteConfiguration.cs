using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Lib.Data.Domain.Entities.Configurations
{
    public class TagFavoriteConfiguration : BaseConfiguration<TagFavorite>
    {
        public override void CreateEntityConfiguration(EntityTypeBuilder<TagFavorite> builder)
        {
            builder.ToTable("TagFavorites");
            builder.HasOne(typeof(Tag)).WithMany().HasForeignKey("TagId");
            builder.HasOne(typeof(User)).WithMany().HasForeignKey("UserId");
        }
    }
}
