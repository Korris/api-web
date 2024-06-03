using Mcsg.Lib.Data.Domain.Entities.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Lib.Data.Domain.Entities
{
    [Table("Sessions")]
    public class Session : AuditableEntity
    {
        public string LoginProvider { get; set; }
        public DateTime LoginDateUtc { get; set; }
        public DateTime ExpiredDateUtc { get; set; }
        public string UserName { get; set; }
        public string ProfileName { get; set; }
        public string ProfileId { get; set; }
        public string UserAvatar { get; set; }
        public string Email { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public Guid UserId { get; set; }
        public string Roles { get; set; }
        public string Claims { get; set; }
        public DateTime LastActionDateUtc { get; set; }
        public DateOnly? PremiumDate { get; set; }

        public bool IsPremium()
        {
            return (PremiumDate != null && PremiumDate > DateOnly.FromDateTime(DateTime.UtcNow));
        }
    }
}