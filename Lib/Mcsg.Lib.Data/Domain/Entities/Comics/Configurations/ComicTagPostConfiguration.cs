using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Lib.Data.Domain.Entities.Configurations
{
    using Mcsg.Lib.Data.Constants;

    public class ComicTagPostConfiguration : BaseConfiguration<ComicTagPost>
    {
        public override void CreateEntityConfiguration(EntityTypeBuilder<ComicTagPost> builder)
        {
            builder.ToTable("ComicTagPosts", DbSchema.Comic);
            builder.HasIndex(x => new { x.TagId, x.PostId });
            builder.HasOne(typeof(Tag)).WithMany().HasForeignKey("TagId");
            builder.HasOne(typeof(ComicPost)).WithMany().HasForeignKey("PostId");
        }
    }
}