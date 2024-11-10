using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class SocialPostShareConfiguration : BaseConfiguration<SocialPostShare>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<SocialPostShare> builder)
    {
        builder.ToTable("SocialPostShares", DbSchema.Social);
    }
}
