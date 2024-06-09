using Mcsg.Lib.Data.Enums;

namespace Mcsg.Social.Api.Models
{
    public class UserMentionModel
    {
        public Guid Id { get; set; }
        public Guid LocationId { get; set; }
        public MentionLocationType LocationType { get; set; }
        public Guid EntityId { get; set; }
        public MentionEntityType EntityType { get; set; }
        public string ProfileName { get; set; }
        public int Length { get; set; }
        public int Offset { get; set; }
        public string Text { get; set; }
    }
}
