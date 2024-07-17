using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Lib.Data.Domain.Entities.Comics.Configurations
{
    using Mcsg.Lib.Data.Constants;
    using Mcsg.Lib.Data.Domain.Entities.Configurations;

    public class ComicSubPostConfiguration : BaseConfiguration<ComicSubPost>
    {
        public override void CreateEntityConfiguration(EntityTypeBuilder<ComicSubPost> builder)
        {
            builder.ToTable("ComicSubPosts", DbSchema.Comic);
            builder.HasIndex(x => new { x.PostId, x.HashId, x.AuthorId }).IsUnique();
            builder.HasOne(typeof(ComicPost)).WithMany().HasForeignKey("PostId");
            builder.HasOne(typeof(User)).WithMany().HasForeignKey("UserId");
        }
    }
}