using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class DocumentPostShareConfiguration : BaseConfiguration<DocumentPostShare>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<DocumentPostShare> builder)
    {
        builder.ToTable("DocumentPostShares", DbSchema.Document);
    }
}
