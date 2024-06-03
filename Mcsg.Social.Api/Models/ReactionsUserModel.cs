using Mcsg.Lib.Data.Enums;

namespace Mcsg.Api.Models
{
    public class ReactionsUserModel
    {
        public ReactionType Type { get; set; }
        public Guid AuthorId { get; set; }
        public string AuthorName { get; set; }
        public string AuthorAvatar { get; set; }
    }
}
