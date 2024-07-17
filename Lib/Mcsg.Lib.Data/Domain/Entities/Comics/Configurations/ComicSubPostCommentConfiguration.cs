using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Lib.Data.Domain.Entities.Comics.Configurations
{
    using Mcsg.Lib.Data.Constants;
    using Mcsg.Lib.Data.Domain.Entities.Configurations;

    public class ComicSubPostCommentConfiguration : BaseConfiguration<ComicSubPostComment>
    {
        public override void CreateEntityConfiguration(EntityTypeBuilder<ComicSubPostComment> builder)
        {
            builder.ToTable("ComicSubPostComments", DbSchema.Comic);
            builder.HasIndex(x => new { x.PostId, x.ParentId, x.AuthorId });
            builder.HasOne(typeof(ComicSubPost)).WithMany().HasForeignKey("PostId");
            builder.HasOne(typeof(User)).WithMany().HasForeignKey("AuthorId");
            builder.HasOne(typeof(ComicResource)).WithMany().HasForeignKey("ResourceId");
        }
    }
}