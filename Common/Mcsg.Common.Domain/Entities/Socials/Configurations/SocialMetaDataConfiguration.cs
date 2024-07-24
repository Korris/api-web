using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class SocialMetaDataConfiguration : BaseConfiguration<SocialMetaData>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<SocialMetaData> builder)
    {
        builder.ToTable("SocialMetaDatas", DbSchema.Social);
    }
}