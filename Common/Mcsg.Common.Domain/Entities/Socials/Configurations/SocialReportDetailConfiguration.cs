using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class SocialReportDetailConfiguration : BaseConfiguration<SocialReportDetail>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<SocialReportDetail> builder)
    {
        builder.ToTable("SocialReportDetails", DbSchema.Social);
        builder.HasOne(typeof(SocialReport)).WithMany().HasForeignKey("ReportId");
        builder.HasOne(typeof(User)).WithMany().HasForeignKey("UserId");
    }
}
