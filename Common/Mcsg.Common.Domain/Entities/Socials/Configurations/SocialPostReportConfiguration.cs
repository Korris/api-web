using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class SocialPostReportConfiguration : BaseConfiguration<SocialPostReport>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<SocialPostReport> builder)
    {
        builder.ToTable("SocialPostReports", DbSchema.Social);
        builder.HasOne(typeof(SocialPost)).WithMany().HasForeignKey("PostId");
        builder.HasOne(typeof(User)).WithMany().HasForeignKey("UserId");
    }
}
