using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Lib.Data.Domain.Entities.Configurations
{
    public class PostLinkEntityConfiguration : BaseEntityConfiguration<PostLink>
    {
        public override void CreateEntityConfiguration(EntityTypeBuilder<PostLink> builder)
        {
            builder.ToTable("PostLinks");
            builder.HasOne(typeof(Post)).WithMany().HasForeignKey("PostId");
        }
    }
}
