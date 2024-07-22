using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations
{
    using Core.Constants;

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