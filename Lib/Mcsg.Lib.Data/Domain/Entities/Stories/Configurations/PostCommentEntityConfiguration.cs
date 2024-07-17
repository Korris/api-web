using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Lib.Data.Domain.Entities.Stories.Configurations
{
    using Mcsg.Lib.Data.Constants;
    using Mcsg.Lib.Data.Domain.Entities.Configurations;

    public class PostCommentEntityConfiguration : BaseEntityConfiguration<PostComment>
    {
        public override void CreateEntityConfiguration(EntityTypeBuilder<PostComment> builder)
        {
            builder.ToTable("PostComments", DbSchema.Story);
            builder.HasIndex(x => new { x.PostId, x.ParentId, x.AuthorId });
            builder.HasOne(typeof(Post)).WithMany().HasForeignKey("PostId");
            builder.HasOne(typeof(User)).WithMany().HasForeignKey("AuthorId");
            builder.HasOne(typeof(Resource)).WithMany().HasForeignKey("ResourceId");
        }
    }
}