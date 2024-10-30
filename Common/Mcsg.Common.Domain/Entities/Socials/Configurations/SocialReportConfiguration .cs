using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class SocialReportConfiguration : BaseConfiguration<SocialReport>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<SocialReport> builder)
    {
        builder.ToTable("SocialReports", DbSchema.Social);
    }
}
