using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class ComicPostShareConfiguration : BaseConfiguration<ComicPostShare>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<ComicPostShare> builder)
    {
        builder.ToTable("ComicPostShares", DbSchema.Comic);
        builder.HasOne(typeof(ComicPost)).WithMany().HasForeignKey("PostId");
        builder.HasOne(typeof(User)).WithMany().HasForeignKey("UserId");
    }
}
