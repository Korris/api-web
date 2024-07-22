using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class StoryPostConfiguration : BaseConfiguration<StoryPost>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<StoryPost> builder)
    {
        builder.ToTable("StoryPosts", DbSchema.Story);
        builder.Property(x => x.HashId).IsRequired();
        builder.HasIndex(x => new { x.HashId, x.UserId, x.Type, x.Id }).IsUnique();
        builder.HasOne(typeof(User)).WithMany().HasForeignKey("UserId");
    }
}