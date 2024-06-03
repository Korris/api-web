using Mcsg.Lib.Data.Analytic.Entities.Base;
using Mcsg.Lib.Model.Enums;

namespace Mcsg.Lib.Data.Analytic.Entities
{
    public class UserViewPost : TrackingEntity
    {
        public PostType PostType { get; set; } //Feed, comic or story
        public Guid PostId { get; set; }
        public Guid SubPostId { get; set; }
        public UserType UserType { get; set; } // guest, free, premium
        public Guid? AuthorId { get; set; }
        public Guid? UserId { get; set; }
        public string? IpAddress { get; set; }
        public string? BrowserAgent { get; set; }
        public string? UserHashString { get; set; } // định danh user - UserLogin: Hash của user id + user type, Guest: IpAddress + BrowserAgent + DateOnly
        public long TimeSpan { get; set; } //Check time in miliseconds from latest view

    }
}
