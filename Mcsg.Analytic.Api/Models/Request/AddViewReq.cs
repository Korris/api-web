namespace Mcsg.Analytic.Api.Models.Request
{
    using Common.SeedWork.Enums;

    public class AddViewReqSimple
    {
        public Guid SubPostId { get; set; }
        //public UserType UserType { get; set; } // guest, free, premium
        //public Guid? UserId { get; set; }
    }
    public class AddViewSimReqSimple
    {
        public Guid SubPostId { get; set; }
        public UserType UserType { get; set; } // guest, free, premium
        public int NumberView { get; set; }
        //public Guid? UserId { get; set; }
    }
    public class AddViewReq : AddViewReqSimple
    {
        public UserType UserType { get; set; } // guest, free, premium
        //public Guid? UserId { get; set; }
        public Guid? SessionId { get; set; }
        public Guid? UserId { get; set; }
        public Lib.Model.Enums.PostType PostType { get; set; } //Feed, comic or story
        public Guid PostId { get; set; }
        public string? IpAddress { get; set; }
        public string? BrowserAgent { get; set; }
    }
}
