using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class PostCommentConfiguration : BaseConfiguration<PostComment>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<PostComment> builder)
    {
        builder.ToTable("PostComments", DbSchema.Social);
        builder.HasIndex(x => new { x.PostId, x.ParentId, x.AuthorId });
        builder.HasOne(typeof(Post)).WithMany().HasForeignKey("PostId");
        builder.HasOne(typeof(User)).WithMany().HasForeignKey("AuthorId");
        builder.HasOne(typeof(Resource)).WithMany().HasForeignKey("ResourceId");
    }
}