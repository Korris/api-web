using Newtonsoft.Json;

namespace Mcsg.Wallet.Api.Models._3rdClass.ZaloPay.Request
{
    public class ZaloPayEmbedData
    {
        [JsonProperty("redirecturl")]
        public string RedirectUrl { get; set; }
        [JsonProperty("transaction-id")]
        public string TransactionId { get; set; }
        [JsonProperty("user-id")]
        public string UserId { get; set; }
    }
}
