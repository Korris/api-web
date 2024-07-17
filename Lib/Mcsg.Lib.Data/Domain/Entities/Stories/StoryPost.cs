using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Lib.Data.Domain.Entities.Stories
{
    using Lib.Data.Domain.Entities.Common;
    using Lib.Data.Enums;
    using Mcsg.Common.Core.Enums;

    [Table("StoryPosts")]
    public partial class StoryPost : AuditableHasPrivateEntity
    {
        public string? Title { get; set; }
        public string? HashId { get; set; }
        public Guid? AuthorId { get; set; }
        public Guid UserId { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? AuthorName { get; set; }
        public string? CoverUrl { get; set; }
        public PostType Type { get; set; }
        public string? Body { get; set; }
        public PostStatus Status { get; set; }
        public string? StatusReason { get; set; }
        public bool? IsMature { get; set; }
        public bool? IsCompleted { get; set; }
        public int ViewCount { get; set; }
        public string? ExternalCode { get; set; }
        public ComicExternalResource ExternalResource { get; set; }
        public string? CustomNote { get; set; }
    }
}