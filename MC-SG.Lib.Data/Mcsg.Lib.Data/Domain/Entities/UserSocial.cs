using Mcsg.Lib.Data.Domain.Entities.Common;

namespace Mcsg.Lib.Data.Domain.Entities
{
    public class UserSocial : AuditableEntity
    {
        public Guid UserId { get; set; }
        public string SocialId { get; set; }
        public string Type { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public bool IsRegisterBySocial { get; set; }
        public string RegisterBySocialPlatform { get; set; }
    }
}