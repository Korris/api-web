namespace Mcsg.Wallet.Api.Models
{
    public class BuyItemResp
    {
        public List<PayMethodResp> PayMethods { get; set; }
        public List<CurrencyTypeRatio> CurrencyTypes { get; set; }
        public ItemSeletedResp Item { get; set; }
        public float CurrentPoint { get; set; }
    }
    public class ItemSeletedResp
    {
        public string Name { get; set; }
        public float Price { get; set; }
    }
}
