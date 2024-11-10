using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class SocialReportDetailConfiguration : BaseConfiguration<SocialReportDetail>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<SocialReportDetail> builder)
    {
        builder.ToTable("SocialReportDetails", DbSchema.Social);
    }
}
