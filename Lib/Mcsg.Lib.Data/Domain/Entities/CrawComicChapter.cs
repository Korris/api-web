using Mcsg.Lib.Data.Domain.Entities.Common;
using Mcsg.Lib.Data.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Lib.Data.Domain.Entities
{
    [Table("CrawComicChapters")]
    public class CrawComicChapter : AuditableEntity
    {
        public Guid SourceComic { get; set; }
        public string? ExternalCode { get; set; }
        public string? Title { get; set; }
        public string? Url { get; set; }
        public string? Name { get; set; }
        public CrawComicChapterStatus Status { get; set; }
        public string? ExternalLastedUpdate { get; set; }
        public DateTime? ExternalLastedUpdateDate { get; set; }
        public string? View { get; set; }
        public int ViewNumber { get; set; }
    }
}
