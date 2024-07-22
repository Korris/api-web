using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations
{
    public class PostLinkConfiguration : BaseConfiguration<PostLink>
    {
        public override void CreateEntityConfiguration(EntityTypeBuilder<PostLink> builder)
        {
            builder.ToTable("PostLinks");
            builder.HasOne(typeof(Post)).WithMany().HasForeignKey("PostId");
        }
    }
}
