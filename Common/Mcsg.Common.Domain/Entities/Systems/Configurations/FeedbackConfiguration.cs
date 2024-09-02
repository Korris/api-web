using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class FeedbackConfiguration : BaseConfiguration<Feedback>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<Feedback> builder)
    {
        builder.ToTable("Feedbacks", DbSchema.System);
        builder.HasOne(typeof(User)).WithMany().HasForeignKey("UserId");
    }
}