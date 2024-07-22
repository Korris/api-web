namespace Mcsg.Common.Domain.Entities
{
    using Common;

    public class UserRefreshToken : AuditableEntity
    {
        public Guid UserId { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}