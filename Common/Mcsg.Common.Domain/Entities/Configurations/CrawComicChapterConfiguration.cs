using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mcsg.Common.Domain.Entities.Configurations
{
    public class CrawComicChapterConfiguration : BaseConfiguration<CrawComicChapter>
    {
        public override void CreateEntityConfiguration(EntityTypeBuilder<CrawComicChapter> builder)
        {
            builder.ToTable("CrawComicChapters");
        }
    }
}
