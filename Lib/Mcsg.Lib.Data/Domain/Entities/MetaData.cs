using Mcsg.Lib.Data.Domain.Entities.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Lib.Data.Domain.Entities
{
    [Table("MetaDatas")]
    public class MetaData : BaseEntity
    {
        public string? Title { get; set; }
        public string? Url { get; set; }
        public string? Description { get; set; }
        public string? Domain { get; set; }
        public Guid? PostId { get; set; }
        public Guid? SubPostId { get; set; }
        public Guid? PostCommentId { get; set; }
        public Guid? SubPostCommentId { get; set; }
    }
}