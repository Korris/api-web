using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Lib.Data.Domain.Entities.Configurations
{
    public class SubPostEntityConfiguration : BaseEntityConfiguration<SubPost>
    {
        public override void CreateEntityConfiguration(EntityTypeBuilder<SubPost> builder)
        {
            builder.ToTable("SubPosts");
            builder.HasIndex(x => new { x.PostId, x.HashId, x.AuthorId }).IsUnique();
            builder.HasOne(typeof(Post)).WithMany().HasForeignKey("PostId");
            builder.HasOne(typeof(User)).WithMany().HasForeignKey("UserId");
        }
    }
}