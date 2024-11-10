using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

public class SmartLookupUserConfiguration : BaseConfiguration<SmartLookupUser>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<SmartLookupUser> builder)
    {
        builder.ToTable("SmartLookupUsers");
        builder.Property(x => x.Keyword).IsRequired();
    }
}
