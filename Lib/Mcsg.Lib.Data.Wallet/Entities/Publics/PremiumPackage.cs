using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Lib.Data.Wallet.Entities
{
    using Mcsg.Common.SeedWork;

    public class PremiumPackage : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int No { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public float Price { get; set; }
        public float PricePerMonth { get; set; }
        public int LiveTimeDay { get; set; }
        public int FirstTimeDiscountPercent { get; set; }
        public float FirstTimePricePerMonth { get; set; }
        public float FirstTimePrice { get; set; }
        public bool IsPackage { get; set; }
    }
}
