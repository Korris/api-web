using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Lib.Data.Domain.Entities.Configurations
{
    public class SmartLookupUserConfiguration : BaseEntityConfiguration<SmartLookupUser>
    {
        public override void CreateEntityConfiguration(EntityTypeBuilder<SmartLookupUser> builder)
        {
            builder.ToTable("SmartLookupUsers");
            builder.Property(x => x.Keyword).IsRequired();
            builder.HasOne(typeof(User)).WithMany().HasForeignKey("UserId");
        }
    }
}
