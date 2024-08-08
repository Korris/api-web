using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class SessionConfiguration : BaseConfiguration<Session>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<Session> builder)
    {
        builder.ToTable("Sessions", DbSchema.Identity);
        builder.Property(x => x.LoginProvider).IsRequired();
        builder.Property(x => x.LoginDateUtc);
        builder.Property(x => x.ExpiredDateUtc);
        builder.Property(x => x.UserName).IsRequired();
        builder.Property(x => x.Email).IsRequired();
        builder.Property(x => x.FirstName);
        builder.Property(x => x.LastName);
        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.Roles);
        builder.Property(x => x.Claims);
        builder.Property(x => x.LastActionDateUtc);
    }
}