using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities;

public class BaseConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : BaseEntity
{
    public void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");
        CreateEntityConfiguration(builder);
    }

    public virtual void CreateEntityConfiguration(EntityTypeBuilder<TEntity> builder)
    {
    }
}
