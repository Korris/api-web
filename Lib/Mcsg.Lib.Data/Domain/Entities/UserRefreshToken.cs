using Mcsg.Lib.Data.Domain.Entities.Common;

namespace Mcsg.Lib.Data.Domain.Entities
{
    public class UserRefreshToken : AuditableEntity
    {
        public Guid UserId { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}