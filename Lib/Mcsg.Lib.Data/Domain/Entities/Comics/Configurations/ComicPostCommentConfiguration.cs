using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Lib.Data.Domain.Entities.Configurations
{
    using Mcsg.Lib.Data.Constants;

    public class ComicPostCommentConfiguration : BaseConfiguration<ComicPostComment>
    {
        public override void CreateEntityConfiguration(EntityTypeBuilder<ComicPostComment> builder)
        {
            builder.ToTable("ComicPostComments", DbSchema.Comic);
            builder.HasIndex(x => new { x.PostId, x.ParentId, x.AuthorId });
            builder.HasOne(typeof(ComicPost)).WithMany().HasForeignKey("PostId");
            builder.HasOne(typeof(User)).WithMany().HasForeignKey("AuthorId");
            builder.HasOne(typeof(ComicResource)).WithMany().HasForeignKey("ResourceId");
        }
    }
}