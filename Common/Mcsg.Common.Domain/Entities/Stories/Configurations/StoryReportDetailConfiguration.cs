using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class StoryReportDetailConfiguration : BaseConfiguration<StoryReportDetail>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<StoryReportDetail> builder)
    {
        builder.ToTable("StoryReportDetails", DbSchema.Story);
    }
}
