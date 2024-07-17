using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Lib.Data.Domain.Entities.Configurations
{
    public class CrawComicConfiguration : BaseConfiguration<CrawComic>
    {
        public override void CreateEntityConfiguration(EntityTypeBuilder<CrawComic> builder)
        {
            builder.ToTable("CrawComics");
        }
    }
}
