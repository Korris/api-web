using Mcsg.Lib.Model.Enums;

namespace Mcsg.Lib.Model.Models
{
    public class AffiliateData
    {
        public Guid AffiliateUserId { get; set; }
        public string EntityHashId { get; set; }
        public AffiliateEntityType EntityType { get; set; }
    }
}
