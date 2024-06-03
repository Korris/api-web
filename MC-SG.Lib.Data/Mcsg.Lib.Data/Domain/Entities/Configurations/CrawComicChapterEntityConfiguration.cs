using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Lib.Data.Domain.Entities.Configurations
{
    public class CrawComicChapterEntityConfiguration : BaseEntityConfiguration<CrawComicChapter>
    {
        public override void CreateEntityConfiguration(EntityTypeBuilder<CrawComicChapter> builder)
        {
            builder.ToTable("CrawComicChapters");
        }
    }
}
