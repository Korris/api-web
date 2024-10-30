using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Stories.Configurations;

using Core.Constants;

public class StoryReportDetailConfiguration : BaseConfiguration<StoryReportDetail>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<StoryReportDetail> builder)
    {
        builder.ToTable("StoryReportDetails", DbSchema.Story);
        builder.HasOne(typeof(StoryReport)).WithMany().HasForeignKey("ReportId");
        builder.HasOne(typeof(User)).WithMany().HasForeignKey("UserId");
    }
}
