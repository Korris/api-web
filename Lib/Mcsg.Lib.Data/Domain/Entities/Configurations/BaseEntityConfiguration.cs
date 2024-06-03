using Mcsg.Lib.Data.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Lib.Data.Domain.Entities.Configurations
{
    public class BaseEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : BaseEntity
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
}