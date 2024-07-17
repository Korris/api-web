using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Lib.Data.Domain.Entities.Comics.Configurations
{
    using Mcsg.Lib.Data.Constants;
    using Mcsg.Lib.Data.Domain.Entities.Configurations;

    public class PostEntityConfiguration : BaseEntityConfiguration<Post>
    {
        public override void CreateEntityConfiguration(EntityTypeBuilder<Post> builder)
        {
            builder.ToTable("Posts", DbSchema.Comic);
            builder.Property(x => x.HashId).IsRequired();
            builder.HasIndex(x => new { x.HashId, x.UserId, x.Type, x.Id }).IsUnique();
            builder.HasOne(typeof(User)).WithMany().HasForeignKey("UserId");
        }
    }
}