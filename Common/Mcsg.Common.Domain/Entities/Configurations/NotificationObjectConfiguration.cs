using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

public class NotificationObjectConfiguration : BaseConfiguration<NotificationObject>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<NotificationObject> builder)
    {
        builder.ToTable("NotificationObjects");
    }
}
