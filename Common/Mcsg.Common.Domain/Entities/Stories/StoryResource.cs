using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Lib.Data.Domain.Entities
{
    using Common;
    using Mcsg.Common.Core.Enums;

    [Table("StoryResources")]
    public class StoryResource : AuditableEntity
    {
        public Guid? AuthorId { get; set; }
        public string? HashId { get; set; }
        public Guid? SubPostId { get; set; }
        public string? Title { get; set; }
        public string? Name { get; set; }
        public string? Url { get; set; }
        public string? ShareUrl { get; set; }
        public int Order { get; set; }
        public double Size { get; set; } // bytes
        public int Width { get; set; }
        public int Height { get; set; }
        public ResourceType Type { get; set; }
        public ResourceLocationType LocationType { get; set; }
        public ResourceStatus Status { get; set; } = ResourceStatus.Done;
    }
}