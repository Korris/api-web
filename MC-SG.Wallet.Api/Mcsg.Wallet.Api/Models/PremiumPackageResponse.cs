namespace Mcsg.Wallet.Api.Models
{
    public class PremiumPackageResponse
    {
        public int No { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float Price { get; set; }
        public float PricePerMonth { get; set; }
        public int LiveTimeDay { get; set; }
        public int DiscountPercent { get; set; }
        public float DiscountPricePerMonth { get; set; }
        public float DiscountPrice { get; set; }
        public bool IsPackage { get; set; }
    }
    public class PremiumPackagePurchaseResponse
    {
        public Guid Id { get; set; }
        public int No { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float Price { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}
