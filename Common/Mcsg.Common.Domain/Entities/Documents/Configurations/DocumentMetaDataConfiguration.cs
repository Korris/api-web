using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations;

using Core.Constants;

public class DocumentMetaDataConfiguration : BaseConfiguration<DocumentMetaData>
{
    public override void CreateEntityConfiguration(EntityTypeBuilder<DocumentMetaData> builder)
    {
        builder.ToTable("DocumentMetaDatas", DbSchema.Document);
    }
}