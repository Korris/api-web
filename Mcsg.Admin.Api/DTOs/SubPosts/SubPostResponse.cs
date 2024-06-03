using Mcsg.Lib.Data.Enums;
using Mcsg.Lib.Model.Enums;

namespace Mcsg.Admin.Api.DTOs.SubPosts
{
    public class SubPostResponse
    {
        public Guid Id { get; set; }
        public string HashId { get; set; }
        public string Title { get; set; }
        public PostPermission Permission { get; set; }
        public PostStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? PublishDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UserId { get; set; }
        public string CreatorNote { get; set; }
    }
}
