using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class ComicPostHideConfiguration : BaseConfiguration<ComicPostHide>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<ComicPostHide> builder)
    {
        builder.ToTable("ComicPostHides", DbSchema.Comic);
        builder.HasOne(typeof(ComicPost)).WithMany().HasForeignKey("PostId");
        builder.HasOne(typeof(User)).WithMany().HasForeignKey("UserId");
    }
}
