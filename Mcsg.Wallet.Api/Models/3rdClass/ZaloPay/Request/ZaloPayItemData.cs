using Newtonsoft.Json;

namespace Mcsg.Wallet.Api.Models._3rdClass.ZaloPay.Request
{
    public class ZaloPayItemData
    {
        [JsonProperty("itemid")]
        public string ItemId { get; set; } = string.Empty;
        [JsonProperty("itename")]
        public string ItemName { get; set; } = string.Empty;
        [JsonProperty("itemprice")]
        public string ItemPrice { get; set; } = string.Empty;
        [JsonProperty("itemquantity")]
        public string ItemQuantity { get; set; } = string.Empty;
    }
}
