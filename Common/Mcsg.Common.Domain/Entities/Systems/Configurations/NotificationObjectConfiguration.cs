using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class NotificationObjectConfiguration : BaseConfiguration<NotificationObject>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<NotificationObject> builder)
    {
        builder.ToTable("NotificationObjects", DbSchema.System);
    }
}
