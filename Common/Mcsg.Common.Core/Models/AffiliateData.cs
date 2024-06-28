namespace Mcsg.Lib.Model.Models
{
    using Common.Core.Enums;

    public class AffiliateData
    {
        public Guid AffiliateUserId { get; set; }
        public string EntityHashId { get; set; }
        public AffiliateEntityType EntityType { get; set; }
    }
}
