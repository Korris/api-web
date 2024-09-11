using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class RatingConfiguration : BaseConfiguration<Rating>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<Rating> builder)
    {
        builder.ToTable("Ratings", DbSchema.System);
        builder.HasOne(typeof(User)).WithMany().HasForeignKey("UserId");
    }
}